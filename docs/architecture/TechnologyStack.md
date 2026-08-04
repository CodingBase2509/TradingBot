# Technologie-Baseline

## Ziel

Die Baseline legt die Haupttechnologien und ihre Versionslinien fest. Exakte
Patchversionen werden beim Projektstart in SDK-Dateien, Paketdateien,
Lockfiles und Container-Digests festgeschrieben. „Aktuell“ bedeutet danach
nicht automatisches Aktualisieren, sondern eine bewusst geprüfte Änderung.

## Beschlossene Basis

| Bereich | V1-Baseline |
|---|---|
| Backend und Laufzeit | .NET 10 LTS und C# 14 |
| Web-API | ASP.NET Core 10 |
| Persistenz | Entity Framework Core 10 |
| PostgreSQL-Treiber | Npgsql EF Core Provider 10.x |
| Frontend | Angular 22 |
| Frontend-Laufzeit | Node.js 24 LTS |
| Paketmanager Frontend | mit Node ausgeliefertes npm; exakte Version im Lockfile und Buildimage fixiert |
| Forschung | CPython 3.14 |
| Python-Paketmanager | pip; exakte Version im Research-Image fixiert |
| Python Parquet | PyArrow |
| Python DataFrames | pandas |
| .NET Parquet | Parquet.Net |
| .NET Datenverarbeitung | typisierte Records, Arrays und Batches; kein allgemeiner DataFrame in V1 |
| Modelllaufzeit .NET | Microsoft.ML.OnnxRuntime |
| Container | Docker Engine mit Docker Compose Plugin |
| Compose-Datei | aktuelle Compose Specification ohne `version:`-Feld |
| Datenbank | PostgreSQL; genaue unterstützte Major-Version vor Implementierung fixieren |

Als geprüfter Stand vom 3. August 2026 sind .NET 10.0.10, Angular 22,
Node.js 24 LTS und Python 3.14.6 aktuelle passende Produktlinien. Diese
Versionsnummern dokumentieren den Planungszeitpunkt; Builds verwenden später
die im Repository fixierten Versionen.

Der erste Projektstand fixiert das .NET SDK auf `10.0.301`, die ASP.NET-
OpenAPI-Integration auf `10.0.10` und die sicherheitsbereinigte transitive
Microsoft.OpenApi-Version auf `2.7.5`. Das Python-Grundprojekt zielt auf
CPython `3.14.x` und fixiert zunächst pandas `3.0.3`, PyArrow `25.0.0`, pytest
`9.1.1`, Ruff `0.16.1` sowie pip `26.1.2`.

## Versionsregeln

- Produktions- und reproduzierbare Testbuilds verwenden keine floating Tags
  wie `latest`.
- .NET SDK, Python, Node, npm, NuGet- und Python-Pakete sowie Containerimages
  werden exakt auflösbar fixiert.
- Lockfiles werden in Git versioniert.
- Sicherheits- und Patchupdates innerhalb der beschlossenen Major-Version
  werden zeitnah, aber erst nach automatisierten Tests übernommen.
- Major-Upgrades benötigen eine bewusste Kompatibilitätsprüfung und bei
  architekturrelevanten Änderungen eine neue ADR.
- Training, ONNX-Export und .NET-Inferenz speichern ihre tatsächlichen
  Laufzeit- und Bibliotheksversionen im Modellmanifest.

## ONNX in .NET

Die Plattform verwendet `Microsoft.ML.OnnxRuntime` direkt für die Inferenz.
ML.NET besitzt zwar ONNX-Integrationen, wird aber nicht allein für einen Wrapper
um ONNX Runtime eingeführt. Sollte eine spätere ML.NET-Funktion einen konkreten
Nutzen bieten, kann das zusätzliche Paket gezielt ergänzt werden.

Die konkrete ONNX-Runtime-Version wird zusammen mit dem unterstützten Opset im
Projekt und im Modellpaket fixiert. Ein Upgrade erfordert die vollständige
Paritätsprüfung der Referenzfälle.

## Parquet und DataFrames

### Python

PyArrow ist die verbindliche Parquet- und Schema-Bibliothek. Sie liest und
schreibt Dateien und Datasets, ermöglicht spalten- und batchweise Verarbeitung
und bildet die Austauschgrenze zu .NET.

pandas ist der V1-DataFrame für Forschung, Features, Labels, Auswertung und die
Anbindung an das ML-Ökosystem. Polars wird nicht parallel eingeführt. Es wird
erst verglichen, wenn ein realer Lauf ein relevantes Zeit- oder
Speicherproblem nachweist.

### .NET

Parquet.Net ist die verbindliche V1-Bibliothek zum Lesen und Schreiben von
Parquet. Die Plattform verarbeitet Daten anschließend mit fachlich typisierten
C#-Records, Arrays und begrenzten Batches.

Eine allgemeine .NET-DataFrame-Schicht gehört nicht zu V1. ParquetSharp bleibt
eine mögliche leistungsorientierte Alternative, falls Messungen einen Engpass
in Parquet.Net nachweisen. Seine nativen C++-/Arrow-Abhängigkeiten werden ohne
diesen Nachweis nicht in die Container aufgenommen.

### Gemeinsame Kompatibilitätsprüfung

Ein kleiner Golden-Datensatz prüft vor dem ersten produktiven Datenimport:

- identische Schemas, UTC-Zeitstempelauflösung, Decimal, UUID, Enumcodes und
  Nullwertbehandlung;
- Lesen und Schreiben derselben Parquet-Dateien in Python und .NET;
- Streaming beziehungsweise Batch-Verarbeitung ohne unnötig hohen Speicher;
- Predicate Pushdown und spaltenweises Lesen für große Datenstände;
- ausgewählte Parquet-Formatversion und Kompression;
- identische Werte und Schemas nach Python→NET→Python-Roundtrip.

Der Test bestätigt die konkrete Version und das gemeinsame Dateiprofil, öffnet
aber nicht erneut die grundsätzliche Bibliotheksauswahl.

## Noch zu fixieren

- PostgreSQL-Major-Version und exakte Npgsql-Patchversion;
- Parquet.Net-Patchversion bei der ersten .NET-Parquet-Implementierung;
- exakte Docker-Engine- und Compose-Plugin-Version der Zielhosts;
- CPU- oder später GPU-Variante von ONNX Runtime anhand des ersten Modells.
