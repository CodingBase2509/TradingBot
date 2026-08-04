# Trading-KI-Plattform – Projektdokumentation

**Stand:** 3. August 2026
**Status:** Planung, noch keine Implementierung

## Worum geht es?

Dieses Projekt entwickelt keine einzelne fest programmierte Strategie, sondern
eine modulare KI-gestützte Trading-Plattform.

Historische Marktdaten werden in einer isolierten Python-Umgebung aufbereitet,
untersucht und zum Trainieren von Modellen verwendet. Freigegebene Modelle
werden als geprüfte ONNX-Pakete an die .NET-Plattform übergeben. Diese
verarbeitet Marktdaten, prüft feste Risikoregeln und führt zulässige Trades
zunächst simuliert und später kontrolliert über einen Broker aus.

Das Modell kann Long, Short oder keinen Trade wählen sowie situationsbezogene
Stop-Loss-, Take-Profit- und Haltedauerwerte bewerten. Die .NET-Plattform bleibt
immer die unveränderbare Sicherheitsinstanz und darf jede Modellentscheidung
ablehnen.

## Aktueller Rahmen

| Bereich | Stand |
|---|---|
| Projektphase | Planung und fachliche Spezifikation |
| V1-Markt | Micro E-mini S&P 500 Future (MES) |
| V1-Betrieb | Paper Trading, kein Echtgeld |
| Broker | Interactive Brokers |
| historische Daten | vorläufig Databento |
| Training und Forschung | Python |
| Trading-Plattform | .NET mit Angular |
| Modellausführung | ONNX direkt in .NET |
| Sicherheit | feste, nicht vom Modell veränderbare Grenzen |

Die beschlossenen Hauptversionen stehen in der
[Technologie-Baseline](./architecture/TechnologyStack.md). Aufbau und
Versionierung der Einstellungen beschreibt der
[Konfigurationsvertrag](./architecture/Configuration.md).

V1 soll zuerst beweisen, dass Daten, Modellentscheidungen, Risiko, Orders,
Positionen und Wiederherstellung korrekt und reproduzierbar funktionieren.
Profitabilität ist eine zu prüfende Hypothese und kein Versprechen.

## Dokumentation öffnen

Das vollständige Inhaltsverzeichnis mit allen Fachseiten, Architekturthemen,
Planungsphasen und Entscheidungen befindet sich in der
[index.md](./index.md).

Für einen ersten Einstieg empfiehlt sich anschließend:

1. [Projektvision](./00_ProjectVision.md)
2. [Projektgrundsätze](./01_ProjectPrinciples.md)
3. [Trading-Konzept](./trading/TradingConcept.md)
4. [Architekturübersicht](./architecture/Overview.md)
5. [Roadmap](./roadmap/Overview.md)

> Die ausführlichen Dokumente beschreiben den aktuellen Planungsstand. Bei
> widersprüchlichen oder noch offenen Punkten gilt
> [Offene Entscheidungen](./05_OpenDecisions.md) als Hinweis, dass noch keine
> endgültige Festlegung getroffen wurde.
