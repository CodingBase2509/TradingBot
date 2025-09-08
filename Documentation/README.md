# KI‑Trading‑Bot (Krypto, 15‑Min) — Projektübersicht

**Stand:** 2025-09-08  
**Ziel:** Ein eventgetriebener, regelstabiler KI‑Trader für Krypto‑USDT‑Paare (zunächst **BTC/USDT, ADA/USDT, XRP/USDT**) auf **15‑Min‑Kerzen**.  
**Kernprinzip:** *Offline trainieren → Modell als Datei (`model.zip`) versionieren → Live nur inferieren*; Trades nur, wenn **Struktur + Kosten + R:R (nach Kosten) ≥ 1.5** stimmig sind.

## Leitplanken
- **Exchanges:** Start mit **Binance** (Testnet/Spot oder USD‑M Perps), **Bitget** später per austauschbarem Adapter.
- **Zeitfenster Training:** mind. 2017–2022; besser bis 2024/25. *Hinweis:* SOL nicht vor 2020.  
- **Timeframe Live:** 15m; *wenige Intraday‑Trades* erwünscht.
- **Lookback Live:** ~**30–45 Tage** (≈ 3k–4k Bars) im Ring‑Buffer für Kanal & S/R.
- **Speicher/Telemetry:** **Parquet** (History/Features), **PostgreSQL** (+ optional TimescaleDB) für Decisions/Orders/Fills/Outcomes, **Grafana** für Dashboards.
- **Nicht im MVP:** Meta‑Modell & DL‑News‑Verarbeitung (können später ergänzt werden).

## Bausteine
- **Feature‑Engine:** Indikatoren (RSI, ATR, MACD, Bollinger, Donchian, EMA‑Ribbon, VWAP/AVWAP) + **Trendkanal**, **S/R‑Clustering**, **Fibonacci**, **Kosten‑Features**.
- **Policy (ML.NET LightGBM):** Score *p ∈ [0,1]* „guter Entry?“. Training per Walk‑Forward.
- **Decision/Risk‑Layer (Code, kein ML):** strukturelle **SL/TP**, **R:R (costed) ≥ 1.5**, Kosten‑/Liquiditätsfilter, **risk‑basiertes Position‑Sizing**, Order‑Typwahl (Market/Stop/Limit), OCO‑Bracket.
- **Execution:** Exchange‑Adapter (Binance/Bitget) für REST/WS, Orders & Fills, Resync.
- **Observability:** Serilog + OpenTelemetry; Postgres‑Schema für Dashboards.

Siehe weitere Details in:  
- `ARCHITECTURE.md` — Solution‑Aufbau & austauschbare Integrationen  
- `DATA_AND_ML.md` — Features, Labeling, Training, Backtest, Model‑Lifecycle  
- `DECISION_AND_EXECUTION.md` — R:R‑Regeln, SL/TP, Playbooks, Position‑Sizing, Orders  
- `STORAGE_AND_TELEMETRY.md` — Parquet, Postgres‑Schema, EF Core, Grafana  
- `RUNTIME_GUIDE.md` — Live‑Loop, Streams, Lookback, Recovery

> **Hinweis/Disclaimer:** Keine Anlageberatung. Live‑Handel erst nach stabiler Paper‑Phase, realistischen Kosten & strengen Risk‑Limits.
