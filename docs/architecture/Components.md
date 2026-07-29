# Komponenten

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

Backtest, Shadow und Paper verwenden diese Module gemeinsam. Austauschbar sind
nur Uhr, Marktdatenquelle und Ausführungsadapter. Die vollständigen Grenzen und
Kommunikationsregeln stehen in
[ADR-022](../decisions/ADR-022-DotNet-Module-Boundaries.md).

Feature & Intelligence sowie Decision werden je Strategy Instance betrieben.
Marktdaten, Account Risk, Execution Router, Brokeradapter, Reconciliation,
Model Management und Operations besitzen gemeinsame Plattform- oder
Kontosichten. Details regelt
[ADR-023](../decisions/ADR-023-Multi-Strategy-Runtime.md).

Trade Management, Execution und Reconciliation verwenden getrennte Trade-,
Order- und Positionszustände gemäß
[ADR-024](../decisions/ADR-024-Trade-Order-And-Position-State-Machines.md).

Die neun Module werden in V1 als Ordner und Namespaces eines ausführbaren
.NET-Plattformprojekts umgesetzt. Sie sind keine neun Dienste oder
zwangsläufig neun Assemblies. Die physische Minimalstruktur regelt
[ADR-031](../decisions/ADR-031-Lean-DotNet-Platform-Structure.md).

## Python-Forschung

| Bereich | Verantwortung |
|---|---|
| Contracts | gemeinsame Datenverträge, Manifeste und Enumcodes |
| Data | Import, Qualität, kanonische Daten, Rollover und Datasets |
| Research | Features, Kandidaten, Labels und historische Simulation |
| Modeling | Training, Evaluation, MLflow, ONNX und Modellpakete |
| Jobs | bekannte reproduzierbare CLI-Abläufe |

Diese Bereiche bilden ein einziges installierbares Python-Paket und keine
eigenständigen Dienste. Details regelt
[ADR-030](../decisions/ADR-030-Lean-Python-Research-Architecture.md).

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
