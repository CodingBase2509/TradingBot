namespace TradingPlatform.Platform.Config;

public sealed record ConfigVersionMetadata
{
    public ConfigVersionMetadata(
        ConfigId configId,
        int version,
        int schemaVersion,
        ConfigStatus status,
        DateTimeOffset createdAtUtc,
        string createdBy,
        string reason,
        ConfigContentHash contentHash)
    {
        ArgumentNullException.ThrowIfNull(configId);
        ArgumentNullException.ThrowIfNull(contentHash);
        ArgumentException.ThrowIfNullOrWhiteSpace(createdBy);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        if (version <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version), version, "A config version must be greater than zero.");
        }

        if (schemaVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(schemaVersion),
                schemaVersion,
                "A config schema version must be greater than zero.");
        }

        if (status is ConfigStatus.Unknown || !Enum.IsDefined(status))
        {
            throw new ArgumentOutOfRangeException(nameof(status), status, "The config status must be known.");
        }

        if (createdAtUtc == default || createdAtUtc.Offset != TimeSpan.Zero)
        {
            throw new ArgumentException("The config creation timestamp must be a non-default UTC value.", nameof(createdAtUtc));
        }

        if (!string.Equals(createdBy, createdBy.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("The config creator must not contain leading or trailing whitespace.", nameof(createdBy));
        }

        if (!string.Equals(reason, reason.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("The config reason must not contain leading or trailing whitespace.", nameof(reason));
        }

        ConfigId = configId;
        Version = version;
        SchemaVersion = schemaVersion;
        Status = status;
        CreatedAtUtc = createdAtUtc;
        CreatedBy = createdBy;
        Reason = reason;
        ContentHash = contentHash;
    }

    public ConfigId ConfigId { get; }

    public int Version { get; }

    public int SchemaVersion { get; }

    public ConfigStatus Status { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public string CreatedBy { get; }

    public string Reason { get; }

    public ConfigContentHash ContentHash { get; }
}
