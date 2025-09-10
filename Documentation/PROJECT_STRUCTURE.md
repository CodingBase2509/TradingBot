# PROJECT_STRUCTURE.md — Solution- & Ordnerstruktur

**Stand:** 2025-09-08

Dieses Dokument beschreibt die **physische** Ordnerstruktur unter `src/` und `tests/`, die **logische** Struktur via **Solution-Folders**, sowie **Projekt-Referenzen**, **Pakete**, **Build-Settings** und **Testaufteilung**. Ziel: glasklare Architektur, austauschbare Integrationen, saubere CI.

---

## 1) Physische Ordnerstruktur

```
/src
  TradingBot.Core/
  TradingBot.Features/
  TradingBot.Labeling/
  TradingBot.Policy/
  TradingBot.Decision/
  TradingBot.Exchanges.Binance/
  TradingBot.Exchanges.Bitget/
  TradingBot.Backtest.Adapter/
  TradingBot.Persistence/
  TradingBot.Telemetry/
  TradingBot.Train/           # EXE
  TradingBot.Backtest/        # EXE
  TradingBot.Live/            # EXE

/tests
  TradingBot.Tests.Core/
  TradingBot.Core.Tests/
  TradingBot.Features.Tests/
  TradingBot.Labeling.Tests/
  TradingBot.Policy.Tests/
  TradingBot.Decision.Tests/
  TradingBot.Exchanges.Binance.Tests/
  TradingBot.Persistence.Tests/
  TradingBot.Backtest.Adapter.Tests/
  TradingBot.Backtest.Tests/          # optional Integration/E2E
  TradingBot.Live.Tests/              # optional Integration/Smoke
```

> **Hinweis:** Namen können angepasst werden. Wichtig ist die **Trennung** zwischen Domäne/Libs (unter `src/`) und Tests (unter `tests/`).

---

## 2) Logische Struktur (Solution-Folders)

In Visual Studio/Rider per *Add → New Solution Folder* anlegen und Projekte hineinziehen:

```
TradingBotBot.sln
├─ src/
│  ├─ Core & Contracts
│  │  └─ TradingBot.Core
│  ├─ ML
│  │  ├─ TradingBot.Features
│  │  ├─ TradingBot.Labeling
│  │  └─ TradingBot.Policy
│  ├─ Strategy
│  │  └─ TradingBot.Decision
│  ├─ Adapters
│  │  ├─ TradingBot.Exchanges.Binance
│  │  ├─ TradingBot.Exchanges.Bitget
│  │  └─ TradingBot.Backtest.Adapter
│  ├─ Infrastructure
│  │  ├─ TradingBot.Persistence
│  │  └─ TradingBot.Telemetry
│  └─ Apps
│     ├─ TradingBot.Train
│     ├─ TradingBot.Backtest
│     └─ TradingBot.Live
└─ tests/
   ├─ Shared
   │  └─ TradingBot.Tests.Core
   ├─ Unit
   │  ├─ TradingBot.Core.Tests
   │  ├─ TradingBot.Features.Tests
   │  ├─ TradingBot.Labeling.Tests
   │  ├─ TradingBot.Policy.Tests
   │  ├─ TradingBot.Decision.Tests
   │  ├─ TradingBot.Exchanges.Binance.Tests
   │  └─ TradingBot.Persistence.Tests
   └─ Integration
      ├─ TradingBot.Backtest.Adapter.Tests
      ├─ TradingBot.Backtest.Tests
      └─ TradingBot.Live.Tests
```

---

## 3) Projekt-Referenzen (Richtung & Regeln)

**Grundregel:** Abhängigkeiten gehen **nach unten** in die Domäne, nicht kreuz und quer.

```
TradingBot.Core            ← wird von allen anderen referenziert

TradingBot.Features        → TradingBot.Core
TradingBot.Labeling        → TradingBot.Core
TradingBot.Policy          → TradingBot.Core
TradingBot.Decision        → TradingBot.Core

TradingBot.Exchanges.*     → TradingBot.Core
TradingBot.Persistence     → TradingBot.Core
TradingBot.Telemetry       → TradingBot.Core

TradingBot.Backtest.Adapter→ TradingBot.Core, (optional) TradingBot.Decision, TradingBot.Policy, TradingBot.Features

TradingBot.Train           → TradingBot.Core, TradingBot.Features, TradingBot.Labeling, TradingBot.Policy, TradingBot.Persistence, TradingBot.Telemetry
TradingBot.Backtest        → TradingBot.Core, TradingBot.Backtest.Adapter, TradingBot.Decision, TradingBot.Policy, TradingBot.Features, TradingBot.Telemetry
TradingBot.Live            → TradingBot.Core, TradingBot.Features, TradingBot.Policy, TradingBot.Decision, TradingBot.Exchanges.*, TradingBot.Persistence, TradingBot.Telemetry
```

**Don’ts:**
- `TradingBot.Decision` darf **nicht** `TradingBot.Exchanges.*` referenzieren.
- `TradingBot.Policy` darf **nicht** `TradingBot.Persistence` referenzieren.
- `TradingBot.Features` darf **nicht** `TradingBot.Train` referenzieren.

---

## 4) Paket- & Build-Management

### 4.1 `Directory.Build.props` (global für Solution)

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Deterministic>true</Deterministic>
    <LangVersion>preview</LangVersion>
    <ContinuousIntegrationBuild>true</ContinuousIntegrationBuild>
  </PropertyGroup>

  <ItemGroup>
    <!-- Analyser/Tools -->
    <PackageReference Include="Microsoft.CodeAnalysis.NetAnalyzers" Version="8.*" PrivateAssets="all" />
    <PackageReference Include="coverlet.collector" Version="6.*" PrivateAssets="all" />
  </ItemGroup>
</Project>
```

### 4.2 `Directory.Packages.props` (zentrale Paketversionen)

```xml
<Project>
  <ItemGroup>
    <PackageVersion Include="Microsoft.ML" Version="3.*" />
    <PackageVersion Include="Microsoft.ML.LightGbm" Version="3.*" />
    <PackageVersion Include="Skender.Stock.Indicators" Version="2.*" />
    <PackageVersion Include="MathNet.Numerics" Version="5.*" />
    <PackageVersion Include="Parquet.Net" Version="4.*" />
    <PackageVersion Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.*" />
    <PackageVersion Include="Serilog.AspNetCore" Version="8.*" />
    <PackageVersion Include="Serilog.Sinks.PostgreSQL" Version="5.*" />
    <PackageVersion Include="OpenTelemetry.Exporter.Prometheus" Version="1.*" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.*" />
    <PackageVersion Include="FluentAssertions" Version="6.*" />
    <PackageVersion Include="Moq" Version="4.*" />
    <PackageVersion Include="xunit" Version="2.*" />
    <PackageVersion Include="xunit.runner.visualstudio" Version="2.*" />
  </ItemGroup>
</Project>
```

> Vorteil: Versionen einmal zentral pflegen; alle `.csproj` können `<PackageReference Update="..."/>` verwenden.

### 4.3 `.editorconfig` (empfohlen, kurz)

```ini
root = true
[*.cs]
dotnet_style_qualification_for_field = true:suggestion
dotnet_diagnostic.CA2007.severity = warning
end_of_line = lf
indent_style = space
indent_size = 2
```

---

## 5) Gemeinsame Testbasis (`TradingBot.Tests.Core`)

**Inhalte:**
- `TestBase` mit gemeinsamen `Fixture`s (Clock, RNG, Config)
- **Builder**: `BarBuilder`, `FeatureRowBuilder`, `WorldStateBuilder`
- **Stubs/Mocks**: `CostEstimatorStub`, `OrderServiceFake`, `KlineStreamFake`
- Test-Helfer: `AlmostEqual(decimal a,b, decimals=6)`; Random-Daten via Bogus/AutoFixture

**NuGets:**
- `FluentAssertions`, `Moq` (oder NSubstitute), `xunit`, `Bogus`

**Konventionen:**
- Namensschema `Method_Should_DoX_When_Y`
- `Category`/`Trait` = `Unit` vs `Integration`
- Coverage via `dotnet test --collect:"XPlat Code Coverage"`

---

## 6) Solution-Referenzen (Beispiel `dotnet` CLI)

```bash
dotnet new sln -n TradingBotBot
# Libraries
dotnet new classlib -n TradingBot.Core -o src/TradingBot.Core
dotnet new classlib -n TradingBot.Features -o src/TradingBot.Features
dotnet new classlib -n TradingBot.Labeling -o src/TradingBot.Labeling
dotnet new classlib -n TradingBot.Policy -o src/TradingBot.Policy
dotnet new classlib -n TradingBot.Decision -o src/TradingBot.Decision
dotnet new classlib -n TradingBot.Exchanges.Binance -o src/TradingBot.Exchanges.Binance
dotnet new classlib -n TradingBot.Exchanges.Bitget -o src/TradingBot.Exchanges.Bitget
dotnet new classlib -n TradingBot.Backtest.Adapter -o src/TradingBot.Backtest.Adapter
dotnet new classlib -n TradingBot.Persistence -o src/TradingBot.Persistence
dotnet new classlib -n TradingBot.Telemetry -o src/TradingBot.Telemetry

# Apps
dotnet new console -n TradingBot.Train -o src/TradingBot.Train
dotnet new console -n TradingBot.Backtest -o src/TradingBot.Backtest
dotnet new worker  -n TradingBot.Live -o src/TradingBot.Live

# Tests
dotnet new xunit -n TradingBot.Tests.Core -o tests/TradingBot.Tests.Core
dotnet new xunit -n TradingBot.Features.Tests -o tests/TradingBot.Features.Tests
# ... (weitere)

# Projekte zur Solution
for p in src/*/*.csproj tests/*/*.csproj; do dotnet sln TradingBotBot.sln add "$p"; done
```

---

## 7) Minimal-Interfaces (Kernelemente)

```csharp
// src/TradingBot.Core
public record Candle(DateTimeOffset Ts, string Symbol, decimal Open, decimal High, decimal Low, decimal Close, decimal Volume, bool IsClosed);
public record BookTick(DateTimeOffset Ts, string Symbol, decimal Bid, decimal Ask);

public interface ICandlesStream { IAsyncEnumerable<Candle> StreamAsync(string symbol, string interval, CancellationToken ct); }
public interface IBookTickerStream { IAsyncEnumerable<BookTick> StreamAsync(string symbol, CancellationToken ct); }
public interface IMarketDataRest { Task<IReadOnlyList<Candle>> GetKlinesAsync(string symbol, string interval, DateTimeOffset from, DateTimeOffset to, CancellationToken ct); }

public record FeatureRow(string Symbol, DateTimeOffset Ts, IReadOnlyDictionary<string,double> Features);
public interface IFeatureEngine { FeatureRow Compute(IReadOnlyList<Candle> window, BookTick? tick, CancellationToken ct); }

public interface IPolicyModel { float Score(FeatureRow row); } // lädt model.zip
public record OrderIntent(
  string Symbol, string Side, string EntryType,
  decimal? TriggerPrice, decimal? LimitPrice, decimal Quantity,
  decimal? StopLossPrice, decimal? TakeProfitPrice,
  double Confidence, string ReasonCode
);
public interface IDecisionService { OrderIntent? Decide(FeatureRow row, BookTick costs, CancellationToken ct); }

public interface IOrderService {
  Task<Guid> PlaceParentAsync(OrderIntent intent, CancellationToken ct);
  Task PlaceOcoBracketAsync(Guid parentOrderId, decimal stop, decimal take, CancellationToken ct);
  Task CancelAsync(Guid orderId, CancellationToken ct);
}
```

---

## 8) Konfiguration (pro App)

`appsettings.json` Beispiel (gekürzt):

```json
{
  "exchange": "Binance",
  "symbols": ["BTCUSDT","ADAUSDT","XRPUSDT"],
  "interval": "15m",
  "lookbackDays": 45,
  "model": { "path": "models/policy-2025-09-08/model.zip", "config": "models/policy-2025-09-08/config.json" },
  "risk": { "riskPct": 0.0075, "rrMinCosted": 1.5, "costRatioMax": 0.15 },
  "db": { "conn": "Host=localhost;Username=bot;Password=secret;Database=TradingBot" }
}
```

**Secrets/Prod:** via Umgebungsvariablen/Secret-Store (kein Plaintext im Repo).

---

## 9) CI/CD (knapp angerissen)

**GitHub Actions (Beispiel):**
- Job **build-and-test**: `dotnet restore/build/test`, Coverage hochladen
- Job **pack-train**: Trainings-EXE builden (optional Docker), Artefakte speichern
- Job **publish-live**: Live-Worker als Docker-Image bauen und pushen

**Docker-Hinweise:**
- Drei Dockerfiles (`Train`, `Backtest`, `Live`)
- Multi-stage: `sdk` → `runtime`
- Healthcheck im `Live`-Container (WS/DB/Model geladen)

---

## 10) Qualitätsleitplanken

- Keine Feature-Berechnung auf „laufender“ Bar → nur **Closed Bars**
- **Walk-Forward** für Training/Eval; kein Shuffle
- **R:R nach Kosten ≥ 1.5** hart im Decision-Layer
- **Logging & Telemetry** von jeder Decision/Order/Fill
- **Feature-/Decision-Config** versionieren; Model-Artefakte eindeutig benennen
- **Rollback** jederzeit möglich (alte `model.zip` + `config.json`)