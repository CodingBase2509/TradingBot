# Trading-KI-Plattform – Projektdokumentation

**Stand:** 27. Juli 2026  
**Status:** Planung, noch keine Implementierung  
**Zweck:** Gemeinsame Wissens- und Entscheidungsbasis für Entwicklung und zukünftige Chats

## Kurzfassung

Geplant ist keine einzelne, fest programmierte Trading-Strategie, sondern eine modulare **Trading-KI-Plattform**. Sie soll Marktdaten sammeln, Modelle in Python trainieren und testen, freigegebene Modelle über ONNX in .NET ausführen und Trades zunächst simuliert, später kontrolliert live ausführen.

Das Modell soll selbst wiederkehrende Marktmuster lernen und daraus Entscheidungen ableiten:

- nicht handeln, Long oder Short;
- gewünschtes Risiko beziehungsweise Positionsgröße;
- Stop Loss (Verlustgrenze);
- Take Profit (Gewinnziel);
- maximale Haltedauer und später eventuell vorzeitiger Ausstieg.

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

## Beschlossener Rahmen

| Bereich | Aktueller Stand |
|---|---|
| Ziel | Modulare, KI-gestützte Trading-Plattform |
| Handelsstil | Day-Trading; noch kein extremes Scalping |
| Haltedauer | typischerweise 30 Minuten bis 8 Stunden, toleriert bis ungefähr 24 Stunden |
| Richtungen | Long und Short |
| Langfristiger Zielmarkt | Futures |
| Erster Paper-Trading-Markt | voraussichtlich Micro E-mini S&P 500 (MES) |
| Weitere Trainingsmärkte | MNQ, MGC und M6E als erste Kandidaten |
| Live-Plattform | .NET |
| Forschung und Training | Python |
| Modellausführung | ONNX direkt in .NET |
| Oberfläche | Angular |
| Sicherheitsgrenzen | fest in .NET, nicht vom Modell veränderbar |
| Verbesserung | offline trainieren, vergleichen, stufenweise freigeben |
| News | spätere Ausbaustufe |

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
