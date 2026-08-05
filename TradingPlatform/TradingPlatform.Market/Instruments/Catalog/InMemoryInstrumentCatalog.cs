using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.Instruments.Catalog;

internal sealed class InMemoryInstrumentCatalog : IInstrumentCatalog
{
    private readonly IReadOnlyList<Instrument> _instruments;
    private readonly Dictionary<InstrumentId, Instrument> _instrumentsById = new();
    private readonly Dictionary<ProviderSymbol, Instrument> _instrumentsByProviderSymbol = new();

    public InMemoryInstrumentCatalog()
        : this([])
    {
    }

    public InMemoryInstrumentCatalog(IEnumerable<Instrument> instruments)
    {
        ArgumentNullException.ThrowIfNull(instruments);

        var snapshot = instruments.ToArray();
        if (snapshot.Any(instrument => instrument is null))
        {
            throw new ArgumentException("The catalog must not contain null instruments.", nameof(instruments));
        }

        foreach (var instrument in snapshot)
        {
            if (!_instrumentsById.TryAdd(instrument.Id, instrument))
            {
                throw new ArgumentException(
                    $"The instrument ID '{instrument.Id}' is assigned more than once.",
                    nameof(instruments));
            }

            foreach (var providerSymbol in instrument.ProviderSymbols)
            {
                if (!_instrumentsByProviderSymbol.TryAdd(providerSymbol, instrument))
                {
                    throw new ArgumentException(
                        $"The provider symbol '{providerSymbol.Provider}:{providerSymbol.Symbol}' is assigned more than once.",
                        nameof(instruments));
                }
            }
        }

        _instruments = Array.AsReadOnly(snapshot);
    }

    public Task<Instrument?> GetAsync(
        InstrumentId id,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);
        cancellationToken.ThrowIfCancellationRequested();

        _instrumentsById.TryGetValue(id, out var instrument);
        return Task.FromResult(instrument);
    }

    public Task<Instrument?> GetAsync(
        ProviderSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(symbol);
        cancellationToken.ThrowIfCancellationRequested();

        _instrumentsByProviderSymbol.TryGetValue(symbol, out var instrument);
        return Task.FromResult(instrument);
    }

    public Task<IReadOnlyList<Instrument>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_instruments);
    }
}
