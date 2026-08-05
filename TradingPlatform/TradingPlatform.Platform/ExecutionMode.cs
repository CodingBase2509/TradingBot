namespace TradingPlatform.Platform;

public enum ExecutionMode
{
    Backtest = 1,
    Shadow = 2,
    SimulatedPaper = 3,
    BrokerPaper = 4,
    Live = 5,
}
