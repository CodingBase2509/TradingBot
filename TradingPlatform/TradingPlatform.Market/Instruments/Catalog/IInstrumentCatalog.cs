using TradingPlatform.Market.Instruments.Providers;
using TradingPlatform.Platform.Identifiers;

namespace TradingPlatform.Market.Instruments.Catalog;

public interface IInstrumentCatalog
{
    Task<InstrumentDefinition?> GetAsync(
        InstrumentId id,
        CancellationToken cancellationToken = default);

    Task<InstrumentDefinition?> GetAsync(
        ProviderSymbol symbol,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<InstrumentDefinition>> GetAllAsync(
        CancellationToken cancellationToken = default);
}
