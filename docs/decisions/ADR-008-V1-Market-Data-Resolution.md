# ADR-008: Marktdatenauflösung der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Features und realistische Ausführungssimulation benötigen unterschiedliche, aber zusammenhängende Auflösungen.

## Entscheidung

- TBBO ist die bevorzugte ereignisnahe Rohbasis; OHLCV-1m ergänzt und prüft Aggregationen.
- Kanonische 1-Minuten-Daten erzeugen deterministisch die 5-, 15- und 60-Minuten-Sichten.
- Entscheidungen entstehen auf abgeschlossenen 5-Minuten-Kerzen; Ereignisdaten bestimmen realistische Ausführungen.

## Begründung

Die Trennung hält Features kompakt, ohne die Reihenfolge von Stop, Ziel und Ausführung unnötig zu verschleiern.

## Folgen

- Geringere historische Auflösung wird gekennzeichnet und konservativ bewertet.

## Verbindliche Dokumentation

- [MarketData](../trading/MarketData.md)
- [FeatureEngineering](../ml/FeatureEngineering.md)
- [Backtesting](../ml/Backtesting.md)
