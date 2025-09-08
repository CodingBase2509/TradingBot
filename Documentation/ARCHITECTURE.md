# Architektur & Solution‑Struktur

## Laufende Anwendungen (3 EXEs)
- **Trading.Train (Console):** Backfill → Features → Labeling → Train (ML.NET) → Artefakte (`model.zip`, `config.json`, `metrics.json`)
- **Trading.Backtest (Console/LEAN):** realistische Backtests (Fees/Spread/Slippage/Partial Fills) mit denselben DLLs
- **Trading.Live (WorkerService):** Echtzeit‑Stream (WS), Features (auf Closed Bars), Score, Decision, Orders, Telemetry

## Libraries
- **Trading.Core:** DTOs, Enums, Interfaces (keine Abhängigkeiten)
- **Trading.Features:** Indikatoren, Trendkanal, S/R‑Clustering, Fibo, Kosten
- **Trading.Labeling:** Triple‑Barrier/Playbook‑Labeling (nur Training)
- **Trading.Policy:** ML.NET‑Loader/Scorer (Inference, lädt `model.zip`)
- **Trading.Decision:** R:R≥1.5, SL/TP‑Kandidaten, Sizing, Caps, Kill‑Switch
- **Trading.Exchanges.(Binance|Bitget):** REST/WS‑Adapter, austauschbar
- **Trading.Backtest.Adapter:** Bridge zu LEAN
- **Trading.Persistence:** EF Core (Postgres), Repositories
- **Trading.Telemetry:** Serilog/OpenTelemetry‑Setup

## Solution‑Folders (logisch)
```
src/
  Core & Contracts
    Trading.Core
  ML
    Trading.Features
    Trading.Labeling
    Trading.Policy
  Strategy
    Trading.Decision
  Adapters
    Trading.Exchanges.Binance
    Trading.Exchanges.Bitget
    Trading.Backtest.Adapter
  Infrastructure
    Trading.Persistence
    Trading.Telemetry
  Apps
    Trading.Train
    Trading.Backtest
    Trading.Live
tests/
  Shared
    Trading.Tests.Core
  Unit
    <Project>.Tests
  Integration
    <Project>.IntegrationTests
```

## Event‑Flow (vereinfacht)
```
WS: Klines(15m) + BookTicker ──▶ Ingestor ──▶ RingBuffer(30–45d) ──▶ FeatureEngine
                                         │
                                         └─▶ CostFeed(Spread/Fees/Slippage)
                                                     │
Closed Bar ──▶ Policy.Score(p) ──▶ Decision (R:R≥1.5, SL/TP, Size) ──▶ Execution(REST/WS, OCO)
                                                                                │
                                                                      Telemetry (Postgres/Grafana)
```

## Austauschbarkeit (Exchanges/Produkte)
- Interfaces in `Trading.Core` (z. B. `ICandlesStream`, `IBookTickerStream`, `IMarketDataRest`, `IOrderService`)
- DI/Config schaltet zwischen **Binance** und **Bitget**, **Spot**/ **Perps**
