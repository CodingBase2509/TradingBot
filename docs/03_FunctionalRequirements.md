# Funktionale Anforderungen

## Marktdaten

- Historische und aktuelle Marktdaten anbinden, vereinheitlichen und speichern.
- Datenlücken, Duplikate, veraltete Zeitstempel und unplausible Werte erkennen.
- 1-, 5-, 15- und 60-Minuten-Sichten bereitstellen.
- konkrete Futures-Kontrakte und deren Rollover nachvollziehbar verwalten.

## Modellentscheidung

Das Modell darf ausgeben:

- Kein Trade, Long oder Short;
- Vertrauen in die Entscheidung;
- Anteil am erlaubten Risikobudget;
- Stop- und Take-Profit-Abstand relativ zur aktuellen Schwankung;
- maximale Haltedauer;
- später optional vorzeitige oder teilweise Schließung.

Das Modell darf keine festen Kontolimits, Brokerzugänge, Freigabestatus oder Schutzregeln verändern.

## Handel und Positionen

- Modellentscheidung in eine technisch gültige Order übersetzen.
- Vertragswert, Tick-Größe, Gebühren, Kontostand und offene Risiken berücksichtigen.
- Orders senden, ändern, stornieren und Ausführungen verarbeiten.
- offene Orders und Positionen regelmäßig mit dem Broker abgleichen.
- Stop, Ziel, Zeitlimit und Teilfüllungen verwalten.
- Backtest, Paper und Live über denselben fachlichen Ausführungskern unterstützen.

## Risiko und Betrieb

- jede Order vor Ausführung gegen feste Grenzen prüfen;
- einzelne Trades begrenzen oder ablehnen;
- das gesamte Trading automatisch oder manuell stoppen;
- bei unklaren Daten-, Broker- oder Positionszuständen keine neuen Trades zulassen;
- definierte Notfallverfahren für offene Positionen ausführen;
- Warnungen und Systemzustände bereitstellen.

## Forschung und Training

- Rohdaten bereinigen, ohne sie zu überschreiben;
- versionierte Trainingsstände erzeugen;
- historische Long-, Short- und Kein-Trade-Varianten simulieren;
- Modelle zeitlich getrennt trainieren und testen;
- Kandidaten nach ONNX exportieren;
- Experimente, Ergebnisse und Abhängigkeiten versionieren;
- Champion und Challenger vergleichen.

## Modelllebenszyklus

Unterstützte Zustände:

```text
Candidate → Backtested → Validated → Shadow → Paper
→ Canary → Production → Retired
```

- Nur kompatible und freigegebene Modelle aktivieren.
- Aktive Modellversion für jede Entscheidung speichern.
- Rückkehr zur letzten stabilen Version ermöglichen.
- Promotion und Rücknahme vollständig protokollieren.

## Dashboard

- Systemzustand, Verbindungen, Datenqualität und Warnungen anzeigen;
- Kontostand, Risiko, Positionen, Orders und Trades darstellen;
- aktive und frühere Modellversionen anzeigen;
- Backtest-, Shadow-, Paper- und Live-Ergebnisse vergleichen;
- Not-Aus und kontrollierte Wiederaufnahme anbieten;
- kritische Bedienhandlungen bestätigen und protokollieren.
