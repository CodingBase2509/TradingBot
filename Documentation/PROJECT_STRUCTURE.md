# PROJECT_STRUCTURE.md — Solution- & Ordnerstruktur

**Stand:** 2025-09-08

Dieses Dokument beschreibt die **physische** Ordnerstruktur unter `src/` und `tests/`, die **logische** Struktur via **Solution-Folders**, sowie **Projekt-Referenzen**, **Pakete**, **Build-Settings** und **Testaufteilung**. Ziel: glasklare Architektur, austauschbare Integrationen, saubere CI.

---

## 1) Physische Ordnerstruktur

```
/src
  Trading.Core/
  Trading.Features/
  Trading.Labeling/
  Trading.Policy/
  Trading.Decision/
  Trading.Exchanges.Binance/
  Trading.Exchanges.Bitget/
  Trading.Backtest.Adapter/
  Trading.Persistence/
  Trading.Telemetry/
  Trading.Train/           # EXE
  Trading.Backtest/        # EXE
  Trading.Live/            # EXE

/tests
  Trading.Tests.Core/
  Trading.Core.Tests/
  Trading.Features.Tests/
  Trading.Labeling.Tests/
  Trading.Policy.Tests/
  Trading.Decision.Tests/
  Trading.Exchanges.Binance.Tests/
  Trading.Persistence.Tests/
  Trading.Backtest.Adapter.Tests/
  Trading.Backtest.Tests/          # optional Integration/E2E
  Trading.Live.Tests/              # optional Integration/Smoke
```

> **Hinweis:** Namen können angepasst werden. Wichtig ist die **Trennung** zwischen Domäne/Libs (unter `src/`) und Tests (unter `tests/`).

---

## 2) Logische Struktur (Solution-Folders)

In Visual Studio/Rider per *Add → New Solution Folder* anlegen und Projekte hineinziehen:

```
TradingBot.sln
├─ src/
│  ├─ Core & Contracts
│  │  └─ Trading.Core
│  ├─ ML
│  │  ├─ Trading.Features
│  │  ├─ Trading.Labeling
│  │  └─ Trading.Policy
│  ├─ Strategy
│  │  └─ Trading.Decision
│  ├─ Adapters
│  │  ├─ Trading.Exchanges.Binance
│  │  ├─ Trading.Exchanges.Bitget
│  │  └─ Trading.Backtest.Adapter
│  ├─ Infrastructure
│  │  ├─ Trading.Persistence
│  │  └─ Trading.Telemetry
│  └─ Apps
│     ├─ Trading.Train
│     ├─ Trading.Backtest
│     └─ Trading.Live
└─ tests/
   ├─ Shared
   │  └─ Trading.Tests.Core
   ├─ Unit
   │  ├─ Trading.Core.Tests
   │  ├─ Trading.Features.Tests
   │  ├─ Trading.Labeling.Tests
   │  ├─ Trading.Policy.Tests
   │  ├─ Trading.Decision.Tests
   │  ├─ Trading.Exchanges.Binance.Tests
   │  └─ Trading.Persistence.Tests
   └─ Integration
      ├─ Trading.Backtest.Adapter.Tests
      ├─ Trading.Backtest.Tests
      └─ Trading.Live.Tests
```

---

## 3) Projekt-Referenzen (Richtung & Regeln)

**Grundregel:** Abhängigkeiten gehen **nach unten** in die Domäne, nicht kreuz und quer.

```
Trading.Core            ← wird von allen anderen referenziert

Trading.Features        → Trading.Core
Trading.Labeling        → Trading.Core
Trading.Policy          → Trading.Core
Trading.Decision        → Trading.Core

Trading.Exchanges.*     → Trading.Core
Trading.Persistence     → Trading.Core
Trading.Telemetry       → Trading.Core

Trading.Backtest.Adapter→ Trading.Core, (optional) Trading.Decision, Trading.Policy, Trading.Features

Trading.Train           → Trading.Core, Trading.Features, Trading.Labeling, Trading.Policy, Trading.Persistence, Trading.Telemetry
Trading.Backtest        → Trading.Core, Trading.Backtest.Adapter, Trading.Decision, Trading.Policy, Trading.Features, Trading.Telemetry
Trading.Live            → Trading.Core, Trading.Features, Trading.Policy, Trading.Decision, Trading.Exchanges.*, Trading.Persistence, Trading.Telemetry
```

**Don’ts:**
- `Trading.Decision` darf **nicht** `Trading.Exchanges.*` referenzieren.
- `Trading.Policy` darf **nicht** `Trading.Persistence` referenzieren.
- `Trading.Features` darf **nicht** `Trading.Train` referenzieren.

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

## 5) Gemeinsame Testbasis (`Trading.Tests.Core`)

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
dotnet new sln -n TradingBot
# Libraries
dotnet new classlib -n Trading.Core -o src/Trading.Core
dotnet new classlib -n Trading.Features -o src/Trading.Features
dotnet new classlib -n Trading.Labeling -o src/Trading.Labeling
dotnet new classlib -n Trading.Policy -o src/Trading.Policy
dotnet new classlib -n Trading.Decision -o src/Trading.Decision
dotnet new classlib -n Trading.Exchanges.Binance -o src/Trading.Exchanges.Binance
dotnet new classlib -n Trading.Exchanges.Bitget -o src/Trading.Exchanges.Bitget
dotnet new classlib -n Trading.Backtest.Adapter -o src/Trading.Backtest.Adapter
dotnet new classlib -n Trading.Persistence -o src/Trading.Persistence
dotnet new classlib -n Trading.Telemetry -o src/Trading.Telemetry

# Apps
dotnet new console -n Trading.Train -o src/Trading.Train
dotnet new console -n Trading.Backtest -o src/Trading.Backtest
dotnet new worker  -n Trading.Live -o src/Trading.Live

# Tests
dotnet new xunit -n Trading.Tests.Core -o tests/Trading.Tests.Core
dotnet new xunit -n Trading.Features.Tests -o tests/Trading.Features.Tests
# ... (weitere)

# Projekte zur Solution
for p in src/*/*.csproj tests/*/*.csproj; do dotnet sln TradingBot.sln add "$p"; done
```

---

## 7) Minimal-Interfaces (Kernelemente)

```csharp
// src/Trading.Core
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
  "db": { "conn": "Host=localhost;Username=bot;Password=secret;Database=trading" }
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