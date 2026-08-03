# ADR-013: Databento als historischer Datenanbieter

- **Status:** vorläufig beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Historische MES-Daten sollen für den Start ausreichend genau, aber finanziell überschaubar beschafft werden.

## Entscheidung

- Databento ist der vorläufige Anbieter.
- Der volle V1-Zeitraum wird primär als TBBO und OHLCV-1m beschafft; MBP-1 nur für kleine relevante Vergleichszeiträume.
- Vor dem Vollkauf werden Lizenz, Kosten, Vollständigkeit, Zeitstempel, Rollover, Rekonstruktion und Speicherrecht an einem repräsentativen Testbestand geprüft.

## Begründung

Diese Auswahl hält den Start kostengünstig und erlaubt später gezielte Investitionen in detailliertere Daten.

## Folgen

- Die Entscheidung wird erst nach praktischem Test endgültig.
- Zusätzliche Käufe bleiben bewusste Budgetentscheidungen.

## Verbindliche Dokumentation

- [MarketData](../trading/MarketData.md)
- [05_OpenDecisions](../05_OpenDecisions.md)
