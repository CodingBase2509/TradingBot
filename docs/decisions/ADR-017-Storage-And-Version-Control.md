# ADR-017: Speicherarchitektur und Git-Versionierung

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Operative Zustände, große Analysedaten, Artefakte, Secrets und Quellstände haben unterschiedliche Speicheranforderungen.

## Entscheidung

- PostgreSQL speichert operative Daten, Parquet große analytische Daten und das Dateisystem Originale sowie Artefakte.
- Git enthält Code, Verträge, Migrationen, kleine Konfigurationen und Golden Samples, aber keine großen Daten, Datenbanken, Pakete oder Secrets.
- IDs, Manifeste und Prüfsummen verbinden Code, Daten, Lauf und Modell.

## Begründung

Die Aufteilung minimiert Infrastruktur und behält Abfragen, Reproduzierbarkeit und große Zeitreihen jeweils in einem passenden Speicher.

## Folgen

- Jede Zone besitzt ihre eigene Datenhoheit und eigene Backups.

## Verbindliche Dokumentation

- [Storage](../architecture/Storage.md)
