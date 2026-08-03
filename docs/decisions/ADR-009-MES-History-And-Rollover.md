# ADR-009: MES-Historie und Rollover

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Futures laufen aus; Training und Ausführung benötigen deshalb eine kausale und reproduzierbare Vertragswahl.

## Entscheidung

- V1 verwendet MES-Daten ab dem offiziellen Handelsstart.
- Rohdaten bleiben je konkretem Vertrag getrennt.
- Der Folgekontrakt gilt ab dem nächsten Handelstag, nachdem sein Volumen an einem vollständig abgeschlossenen Handelstag höher war als das des bisherigen Frontkontrakts.

## Begründung

Eine volumenbasierte, erst nach Tagesabschluss wirksame Regel vermeidet rückwirkendes Wissen und folgt der handelbaren Liquidität.

## Folgen

- Kontinuierliche Analysereihen bleiben von der tatsächlich simulierten Vertragsausführung getrennt.
- Für fehlerhafte Volumendaten ist noch eine Ersatzregel festzulegen.

## Verbindliche Dokumentation

- [MarketData](../trading/MarketData.md)
- [05_OpenDecisions](../05_OpenDecisions.md)
