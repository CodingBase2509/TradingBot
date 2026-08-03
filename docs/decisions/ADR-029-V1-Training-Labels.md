# ADR-029: Historische Trainingslabels der V1

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Der Candidate Scorer benötigt realistische Netto-R- und Haltedauerlabels mit korrekter Behandlung künstlicher Schließungen.

## Entscheidung

- Labels verwenden ausführbare Bid-/Ask-Seiten, Reaktionszeit, Gebühren, Spread, Slippage und konservative mehrdeutige Ausgänge.
- Echtes Netto-R bleibt erhalten; ein getrennt begrenztes Ziel dient dem Training.
- P50 und P90 beziehen sich auf aktive Marktzeit. Freitag, Full-Stop, technische Schließung, Datenende und ungültige Daten werden ausdrücklich als abgeschnitten oder ungültig gekennzeichnet.

## Begründung

Kostenrealistische Labels und Censoring verhindern optimistische Ergebnisse und falsche Haltedauerziele.

## Folgen

- Label Generator und Backtest teilen Ausführungs-, Kosten- und Kalenderlogik.

## Verbindliche Dokumentation

- [Backtesting](../ml/Backtesting.md)
- [Training](../ml/Training.md)
