# ADR-011: Kosten- und Ausführungsmodell im V1-Backtest

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Ein Backtest ohne Marktseite, Kosten und Ausführungsunsicherheit würde die Handelsqualität systematisch überschätzen.

## Entscheidung

- Long verwendet Ask beim Einstieg und Bid beim Ausstieg, Short spiegelbildlich.
- Gebühren, Spread, Reaktionszeit und mindestens ein Tick Basisslippage je relevante Orderseite werden versioniert berücksichtigt.
- Stressstufen verschlechtern Slippage; unklare Stop-/TP-Reihenfolge wird konservativ behandelt.

## Begründung

Konservative ausführbare Preise machen Ergebnisse näher an Paper und späterem Live-Betrieb.

## Folgen

- Backtest, Labels und Paper-Auswertung teilen dasselbe Kostenmodell.

## Verbindliche Dokumentation

- [Backtesting](../ml/Backtesting.md)
