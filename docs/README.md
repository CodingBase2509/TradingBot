# Trading-KI-Plattform – Projektdokumentation

**Stand:** 27. Juli 2026  
**Status:** Planung, noch keine Implementierung  
**Zweck:** Gemeinsame Wissens- und Entscheidungsbasis für Entwicklung und zukünftige Chats

## Kurzfassung

Geplant ist keine einzelne, fest programmierte Trading-Strategie, sondern eine modulare **Trading-KI-Plattform**. Sie soll Marktdaten sammeln, Modelle in Python trainieren und testen, freigegebene Modelle über ONNX in .NET ausführen und Trades zunächst simuliert, später kontrolliert live ausführen.

Das Modell soll selbst wiederkehrende Marktmuster lernen und daraus Entscheidungen ableiten:

- nicht handeln, Long oder Short;
- Stop Loss (Verlustgrenze);
- Take Profit (Gewinnziel);
- geschätzte Haltedauer und später eventuell vorzeitiger Ausstieg.

In V1 bestimmt die feste Risikopolitik die Positionsgröße. Eine vom Modell
vorgeschlagene Risikofraktion ist eine spätere Erweiterung.

Die .NET-Plattform bleibt die unveränderbare Sicherheitsebene. Sie darf jede Modellentscheidung begrenzen oder ablehnen. Neue Modelle gelangen nie unmittelbar nach dem Training in den Live-Betrieb, sondern durchlaufen historische Tests, Paper Trading und eine kontrollierte Freigabe.

> Profitabilität kann nicht garantiert werden. Das Projekt optimiert auf belastbare Tests, kontrollierte Risiken, Nachvollziehbarkeit und stetige, messbare Verbesserung.

## Kanonische Dokumentstruktur

Die hier aufgeführten Dateien und Unterordner bilden die langfristige Quelle der
Wahrheit für die Projektplanung.

### Grundlagen

1. [Vision](./00_ProjectVision.md)
2. [Projektgrundsätze](./01_ProjectPrinciples.md)
3. [Glossar](./02_Glossary.md)
4. [Funktionale Anforderungen](./03_FunctionalRequirements.md)
5. [Nichtfunktionale Anforderungen](./04_NonFunctionalRequirements.md)
6. [Offene Entscheidungen](./05_OpenDecisions.md)

### Architektur

- [Übersicht](./architecture/Overview.md)
- [Komponenten](./architecture/Components.md)
- [Kommunikation](./architecture/Communication.md)
- [Deployment und Betrieb](./architecture/Deployment.md)

### Machine Learning

- [Training](./ml/Training.md)
- [Feature Engineering](./ml/FeatureEngineering.md)
- [Backtesting](./ml/Backtesting.md)
- [Evaluation](./ml/Evaluation.md)
- [Modelllebenszyklus](./ml/ModelLifecycle.md)

### Trading

- [Trading-Konzept](./trading/TradingConcept.md)
- [Risiko](./trading/RiskManagement.md)
- [Markt und Daten](./trading/MarketData.md)
- [Ausführung](./trading/Execution.md)

### Roadmap

- [Übersicht](./roadmap/Overview.md)
- [Phase 0 – Planung](./roadmap/Phase0.md)
- [Phase 1 – Plattformkern](./roadmap/Phase1.md)
- [Phase 2 – erstes ML-Modell](./roadmap/Phase2.md)

### Entscheidungen

- [ADR-001 – Python und .NET](./decisions/ADR-001-Python-And-DotNet.md)
- [ADR-002 – ONNX](./decisions/ADR-002-ONNX.md)
- [ADR-003 – Futures](./decisions/ADR-003-Futures-Target-Markets.md)
- [ADR-004 – Interactive Brokers](./decisions/ADR-004-Interactive-Brokers.md)
- [ADR-005 – V1-Produktumfang](./decisions/ADR-005-V1-Product-Scope.md)
- [ADR-006 – Handels- und Risikopolitik](./decisions/ADR-006-V1-Trading-And-Risk-Policy.md)
- [ADR-007 – V1-Lernziel](./decisions/ADR-007-V1-Learning-Objective.md)
- [ADR-008 – V1-Marktdatenauflösung](./decisions/ADR-008-V1-Market-Data-Resolution.md)
- [ADR-009 – MES-Historie und Rollover](./decisions/ADR-009-MES-History-And-Rollover.md)
- [ADR-010 – Datenqualität und Lückentoleranz](./decisions/ADR-010-V1-Data-Quality-And-Gaps.md)
- [ADR-011 – V1-Backtest-Kostenmodell](./decisions/ADR-011-V1-Backtest-Cost-Model.md)
- [ADR-012 – V1-Modellevaluation](./decisions/ADR-012-V1-Model-Evaluation-And-Promotion.md)
- [ADR-013 – Databento für historische Daten](./decisions/ADR-013-Databento-Historical-Data.md)
- [ADR-014 – Datenimport, Aufbewahrung und Backup](./decisions/ADR-014-Data-Import-Retention-And-Backup.md)
- [ADR-015 – Börsenkalender und Handelszeiten](./decisions/ADR-015-Exchange-Calendar.md)
- [ADR-016 – Umgebungen und modularer Monolith](./decisions/ADR-016-Environments-And-Modular-Monolith.md)
- [ADR-017 – Speicherarchitektur und Git-Versionierung](./decisions/ADR-017-Storage-And-Version-Control.md)

## Beschlossener Rahmen

| Bereich | Aktueller Stand |
|---|---|
| Ziel | Modulare, KI-gestützte Trading-Plattform |
| Handelsstil | Day-Trading; noch kein extremes Scalping |
| Haltedauer | vom Modell geschätzt; spätestens Freitagsschließung |
| Richtungen | Long und Short |
| Langfristiger Zielmarkt | Futures |
| V1-Markt | ausschließlich Micro E-mini S&P 500 (MES) im Paper Trading |
| Weitere Trainingsmärkte | MNQ, MGC und M6E als erste Kandidaten |
| Live-Plattform | .NET |
| Forschung und Training | Python |
| Modellausführung | ONNX direkt in .NET |
| Oberfläche | Angular |
| Broker | Interactive Brokers für V1-Paper und bevorzugt für späteres Echtgeld |
| Historische Daten | Databento vorläufig; vollständig TBBO + OHLCV-1m, MBP-1 nur als Stichprobe |
| Sicherheitsgrenzen | fest in .NET, nicht vom Modell veränderbar |
| Verbesserung | offline trainieren, vergleichen, stufenweise freigeben |
| News | spätere Ausbaustufe |
| Live-Handel | außerhalb des V1-Umfangs |

„Voraussichtlich“ und „Kandidat“ bedeuten, dass die Entscheidung vor Beschaffung oder Implementierung noch geprüft werden muss.

## System in einem Ablauf

```text
Historische und aktuelle Marktdaten
                ↓
       Python: Training und Tests
                ↓
        neues Modell als Kandidat
                ↓
 historische Tests auf unbekannten Daten
                ↓
    Shadow Mode und Paper Trading
                ↓
 Vergleich mit dem aktuellen Modell
                ↓
       kontrollierte Freigabe
                ↓
 .NET: sichere Ausführung über den Broker
                ↓
 Ergebnisse werden vollständig gespeichert
                ↓
 Daten fließen in spätere Trainingszyklen ein
```

## Nutzung in einem neuen Chat

Am besten diese Datei zusammen mit dem jeweils betroffenen Fachdokument bereitstellen. Als kurzer Startkontext genügt:

> Wir planen eine modulare Trading-KI-Plattform gemäß dieser Dokumentation. Bitte behandle als beschlossen nur Punkte, die dort so markiert sind. Erkläre ML- und Trading-Fachbegriffe kurz und verständlich. Wir befinden uns noch in der Planung und schreiben erst Code, nachdem Produkt-, Fach-, Daten-, Sicherheits- und Architekturentscheidungen ausreichend konkret sind.

## Pflege der Dokumentation

- Jede wesentliche Entscheidung erhält Datum, Begründung und Status.
- Vorschläge werden nicht stillschweigend zu Beschlüssen.
- Änderungen an Daten, Features, Training und Tests werden versioniert.
- Zahlen zu Risiko oder Modellfreigabe sind bis zur Validierung Arbeitswerte.
- Technische Details folgen erst nach dem fachlichen Konzept.
