# ADR-020: Datenstufen und Aufbewahrung

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Zwischenergebnisse sollen nicht unnötig wachsen, offizielle Ergebnisse und ihre Herkunft dürfen aber nicht verloren gehen.

## Entscheidung

- Die Datenhaltung trennt Originaldaten, kanonische Marktdaten, eingefrorene Trainingsstände sowie Läufe und Modellpakete.
- Nur eingefrorene Datenstände dürfen offizielle Modelle und Evaluationen erzeugen.
- Automatisch gelöscht werden ausschließlich reproduzierbare, temporäre und nicht referenzierte Daten.

## Begründung

Explizite Stufen halten Speicherverbrauch klein und verhindern, dass wichtige Modellherkunft versehentlich bereinigt wird.

## Folgen

- Tradebeobachtungen einschließlich Haltedauer und Censoring bleiben dauerhaft nachvollziehbar.

## Verbindliche Dokumentation

- [Storage](../architecture/Storage.md)
