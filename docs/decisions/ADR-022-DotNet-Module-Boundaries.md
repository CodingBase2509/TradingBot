# ADR-022: Modulgrenzen des .NET-Plattformkerns

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Backtest, Shadow, Paper und später Live sollen denselben fachlichen Handels-,
Risiko- und Positionskern verwenden. Gleichzeitig darf V1 nicht durch
Microservices, Netzwerkkommunikation oder viele unabhängig betriebene
Komponenten unnötig komplex werden.

## Entscheidung

Der .NET-Kern bleibt ein modularer Monolith und besitzt neun fachliche Module.
Die Module laufen zunächst in einem Prozess, besitzen aber getrennte
Verantwortlichkeiten und versionierte Verträge.

Gemäß ADR-023 kann der Kern mehrere Strategy Instances hosten. Market,
Model Management, Reconciliation sowie Operations & Audit besitzen gemeinsame
Plattform- und Kontosichten. Feature & Intelligence und Decision laufen je
Strategy Instance. Risk Guard und Trade Management verbinden
strategiebezogene Zustände mit globalen Konto- und Brokergrenzen.

### 1. Market

Verantwortet:

- Marktquellen und geordnete Marktereignisse;
- kanonische abgeschlossene Kerzen;
- Datenqualität und Datenalter;
- Exchange Calendar und fachlichen Handelstag;
- konkrete Futures-Kontrakte und Rollover;
- Zeitsteuerung und Auslösung des 5-Minuten-Entscheidungstakts.

Market erzeugt keine Handelsentscheidung.

### 2. Feature & Intelligence

Verantwortet:

- versionierte Feature-Berechnung;
- Erkennung der vorherigen Marktstruktur;
- adaptive TP-/SL-Kandidatenerzeugung;
- ONNX-Inferenz für alle zulässigen Kandidaten;
- technische Prüfung der Modellausgaben.

Das Modul kennt weder Kontostand noch Brokerorders und kann keine Position
eröffnen.

### 3. Decision

Verantwortet:

- Vergleich gültiger Kandidaten mit `NoTrade`;
- Anwendung der eingefrorenen Entscheidungsschwelle;
- Prüfung des modellbezogenen Mindestvorteils und Netto-Risk-to-Reward;
- Auswahl eines begründeten Kandidaten;
- Erzeugung einer `TradeIntent` oder einer dokumentierten
  `NoTrade`-Entscheidung.

Eine `TradeIntent` ist eine Handelsabsicht und keine Orderfreigabe.

### 4. Risk Guard

Verantwortet alle festen, modellunabhängigen Schutzregeln:

- Trade-, Gesamt- und Tagesrisiko;
- Kontostand und handelbare Mindestmenge;
- Parallelitäts-, Richtungs- und Tradezahlgrenzen;
- Verlustserien und technische Fehlersperren;
- Handelszeit und Einstiegsschluss;
- Daten-, Modell-, System- und Not-Aus-Status.

Er erzeugt einen `ApprovedTradePlan` oder eine Ablehnung mit
maschinenlesbarem Grund. Er darf Größe reduzieren oder den Trade ablehnen,
aber keine Modellbewertung verändern.

### 5. Trade Management

Verantwortet:

- fachlichen Trade-Lebenszyklus;
- Trade Controller und Position Manager;
- unveränderte TP-/SL-Vorgaben nach Einstieg;
- Zuordnung von bis zu drei logischen Trades zur aggregierten
  Broker-Nettoposition;
- Behandlung von Teilfüllungen;
- Freitagsschließung und andere fachliche Ausstiege.

Das Modul unterscheidet strikt zwischen Handelsidee (`Trade`), Brokerauftrag
(`Order`) und tatsächlich gehaltener Nettomenge (`Position`).

### 6. Execution

Verantwortet:

- Übersetzung freigegebener Pläne in konkrete Orders;
- Senden, Ändern und Stornieren;
- Bestätigungen, Ablehnungen und Teilfüllungen;
- Platzierung und Pflege gekoppelter brokerseitiger Schutzorders;
- kontrollierte Schließungs- und Eskalationsorders.

Execution arbeitet gegen `ExecutionVenue` und kennt den konkreten Broker nur
über einen Adapter.

### 7. Reconciliation

Prüft unabhängig von Execution:

- offene Positionen und Orders;
- Mengen und Durchschnittspreise;
- Ausführungen;
- bestätigte TP-/SL-Abdeckung;
- Kontostand und Brokerstatus.

Abweichungen blockieren zuerst neue Trades. Ungeschützte Positionen lösen den
beschlossenen Korrektur- und Schließungsablauf aus.

### 8. Model Management

Verantwortet:

- Modellpaket-, Prüfsummen- und Vertragsprüfung;
- Python-/ONNX-/NET-Referenzvergleich;
- getrennte Freigaben je Umgebung;
- aktive Paket-ID;
- Aktivierung und Rollback;
- Aufbewahrung der letzten stabilen Version.

MLflow gehört nicht zu diesem Modul. Model Management kennt nur exportierte
Modellpakete und deren Herkunftsreferenz.

### 9. Operations & Audit

Verantwortet:

- Systemzustand, Monitoring und Warnungen;
- append-only Auditjournal;
- kontrollierten Start und Wiederanlauf;
- manuellen Systemstopp und Full-Stop;
- Berechtigungen für bestätigungspflichtige Aktionen;
- Zustandsbereitstellung für REST und SignalR.

Operations darf den Handel blockieren und Notfallaktionen koordinieren, baut
aber keine fachliche Logik anderer Module nach.

### Gemeinsame Adapter

Der Fachkern verwendet mindestens:

```text
Clock
├── HistoricalClock
└── SystemClock

MarketDataSource
├── HistoricalDataSource
└── InteractiveBrokersMarketDataSource

ExecutionVenue
├── SimulatedBroker
├── InteractiveBrokersPaper
└── InteractiveBrokersLive
```

Der Live-Adapter bleibt in V1 deaktiviert. Backtest und Paper tauschen nur
Zeit-, Daten- und Ausführungsadapter aus; Decision, Risk Guard, Trade
Management und Reconciliation bleiben fachlich identisch.

Execution Requests mehrerer Strategy Instances werden durch einen gemeinsamen
Execution Router koordiniert, bevor sie einen Brokeradapter erreichen.

### Kommunikation

- Synchrone direkte Modulverträge werden für Prüfungen mit unmittelbarer
  Antwort verwendet.
- Bereits geschehene Zustandsänderungen werden als interne Ereignisse
  veröffentlicht.
- Fachlich relevante Ereignisse werden zusätzlich dauerhaft protokolliert.
- Kein Modul greift direkt auf Tabellen oder Implementierungsdetails eines
  anderen Moduls zu.
- Es wird in V1 kein externer Message Broker eingesetzt.

Beispiele:

```text
RiskGuard.Evaluate(TradeIntent) → ApprovedTradePlan | Rejection

OrderPartiallyFilled
PositionProtected
TradeClosed
DailyLossLimitReached
```

### Abhängigkeitsrichtung

Broker, PostgreSQL, ONNX, Systemzeit und Web liegen hinter Schnittstellen.
Der Fachkern hängt nicht von konkreten Infrastrukturklassen ab.

```text
externe Systeme
→ Adapter und Infrastruktur
→ versionierte Modulverträge
→ fachlicher Kern
```

## Begründung

Die neun Module bilden klare Sicherheits- und Verantwortungsgrenzen, ohne neun
Dienste betreiben zu müssen. Insbesondere verhindern getrennte Decision- und
Risk-Module, dass ein Modell Schutzregeln umgeht. Die Trennung von Execution
und Reconciliation stellt Ausführungsabsicht und tatsächlichen Brokerzustand
unabhängig gegenüber.

Austauschbare Zeit-, Daten- und Brokeradapter sorgen dafür, dass derselbe
fachliche Weg historisch und im Paper-Betrieb geprüft wird.

## Folgen

- Die Projektstruktur muss die Modulgrenzen sichtbar machen und durch
  Architekturtests schützen.
- Modulverträge erhalten eigene Kompatibilitätstests.
- Persistenz wird je Modul gekapselt; gemeinsame Tabellenzugriffe sind
  unzulässig.
- Trade-, Order- und Positionszustände benötigen als nächsten Schritt eine
  explizite Zustandsmaschine.
- Die Module starten gemäß ADR-031 als getrennte Bereiche und Namespaces in
  einem ausführbaren Plattformprojekt. Eigene Projekte oder Dienste benötigen
  einen nachgewiesenen Nutzen.
