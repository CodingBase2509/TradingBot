# ADR-005: Produktumfang der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Für die erste Version musste festgelegt werden, was als erfolgreicher Plattformnachweis gilt, ohne Profitabilität oder Echtgeldbetrieb vorauszusetzen.

## Entscheidung

- V1 handelt ausschließlich MES im Paper Trading.
- Mehrere Shadow- und Simulated-Paper-Strategien sind erlaubt; Broker Paper bleibt je Instrument auf eine Ausführungsgruppe begrenzt.
- Der Plattformkern wird zuerst mit einer festen Teststrategie nachgewiesen. Live-Handel, News und Orderbuch gehören nicht zu V1.

## Begründung

Ein einzelner Markt und feste Betriebsgrenzen machen Daten-, Risiko-, Order- und Positionsverarbeitung deterministisch prüfbar.

## Folgen

- Die Architektur bleibt auf spätere Märkte und Live-Betrieb erweiterbar.
- Profitabilität ist kein V1-Abnahmekriterium.

## Verbindliche Dokumentation

- [00_ProjectVision](../00_ProjectVision.md)
- [TradingConcept](../trading/TradingConcept.md)
- [Phase1](../roadmap/Phase1.md)
