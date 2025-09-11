using Trading.Core.Features;

namespace Trading.Foundation.Features;

/// <summary>
/// Immutable name-to-index lookup for a given <see cref="FeatureSchema"/>.
/// Built once, then used for fast index resolution in hot paths.
/// </summary>
public sealed class FeatureSchemaIndex
{
    private readonly FeatureSchema schema;
    private readonly Dictionary<string, int> nameToIndex;

    /// <summary>
    /// Creates the index for the provided schema.
    /// </summary>
    /// <param name="schema">Feature schema with a stable name order.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="schema"/> is null.</exception>
    public FeatureSchemaIndex(FeatureSchema schema)
    {
        this.schema = schema ?? throw new ArgumentNullException(nameof(schema));
        nameToIndex = new Dictionary<string, int>(schema.Names.Count, StringComparer.Ordinal);
        for (var i = 0; i < schema.Names.Count; i++)
            nameToIndex[schema.Names[i]] = i;
    }

    /// <summary>Gets the number of features.</summary>
    public int Count => schema.Names.Count;

    /// <summary>
    /// Tries to get the index for a feature name.
    /// </summary>
    /// <param name="name">Feature name (case-sensitive, ordinal).</param>
    /// <param name="index">Out: the zero-based index if found.</param>
    /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
    public bool TryGetIndex(string name, out int index) =>
        nameToIndex.TryGetValue(name, out index);

    /// <summary>
    /// Gets the index for a feature name or throws if it does not exist.
    /// </summary>
    /// <param name="name">Feature name (case-sensitive, ordinal).</param>
    /// <returns>Zero-based index into the schema.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if <paramref name="name"/> is not defined.</exception>
    public int GetIndexOrThrow(string name) =>
        nameToIndex.TryGetValue(name, out var index)
            ? index
            : throw new KeyNotFoundException($"Feature '{name}' is not defined in the schema.");
}