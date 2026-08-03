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
Logik aus dem [Feature Engineering](./FeatureEngineering.md). Ein historischer
Swing darf erst ab seiner damaligen Bestätigung verfügbar sein.

Historische Kandidaten werden mit ausführbarer Marktseite, Kosten, Netto-`R`,
aktiver Haltedauer und Censoring gelabelt.

Der Einstieg wird 250 ms nach der abgeschlossenen Entscheidungskerze zum
ersten realistisch ausführbaren Preis simuliert: Long auf Ask, Short auf Bid,
im Basisszenario mit einem Tick Slippage. Ausgänge verwenden für Long die
Bid- und für Short die Ask-Seite. Stop und Freitagsschließung erhalten im
Basisszenario einen Tick zusätzliche Slippage, Take Profit keine positive
Preisverbesserung. Stressläufe verwenden zwei und drei Ticks; bei Take Profit
werden zusätzlich ein und zwei Ticks negative Abweichung geprüft.

Ist die Reihenfolge von Stop und Ziel nicht rekonstruierbar, wird konservativ
der Stop verwendet und der Fall als mehrdeutig markiert. `1 R` ist der geplante
Nettoverlust am Stop einschließlich Preisverlust, Gebühren, Spread und
Basisslippage. Der echte Netto-R-Wert bleibt unverändert; nur das erste
Trainingsziel wird auf `[-2 R; +4 R]` begrenzt.

Ausgangsklassen sind mindestens `TakeProfit`, `StopLoss`,
`FridayCloseProfit`, `FridayCloseLoss`, `FridayCloseFlat`, `Censored` und
`Invalid`. Der neutrale Startbereich liegt zwischen `-0,05 R` und `+0,05 R`.
TP und SL liefern vollständige Zeitbeobachtungen. Freitag, Full-Stop, manuelle
oder technische Schließung und Datenende werden ihrer Ursache entsprechend als
abgeschnitten gekennzeichnet; rote nicht rekonstruierbare Datenlücken sind
ungültig.

Gespeichert werden vergangene Minuten einschließlich Börsenpause und aktive
Marktminuten ohne Pause. Das Startziel verwendet
`log(1 + activeMarketHoldingMinutes)`; das Modell liefert P50 und P90, die
getrennt kalibriert werden. Alle Reaktions-, Kosten-, Neutral-, Begrenzungs-
und Quantilwerte sind versionierte Startparameter.

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
