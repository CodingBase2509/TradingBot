# Backtesting

Backtesting simuliert Entscheidungen auf historischen Daten, als wäre die Zukunft unbekannt.

## Gemeinsamer Kern

Backtest, Paper und Live sollen dieselbe Order-, Positions- und Risikologik verwenden. Ausgetauscht werden Datenzufuhr und Brokeradapter.

## Zu simulierende Bedingungen

- Bid/Ask und Spread;
- Gebühren;
- Slippage;
- Orderverzögerung;
- Teilfüllungen;
- Tick-Größe und Mindestmenge;
- Liquiditätsgrenzen;
- Stop Loss, Take Profit und maximale Haltedauer;
- Handelszeiten;
- Futures-Rollover und tatsächlich handelbarer Vertrag.

## Historische Lernvorlagen

Pro geeignetem Zeitpunkt werden kein Trade sowie Long und Short mit mehreren Stop-, Ziel- und Haltedauer-Kombinationen simuliert.

Frühes Arbeitsraster:

- Stop: 0,5 / 1,0 / 1,5 / 2,0 × aktuelle Schwankung;
- Ziel: 1,0 / 1,5 / 2,0 / 3,0 × aktuelle Schwankung;
- Haltedauer: 30 / 60 / 120 / 240 / 480 Minuten.

Diese Werte sind Vorschläge und werden vor Implementierung mathematisch geprüft.

## Konservative Regeln

- Sind Stop und Ziel innerhalb derselben Kerze berührt und die Reihenfolge ist unbekannt, wird konservativ entschieden oder eine feinere Auflösung benötigt.
- Rohdaten bleiben unverändert.
- Unbekannte spätere Daten dürfen weder Feature noch Modellwahl beeinflussen.
- Ein Trade muss „kein Trade“ nach Kosten und Risikostrafe deutlich schlagen.

## Robustheitstests

Gebühren, Spread und Verzögerung werden absichtlich verschlechtert. Zusätzlich werden Einstiege verschoben und einzelne Trades ausgelassen. Ein Modell, dessen Ergebnis dabei sofort zusammenbricht, wird nicht freigegeben.
