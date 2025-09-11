using Trading.Core.Features;

namespace Trading.Foundation.Features;

/// <summary>
/// Utilities to build dense feature vectors (double[]) in the order defined by a <see cref="FeatureSchema"/>.
/// </summary>
public static class FeatureVectorBuilder
{
    /// <summary>
    /// Builds a new vector by looking up each schema name in <paramref name="source"/>.
    /// </summary>
    /// <param name="schema">Feature schema (defines order).</param>
    /// <param name="source">Name->value map.</param>
    /// <param name="defaultValue">Value to use when a name is missing in <paramref name="source"/>.</param>
    /// <returns>Newly allocated vector with length <c>schema.Names.Count</c>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="schema"/> or <paramref name="source"/> is null.</exception>
    public static double[] BuildVector(
        FeatureSchema schema,
        IReadOnlyDictionary<string, double> source,
        double defaultValue = 0.0)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(source);

        var count = schema.Names.Count;
        var vector = new double[count];
        for (var i = 0; i < count; i++)
        {
            var name = schema.Names[i];
            vector[i] = source.GetValueOrDefault(name, defaultValue);
        }

        return vector;
    }

    /// <summary>
    /// Fills a caller-provided <paramref name="destination"/> in schema order using <paramref name="source"/>.
    /// </summary>
    /// <param name="schema">Feature schema (defines order).</param>
    /// <param name="source">Name->value map.</param>
    /// <param name="destination">Target span; length must be ≥ <c>schema.Names.Count</c>.</param>
    /// <param name="defaultValue">Value to use when a name is missing.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="schema"/> or <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentException">If <paramref name="destination"/> is too small.</exception>
    public static void FillVector(
        FeatureSchema schema,
        IReadOnlyDictionary<string, double> source,
        Span<double> destination,
        double defaultValue = 0.0)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(source);

        var count = schema.Names.Count;
        if (destination.Length < count)
            throw new ArgumentException("Destination span is too small.", nameof(destination));

        destination[..count].Fill(defaultValue);

        for (var i = 0; i < count; i++)
        {
            var name = schema.Names[i];
            if (source.TryGetValue(name, out var value))
                destination[i] = value;
        }
    }

    /// <summary>
    /// Fills a caller-provided <paramref name="destination"/> using a prebuilt <see cref="FeatureSchemaIndex"/>.
    /// </summary>
    /// <param name="schema">Feature schema (only used for count).</param>
    /// <param name="index">Name->index lookup for the schema.</param>
    /// <param name="source">Name→value map.</param>
    /// <param name="destination">Target span; length must be ≥ <c>schema.Names.Count</c>.</param>
    /// <param name="defaultValue">Value to use for positions not present in <paramref name="source"/>.</param>
    /// <exception cref="ArgumentNullException">If any argument except <paramref name="destination"/> is null.</exception>
    /// <exception cref="ArgumentException">If <paramref name="destination"/> is too small.</exception>
    public static void FillVector(
        FeatureSchema schema,
        FeatureSchemaIndex index,
        IReadOnlyDictionary<string, double> source,
        Span<double> destination,
        double defaultValue = 0.0)
    {
        ArgumentNullException.ThrowIfNull(schema);
        ArgumentNullException.ThrowIfNull(index);
        ArgumentNullException.ThrowIfNull(source);

        var count = schema.Names.Count;
        if (destination.Length < count)
            throw new ArgumentException("Destination span is too small.", nameof(destination));

        destination[..count].Fill(defaultValue);

        foreach (var pair in source)
        {
            if (index.TryGetIndex(pair.Key, out var i) && (uint)i < (uint)count)
                destination[i] = pair.Value;
        }
    }
}
