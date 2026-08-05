namespace TradingPlatform.Platform.Config;

public sealed record ConfigContentHash
{
    private const int Sha256HexLength = 64;

    public ConfigContentHash(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length != Sha256HexLength || value.Any(character => !char.IsAsciiHexDigit(character)))
        {
            throw new ArgumentException("A config content hash must be a SHA-256 value encoded as 64 hexadecimal characters.", nameof(value));
        }

        Value = value.ToLowerInvariant();
    }

    public string Value { get; }

    public override string ToString() => Value;
}
