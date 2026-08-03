# ADR-031: Schlanke physische Struktur der .NET-Plattform

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Die fachlichen .NET-Module benötigen eine einfache physische Struktur ohne eine Assembly oder einen Dienst je Modul.

## Entscheidung

- V1 startet mit einer Solution, einem ausführbaren Plattformprojekt und einem kompakten Testprojekt.
- Die neun Module werden durch Ordner, Namespaces, interne Sichtbarkeit und Architekturtests getrennt.
- Angular wird gebaut und vom Plattformhost ausgeliefert; zusätzliche Frameworkschichten und Dienste benötigen konkreten Nutzen.

## Begründung

Die Struktur wahrt Modulgrenzen bei geringem Navigations-, Build- und Betriebsaufwand.

## Folgen

- EF Core wird direkt ohne generische Repository- oder Unit-of-Work-Hülle verwendet.

## Verbindliche Dokumentation

- [Components](../architecture/Components.md)
- [Deployment](../architecture/Deployment.md)
