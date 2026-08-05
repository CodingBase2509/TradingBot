using NodaTime;

namespace TradingPlatform.Market.Calendars;

public sealed record MarketSession
{
    internal MarketSession(
        LocalDate tradingDate,
        Instant opensAt,
        Instant closesAt,
        MarketSessionType type)
    {
        if (closesAt <= opensAt)
        {
            throw new ArgumentException("A market session must close after it opens.", nameof(closesAt));
        }

        if (type is MarketSessionType.Unknown || !Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), type, "The market session type must be known.");
        }

        TradingDate = tradingDate;
        OpensAt = opensAt;
        ClosesAt = closesAt;
        Type = type;
    }

    public LocalDate TradingDate { get; }

    public Instant OpensAt { get; }

    public Instant ClosesAt { get; }

    public MarketSessionType Type { get; }

    public bool Contains(Instant instant) => instant >= OpensAt && instant < ClosesAt;
}
