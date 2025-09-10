# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

Project: KI‑Trading‑Bot (Crypto, 15‑min)

Commands
- Prereqs: Install .NET SDK 9.x (see Directory.Build.props). Solution file: TradingBot.sln
- Restore and build all projects:
  - dotnet restore TradingBot.sln
  - dotnet build TradingBot.sln -c Debug --nologo
- Run tests with coverage (when tests exist):
  - dotnet test TradingBot.sln --collect:"XPlat Code Coverage" -v minimal
- Run a single test project (example):
  - dotnet test tests/Trading.Features.Tests/Trading.Features.Tests.csproj -v minimal
- Run a single test by filter (example):
  - dotnet test TradingBot.sln --filter "FullyQualifiedName~Trading.Features.Tests.Indicators.RsiTests"
- Create solution and scaffold projects (if src/tests are not yet created):
  - See Documentation/PROJECT_STRUCTURE.md section 6) for the full script. Quick start snippet:
    - dotnet new sln -n TradingBot
    - dotnet new classlib -n Trading.Core -o src/Trading.Core
    - dotnet new xunit -n Trading.Tests.Core -o tests/Trading.Tests.Core
    - for p in src/*/*.csproj tests/*/*.csproj; do dotnet sln TradingBot.sln add "$p"; done
- Lint/analyzers: Roslyn analyzers are enabled via Microsoft.CodeAnalysis.NetAnalyzers in Directory.Build.props. TreatWarningsAsErrors=true. Use:
  - dotnet build -warnaserror
- Docker (placeholders exist): docker/Dockerfile.train, docker/Dockerfile.backtest, docker/Dockerfile.live and docker-compose.yml are present but currently empty.

Architecture (big picture)
- Executables (3 apps)
  - Trading.Train: Offline pipeline → backfill, feature generation, labeling, ML.NET training → artifacts: model.zip, config.json, metrics.json
  - Trading.Backtest: Realistic backtests (fees/spread/slippage/partial fills) using the same libraries
  - Trading.Live: Live 15‑min loop: ingest streams, compute features on closed bars, score policy, decide, execute orders, emit telemetry
- Libraries (layered)
  - Trading.Core: Core contracts/DTOs/interfaces with no external deps
  - Trading.Features: Indicator and structure features (RSI/ATR/MACD/Bollinger/Donchian/EMA‑Ribbon/VWAP, trend channel, S/R clustering, Fibonacci, cost features)
  - Trading.Labeling: Triple‑barrier / playbook labeling for training only
  - Trading.Policy: ML.NET model loader/scorer (inference only; loads versioned model.zip)
  - Trading.Decision: Deterministic risk/decision layer (R:R after costs ≥ 1.5, SL/TP candidates, position sizing, kill‑switch)
  - Trading.Exchanges.(Binance|Bitget): Exchange adapters (REST/WS); pluggable via DI/config
  - Trading.Backtest.Adapter: Bridge to the backtest engine (e.g., LEAN)
  - Trading.Persistence: EF Core for Postgres; repositories for decisions/orders/fills/outcomes
  - Trading.Telemetry: Serilog + OpenTelemetry setup
- Solution folders (logical; map to src/tests)
  - src/: Core & Contracts; ML; Strategy; Adapters; Infrastructure; Apps
  - tests/: Shared (Trading.Tests.Core), Unit (<Project>.Tests), Integration (<Project>.IntegrationTests)

Runtime flow (from Documentation)
- WS: kline_15m + bookTicker → Ingestor → RingBuffer (30–45 days) → FeatureEngine
- On closed bar: Policy.Score(p) → Decision (R:R≥1.5, SL/TP, size) → Execution (REST/WS, OCO)
- Telemetry: Postgres + Grafana; logs/metrics via Serilog/OpenTelemetry

Data/ML key notes
- Offline training only. Walk‑forward splits by time; LightGBM (binary) via ML.NET. Model artifacts are versioned (e.g., policy‑YYYY‑MM‑DD) and loaded by Trading.Policy at runtime.
- Features and labeling are configured via a versioned config.json stored alongside model.zip; R:R thresholds and cost constraints enforced in Decision.

Configuration
- Each app uses appsettings.json (see Documentation/PROJECT_STRUCTURE.md §8 for example). Secrets via environment variables or secret stores; do not commit secrets.
- Exchange choice (Binance/Bitget), symbols, interval (15m), lookbackDays (30–45), model paths, risk params, and DB connection are configuration‑driven.

Storage & Telemetry
- Historical/features: Parquet
- Relational: PostgreSQL (+ TimescaleDB optional) for decisions, orders, fills, outcomes
- Dashboards: Grafana

Getting productive in this repo
- Start by creating the src/ and tests/ projects per Documentation/PROJECT_STRUCTURE.md if they are not yet present.
- Keep project dependencies pointing inward to domain libraries (see dependency rules in Documentation/PROJECT_STRUCTURE.md §3). Do not reference adapters from decision/policy layers.
- Enforce closed‑bar feature computation in live loop and uphold R:R after costs ≥ 1.5 in Decision.


Coding Standards
- Use async/await for all I/O and network-bound work; avoid blocking calls.
- Treat analyzer warnings as errors (already enabled). Keep the build clean.
- Prefer pure/functional helpers in Features; keep side effects in Apps/Adapters.
- Log with Serilog at Information for lifecycle, Debug for details; never log secrets.
- Public APIs: XML doc comments; internal where possible.
- Unit tests target >80% coverage for core logic; focus on Decision/Policy edge cases.

Debugging & Troubleshooting
- Dry-run live app without placing orders:
  - dotnet run --project src/Trading.Live -- --dry-run
- Run a single test by trait or FQN (examples in Commands section above).
- Check logs for runtime issues (configure sinks via appsettings.json). Consider writing to ./logs/ locally during development.
- Validate exchange connectivity with sandbox keys before live trading.

Development Workflow
- Branching: feature/<short-name>, fix/<short-name> from main.
- Open PRs with a short rationale and include relevant metrics for trading changes.
- CI suggestions: restore, build with -warnaserror, run tests with coverage, and publish artifacts (model.zip, metrics.json) on training PRs.

AI Agent Profiles
- DefaultProfile: Code Buddy
- Profiles:
  - Code Buddy:
    - Goal: Be a concise, code-first .NET assistant optimized for this repository with expert knowledge in ML.NET, ASP.NET, and financial market indicators.
    - Behavior:
      - Prefer making changes via diffs; keep edits minimal, scoped, and reversible.
      - When context is missing, ask 1-2 targeted questions; otherwise proceed with sensible defaults.
      - For terminal actions: use non-interactive, non-paged commands, include --no-pager for git, avoid cd unless necessary.
      - For coding tasks: adhere to existing patterns and Directory.Build.props analyzers; keep public surface minimal.
      - Security: never request or print secrets; use environment variables if a secret is needed.
      - Formatting: follow the required code block metadata (language, path, start) when showing code.
      - Version control: do not commit/push without explicit approval; suggest next steps instead.
      - .NET conventions: enable nullable, use records/structs appropriately, prefer DI for services, keep adapters behind interfaces in Trading.Core.
      - Testing: add or update tests when code changes affect public behavior.
    - Expert Knowledge Areas:
      - ML.NET: Binary classification, regression, time-series forecasting, model training pipelines, feature engineering, cross-validation, model evaluation metrics (AUC, precision/recall), AutoML, LightGBM integration, model serialization/versioning.
      - ASP.NET Core: Web APIs, dependency injection, configuration patterns, middleware, authentication/authorization, hosted services, health checks, logging, OpenAPI/Swagger integration.
      - Technical Indicators: RSI, MACD, Bollinger Bands, ATR, EMA/SMA, Stochastic, Williams %R, Fibonacci retracements, support/resistance levels, volume indicators (VWAP, OBV), momentum oscillators, trend-following vs. mean-reversion strategies.
      - Financial Markets: Order types (market, limit, stop-loss, take-profit, OCO), bid/ask spreads, slippage, position sizing, risk management (R:R ratios), portfolio theory, backtesting methodologies, walk-forward analysis.
      - Time Series: Candlestick patterns, price action analysis, multi-timeframe analysis, seasonality detection, statistical arbitrage concepts, feature lagging, rolling windows, resampling.
    - When uncertain: propose a brief plan, then ask "Proceed?" before large changes.
