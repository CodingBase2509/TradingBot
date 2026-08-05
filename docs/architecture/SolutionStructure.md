# .NET-Solution- und Projektarchitektur

## Status und Zweck

Diese Seite legt die aktuelle physische Architektur der .NET-Plattform und die
fachliche Verantwortung jedes Projekts verbindlich fest. Die fachlichen
Abläufe beschreibt [Komponenten](./Components.md); die Gründe für die
projektbasierte Trennung dokumentiert
[ADR-039](../decisions/ADR-039-Project-Based-Modular-Monolith.md).

Die Plattform ist ein **modularer Monolith**: Die Module werden getrennt
entwickelt und kompiliert, aber gemeinsam als eine Anwendung ausgeliefert und
betrieben. Ein Projekt ist eine Code- und Abhängigkeitsgrenze, kein eigener
Dienst.

## Solution-Struktur

```text
TradingPlatform.slnx
├── TradingPlatform                    ASP.NET-Core-Host
├── TradingPlatform.Platform           kleiner Shared Kernel
│
├── TradingPlatform.Market
├── TradingPlatform.FeatureIntelligence
├── TradingPlatform.Decision
├── TradingPlatform.RiskGuard
├── TradingPlatform.TradeManagement
├── TradingPlatform.Execution
├── TradingPlatform.Reconciliation
├── TradingPlatform.ModelManagement
├── TradingPlatform.Operations
│
├── TradingPlatform.Tests              gemeinsame Testbibliothek
├── TradingPlatform.Tests.Unit         ausführbare Unit Tests
└── TradingPlatform.Tests.Integration  ausführbare Integrationstests
```

## Grundregeln

1. `TradingPlatform.Platform` referenziert kein fachliches Modul.
2. Jedes fachliche Modul darf den Shared Kernel referenzieren.
3. Modul-zu-Modul-Referenzen entstehen nur für einen konkreten fachlichen
   Vertrag und folgen dem fachlichen Datenfluss.
4. Der Host referenziert alle Module und ist die einzige Composition Root.
5. Kein Produktionsprojekt referenziert ein Testprojekt.
6. Zyklische Projektverweise sind nicht zulässig.
7. Ein Modul schreibt ausschließlich in seine eigenen Tabellen und
   PostgreSQL-Schemas. Direkte Schreibzugriffe auf den Zustand eines anderen
   Moduls sind nicht zulässig.
8. Module kommunizieren in V1 direkt und typisiert über bewusst öffentliche
   Verträge. Ein allgemeiner In-Process-Bus oder Mediator wird nicht
   vorsorglich eingeführt.
9. Innerhalb eines Moduls entstehen Unterordner nach fachlichem Bedarf. Eigene
   Projekte für Domain, Application oder Infrastructure benötigen einen
   nachgewiesenen zusätzlichen Nutzen.

## Produktionsprojekte

### `TradingPlatform`

Der ASP.NET-Core-Host ist Startpunkt und Composition Root der Anwendung.

**Besitzt:**

- Prozessstart und kontrolliertes Herunterfahren;
- Laden der Bootstrapkonfiguration und Umgebungsgrenzen;
- Dependency Injection und Reihenfolge der Modulregistrierung;
- ASP.NET-Core-Middleware, OpenAPI und technische Hostendpunkte;
- Start und Lebenszyklus registrierter Hintergrunddienste;
- Auslieferung des gebauten Angular-Frontends.

**Besitzt nicht:** Handels-, Risiko-, Modell-, Order- oder Marktdatenlogik. Ein
HTTP-Endpunkt delegiert an das verantwortliche Modul.

### `TradingPlatform.Platform`

Der Shared Kernel enthält nur kleine, stabile Konzepte, die mehrere Module
wirklich gemeinsam benötigen.

**Besitzt:**

- modulübergreifende Identitäten wie `InstrumentId` und
  `StrategyInstanceId`;
- globale Laufzeitbegriffe wie `ExecutionMode` und `PlatformEnvironment`;
- später gemeinsame Metadaten für Versionierung, Korrelation und technische
  Fehler, sofern mindestens zwei Module sie tatsächlich benötigen;
- technische Grundlagen ohne Bezug zu einem einzelnen Fachmodul.

**Besitzt nicht:** fachliche Entitäten wie Trade, Order, Position oder
RiskDecision; Brokeradapter; EF-Core-Entitäten eines Moduls; allgemeine
`Helpers`, `Utils` oder beliebige gemeinsame DTOs.

### `TradingPlatform.Market`

Market besitzt den vollständigen instrument- und marktdatenbezogenen Kontext.

**Besitzt:**

- Instrumentdefinitionen, Fähigkeiten und versionierte Instrumentprofile;
- Zuordnung interner Instrument-IDs zu Datenanbieter- und Brokersymbolen;
- Futures-Kontrakte, Ablauf, Rollover und kontinuierliche Historie;
- Börsenkalender, Sitzungen, Pausen und Entscheidungstakt;
- Aufnahme und Normalisierung historischer und aktueller Marktdaten;
- Datenqualitätsprüfungen, Lücken, Duplikate und veraltete Daten;
- kanonische Bars sowie 1-, 5-, 15- und 60-Minuten-Sichten;
- Parquet-Zugriff für Plattform-Marktdaten.

Market entscheidet nicht über Trades und führt keine Orders aus.

### `TradingPlatform.FeatureIntelligence`

Feature Intelligence verwandelt einen gültigen Marktstand in reproduzierbare
Modelleingaben und bewertete Kandidaten.

**Besitzt:**

- kausale Featureberechnung und Featureversionen;
- Missing-Data-Vertrag für Modelleingaben;
- adaptive Long-/Short-, Stop- und Zielkandidaten;
- Aufbau, Prüfung und Ausführung von ONNX-Eingaben;
- ONNX-Inferenz und technische Inferenzresultate;
- Python-/NET-Paritätslogik auf der .NET-Seite.

Das Projekt entscheidet nicht über feste Risiken und aktiviert keine
Modellpakete selbst.

### `TradingPlatform.Decision`

Decision trifft die fachliche Strategieentscheidung aus gültigen Kandidaten
und Modellbewertungen.

**Besitzt:**

- Vergleich von Long-, Short- und Kein-Trade-Alternativen;
- Schwellen- und Auswahlregeln einer Strategy Instance;
- Erzeugung einer begründeten Handelsabsicht (`TradeIntent`);
- maschinenlesbare Kein-Trade- und Ablehnungsgründe der Entscheidungsebene;
- Zuordnung jeder Entscheidung zu Daten-, Feature-, Konfigurations- und
  Modellversionen.

Decision reserviert kein Risiko und erzeugt keine Brokerorder.

### `TradingPlatform.RiskGuard`

Risk Guard ist die unveränderbare finanzielle und technische
Sicherheitsgrenze vor einer Order.

**Besitzt:**

- Strategy-, Trade-, Konto- und Portfoliorisikoprüfungen;
- Positionsgrößen- und Verlustgrenzen;
- Prüfung des Netto-Risk-to-Reward;
- Richtungs-, Frequenz-, Tages- und Wochenendregeln;
- atomare Reservierung von Risiko, Tradeplatz und Trade-Token;
- Risikofreigaben, Reservierungs-IDs und maschinenlesbare Ablehnungsgründe;
- risikoabhängige Entry Locks und Eskalationssignale.

Risk Guard verwendet Markt- und Kalenderinformationen, besitzt diese aber
nicht. Er verändert keine Modellentscheidung, sondern erlaubt oder verweigert
ihre Ausführung.

### `TradingPlatform.TradeManagement`

Trade Management besitzt den Lebenszyklus eines logischen Trades und dessen
Zuordnung zur gemeinsamen Brokerposition.

**Besitzt:**

- Tradezustandsmaschine von Planung bis Abschluss;
- Zuordnung zu Strategy Instance, Instrument und Risikoreservierung;
- logische Teilmengen und Strategiepositionen innerhalb einer
  Broker-Nettoposition;
- fachliche Schutz-, Ausstiegs- und Zeitlimitabsichten;
- Behandlung von Teilfüllungen auf Tradeebene;
- Schließungsgrund und abschließenden Tradezustand.

Trade Management sendet keine Brokerbefehle und erfindet keine
Ausführungsbestätigungen.

### `TradingPlatform.Execution`

Execution besitzt Orderrouting, Brokerkommunikation und bestätigte
Ausführungsereignisse.

**Besitzt:**

- Orderzustandsmaschine und stabile Client-Order-IDs;
- Execution Router für Backtest, simuliertes Paper, Broker Paper und später
  Live;
- simulierten Ausführungsadapter und IBKR-Adapter;
- Senden, Ändern und Stornieren von Orders;
- brokerseitige Stop-Loss- und Take-Profit-Orders;
- Teil- und Vollausführungen, Gebühren und Ausführungspreise;
- Idempotenz und sichere Behandlung unklarer Übermittlungsergebnisse.

Execution darf keine Order ohne gültige Risikofreigabe übermitteln.

### `TradingPlatform.Reconciliation`

Reconciliation prüft den internen Handelszustand unabhängig gegen Broker oder
Simulation und stellt ihn nach Neustarts wieder her.

**Besitzt:**

- regelmäßigen Abgleich von Orders, Ausführungen und Nettopositionen;
- Erkennung unbekannter, fehlender oder widersprüchlicher Zustände;
- Rekonstruktion nach Prozess- oder Verbindungsunterbrechung;
- Abweichungsfälle, Reconciliation-Status und Eskalationsbedarf;
- Bestätigung, dass Trades und Positionen endgültig abgeglichen sind.

Reconciliation korrigiert sicherheitskritische Unklarheiten nicht still. Es
blockiert beziehungsweise eskaliert den kleinsten erforderlichen Bereich.

### `TradingPlatform.ModelManagement`

Model Management besitzt Modellpakete und ihren kontrollierten Lebenszyklus in
der Plattformumgebung.

**Besitzt:**

- Erkennung und idempotente Registrierung kopierter Modellpakete;
- Prüfung von Manifest, Dateien, Prüfsummen, Schemas und Kompatibilität;
- Modellstatus von Candidate bis Retired;
- Umgebungsfreigabe, Aktivierung, Rollback und letzte stabile Version;
- Bereitstellung eines geprüften Modellpakets für Feature Intelligence;
- Auditdaten zu Promotion, Aktivierung und Rücknahme.

Model Management trainiert keine Modelle und greift in Test oder Produktion
nicht direkt auf MLflow zu.

### `TradingPlatform.Operations`

Operations bündelt Betrieb, Kontrolle, Nachvollziehbarkeit und die
plattformweite Außensicht.

**Besitzt:**

- unveränderliche beziehungsweise append-only Audit-Einträge;
- System-, Modul-, Broker-, Daten- und Modellzustände;
- Warnungen, technische Metriken und fachliche Betriebsereignisse;
- kontrollierten Stopp, Full-Stop, Not-Aus und Wiederaufnahme;
- Fehlerisolation auf Strategy-, Instrument-, Konto- und Plattformebene;
- API- und SignalR-Sichten für Dashboard und Betrieb;
- Health- und Readiness-Status der fachlichen Komponenten.

Operations beobachtet und koordiniert, übernimmt aber keine fachliche
Entscheidungs-, Risiko- oder Orderlogik anderer Module.

## Fachlicher Hauptfluss

```text
Market
→ FeatureIntelligence
→ Decision
→ RiskGuard
→ TradeManagement
→ Execution
→ Reconciliation
→ Operations
```

`ModelManagement` stellt Feature Intelligence ein geprüftes Modellpaket bereit.
Der Host setzt die Module zusammen, nimmt aber nicht am fachlichen Ablauf teil.

Projektverweise folgen diesem Fluss nicht automatisch vollständig. Ein Modul
erhält erst dann eine Referenz auf ein anderes Modul, wenn es einen konkreten
öffentlichen Typ oder Dienst dieses Moduls verwendet.

## Besitz von Zuständen und Persistenz

| Zustand oder Datenart | Verantwortliches Projekt | PostgreSQL-Schema beziehungsweise Speicher |
|---|---|---|
| Instrumente, Verträge, Kalender, Datenqualität | `TradingPlatform.Market` | `market`, Parquet |
| Strategy Instances und Strategy-Konfigurationen | `TradingPlatform.Decision` | `strategy` |
| Entscheidungen und Entscheidungsgründe | `TradingPlatform.Decision` | `trading` |
| Candidate-Konfiguration und Inferenzmetadaten | `TradingPlatform.FeatureIntelligence` | `model` |
| Modellpakete und Freigabestatus | `TradingPlatform.ModelManagement` | `model`, Artefaktablage |
| Kontolimits und Risikoreservierungen | `TradingPlatform.RiskGuard` | `risk` |
| logische Trades und Strategiepositionen | `TradingPlatform.TradeManagement` | `trading` |
| Orders und Ausführungen | `TradingPlatform.Execution` | `execution` |
| Abgleichläufe und Brokerabweichungen | `TradingPlatform.Reconciliation` | `execution` |
| Audit, Warnungen und Betriebszustand | `TradingPlatform.Operations` | `operations` |

Gemeinsam genutzte PostgreSQL-Schemas bedeuten nicht gemeinsam besessene
Tabellen. Jede Tabelle hat genau ein schreibendes Modul. Andere Module greifen
über öffentliche Verträge oder ausdrücklich definierte Read Models zu.

## Testprojekte

### `TradingPlatform.Tests`

Nicht ausführbare, gemeinsame Testbibliothek für:

- fachlich benannte Testdaten-Builder;
- feste Profile und Golden Samples;
- kontrollierte Zeit und andere deterministische Fakes;
- gemeinsame Fixtures für Datenbank, Dateien, HTTP und Container;
- wiederverwendbare Assertions und Vertragstest-Grundlagen.

Die Bibliothek enthält keine konkreten Tests, die eigenständig entdeckt und
ausgeführt werden sollen.

### `TradingPlatform.Tests.Unit`

Ausführbare schnelle Tests für einzelne fachliche Regeln, Value Objects,
Berechnungen und Zustandsübergänge. Sie verwenden kein Netzwerk, keinen echten
Broker und grundsätzlich keine echte Datenbank.

### `TradingPlatform.Tests.Integration`

Ausführbare Tests für den ASP.NET-Core-Host, Dependency Injection, HTTP,
Persistenz, Migrationen, PostgreSQL, Parquet und externe Adaptergrenzen. Echte
externe Broker- oder Datenanbieter werden nicht unkontrolliert angesprochen;
dafür dienen Simulationen, Stubs oder explizit freigegebene Testumgebungen.

## Öffentliche Moduloberfläche

Jedes Modul besitzt einen kleinen öffentlichen Registrierungspunkt, zum
Beispiel `AddMarketModule`. Darüber registriert es eigene Dienste und
Infrastruktur. Darüber hinaus sind nur Typen öffentlich, die ein anderer
Baustein wirklich benötigt.

Neue öffentliche Typen beantworten vor ihrer Einführung drei Fragen:

1. Welches Modul besitzt ihre Bedeutung und Weiterentwicklung?
2. Welcher konkrete Verbraucher benötigt sie?
3. Ist Rückwärtskompatibilität oder Versionierung an dieser Grenze notwendig?

## Änderungsregel

Eine neue Funktion wird dem Projekt zugeordnet, das ihren fachlichen Zustand
und ihre Regeln besitzt. Benötigt sie mehrere Module, bleibt die Orchestrierung
entlang des Hauptflusses verteilt; sie wird nicht in den Host oder Shared
Kernel verschoben. Änderungen der hier definierten Besitz- oder
Abhängigkeitsgrenzen werden zuerst auf dieser Seite und bei grundlegender
Auswirkung zusätzlich in einer ADR dokumentiert.
