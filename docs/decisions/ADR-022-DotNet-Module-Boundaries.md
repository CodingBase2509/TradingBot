# ADR-022: Modulgrenzen des .NET-Plattformkerns

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Der Plattformkern benötigt klare Verantwortungen, ohne daraus frühzeitig verteilte Dienste zu machen.

## Entscheidung

- Der Kern besteht fachlich aus Market, Feature & Intelligence, Decision, Risk Guard, Trade Management, Execution, Reconciliation, Model Management sowie Operations & Audit.
- Backtest, Shadow und Paper verwenden dieselben Module; nur Uhr, Daten- und Ausführungsadapter wechseln.
- Direkte typisierte Kommunikation ist der V1-Standard.

## Begründung

Die Grenzen machen den Handelsweg nachvollziehbar und testbar, während ein Prozess den Betrieb einfach hält.

## Folgen

- Anbieter- und Infrastrukturdetails bleiben an den Rändern.

## Verbindliche Dokumentation

- [Components](../architecture/Components.md)
- [Overview](../architecture/Overview.md)
