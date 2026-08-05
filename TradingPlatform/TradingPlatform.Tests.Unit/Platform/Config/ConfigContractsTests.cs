using TradingPlatform.Platform.Config;

namespace TradingPlatform.Tests.Unit.Platform.Config;

public sealed class ConfigContractsTests
{
    private const string Sha256Hash = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";

    [Fact]
    public void ConfigIdCreatesANonEmptyUuidVersionSeven()
    {
        var configId = ConfigId.Create();

        Assert.NotEqual(Guid.Empty, configId.Value);
        Assert.Equal(7, configId.Value.Version);
    }

    [Fact]
    public void ConfigIdRejectsAnEmptyGuid()
    {
        Assert.Throws<ArgumentException>(() => new ConfigId(Guid.Empty));
    }

    [Fact]
    public void ContentHashNormalizesHexadecimalCharacters()
    {
        var hash = new ConfigContentHash(Sha256Hash.ToUpperInvariant());

        Assert.Equal(Sha256Hash, hash.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("gggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggggg")]
    public void ContentHashRejectsInvalidSha256Values(string value)
    {
        Assert.Throws<ArgumentException>(() => new ConfigContentHash(value));
    }

    [Fact]
    public void MetadataAcceptsACompleteConfigVersion()
    {
        var metadata = CreateMetadata();

        Assert.Equal(1, metadata.Version);
        Assert.Equal(1, metadata.SchemaVersion);
        Assert.Equal(ConfigStatus.Draft, metadata.Status);
        Assert.Equal("user:test", metadata.CreatedBy);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MetadataRejectsInvalidConfigVersions(int version)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateMetadata(version: version));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void MetadataRejectsInvalidSchemaVersions(int schemaVersion)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateMetadata(schemaVersion: schemaVersion));
    }

    [Theory]
    [InlineData(ConfigStatus.Unknown)]
    [InlineData((ConfigStatus)999)]
    public void MetadataRejectsUnknownStatuses(ConfigStatus status)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateMetadata(status: status));
    }

    [Fact]
    public void MetadataRejectsNonUtcTimestamps()
    {
        var timestamp = new DateTimeOffset(2026, 8, 5, 12, 0, 0, TimeSpan.FromHours(2));

        Assert.Throws<ArgumentException>(() => CreateMetadata(createdAtUtc: timestamp));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" user:test")]
    [InlineData("user:test ")]
    public void MetadataRejectsInvalidCreatorValues(string createdBy)
    {
        Assert.Throws<ArgumentException>(() => CreateMetadata(createdBy: createdBy));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" initial version")]
    [InlineData("initial version ")]
    public void MetadataRejectsInvalidReasons(string reason)
    {
        Assert.Throws<ArgumentException>(() => CreateMetadata(reason: reason));
    }

    private static ConfigVersionMetadata CreateMetadata(
        int version = 1,
        int schemaVersion = 1,
        ConfigStatus status = ConfigStatus.Draft,
        DateTimeOffset? createdAtUtc = null,
        string createdBy = "user:test",
        string reason = "Initial version") => new(
            ConfigId.Create(),
            version,
            schemaVersion,
            status,
            createdAtUtc ?? new DateTimeOffset(2026, 8, 5, 12, 0, 0, TimeSpan.Zero),
            createdBy,
            reason,
            new ConfigContentHash(Sha256Hash));
}
