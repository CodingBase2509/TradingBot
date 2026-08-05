# Komponenten

Diese Seite beschreibt die logischen Komponenten. Die verbindliche Zuordnung
zu konkreten .NET-Projekten, Abhängigkeitsregeln und Besitz von Zuständen steht
in der [.NET-Solution- und Projektarchitektur](./SolutionStructure.md).

## .NET-Plattform

| Modul | Verantwortung |
|---|---|
| Market | Marktdaten, Qualität, Kalender, Kontrakte und Entscheidungstakt |
| Feature & Intelligence | Features, adaptive Kandidaten und ONNX-Inferenz |
| Decision | Kandidaten vergleichen und begründete Handelsabsichten erzeugen |
| Risk Guard | unveränderbare Handels-, Konto- und Technikgrenzen prüfen |
| Trade Management | logische Trades, Brokerpositionen, Schutz und Ausstiege verwalten |
| Execution | simulierte oder IBKR-Orders ausführen und Schutzorders pflegen |
| Reconciliation | internen Zustand unabhängig mit Broker oder Simulation abgleichen |
| Model Management | Modellpakete, Freigaben, Aktivierung und Rollback verwalten |
| Operations & Audit | Systemzustand, Not-Aus, Monitoring, Audit und API-Status |

Keines dieser Module darf MES als technischen Sonderfall voraussetzen. Das
Market-Modul löst die interne `InstrumentId` in datenquellen- und
brokerspezifische Symbole beziehungsweise Verträge auf. Risk Guard, Decision,
Trade Management und Execution arbeiten anschließend mit dem gemeinsamen
Instrumentvertrag und seinen Fähigkeiten.

Backtest, Shadow und Paper verwenden diese Module gemeinsam. Austauschbar sind
nur Uhr, Marktdatenquelle und Ausführungsadapter. Fachliche Abhängigkeiten
laufen vom Markt über Entscheidung und Risiko zur Ausführung; Broker- und
Speicherdetails bleiben an den jeweiligen Randadaptern.

Feature & Intelligence sowie Decision werden je Strategy Instance betrieben.
Marktdaten, Account Risk, Execution Router, Brokeradapter, Reconciliation,
Model Management und Operations besitzen gemeinsame Plattform- oder
Kontosichten.

Jede Strategy Instance besitzt eine unveränderliche ID und versionierte
Konfiguration für Instrument, Zeitrahmen, Datenquelle, Feature- und
Candidate-Version, Modellpaket, Schwelle, Modus und Risikoprofil. Eine Änderung
erzeugt eine neue Konfigurationsversion und einen Audit-Eintrag.

Ausführungsmodi sind `Backtest`, `Shadow`, `SimulatedPaper`, `BrokerPaper` und
später `Live`. Shadow sendet keine Orders. Simulated Paper verwendet virtuelle
Konten. Broker Paper und Live teilen die konto-, risiko- und
nettopositionsbezogene Sicht. V1 erlaubt je Instrument nur eine
Broker-Paper-Ausführungsgruppe.

Trade Management, Execution und Reconciliation verwenden getrennte Trade-,
Order- und Positionszustände. Ein Trade durchläuft Planung, Risikofreigabe,
Einstieg, Schutz, aktive Verwaltung, Schließung und abschließenden Abgleich.
Orders besitzen davon unabhängig Brokerzustände wie erstellt, gesendet,
bestätigt, teilweise oder vollständig ausgeführt, storniert und abgelehnt.
Positionen werden aus bestätigten Ausführungen berechnet und mit der
Broker-Nettoposition abgeglichen. Unbekannte oder unmögliche Übergänge blockieren
den kleinsten sicheren Bereich.

Die neun Module werden in V1 als eigene Projekte und Assemblies eines gemeinsam
ausgelieferten modularen .NET-Monolithen umgesetzt. Sie sind keine neun Dienste.
Ein kleiner Shared Kernel enthält nur tatsächlich modulübergreifende stabile
Typen; der ASP.NET-Core-Host registriert alle Module als Composition Root.

## Python-Forschung

| Bereich | Verantwortung |
|---|---|
| Contracts | gemeinsame Datenverträge, Manifeste und Enumcodes |
| Data | Import, Qualität, kanonische Daten, Rollover und Datasets |
| Research | Features, Kandidaten, Labels und historische Simulation |
| Modeling | Training, Evaluation, MLflow, ONNX und Modellpakete |
| Jobs | bekannte reproduzierbare CLI-Abläufe |

Diese Bereiche bilden ein einziges installierbares Python-Paket und keine
eigenständigen Dienste. Offizielle Abläufe werden über eine kleine CLI mit
bekannten Befehlen gestartet. Eine versionierte Laufkonfiguration wird früh
validiert. Notebooks dürfen erkunden und visualisieren, enthalten aber nie die
einzige offizielle Logik. Nur notwendige Ergebnisse werden dauerhaft
gespeichert.

## Angular

- System- und Verbindungszustand;
- Risiko, Kontostand, Positionen und Orders;
- Entscheidungen und Ablehnungsgründe;
- Modellversionen und Freigabestufen;
- Backtest-, Paper- und Live-Vergleiche;
- Datenqualität, Warnungen und Not-Aus.

### Spätere Forschungsoberfläche

Nach Stabilisierung der skriptbaren Trainingsabläufe kann Angular in der
Trainingsumgebung zusätzlich Datenstände, Trainingskonfigurationen, Aufträge,
Fortschritt, Logs, Experimente und Prüfberichte bedienen.

Die Oberfläche kommuniziert ausschließlich mit einem Training Orchestrator und
führt keine beliebigen Python-Befehle aus. Datenkauf, Modellpromotion und
Echtgeldaktivierung bleiben getrennte bestätigungspflichtige Aktionen.

Training Orchestrator und Forschungsoberfläche werden ausschließlich in der
isolierten Trainingszone ausgeliefert. Test und Produktion besitzen weder
diese Komponenten noch einen direkten Zugriff auf MLflow.

## Training Orchestrator

Der später ergänzbare Orchestrator:

- nimmt bekannte, versionierte Auftragstypen entgegen;
- startet Import-, Dataset-, Training-, Backtest- und Evaluationsläufe;
- verwaltet Status, Abbruch, Logs und Ressourcen;
- speichert Konfiguration, Codeversion, Umgebung und Zufallsstartwerte;
- veröffentlicht Forschungsresultate in MLflow und referenziert
  plattformspezifische Ergebnisse;
- besitzt keine Broker- oder Echtgeldberechtigung.

## Modellartefakt

Ein Artefakt enthält mindestens ONNX-Datei, Prüfsumme, Modell-ID, Feature-Vertrag, unterstützte Märkte, Datensatz-ID, Testbericht, Freigabestatus und bekannte Einschränkungen.

## Speicherzuordnung

- PostgreSQL: getrennte Bereiche für MLflow-Metadaten sowie
  Plattformmetadaten, Entscheidungen, Risiko, Orders, Positionen,
  Reconciliation und Audit;
- Parquet: Marktdaten, Features, Labels, Trainingsstände und große
  Backtestergebnisse;
- Datei-/Objektablage: Originaldownloads, ONNX-Artefakte, Manifeste und
  Berichte;
- Secret Store: Broker-, Datenanbieter-, Datenbank- und Backup-Zugänge;
- Git: Code, Verträge, Konfigurationen, Migrationen und kleine Golden Samples.

Die Plattformbereiche werden in die fachlichen Schemas `market`, `strategy`,
`model`, `risk`, `trading`, `execution` und `operations` gegliedert. IDs
verwenden UUID Version 7; Zustände und Ereignistypen werden im C#-Fachcode als
stabile Enums modelliert.

Die verbindliche Speicherzuordnung und Aufbewahrung beschreibt
[Speicher und Datenhaltung](./Storage.md).

## Physische Codegrenzen

V1 verwendet eine .NET-Solution mit einem ausführbaren Host, einem kleinen
Shared Kernel, neun fachlichen Modulprojekten, einer gemeinsamen Testbibliothek
sowie getrennten Unit- und Integrationstestprojekten. Alle Produktionsprojekte
werden gemeinsam als ein modularer Monolith ausgeliefert. Infrastruktur wird
nah an ihrer fachlichen Grenze gehalten; EF Core wird ohne generische
Repository- oder Unit-of-Work-Hülle verwendet. Python startet als ein
installierbares Paket mit den fünf genannten Bereichen.

Weitere Services, Assemblies, Frameworkschichten und allgemeine Abstraktionen
entstehen nur für einen realen zweiten Anwendungsfall, eine notwendige
Sicherheitsgrenze oder einen gemessenen Nutzen. Für beide Sprachen gelten
fachlich benannte, möglichst lineare Abläufe, frühe Validierung, sichtbare
Fehler und Tests des beobachtbaren Verhaltens. Einfachheit darf Risiko-,
Schutz-, Reconciliation-, Audit-, Datenqualitäts- oder Paritätsfunktionen nicht
entfernen.
