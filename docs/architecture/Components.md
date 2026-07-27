# Komponenten

## .NET-Plattform

| Komponente | Verantwortung |
|---|---|
| Market Data Service | Anbieter anbinden, Daten vereinheitlichen und Qualität prüfen |
| Feature Engine | versionierte Modelleingaben berechnen |
| Model Runtime | freigegebene ONNX-Modelle laden und ausführen |
| Risk Guard | unveränderbare Handels-, Konto- und Technikgrenzen prüfen |
| Trade Controller | Modellausgabe in gültige Ordergrößen und Ordertypen übersetzen |
| Broker Adapter | brokerabhängige API kapseln |
| Position Manager | Orders, Ausführungen, Stop, Ziel und Haltedauer verwalten |
| Data Recorder | Entscheidungen, Prüfungen, Orders, Kosten und Ergebnisse speichern |
| Model Manager | Kandidaten, aktive Version, Promotion und Rollback verwalten |
| Backtest Core | gemeinsame Ausführungs- und Positionslogik für Simulation |
| Monitoring | Datenalter, Verbindungen, Modelllaufzeit und Abweichungen überwachen |

## Python-Forschung

| Komponente | Verantwortung |
|---|---|
| Data Import & Quality | historische Daten importieren, prüfen und vereinheitlichen |
| Dataset Builder | unveränderliche Trainingsstände und Features erzeugen |
| Label Generator | historische Entscheidungsvarianten realistisch bewerten |
| Training | Modelle trainieren und reproduzierbar speichern |
| Evaluation | unbekannte Daten, Robustheit und Vergleichsmodelle prüfen |
| Export | kompatible Kandidaten als vollständige Artefakte bereitstellen |
| Experiment Registry | Daten-, Feature-, Code-, Modell- und Testversion verbinden |

## Angular

- System- und Verbindungszustand;
- Risiko, Kontostand, Positionen und Orders;
- Entscheidungen und Ablehnungsgründe;
- Modellversionen und Freigabestufen;
- Backtest-, Paper- und Live-Vergleiche;
- Datenqualität, Warnungen und Not-Aus.

## Modellartefakt

Ein Artefakt enthält mindestens ONNX-Datei, Prüfsumme, Modell-ID, Feature-Vertrag, unterstützte Märkte, Datensatz-ID, Testbericht, Freigabestatus und bekannte Einschränkungen.
