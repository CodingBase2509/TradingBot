# Modellevaluation

## Bewertungsgruppen

### Profitabilität

- Nettoergebnis nach allen Kosten;
- durchschnittliches Ergebnis pro Trade;
- Profit Factor;
- Anteil positiver Trades;
- Ergebnis je gebundenem Risiko.

### Risiko

- maximaler Drawdown;
- größter Tagesverlust;
- Verlustserien;
- extremes Verlustrisiko;
- Exposure und Gleichlauf offener Positionen.

### Stabilität

- verschiedene Jahre und Marktphasen;
- steigende, fallende, ruhige und hektische Märkte;
- unterschiedliche Tageszeiten;
- mehrere Instrumente;
- ausreichende Zahl unabhängiger Trades.

### Robustheit

- höhere Kosten und Spreads;
- schlechtere Ausführung und längere Verzögerung;
- kleine Änderungen an Parametern;
- ausgelassene Trades;
- getrennte Prüfung bekannter und unbekannter Märkte.

## Vergleichsmaßstäbe

Jeder Kandidat wird verglichen mit:

- keinem Handel;
- einfachen Trend- oder Mean-Reversion-Baselines;
- einer Zufallsentscheidung bei gleichem Risiko;
- dem aktuellen Champion.

## Marktübergreifende Tests

1. bekannter Markt, spätere unbekannte Zeit;
2. mehrere bekannte Märkte, spätere Zeit;
3. vollständig zurückgehaltener Markt.

Der dritte Test misst Übertragbarkeit, ersetzt aber keine marktspezifische Validierung.

## Promotion

Für die V1 gelten mindestens:

- 300 abgeschlossene Signalgruppen über mindestens 24 Monate unbekannter Daten;
- mindestens fünf Walk-Forward-Fenster, davon vier mit positivem Nettoergebnis;
- Profit Factor mindestens `1,20` nach vollständigen Basiskosten;
- maximal `20 %` Drawdown;
- bei zwei Ticks Slippage je Orderseite weiterhin profitabel;
- bei drei Ticks Slippage Profit Factor mindestens `0,90` und Drawdown höchstens
  das 1,5-Fache des Basiswerts;
- keine einzelne Signalgruppe mit mehr als `10 %` des gesamten Nettogewinns;
- kein Testfenster mit mehr als `40 %` der positiven Fensterergebnisse.

Mehrere gleichgerichtete Trades zwischen erster Eröffnung und vollständiger
Glattstellung zählen für die Mindestzahl als eine Signalgruppe. Ein Kandidat
muss außerdem vier Wochen Shadow Mode sowie acht Wochen und 100 Signalgruppen
im Paper Trading bestehen.
