using NodaTime;

namespace TradingPlatform.Market.Calendars.Config;

internal static class MarketCalendarConfigValidator
{
    public static MarketCalendarConfigValidationResult Validate(MarketCalendarConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var issues = new List<MarketCalendarConfigIssue>();

        if (config.Metadata is null)
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.MissingMetadata, "Config metadata is required.");
        }

        if (string.IsNullOrWhiteSpace(config.CalendarId))
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.MissingCalendarId, "A calendar ID is required.");
        }
        else if (!Guid.TryParse(config.CalendarId, out var calendarId) || calendarId == Guid.Empty)
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.InvalidCalendarId, "The calendar ID must be a non-empty GUID.");
        }

        if (string.IsNullOrWhiteSpace(config.Name))
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.MissingName, "A calendar name is required.");
        }

        if (string.IsNullOrWhiteSpace(config.TimeZoneId))
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.MissingTimeZone, "A time zone is required.");
        }
        else if (DateTimeZoneProviders.Tzdb.GetZoneOrNull(config.TimeZoneId) is null)
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.UnknownTimeZone, "The time zone must be a known TZDB ID.");
        }

        ValidateWeeklySessions(config.WeeklySessions, issues);
        ValidateDateOverrides(config.DateOverrides, issues);

        return new MarketCalendarConfigValidationResult(issues.ToArray());
    }

    private static void ValidateWeeklySessions(
        IReadOnlyList<MarketSessionConfig>? sessions,
        ICollection<MarketCalendarConfigIssue> issues)
    {
        if (sessions is null || sessions.Count == 0)
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.MissingWeeklySessions, "At least one weekly session is required.");
            return;
        }

        foreach (var session in sessions)
        {
            if (session is null)
            {
                AddIssue(issues, MarketCalendarConfigIssueCode.InvalidSession, "Weekly sessions must not contain null entries.");
                continue;
            }

            if (session.DayOfWeek is < IsoDayOfWeek.Monday or > IsoDayOfWeek.Sunday)
            {
                AddIssue(issues, MarketCalendarConfigIssueCode.InvalidDayOfWeek, "A weekly session must use a valid ISO day of week.");
            }

            ValidateSessionType(session.Type, issues);
        }

        var validSessions = sessions.Where(session =>
            session is not null &&
            session.DayOfWeek is >= IsoDayOfWeek.Monday and <= IsoDayOfWeek.Sunday &&
            session.Type is not MarketSessionType.Unknown &&
            Enum.IsDefined(session.Type));

        if (HasWeeklyOverlap(validSessions))
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.OverlappingSessions, "Weekly market sessions must not overlap.");
        }
    }

    private static void ValidateDateOverrides(
        IReadOnlyList<MarketCalendarDateOverrideConfig>? dateOverrides,
        ICollection<MarketCalendarConfigIssue> issues)
    {
        if (dateOverrides is null)
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.MissingDateOverrides, "The date override collection is required.");
            return;
        }

        var dates = new HashSet<LocalDate>();
        foreach (var dateOverride in dateOverrides)
        {
            if (dateOverride is null)
            {
                AddIssue(issues, MarketCalendarConfigIssueCode.InvalidDateOverride, "Date overrides must not contain null entries.");
                continue;
            }

            if (!dates.Add(dateOverride.Date))
            {
                AddIssue(issues, MarketCalendarConfigIssueCode.DuplicateDateOverride, "A date may only be overridden once.");
            }

            if (dateOverride.Sessions is null)
            {
                AddIssue(issues, MarketCalendarConfigIssueCode.InvalidDateOverride, "The override session collection is required.");
                continue;
            }

            foreach (var session in dateOverride.Sessions)
            {
                if (session is null)
                {
                    AddIssue(issues, MarketCalendarConfigIssueCode.InvalidSession, "Override sessions must not contain null entries.");
                    continue;
                }

                ValidateSessionType(session.Type, issues);
            }

            var validSessions = dateOverride.Sessions.Where(session =>
                session is not null &&
                session.Type is not MarketSessionType.Unknown &&
                Enum.IsDefined(session.Type));

            if (HasDailyOverlap(validSessions.Select(session => (session.OpensAt, session.ClosesAt))))
            {
                AddIssue(issues, MarketCalendarConfigIssueCode.OverlappingSessions, "Sessions in a date override must not overlap.");
            }
        }
    }

    private static void ValidateSessionType(
        MarketSessionType type,
        ICollection<MarketCalendarConfigIssue> issues)
    {
        if (type is MarketSessionType.Unknown || !Enum.IsDefined(type))
        {
            AddIssue(issues, MarketCalendarConfigIssueCode.UnknownSessionType, "The market session type must be known.");
        }
    }

    private static bool HasWeeklyOverlap(IEnumerable<MarketSessionConfig> sessions)
    {
        var weekTicks = NodaConstants.TicksPerDay * 7;
        var intervals = sessions
            .Select(session =>
            {
                var start = ((long)session.DayOfWeek - 1) * NodaConstants.TicksPerDay + session.OpensAt.TickOfDay;
                return CreateInterval(start, session.OpensAt, session.ClosesAt);
            })
            .ToArray();

        for (var leftIndex = 0; leftIndex < intervals.Length; leftIndex++)
        {
            for (var rightIndex = leftIndex + 1; rightIndex < intervals.Length; rightIndex++)
            {
                var left = intervals[leftIndex];
                var right = intervals[rightIndex];
                if (Overlaps(left, right) ||
                    Overlaps(left, right with { Start = right.Start - weekTicks, End = right.End - weekTicks }) ||
                    Overlaps(left, right with { Start = right.Start + weekTicks, End = right.End + weekTicks }))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool HasDailyOverlap(IEnumerable<(LocalTime OpensAt, LocalTime ClosesAt)> sessions)
    {
        var intervals = sessions
            .Select(session => CreateInterval(session.OpensAt.TickOfDay, session.OpensAt, session.ClosesAt))
            .ToArray();

        for (var leftIndex = 0; leftIndex < intervals.Length; leftIndex++)
        {
            for (var rightIndex = leftIndex + 1; rightIndex < intervals.Length; rightIndex++)
            {
                if (Overlaps(intervals[leftIndex], intervals[rightIndex]))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static SessionInterval CreateInterval(long start, LocalTime opensAt, LocalTime closesAt)
    {
        var duration = closesAt > opensAt
            ? closesAt.TickOfDay - opensAt.TickOfDay
            : NodaConstants.TicksPerDay - opensAt.TickOfDay + closesAt.TickOfDay;

        return new SessionInterval(start, start + duration);
    }

    private static bool Overlaps(SessionInterval left, SessionInterval right) =>
        left.Start < right.End && right.Start < left.End;

    private static void AddIssue(
        ICollection<MarketCalendarConfigIssue> issues,
        MarketCalendarConfigIssueCode code,
        string message) =>
        issues.Add(new MarketCalendarConfigIssue(code, message));

    private readonly record struct SessionInterval(long Start, long End);
}
