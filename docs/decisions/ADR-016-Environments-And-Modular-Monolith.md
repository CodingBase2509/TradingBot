# ADR-016: Umgebungen und modularer Plattformkern

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Training, Test und Live benötigen Isolation, sollen aber nicht durch voreilige Dienste unnötig komplex werden.

## Entscheidung

- Es gibt getrennte Zonen für Training, Test und Produktion.
- Die .NET-Plattform startet als modularer Monolith; Datenquelle, Uhr und Ausführungsadapter unterscheiden die Betriebsarten.
- Eine spätere Forschungsoberfläche darf nur bekannte reproduzierbare Jobs auslösen.

## Begründung

Isolation schützt Produktionszustände, während ein Monolith Entwicklung und Betrieb übersichtlich hält.

## Folgen

- Eine Diensttrennung benötigt später einen konkreten betrieblichen Nutzen.

## Verbindliche Dokumentation

- [Overview](../architecture/Overview.md)
- [Components](../architecture/Components.md)
- [Deployment](../architecture/Deployment.md)
