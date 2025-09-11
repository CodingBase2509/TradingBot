namespace Trading.Core.Abstractions;

/// <summary>Writes data rows into Parquet partitions (e.g., per year/symbol).</summary>
public interface IParquetSink<T>
{
    Task WriteAsync(string path, IReadOnlyList<T> rows, CancellationToken ct = default);
}