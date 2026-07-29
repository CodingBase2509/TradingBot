# ADR-016: Umgebungen und modularer Plattformkern

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Training, historische Tests, Paper Trading und späterer Echtgeldbetrieb
benötigen weitgehend dieselben fachlichen Verträge, dürfen aber nicht dieselben
Zugangsdaten, Zustände oder Sicherheitsgrenzen teilen. Gleichzeitig soll V1
nicht durch frühzeitige Microservices unnötig komplex werden.

## Entscheidung

### Drei getrennte Umgebungen

Das Projekt besitzt drei fachlich und betrieblich getrennte Umgebungen:

1. **Training**
   - manueller Databento-Import;
   - Datenprüfung und Dataset-Erzeugung;
   - Feature-Forschung und Label-Erzeugung;
   - Training, Validierung, ONNX-Export und Experimentverwaltung mit MLflow;
   - keine Brokerorders und keine Echtgeld-Zugangsdaten.
2. **Test**
   - historischer Backtest mit simuliertem Broker;
   - Shadow Mode mit Live-Daten ohne Brokerorders;
   - Paper Trading über das IBKR-Paper-Konto;
   - produktionsnaher .NET-Kern mit vollständiger Risiko-, Order- und
     Positionslogik.
3. **Produktion**
   - späterer Echtgeldbetrieb über einen getrennten IBKR-Live-Zugang;
   - ausschließlich geprüfter .NET-Handelskern und freigegebene Artefakte;
   - kein Training, keine Label-Erzeugung und keine autonome Modellfreigabe;
   - wird in V1 architektonisch berücksichtigt, aber nicht aktiviert.

Live-Daten sind kein Synonym für Echtgeldhandel: Shadow und Paper verwenden
Live-Daten innerhalb der Testumgebung.

### Gemeinsamer modularer Monolith

Der stabile Handels- und Plattformkern wird als modularer .NET-Monolith
entwickelt. Seine fachlichen Module laufen zunächst in einem Prozess, besitzen
aber klare interne Verträge und dürfen nicht über fremde Datenbanktabellen oder
Implementierungsdetails gekoppelt werden.

Die feinen Komponenten werden gemäß ADR-022 in neun fachliche Module
zusammengefasst: Market, Feature & Intelligence, Decision, Risk Guard, Trade
Management, Execution, Reconciliation, Model Management sowie Operations &
Audit.

Ein Modul wird erst als eigener Dienst ausgelagert, wenn Last, Isolation,
Verfügbarkeit oder unabhängiger Betrieb einen messbaren Nutzen begründen.

### Austauschbare Laufzeitadapter

Der gemeinsame Kern verwendet versionierte Schnittstellen:

```text
MarketDataSource
├─ HistoricalDataAdapter
└─ InteractiveBrokersMarketDataAdapter

ExecutionVenue
├─ SimulatedBrokerAdapter
├─ InteractiveBrokersPaperAdapter
└─ InteractiveBrokersLiveAdapter
```

Backtest, Paper und später Live teilen Risk Guard, Trade Controller,
Position Manager und Audit. Nur Zeit-, Daten- und Ausführungsadapter werden
ausgetauscht.

### Technische Trennung

Die Umgebungen besitzen:

- getrennte Konfigurationen und Geheimnisse;
- getrennte Datenbanken beziehungsweise Schemas und Artefaktbereiche;
- getrennte Brokerbenutzer und Kontoziele;
- eindeutig sichtbare Umgebungskennzeichnungen;
- einseitige, kontrollierte Artefaktpromotion;
- keinen einfachen Laufzeit-Umschalter von Paper auf Echtgeld.

Training darf weder in Test- noch Produktionszustände schreiben. Ergebnisse aus
Paper und später Produktion gelangen nur als unveränderliche Datenkopien zurück
in die Forschung.

Die Trennung wird gemäß ADR-027 als vollständige Laufzeit- und
Sicherheitsisolation umgesetzt: keine gemeinsamen beschreibbaren Datenbanken,
Speicherwurzeln, Geheimnisse oder direkten Laufzeitabhängigkeiten. Austausch
erfolgt nur über kontrollierte unveränderliche Exporte und Modellpakete.

### Spätere Trainings- und Forschungsoberfläche

Training und Forschung beginnen mit reproduzierbaren Kommandozeilen- und
Konfigurationsabläufen. Später kann Angular um eine ausschließlich in der
Trainingsumgebung verfügbare Forschungsoberfläche erweitert werden.

Die Oberfläche darf:

- Datenstände und Qualitätsberichte auswählen und anzeigen;
- versionierte Feature-, Label- und Trainingskonfigurationen erstellen;
- Trainings-, Backtest- und Evaluationsaufträge starten;
- Fortschritt, Ressourcenverbrauch, Logs und Fehler überwachen;
- Experimente, Modelle und Prüfberichte vergleichen;
- Aufträge kontrolliert abbrechen;
- geprüfte Modellartefakte zur Promotion vorschlagen.

Die Oberfläche führt keine beliebigen Python-Befehle aus. Sie spricht mit einem
Training Orchestrator, der ausschließlich bekannte, versionierte Auftragstypen
ausführt. Jeder Auftrag speichert Eingaben, Codeversion, Umgebung,
Zufallsstartwerte, Status und Ergebnis.

Modellfreigabe, Datenkauf und Echtgeldaktivierung bleiben gesonderte manuelle
Aktionen mit Berechtigung, Bestätigung und Audit-Eintrag.

## Begründung

Getrennte Umgebungen verhindern, dass Forschung oder Tests auf Echtgeldzustände
zugreifen. Der modulare Monolith hält Entwicklung und Betrieb der V1
überschaubar, ohne fachliche Grenzen aufzugeben. Austauschbare Adapter sorgen
dafür, dass derselbe Entscheidungsweg in Backtest, Paper und später Live
verwendet wird.

Eine spätere Forschungsoberfläche verbessert Bedienbarkeit und Vergleichbarkeit,
ohne Reproduzierbarkeit oder Sicherheit aufzugeben.

## Folgen

- Projekt- und Deploymentstruktur müssen die drei Umgebungen sichtbar
  unterscheiden.
- Der Live-Adapter bleibt in V1 technisch deaktiviert beziehungsweise wird nicht
  ausgeliefert.
- Fachmodule erhalten eigene Verträge und Tests.
- Der Training Orchestrator und seine UI werden erst nach stabilen,
  skriptbaren Trainingsabläufen entwickelt.
- Die skriptbaren Abläufe beginnen gemäß ADR-030 als ein installierbares
  Python-Paket mit einem CLI und fünf Quellbereichen.
- Konkrete Datenbank-, Datei-, Artefakt- und Hostingprodukte bleiben separat
  festzulegen.
