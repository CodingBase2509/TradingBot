# ADR-038: Parquet- und DataFrame-Bibliotheken der V1

- **Status:** beschlossen
- **Datum:** 3. August 2026

## Kontext

Python und .NET müssen dieselben Parquet-Daten zuverlässig austauschen. Die
Plattform soll dabei nicht mehrere DataFrame-Systeme oder native Abhängigkeiten
ohne nachgewiesenen Nutzen einführen.

## Entscheidung

- Python verwendet PyArrow für Parquet, Datasets und Schemas.
- Python verwendet pandas als V1-DataFrame für Forschung und ML-Vorbereitung.
- .NET verwendet Parquet.Net für Parquet-I/O.
- Die .NET-Verarbeitung verwendet typisierte Records, Arrays und Batches statt
  einer allgemeinen DataFrame-Bibliothek.
- Polars und ParquetSharp werden nur bei einem gemessenen Zeit-, Speicher- oder
  Durchsatzproblem geprüft.

## Begründung

PyArrow bietet die etablierte Apache-Arrow-/Parquet-Anbindung und integriert
mit pandas. Parquet.Net ist vollständig verwaltet und vermeidet zusätzliche
native Laufzeitabhängigkeiten. Eine einzelne DataFrame-Welt in Python und
explizite Typen in .NET halten die V1 übersichtlich.

## Folgen

- Ein gemeinsamer Golden-Datensatz prüft Schema und Roundtrip zwischen beiden
  Sprachen.
- Exakte Patchversionen und das Dateiprofil werden beim Projektstart fixiert.
- Eine spätere Optimierung benötigt Messwerte und einen identischen
  Ergebnisvergleich.

## Verbindliche Dokumentation

- [Technologie-Baseline](../architecture/TechnologyStack.md)
- [Speicher und Datenhaltung](../architecture/Storage.md)
