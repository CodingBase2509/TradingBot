# ADR-015: Börsenkalender und Handelszeiten

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Feste Uhrzeiten bilden Feiertage, verkürzte Sitzungen, Wartungspausen und Vertragsänderungen nicht zuverlässig ab.

## Entscheidung

- Ein versionierter offizieller CME-Kalender ist die fachliche Quelle für Handelstage und Sitzungsgrenzen.
- IBKR dient im Betrieb als Gegenprüfung, überschreibt aber nicht stillschweigend die Version des Fachkalenders.
- Alle Einstiegs- und Freitagsschlusszeiten werden relativ zu diesem Kalender berechnet.

## Begründung

Ein versionierter Kalender hält Backtest und Laufzeit deterministisch und behandelt Sondertage korrekt.

## Folgen

- Kalenderabweichungen blockieren im Zweifel neue Trades.

## Verbindliche Dokumentation

- [MarketData](../trading/MarketData.md)
- [Execution](../trading/Execution.md)
