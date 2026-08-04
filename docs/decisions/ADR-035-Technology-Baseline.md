# ADR-035: Technologie-Baseline der V1

- **Status:** beschlossen mit offenen Bibliotheksdetails
- **Datum:** 3. August 2026

## Kontext

Die Implementierung benötigt moderne, unterstützte Technologien, ohne durch
„immer aktuell“ reproduzierbare Builds oder sichere Updates zu verlieren.

## Entscheidung

- Die Plattform verwendet .NET, ASP.NET Core und EF Core 10 sowie Npgsql 10.x.
- Das Frontend verwendet Angular 22 mit Node.js 24 LTS und npm.
- Forschung verwendet CPython 3.14 und pip.
- ONNX wird in .NET direkt mit `Microsoft.ML.OnnxRuntime` ausgeführt.
- Container verwenden Docker und die aktuelle Compose Specification ohne alte
  `version: "3.7"`-Deklaration.
- Parquet-/DataFrame-Bibliotheken werden vor der Implementierung anhand eines
  gemeinsamen Python-/NET-Testdatensatzes ausgewählt.

## Begründung

LTS- beziehungsweise aktiv unterstützte Produktlinien geben einen stabilen
Startpunkt. Exakt fixierte Builds und bewusst geprüfte Updates verbinden
Aktualität mit Reproduzierbarkeit. Die Compose Specification ersetzt die
veralteten 2.x-/3.x-Dateiformate.

## Folgen

- Exakte Patch- und Paketversionen werden in Repository und Images fixiert.
- Major-Upgrades erfolgen nicht automatisch.
- ML.NET wird nur ergänzt, wenn eine konkrete Funktion über direkte ONNX-
  Inferenz hinaus benötigt wird.

## Verbindliche Dokumentation

- [Technologie-Baseline](../architecture/TechnologyStack.md)
- [Deployment und Betrieb](../architecture/Deployment.md)
- [Kommunikation](../architecture/Communication.md)
