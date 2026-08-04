# ADR-036: Versionierte und typisierte Konfiguration

- **Status:** beschlossen
- **Datum:** 3. August 2026

## Kontext

Mehrere Umgebungen, Konten, Instrumente und Strategy Instances benötigen
veränderbare Einstellungen, ohne Sicherheitsgrenzen, Herkunft und verwendete
Werte unklar werden zu lassen.

## Entscheidung

- Plattform, Konto, Instrument, Strategy, Candidate Generator und Research Run
  besitzen getrennte Konfigurationsverträge.
- Jeder dauerhaft gespeicherte fachliche Typ erhält eine eigene Tabelle statt
  einer generischen Konfigurationstabelle.
- Aktive Konfigurationen sind unveränderlich; Änderungen erzeugen versionierte
  Nachfolger mit UUID Version 7, Herkunft und Prüfsumme.
- JSON und JSON Schema sind die gemeinsamen Dateiverträge.
- Bootstrapwerte liegen read-only im Container, Secrets ausschließlich in
  Environment Variables.
- Untergeordnete Risikoangaben dürfen globale Grenzen nur verschärfen.

## Begründung

Eigene typisierte Tabellen machen Beziehungen, Constraints und Zuständigkeiten
sichtbar. Unveränderliche Versionen ermöglichen reproduzierbare Entscheidungen,
Audits und sichere Wiederherstellung ohne eine flexible, schwer prüfbare
Überschreibungslogik.

## Folgen

- Jeder Trade verweist auf die tatsächlich verwendeten Versionen.
- Änderungen und Aktivierungen werden validiert und auditiert.
- Neue Konfigurationstypen benötigen einen konkreten fachlichen Zweck.

## Verbindliche Dokumentation

- [Konfigurationsvertrag](../architecture/Configuration.md)
- [Speicher und Datenhaltung](../architecture/Storage.md)
- [Deployment und Betrieb](../architecture/Deployment.md)
