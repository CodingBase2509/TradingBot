# ADR-029: Historische Trainingslabels der V1

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Der Candidate Scorer soll erwartetes Nettoergebnis und Haltedauer einer
adaptiven TP-/SL-Alternative lernen. Dafür müssen Einstieg, Ausführungskosten,
unklare Ereignisreihenfolgen, Freitagsschließung, Datenende und technische
Eingriffe reproduzierbar gelabelt werden.

## Entscheidung

### Einstieg und Ausführung

Der historische Einstieg erfolgt am ersten realistisch ausführbaren Preis nach
der abgeschlossenen Entscheidungskerze:

- simulierte Reaktionszeit: `250 ms`;
- Long auf Ask, Short auf Bid;
- Basisslippage: `1 Tick`;
- Stressstufen: `2` und `3 Ticks`.

Fehlt ein verlässlich ausführbarer Preis, ist der Kandidat ungültig.

Die Simulation folgt dem ersten Ereignis aus Take Profit, Stop Loss,
Freitagsschließung, ungültiger Datenlage oder Ende des Datenstands. Positionen
dürfen über tägliche Börsenpausen laufen.

Für Long-Ausgänge wird Bid, für Short-Ausgänge Ask verwendet:

| Ausführung | zusätzliche Basisslippage |
|---|---:|
| Einstieg | 1 Tick |
| Stop Loss | 1 Tick |
| Take Profit | 0 Ticks, keine positive Preisverbesserung |
| Freitagsschließung | 1 Tick |

Stressläufe verwenden `2` und `3 Ticks`; beim Take Profit werden zusätzlich
`1` und `2 Ticks` negative Abweichung geprüft.

Ist die Reihenfolge von TP und SL nicht eindeutig, wird konservativ der Stop
angenommen und `ambiguousExit = true` gespeichert.

### Netto-R

`1 R` ist der geplante Nettoverlust beim Stop einschließlich Preisverlust,
Gebühren, Spread und Basisslippage.

```text
realizedNetR =
  tatsächliches Nettoergebnis
  ───────────────────────────
  geplanter Nettoverlust am Stop
```

Der echte Wert wird unverändert gespeichert und für Backtest, Risiko und
Berichte verwendet. Für das erste Modelltraining wird der Zielwert auf
`[-2,0 R; +4,0 R]` begrenzt, damit einzelne Ausreißer das Lernen nicht
dominieren.

### Ausgangsklassen

Mindestens:

```text
TakeProfit
StopLoss
FridayCloseProfit
FridayCloseLoss
FridayCloseFlat
Censored
Invalid
```

Der neutrale Startbereich ist `-0,05 R` bis `+0,05 R`. `positiveOutcome` darf
als Hilfslabel gespeichert werden, ersetzt aber nicht `realizedNetR`.

### Haltedauer

Gespeichert werden normale vergangene Minuten einschließlich Börsenpause,
aktive Marktminuten ohne Börsenpause und ein Censoring-Kennzeichen.

Das Modell lernt aktive Marktminuten mit der Starttransformation:

```text
log(1 + activeMarketHoldingMinutes)
```

Es liefert P50 als `estimatedHoldingMinutes` und P90 als
`holdingTimeP90Minutes`. Beide Quantile werden mit geeigneten Quantilverlusten
trainiert und separat auf Kalibrierung geprüft.

### Vollständige und abgeschnittene Beobachtungen

- TP und SL liefern vollständiges Netto-`R` und vollständige Haltedauer.
- Freitagsschließung liefert gültiges Netto-`R`, aber eine für die natürliche
  TP-/SL-Dauer abgeschnittene Zeitbeobachtung.
- Full-Stop, manuelle oder technische Schließung werden gespeichert, aber
  standardmäßig nicht als normale Ergebnis- oder Haltedauerlabels verwendet.
- Ende des Datenstands ist `Censored`, nicht NoTrade und nicht automatisch
  Verlust.
- Eine nicht rekonstruierbare rote Datenlücke ist `Invalid` und wird nicht für
  Ergebnis- oder Haltedauertraining verwendet.

### Freitag

Die Simulation verwendet den versionierten Börsenkalender. Ab dem
Einstiegsschluss entstehen keine neuen Kandidaten. Noch offene Trades werden
zum ersten realistisch ausführbaren Preis des 60 Minuten vor Wochenschluss
beginnenden Schließungsablaufs beendet und einschließlich Kosten als
Friday-Close-Klasse gespeichert.

### Mindestinhalt je Label

Mindestens gespeichert werden:

- Candidate-, Decision-, Daten- und Labelversion;
- Entscheidungs-, Einstiegs- und Ausstiegszeit;
- Einstieg, Stop, Ziel und Ausstieg in Ticks;
- Bruttoergebnis, Gebühren, Spread-, Slippage- und Nettoergebnis;
- geplanter Stopverlust, echtes und begrenztes Netto-`R`;
- Ausgangsklasse, Schließungsgrund und Mehrdeutigkeitskennzeichen;
- normale und aktive Haltedauer sowie Censoring-Kennzeichen;
- Datenqualität und Kostenmodellversion.

## Parameteränderungen

Reaktionszeit, Slippage, Neutralbereich, Zielwertbegrenzung, Quantile und
Transformation sind versionierte Startwerte. Änderungen werden auf
Trainings-/Validierungsdaten und in vorab definierten Kostenstressszenarien
geprüft. Der unbekannte Abschlusstest darf nicht zur nachträglichen Abstimmung
verwendet werden.

## Begründung

Ausführbare Marktseiten und Kosten verhindern optimistische Labels.
Konservative Behandlung unklarer Reihenfolgen schützt vor historischem
Informationsvorteil. Netto-`R` macht Kandidaten mit verschiedenen Stopabständen
vergleichbar.

Getrennte aktive und vergangene Zeit behandelt die tägliche Börsenpause
sachgerecht. Censoring verhindert, dass Freitag, Full-Stop oder Datenende als
natürliche Trade-Dauer fehlinterpretiert werden.

## Folgen

- Label Generator und Backtest teilen Ausführungs-, Kosten- und Kalenderlogik.
- Haltedauermodelle beziehungsweise Ausgabeköpfe benötigen Quantil- und
  Censoring-Unterstützung.
- Echte und für Training begrenzte `R`-Werte bleiben getrennte Felder.
- Golden Samples decken TP, SL, Mehrdeutigkeit, Pause, Freitag, Datenende,
  Full-Stop und rote Datenlücke ab.
