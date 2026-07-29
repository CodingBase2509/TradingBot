# Backtesting

Backtesting simuliert Entscheidungen auf historischen Daten, als wäre die Zukunft unbekannt.

Für Ausführung und Stop-/TP-Reihenfolge werden ereignisnahe Trade- und
Bid-/Ask-Daten bevorzugt. Die Modellentscheidung verwendet davon getrennte,
abgeschlossene 1-, 5-, 15- und 60-Minuten-Sichten.

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
- Stop Loss, Take Profit und Haltedauerschätzung;
- Handelszeiten;
- tägliche Börsenpausen, verkürzte Handelstage und Feiertage;
- Kurslücken, in denen Schutzorders erst nach Wiedereröffnung ausführbar sind;
- Einstiegsschluss und verpflichtende Freitagsschließung;
- Futures-Rollover und tatsächlich handelbarer Vertrag.

Long-Einstiege verwenden die erreichbare Ask-Seite, Short-Einstiege die
Bid-Seite. Das Basisszenario nimmt zusätzlich mindestens einen Tick Slippage je
Orderseite an; Robustheitstests verwenden mindestens zwei und drei Ticks.
Gebühren werden pro Kontrakt und Orderseite mit einer zeitabhängigen,
versionierten Konfiguration berücksichtigt.

Limit- und Stop-Orders gelten nicht allein aufgrund einer Kerzenberührung
automatisch zum gewünschten Preis als ausgeführt. Ausführungsreihenfolge,
verfügbare Ereignisdaten, Teilfüllung und Verzögerung werden berücksichtigt.

## Historische Lernvorlagen

Pro geeignetem Zeitpunkt werden kein Trade sowie Long und Short mit mehreren
adaptiv aus der vorherigen Marktstruktur erzeugten Stop-/Zielkombinationen
simuliert. Zusätzlich werden aktive Marktzeit, normale vergangene Zeit und
Schließungsgrund als Grundlage der Haltedauerschätzung erzeugt.

Historischer und laufender Candidate Generator verwenden dieselbe versionierte
ADR-028-Logik. Ein historischer Swing darf erst ab seiner damaligen
Bestätigung verfügbar sein.

Historische Kandidaten werden gemäß ADR-029 mit ausführbarer Marktseite,
Kosten, Netto-`R`, aktiver Haltedauer und Censoring gelabelt.

Kombinationen werden nur berücksichtigt, wenn ihr erwarteter Nettogewinn am
Take Profit mindestens so groß ist wie ihr erwarteter Nettoverlust am Stop Loss.
Gebühren, Spread, Slippage und Tick-Rundung fließen in dieses Verhältnis ein.

## Konservative Regeln

- Sind Stop und Ziel innerhalb derselben Kerze berührt und die Reihenfolge ist unbekannt, wird konservativ entschieden oder eine feinere Auflösung benötigt.
- Rohdaten bleiben unverändert.
- Unbekannte spätere Daten dürfen weder Feature noch Modellwahl beeinflussen.
- Ein Trade muss „kein Trade“ nach Kosten und Risikostrafe deutlich schlagen.
- Historische Ergebnisse werden als Netto-`R` ausgedrückt; `1 R` entspricht dem
  geplanten Verlust am Stop einschließlich angenommener Kosten.

## Robustheitstests

Gebühren, Spread und Verzögerung werden absichtlich verschlechtert. Zusätzlich werden Einstiege verschoben und einzelne Trades ausgelassen. Ein Modell, dessen Ergebnis dabei sofort zusammenbricht, wird nicht freigegeben.
