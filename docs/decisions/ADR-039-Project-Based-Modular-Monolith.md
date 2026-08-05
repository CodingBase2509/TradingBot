# ADR-039: Projektbasierter modularer .NET-Monolith

- **Status:** beschlossen
- **Datum:** 4. August 2026

## Kontext

Die fachlichen Modulgrenzen sollen bereits durch den Compiler sichtbar und
prüfbar sein. Eine reine Ordner- und Namespacetrennung bietet dafür zu wenig
Schutz, während eigene Schichtprojekte je Modul unnötigen Aufwand erzeugen
würden.

## Entscheidung

- Die Plattform bleibt ein gemeinsam ausgelieferter und betriebener modularer
  Monolith mit einem ASP.NET-Core-Host.
- Der kleine Shared Kernel `TradingPlatform.Platform` enthält ausschließlich
  stabile, tatsächlich modulübergreifende Identitäten, Verträge und technische
  Grundlagen und referenziert kein fachliches Modul.
- Jedes der neun fachlichen Module wird als eigenes Projekt geführt und
  referenziert den Shared Kernel. Weitere Modulreferenzen entstehen nur für
  konkrete fachliche Abhängigkeiten.
- Der Host referenziert und registriert alle Module als Composition Root.
- Innerhalb eines Moduls entstehen keine zusätzlichen Domain-, Application-
  oder Infrastructure-Projekte ohne nachgewiesenen Nutzen.
- `TradingPlatform.Tests` ist eine nicht ausführbare gemeinsame Testbibliothek.
  Unit- und Integrationstests bleiben getrennte ausführbare Testprojekte.
- Projektverweise und öffentliche Moduloberflächen sichern die
  Abhängigkeitsrichtung; allgemeine Architekturtest-Frameworks werden dafür
  nicht eingeführt.

## Begründung

Projektgrenzen verhindern unzulässige Abhängigkeiten bereits beim Kompilieren
und halten die fachlichen Verantwortungen sichtbar. Der gemeinsame Host erhält
gleichzeitig den einfachen Build-, Deployment- und Betriebsweg eines
Monolithen.

## Folgen

- Öffentliche Typen eines Moduls bilden eine bewusste Schnittstelle; übrige
  Implementierungen bleiben möglichst intern.
- Der Shared Kernel darf nicht zu einer allgemeinen Ablage für fachliche
  Modelle, Hilfsklassen oder Infrastruktur werden.
- EF Core wird weiterhin direkt und ohne generische Repository- oder
  Unit-of-Work-Hülle verwendet.
- Neue Projekte benötigen eine konkrete fachliche oder technische Grenze.

## Ersetzt

- [ADR-031: Schlanke physische Struktur der .NET-Plattform](./ADR-031-Lean-DotNet-Platform-Structure.md)

## Verbindliche Dokumentation

- [Components](../architecture/Components.md)
- [Deployment](../architecture/Deployment.md)
