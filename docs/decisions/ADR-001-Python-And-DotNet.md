# ADR-001: Python und .NET kombinieren

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Die Plattform benötigt ein starkes ML-Forschungsumfeld und zugleich einen stabilen, sicherheitskritischen Live-Kern. Der Entwicklerhintergrund liegt primär bei .NET und Angular.

## Entscheidung

- Python übernimmt Datenanalyse, Feature-Forschung, Training, Optimierung und Modellevaluation.
- .NET übernimmt Live-Daten, Inferenz, Risiko, Orders, Positionen, Broker, Audit und Betrieb.
- Python sendet keine Live-Orders.

## Begründung

Python bietet das breitere ML-Ökosystem. .NET passt zur vorhandenen Erfahrung und eignet sich für den kontrollierten Produktionskern. Die Trennung hält Forschung aus dem kritischen Live-Pfad heraus.

## Folgen

- Features benötigen einen streng versionierten, in beiden Umgebungen geprüften Vertrag.
- Modellübergabe benötigt ein standardisiertes Artefakt.
- Zwei Laufzeitumgebungen müssen reproduzierbar betrieben werden.
- ML.NET kann ergänzend genutzt werden, ist aber nicht die primäre Trainingsplattform.
