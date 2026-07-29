# ADR-015: Börsenkalender und Handelszeiten

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Handelszeiten, Wartungspausen, Feiertage, verkürzte Sitzungen,
Freitagsschließung und fachliche Handelstage müssen in Backtest, Paper Trading
und späterem Betrieb identisch bestimmt werden. Lokale Uhrzeiten oder alleinige
Brokerangaben reichen dafür nicht aus.

## Entscheidung

- Veröffentlichungen und Kalenderdaten der CME sind die fachlich maßgebliche
  Quelle für MES-Sitzungen, Feiertage, Wartungspausen und Sonderzeiten.
- Zeiten werden intern als UTC gespeichert. Die originale Börsenzeitzone und
  der fachliche CME-Handelstag bleiben zusätzlich erhalten.
- Eingelesene Kalenderstände werden versioniert und nachträglich nicht
  überschrieben.
- Jeder Backtest und Trainingsstand verweist auf die verwendete
  Kalenderversion.
- Interactive-Brokers-Vertrags- und Handelszeitangaben werden operativ als
  Gegenprüfung verwendet.
- Widersprechen IBKR und der freigegebene CME-Kalender einander, werden neue
  Trades blockiert und die Abweichung geprüft.
- Sondermeldungen und kurzfristige Börsenänderungen erzeugen eine neue
  Kalenderversion.
- Handelsgrenzen wie Einstiegsschluss und Freitagsschließung werden relativ zu
  den Sitzungsgrenzen berechnet, nicht als feste deutsche Uhrzeit.

## Begründung

Die Börse definiert, wann ihr Produkt handelbar ist. Versionierung verhindert,
dass spätere Kalenderkorrekturen historische Ergebnisse unbemerkt verändern.
Der Brokervergleich erkennt operative Abweichungen, ohne brokerabhängige Angaben
zur fachlichen Wahrheit zu machen.

## Folgen

- Ein Kalenderimport und eine Validierung gegen IBKR werden benötigt.
- Sommerzeitwechsel werden über Zeitzonendaten statt manuell berechneter
  Zeitverschiebungen behandelt.
- Unbekannte oder widersprüchliche Sitzungszustände führen zu „kein Trade“.
- Anbieter, Format, Aktualisierungsintervall und technische Speicherung des
  CME-Kalenders werden im technischen Design festgelegt.
