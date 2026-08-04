# Konfigurationsvertrag

## Grundsatz

Jede wirksame Handelsentscheidung verweist auf vollständige, validierte und
unveränderliche Konfigurationsversionen. Eine aktive Version wird nie
überschrieben; jede Änderung erzeugt eine neue UUID-v7-ID, Versionsnummer und
SHA-256-Prüfsumme.

Die Konfiguration ist in fünf Verantwortungsbereiche getrennt:

```text
Plattform → Konto → Instrument → Strategy Instance
                         └────→ Candidate Generator

Training/Forschung → eigener Research Run
```

Eine untergeordnete Konfiguration darf globale Regeln nur weiter einschränken,
nicht lockern. Das Modell kann keine Konfiguration verändern.

## Nicht konfigurierbare Sicherheitsregeln

- keine Order ohne gültige Risikoreservierung;
- bestätigter brokerseitiger TP und SL für jede offene Menge;
- unklarer Broker- oder Positionszustand blockiert neue Trades;
- kein automatischer Wechsel von Paper zu Live;
- keine automatische Modellpromotion;
- kein Trade mit Netto-Risk-to-Reward unter `1:1`;
- keine Position über das Börsenwochenende;
- unbekannte Vertrags-, Schema-, Enum- oder Modellversionen werden abgelehnt.

Änderungen daran sind Code- und Dokumentationsänderungen, keine normale
Konfigurationsänderung.

## Gemeinsame Metadaten

Jede persistierte fachliche Konfiguration enthält mindestens:

```json
{
  "configId": "<uuid-v7>",
  "version": 3,
  "schemaVersion": 1,
  "status": "Active",
  "createdAtUtc": "2026-08-03T12:00:00Z",
  "createdBy": "user:<id>",
  "reason": "Risiko für Paper-Test reduziert",
  "contentHash": "<sha-256>"
}
```

Zulässige Zustände sind:

```text
Draft → Validated → Active → Superseded
                     └──────→ Retired
```

## Platform Configuration

Steuert den technischen Betrieb einer installierten Plattformzone.

```json
{
  "schemaVersion": 1,
  "environment": "Test",
  "allowedExecutionModes": [
    "Backtest",
    "Shadow",
    "SimulatedPaper",
    "BrokerPaper"
  ],
  "databaseConnectionSecret": "TRADING_DB_CONNECTION",
  "storage": {
    "marketDataPath": "/data/market",
    "modelPath": "/data/models",
    "temporaryPath": "/data/temp"
  },
  "brokerAdapter": "InteractiveBrokers",
  "reconciliationIntervalSeconds": 15,
  "protectionConfirmationTimeoutSeconds": 10,
  "modelScanIntervalSeconds": 60
}
```

- `environment`: `Development`, `Test` oder `Production`; bestimmt die
  Sicherheitszone und darf zur Laufzeit nicht wechseln.
- `allowedExecutionModes`: begrenzt die in dieser Zone erlaubten Modi.
- `databaseConnectionSecret`: Name der Environment Variable, niemals ihr Wert.
- `storage`: zoneneigene Containerpfade.
- `brokerAdapter`: `Simulated` oder `InteractiveBrokers`.
- Intervalle und Timeoutwerte steuern Abgleich, Schutzprüfung und Modellscan.

Bootstrapwerte, ohne die PostgreSQL nicht erreichbar wäre, liegen in einer
read-only JSON-Datei. Nach dem Start wirksame versionierbare Plattformwerte
liegen in `operations.platform_config_versions`.

## Account Configuration

Definiert gemeinsame Handels- und Risikogrenzen eines ausführenden Kontos.

```json
{
  "schemaVersion": 1,
  "accountConfigId": "<uuid-v7>",
  "accountReference": "IBKR_PAPER_MAIN",
  "executionMode": "BrokerPaper",
  "riskLimits": {
    "maxRiskPerTradePercent": 2.0,
    "maxAggregateRiskPercent": 6.0,
    "dailyLossEntryLockPercent": 8.0
  },
  "tradingLimits": {
    "maxOpenTrades": 3,
    "maxTradesPerTradingDay": 10,
    "maxConsecutiveLosses": 3,
    "maxConsecutiveOrderErrors": 3,
    "sameDirectionOnly": true
  }
}
```

- Risikowerte begrenzen Einzeltrade, Gesamtrisiko und Tageseinstieg.
- Handelslimits begrenzen Parallelität, Frequenz und Fehler-/Verlustserien.
- `executionMode` ist `BrokerPaper` oder später `Live` und muss zur Zone passen.
- `accountReference` referenziert das Konto, enthält aber keine Zugangsdaten.

Persistenz: `risk.account_config_versions`.

## Instrument Configuration

Beschreibt feste technische und marktbezogene Eigenschaften eines Instruments.
Das folgende MES-Objekt ist ein V1-Beispiel und nicht das Schema der Plattform
für ein einziges Symbol.

```json
{
  "schemaVersion": 1,
  "instrumentId": "MES",
  "name": "Micro E-mini S&P 500 Future",
  "instrumentType": "Future",
  "exchange": "CME",
  "currency": "USD",
  "providerSymbols": {
    "interactiveBrokers": "MES",
    "databento": "MES.FUT"
  },
  "tickSize": 0.25,
  "tickValue": 1.25,
  "minimumQuantity": 1,
  "calendarId": "CME_EQUITY_INDEX_V1",
  "rolloverRuleId": "MES_VOLUME_NEXT_DAY_V1",
  "capabilities": {
    "supportsLong": true,
    "supportsShort": true,
    "hasExpiringContracts": true,
    "requiresRollover": true
  },
  "candidateLimits": {
    "minimumStopTicks": 2,
    "maximumStopTicks": 200,
    "maximumCandidates": 24
  },
  "costModelId": "MES_COSTS_V1"
}
```

- Tickwerte, Währung und Mindestmenge ermöglichen korrekte Preis- und
  Risikorechnung.
- `providerSymbols` ordnet dieselbe interne InstrumentId den Symbolen und
  Vertragskennungen der jeweiligen Adapter zu.
- `instrumentType` und `capabilities` aktivieren nur tatsächlich benötigte
  instrumentbezogene Abläufe; sie verteilen keine Symbol-Sonderfälle im Code.
- Kalender und Rollover bestimmen handelbare Zeiten und Verträge.
- Candidate Limits verhindern technisch unplausible Ausstiege.
- Das Kostenmodell liefert Gebühren-, Spread- und Slippageannahmen.

Persistenz: `market.instrument_config_versions`. Konkrete Stopgrenzen bleiben
bis zur Datenvalidierung Startwerte.

Eine neue Instrumentkonfiguration kann beispielsweise einen weiteren Future,
eine Aktie oder ein Währungspaar beschreiben. Voraussetzung sind passende
Broker- und Datenadapter sowie alle benötigten Stammdaten. Der gemeinsame
Tradingkern bleibt davon unverändert.

## Strategy Configuration

Verbindet Daten, Instrument, Featureberechnung, Modell und Modus zu einer
Strategy Instance.

```json
{
  "schemaVersion": 1,
  "strategyInstanceId": "<uuid-v7>",
  "name": "MES Gradient Boosting 01",
  "instrumentId": "MES",
  "dataSource": "IbkrLive",
  "decisionTimeframe": "M5",
  "contextTimeframes": ["M1", "M5", "M15", "H1"],
  "featureVersion": 1,
  "candidateGeneratorVersion": 1,
  "modelPackageId": "<uuid-v7>",
  "decisionThreshold": null,
  "executionMode": "Shadow",
  "riskProfile": {
    "maxRiskPerTradePercent": 1.0
  },
  "enabled": false
}
```

- `dataSource`: `Historical` oder `IbkrLive`.
- `decisionTimeframe`: V1 verwendet `M5`; Kontext ist M1, M5, M15 und H1.
- Versionsfelder müssen exakt mit Modellpaket und Runtime kompatibel sein.
- `decisionThreshold: null` bedeutet noch nicht validiert und nicht ausführbar.
- `executionMode`: `Backtest`, `Shadow`, `SimulatedPaper`, `BrokerPaper` oder
  später `Live`.
- Das Strategy-Risikoprofil darf das Account-Limit nur unterschreiten.
- `enabled` ersetzt keine Aktivierungs- und Freigabeprüfung.

Persistenz: `strategy.strategy_config_versions`. Die unveränderliche
`strategy.strategies`-Identität bleibt von ihren Versionen getrennt.

## Candidate Generator Configuration

Bestimmt die reproduzierbare Erzeugung adaptiver Long-/Short-Kandidaten.

```json
{
  "schemaVersion": 1,
  "generatorVersion": 1,
  "lookbacks": {
    "M1": 120,
    "M5": 288,
    "M15": 192,
    "H1": 120
  },
  "swingWindows": {
    "M1": { "left": 3, "right": 3 },
    "M5": { "left": 3, "right": 3 },
    "M15": { "left": 2, "right": 2 },
    "H1": { "left": 2, "right": 2 }
  },
  "maximumStopsPerDirection": 4,
  "maximumTargetsPerDirection": 4,
  "maximumCandidatesPerDirection": 12,
  "maximumCandidatesTotal": 24,
  "minimumNetRiskReward": 1.0,
  "targetRuntimeMilliseconds": 100,
  "hardRuntimeLimitMilliseconds": 500
}
```

- Lookbacks und Swingfenster bestimmen die verfügbare kausale Marktstruktur.
- Mengenlimits begrenzen Kombinationen und Modellbewertungen.
- `minimumNetRiskReward` ist mindestens `1.0` und darf nicht gelockert werden.
- Laufzeitgrenzen verhindern verspätete Handelsentscheidungen.

Persistenz: `model.candidate_generator_config_versions`. Jede Änderung benötigt
eine neue Version sowie Golden- und Paritätstests.

## Research Run Configuration

Beschreibt einen reproduzierbaren Python-Import-, Trainings-, Backtest- oder
Evaluationslauf in der isolierten Trainingszone.

```json
{
  "schemaVersion": 1,
  "runId": "<uuid-v7>",
  "jobType": "Train",
  "datasetId": "<uuid-v7>",
  "featureVersion": 1,
  "candidateGeneratorVersion": 1,
  "labelVersion": 1,
  "costModelVersion": 1,
  "modelType": "GradientBoosting",
  "hyperparameters": {
    "numberOfTrees": 300,
    "maximumDepth": 6,
    "learningRate": 0.05
  },
  "timeSplitId": "MES_WALK_FORWARD_V1",
  "randomSeed": 42,
  "evaluationProfileId": "MES_EVALUATION_V1"
}
```

- `jobType`: `Import`, `Dataset`, `Train`, `Backtest`, `Evaluate` oder `Export`.
- Dataset- und Vertragsversionen legen die exakten Eingaben fest.
- Modelltyp und Hyperparameter bestimmen das Training.
- Zeitaufteilung verhindert zufällige zeitliche Vermischung.
- Random Seed und Evaluationsprofil ermöglichen Reproduzierbarkeit.

Persistenz: eigene Tabelle `research_run_configs` in der Trainingsdatenbank;
MLflow referenziert dieselbe Run-ID. Test und Produktion besitzen diese Tabelle
nicht.

## Eigene Tabelle je Konfigurationstyp

| Konfiguration | Tabelle beziehungsweise Quelle |
|---|---|
| Platform Bootstrap | read-only `platform.json` im Container |
| Platform Runtime | `operations.platform_config_versions` |
| Account | `risk.account_config_versions` |
| Instrument | `market.instrument_config_versions` |
| Strategy | `strategy.strategy_config_versions` |
| Candidate Generator | `model.candidate_generator_config_versions` |
| Research Run | `research_run_configs` der Trainingsdatenbank |
| Secrets | ausschließlich Environment Variables |

Es gibt keine generische `configurations`-Tabelle mit beliebigen Typen und
unstrukturierten Inhalten. Jede Tabelle erhält fachlich passende Spalten,
Constraints und Fremdschlüssel. Kleine verschachtelte, zusammengehörige Werte
dürfen zusätzlich als validiertes JSON gespeichert werden, wenn relationale
Spalten keinen Abfrage- oder Konsistenzvorteil bieten.

## Änderung und Aktivierung

- Entwürfe werden vollständig gegen ihr JSON Schema und fachliche Grenzen
  validiert.
- Verschärfungen von Risikolimits dürfen sofort neue Trades begrenzen.
- Lockerungen benötigen bewusste Bestätigung und einen eindeutigen
  Brokerzustand.
- Bestehende Trades behalten ihre ursprünglichen Konfigurationsreferenzen.
- Strategy-Änderungen werden erst nach kontrolliertem Stop und erneuter
  Aktivierung wirksam.
- Ein Neustart lädt dieselben aktiven Versionen; fehlerhafte neue Versionen
  lassen die letzte gültige Version aktiv.
- Unbekannte Felder und Schemaversionen werden nicht stillschweigend ignoriert.

## Dateien und Secrets

Gemeinsame Verträge, JSON Schemas und sichere Beispiele liegen in Git. Normale
Konfigurationen verwenden JSON; zusätzliche YAML-/TOML-Varianten werden in V1
nicht eingeführt.

Secrets werden als Environment Variables übergeben, zum Beispiel:

```text
TRADING_DB_CONNECTION
IBKR_USERNAME
IBKR_PASSWORD
IBKR_ACCOUNT_ID
DATABENTO_API_KEY
MLFLOW_DB_CONNECTION
```

Konfigurationen speichern nur den Variablennamen. Secretwerte gelangen nicht
in Git, PostgreSQL-Konfigurationstabellen, Logs, Modellpakete oder
Fehlerberichte.
