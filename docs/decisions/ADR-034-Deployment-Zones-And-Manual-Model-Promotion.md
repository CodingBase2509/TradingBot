# ADR-034: Deploymentzonen und manuelle Modellpromotion

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Lokale Entwicklung, Home-Server-Training/Test und späteres Cloud-Live benötigen ein konkretes, isoliertes Betriebsbild.

## Entscheidung

- Es gibt zwei eigene Images: trading-research und trading-platform. Training und Test bleiben auf dem Home-Server in vollständig getrennten Zonen; Live läuft später dediziert in der Cloud.
- Modellpakete werden unverändert und manuell von Training nach Test und später nach Live kopiert, dort vollständig geprüft und registriert.
- Nur eine bewusste UI-Auswahl eines verfügbaren Pakets erzeugt eine Strategy Instance; Entdeckung aktiviert nie Trading.

## Begründung

Physische und manuelle Freigabegrenzen sind für den Start verständlich, sicher und ohne unnötige Plattformautomatisierung umsetzbar.

## Folgen

- Secrets kommen per Environment Variables; Backups bleiben hostgesteuert; Python und Trainingsdaten fehlen in Live.

## Verbindliche Dokumentation

- [Deployment](../architecture/Deployment.md)
- [Storage](../architecture/Storage.md)
- [ModelLifecycle](../ml/ModelLifecycle.md)
