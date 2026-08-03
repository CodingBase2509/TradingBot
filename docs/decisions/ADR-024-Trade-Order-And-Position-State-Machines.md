# ADR-024: Zustandsmaschinen für Trade, Order und Position

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Teilfüllungen, Schutzorders, Stornierungsrennen und Neustarts lassen sich nicht sicher in einem einzigen Status abbilden.

## Entscheidung

- Trade, Order und Position besitzen getrennte, persistierte Lebenszyklen.
- Zustandswechsel erfolgen nur durch definierte, idempotente Ereignisse; Brokerfills sind die Quelle für tatsächliche Positionen.
- Nach Neustart werden Zustände geladen, neue Trades blockiert und anschließend Orders, Fills, Positionen und Schutz mit Broker oder Simulation abgeglichen.

## Begründung

Getrennte Zustände vermeiden Mehrdeutigkeit zwischen Handelsabsicht, Brokerorder und tatsächlicher Position.

## Folgen

- Unmögliche Übergänge blockieren den kleinsten sicheren Bereich und erzeugen Audit sowie Alarm.

## Verbindliche Dokumentation

- [Execution](../trading/Execution.md)
- [Components](../architecture/Components.md)
