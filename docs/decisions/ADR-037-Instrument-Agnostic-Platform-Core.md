# ADR-037: Instrumentneutraler Plattformkern

- **Status:** beschlossen
- **Datum:** 3. August 2026

## Kontext

MES ist das erste Instrument der V1. Die Plattform soll später weitere Symbole,
Märkte und Strategien betreiben können, ohne dass MES-spezifische Annahmen den
gemeinsamen Handelskern bestimmen.

## Entscheidung

- Der Plattformkern arbeitet ausschließlich mit einer internen `InstrumentId`
  und einem versionierten Instrumentvertrag.
- Symbole und Vertragskennungen je Broker und Datenquelle werden durch Adapter
  und Instrumentkonfiguration zugeordnet.
- Tickwerte, Multiplikator, Währung, Börse, Kalender, Sitzungen, Kosten,
  Rollover und Handelsgrenzen werden nicht als MES-Konstanten im Kern codiert.
- Instrumentspezifische Fähigkeiten wie auslaufende Verträge und Rollover sind
  explizit und nur bei passenden Instrumenten aktiv.
- MES bleibt das verbindliche erste V1-Profil und Abnahmeinstrument.

## Begründung

Ein gemeinsamer instrumentneutraler Weg erlaubt neue Symbole durch Stammdaten,
Konfiguration und Adapterzuordnung. Gleichzeitig muss V1 nicht vorzeitig alle
Instrumentarten implementieren oder testen.

## Folgen

- Neue Instrumente benötigen vollständige validierte Stammdaten sowie
  Unterstützung durch Datenquelle und Broker.
- Fachlogik darf keine Abfragen wie `if symbol == "MES"` enthalten.
- Instrumentübergreifende Golden Tests werden ergänzt, sobald das zweite
  Instrument aufgenommen wird.

## Verbindliche Dokumentation

- [Architekturübersicht](../architecture/Overview.md)
- [Komponenten](../architecture/Components.md)
- [Konfigurationsvertrag](../architecture/Configuration.md)
