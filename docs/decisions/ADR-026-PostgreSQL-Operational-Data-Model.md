# ADR-026: Operatives PostgreSQL-Datenmodell

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Operative Plattformzustände benötigen stabile Identitäten, fachliche Grenzen und eindeutige Persistenzregeln.

## Entscheidung

- PostgreSQL wird in die Schemas market, strategy, model, risk, trading, execution und operations gegliedert.
- C# verwendet Guid mit UUID Version 7 sowie stabile Enums für Zustände und Ereignisse; UTC, Decimal und Tickwerte verhindern mehrdeutige Zahlen.
- Audit und Ereignisse sind append-only, Vorgänge idempotent; große analytische Daten bleiben in Parquet oder Dateien.

## Begründung

Fachliche Schemas halten Beziehungen sichtbar, ohne früh mehrere Datenbanken oder eine generische Persistenzschicht einzuführen.

## Folgen

- Spalten, Indizes und Constraints werden proportional zur jeweiligen Implementierungsphase konkretisiert.

## Verbindliche Dokumentation

- [Storage](../architecture/Storage.md)
- [05_OpenDecisions](../05_OpenDecisions.md)
