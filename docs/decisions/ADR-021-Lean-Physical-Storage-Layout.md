# ADR-021: Schlanke physische Speicherstruktur

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Die logischen Datenstufen benötigen eine einfache physische Ablage ohne tiefe Ordnerhierarchien.

## Entscheidung

- Jede Zone beginnt mit den fünf Bereichen raw, market, datasets, models und temp.
- Offizielle Dateien werden vollständig temporär geschrieben, geprüft und atomar veröffentlicht.
- Weitere Partitionierung entsteht nur bei fachlichem Bedarf oder gemessenen Größen- und Laufzeitproblemen.

## Begründung

Eine flache, stabile Struktur ist leicht zu sichern und zu verstehen, ohne spätere Skalierung auszuschließen.

## Folgen

- PostgreSQL hält Metadaten und Zustände; große Inhalte bleiben in Dateien.

## Verbindliche Dokumentation

- [Storage](../architecture/Storage.md)
- [Deployment](../architecture/Deployment.md)
