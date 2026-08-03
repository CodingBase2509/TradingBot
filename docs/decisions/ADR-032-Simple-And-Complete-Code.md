# ADR-032: Einfacher, verständlicher und vollständiger Code

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Finanz- und sicherheitsrelevanter Code muss vollständig sein, ohne durch unnötige Patterns und Abstraktionen schwer lesbar zu werden.

## Entscheidung

- Python und C# verwenden die kleinste verständliche Implementierung, die Funktion, Sicherheit, Fehlerbehandlung und Tests vollständig erhält.
- Abstraktionen entstehen erst durch einen realen zweiten Anwendungsfall, Test- oder Sicherheitsbedarf oder eine nachgewiesene wiederkehrende Änderung.
- Fachliche Namen, früher Kontrollfluss und sichtbare Fehler werden cleveren Kurzformen und allgemeinen Frameworkschichten vorgezogen.

## Begründung

Verständlichkeit reduziert Fehler und Wartungsaufwand; notwendige Sicherheitskomplexität bleibt ausdrücklich sichtbar.

## Folgen

- Risiko, Schutz, Reconciliation, Audit, Datenqualität, Isolation und Parität dürfen nicht vereinfacht entfallen.

## Verbindliche Dokumentation

- [01_ProjectPrinciples](../01_ProjectPrinciples.md)
- [04_NonFunctionalRequirements](../04_NonFunctionalRequirements.md)
- [Components](../architecture/Components.md)
