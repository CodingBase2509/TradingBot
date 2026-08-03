# ADR-007: Lernziel und historische Handelsalternativen der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Die Lernaufgabe musste so formuliert werden, dass sie Kosten, NoTrade, adaptive Ausstiege und feste Risikogrenzen korrekt trennt.

## Entscheidung

- Ein gemeinsamer Scorer bewertet adaptive Long- und Short-Kandidaten gegen NoTrade.
- Er schätzt erwartetes Netto-R sowie P50 und P90 der aktiven Haltedauer.
- Nur Kandidaten mit Netto-Risk-to-Reward von mindestens 1:1 gelangen zur Bewertung; die Plattform, nicht das Modell, bestimmt das Risiko.

## Begründung

Die Bewertung konkreter Alternativen ist besser prüfbar als eine freie Preisvorhersage und hält Kontoentscheidungen aus dem Modell heraus.

## Folgen

- Historische und laufende Kandidatenerzeugung müssen identisch sein.
- Die Entscheidungsschwelle wird auf Validierungsdaten gewählt.

## Verbindliche Dokumentation

- [Training](../ml/Training.md)
- [Backtesting](../ml/Backtesting.md)
- [TradingConcept](../trading/TradingConcept.md)
