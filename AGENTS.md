# Repository Guidelines

>This guide standardizes structure, workflows, and conventions for this .NET 9 solution so changes stay easy to review and ship.

## Project Structure & Module Organization
- Source in `src/*` (libraries and apps):
  - Core/libs: `Trading.Core`, `Trading.Features`, `Trading.Labeling`, `Trading.Policy`, `Trading.Decision`, `Trading.Persistence`, `Trading.Telemetry`, adapters `Trading.Exchanges.*`, backtest adapter `Trading.Backtest.Adapter`.
  - Apps: `Trading.Train`, `Trading.Backtest`, `Trading.Live`.
- Tests mirror under `tests/*` with `*.Tests` projects and shared helpers in `tests/Trading.Tests.Core`.
- Docs in `Documentation/`. Central config: `Directory.Build.props` (net9.0, nullable, warnings-as-errors) and `Directory.Packages.props` (package versions).

## Build, Test, and Development Commands
- Restore/build: `dotnet restore` · `dotnet build -c Release`.
- Test (with coverage): `dotnet test -c Release --collect:"XPlat Code Coverage"`.
- Run apps: `dotnet run -p src/Trading.Backtest`, `dotnet run -p src/Trading.Train`, `dotnet run -p src/Trading.Live`.
- Docker: `docker build -f docker/Dockerfile.live -t trading-live .` · `docker compose up -d` (runs `live` + `postgres`).

## Coding Style & Naming Conventions
- C#/.NET: 4-space indent, LF line endings (`.editorconfig`). `Nullable` and `ImplicitUsings` enabled; treat warnings as errors.
- Namespaces mirror folders; projects use `Trading.*` prefix. Types/methods: PascalCase. Private fields: `_camelCase`. Files match primary type name.
- Use built-in analyzers (SDK). Optional: `dotnet format` before PRs.

## Testing Guidelines
- Frameworks: xUnit, FluentAssertions, Moq; coverage via `coverlet.collector` (already referenced for test projects).
- Place shared builders/fakes in `tests/Trading.Tests.Core`.
- Test names: `Method_Should_DoX_When_Y`. Run subsets with `dotnet test --filter "FullyQualifiedName~ClassOrNamespace"`.

## Commit & Pull Request Guidelines
- History favors short imperative messages. Prefer Conventional Commits when possible (`feat:`, `fix:`, `chore:`). Keep subjects ≤72 chars; reference issues (`#123`).
- PRs: clear description, linked issue, rationale, before/after notes, and updated/added tests. Ensure `dotnet test` passes locally. CI (`.github/workflows/ci.yml`) runs restore/build/test with coverage on PRs.

## Security & Configuration Tips
- Never commit secrets. Use environment variables or user-secrets; configure Postgres via env in `docker-compose.yml`.
- Version model/config artifacts as needed; avoid embedding credentials in configs.
- See `Documentation/PROJECT_STRUCTURE.md` and related docs for deeper architecture context.

## Architecture & Dependency Rules (from Documentation)
- Layers (libs):
  - `Trading.Core` (contracts, DTOs, interfaces) with no external deps.
  - `Trading.Features` (indicators, channels, S/R clustering, Fibonacci, cost features).
  - `Trading.Labeling` (triple-barrier/playbook labeling; training only).
  - `Trading.Policy` (ML.NET model loader/scorer; loads versioned `model.zip`).
  - `Trading.Decision` (deterministic risk/decision; enforces R:R after costs ≥ 1.5, SL/TP, sizing, kill-switch).
  - `Trading.Exchanges.(Binance|Bitget)` (REST/WS adapters; DI-switchable).
  - `Trading.Backtest.Adapter` (bridge to backtest engine).
  - `Trading.Persistence` (EF Core Postgres; repositories), `Trading.Telemetry` (Serilog + OpenTelemetry setup).
- Apps: `Trading.Train` (offline pipeline), `Trading.Backtest` (realistic backtests), `Trading.Live` (15m loop).
- Dependency rules (non-negotiable):
  - All depend inward on `Trading.Core` only; do not cross-reference sideways.
  - `Trading.Decision` must not reference `Trading.Exchanges.*`.
  - `Trading.Policy` must not reference `Trading.Persistence`.
  - `Trading.Features` must not reference any app (`Trading.Train`/`Backtest`/`Live`).

## Runtime Flow (Live)
- Streams: WebSocket `kline_15m` (with isFinal) + `bookTicker` (bid/ask → spread); REST backfill on startup and resyncs.
- Warm-up: load 30–45 days of history; gate signals until all features are warm (`HasWarmup==true`).
- On closed bar only:
  1) Compute feature vector
  2) Score via `Trading.Policy` (load model.zip)
  3) Decide in `Trading.Decision` (SL/TP from structure, R:R costed ≥ 1.5, cost filter, risk sizing, order type)
  4) Place orders (Market/Stop/Limit) and attach OCO after fill
  5) Intrabar: triggers, amend/cancel on structure break, spread guard
  6) Telemetry: decisions/orders/fills/outcomes → Postgres; update logs/metrics

## Data & ML (key requirements)
- Offline training only (in `Trading.Train`); walkthrough in `Documentation/DATA_AND_ML.md`.
- Features per closed bar: RSI/MACD/ATR/Bollinger/Donchian, EMA ribbon, regression channel (30d), S/R clustering, Fibonacci, cost/liquidity features.
- Labeling (training): SL below swing with ATR buffer, realistic TP (next R/channel top or ≤3×ATR), horizon check (e.g., 48 bars). Keep only candidates with R:R (costed) ≥ 1.5.
- Model: ML.NET LightGBM (binary). Walk-forward time splits; evaluate with AUC/LogLoss plus backtest PnL/Sharpe/MaxDD after costs.
- Artifacts and config: versioned `model.zip` + `config.json` + `metrics.json` (e.g., `policy-YYYY-MM-DD`). `Trading.Policy` loads by version at runtime.

## Storage & Telemetry
- Historical/features: Parquet files (typed schemas) for backfill/training/backtests.
- Live OLTP: PostgreSQL for `strategy_decision`, `orders`, `fills`, `trades_outcome` (see `Documentation/STORAGE_AND_TELEMETRY.md`).
- Observability: Serilog (structured logs; enrich with `trade_id`, `model_version`) and OpenTelemetry (metrics/traces). Dashboards via Grafana (e.g., Daily Net PnL, Trades table queries in docs).
- Retention: keep 6–12 months hot in OLTP; roll older events to Parquet; backup DB snapshots + Parquet.

## Configuration (apps)
- Each app uses `appsettings.json` with keys: `exchange`, `symbols`, `interval` (`15m`), `lookbackDays` (30–45), model paths (`model.path`, `model.config`), `risk` (e.g., `riskPct`, `rrMinCosted`, `costRatioMax`), and `db.conn`.
- Use environment variables or secret stores for credentials; never commit secrets.

## Commands & Scaffolding (from WARP.md)
- Restore/build/test: `dotnet restore TradingBot.sln` · `dotnet build -c Debug --nologo` · `dotnet test --collect:"XPlat Code Coverage" -v minimal`.
- Run a single test project: `dotnet test tests/Trading.Features.Tests/Trading.Features.Tests.csproj -v minimal`.
- Filter a single test: `dotnet test TradingBot.sln --filter "FullyQualifiedName~NamespaceOrClass.TestName"`.
- Quick scaffold (see `Documentation/PROJECT_STRUCTURE.md §6` for full script): create `src/*` and `tests/*`, add to solution with `for p in src/*/*.csproj tests/*/*.csproj; do dotnet sln TradingBot.sln add "$p"; done`.
- Analyzers: TreatWarningsAsErrors is on; use `dotnet build -warnaserror`.
- Docker placeholders exist under `docker/` for `train/backtest/live` and `docker-compose.yml` (wire Postgres + live app).

## Coding Standards (addendum)
- Prefer async/await for all I/O and network; avoid blocking calls.
- Keep analyzer warnings at zero; treat as errors (already enforced by SDK/net9).
- Keep `Trading.Features` largely pure; side effects live in apps/adapters.
- Logging: Serilog Information for lifecycle, Debug for details; never log secrets.
- Public APIs with XML docs where applicable; internal visibility where possible.
- Tests: prioritize Decision/Policy edge cases; aim ≥80% coverage for core logic.

## Debugging & Troubleshooting
- Dry-run live without placing orders: `dotnet run --project src/Trading.Live -- --dry-run`.
- Inspect specific tests via filters. Write local logs to `./logs/` during dev if helpful.
- Validate exchange connectivity with sandbox keys before live trading.

## Development Workflow
- Branches: `feature/<name>`, `fix/<name>` from `main`.
- PRs: short rationale; include trading metrics for strategy changes. CI should restore/build with `-warnaserror`, run tests with coverage, and (optionally) publish training artifacts.

## Agent Behavior Profile
- Be concise and code-first; prefer minimal, reversible diffs respecting existing patterns and analyzers.
- When context is missing: ask 1–2 targeted questions; otherwise proceed with sensible defaults.
- Terminal: use non-interactive, non-paged commands; avoid `cd` unless necessary.
- Do not commit/push without explicit approval; propose next steps.
- Adhere to the dependency rules, runtime flow (closed bars only), and enforce Decision’s R:R after costs ≥ 1.5 in any related code.
