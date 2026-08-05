using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.Instruments.Catalog;

public interface IInstrumentCatalog
{
    Task<Instrument?> GetAsync(
        InstrumentId id,
        CancellationToken cancellationToken = default);

    Task<Instrument?> GetAsync(
        ProviderSymbol symbol,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Instrument>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
