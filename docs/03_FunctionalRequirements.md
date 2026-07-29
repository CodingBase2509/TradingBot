# Funktionale Anforderungen

## Marktdaten

- Historische und aktuelle Marktdaten anbinden, vereinheitlichen und speichern.
- Datenlücken, Duplikate, veraltete Zeitstempel und unplausible Werte erkennen.
- rekonstruierbare, tolerierbare und entscheidungsrelevante Datenlücken
  unterscheiden;
- abgeleitete Kerzen aus vollständigen Rohdaten deterministisch neu erzeugen;
- 1-, 5-, 15- und 60-Minuten-Sichten bereitstellen.
- konkrete Futures-Kontrakte und deren Rollover nachvollziehbar verwalten.

## Modellentscheidung

Das Modell darf ausgeben:

- Kein Trade, Long oder Short;
- Vertrauen in die Entscheidung;
- adaptive Stop- und Take-Profit-Niveaus aus der vorherigen Marktstruktur;
- geschätzte aktive Haltedauer und Unsicherheit der Schätzung;
- später optional vorzeitige oder teilweise Schließung.

Das Modell darf keine festen Kontolimits, Brokerzugänge, Freigabestatus oder Schutzregeln verändern.

In V1 wird das Risikobudget nicht vom Modell gewählt. Die Plattform verwendet
die beschlossene Risikopolitik und verlangt vor jeder Order ein erwartetes
Netto-Risk-to-Reward von mindestens `1:1`.

## Handel und Positionen

- mehrere versionierte Strategy Instances mit eigener Markt-, Modell-,
  Zeitrahmen-, Schwellen-, Modus- und Risikokonfiguration betreiben;
- mehrere Shadow- und Simulated-Paper-Instanzen parallel und vergleichbar
  ausführen;
- Modellentscheidung in eine technisch gültige Order übersetzen.
- Vertragswert, Tick-Größe, Gebühren, Kontostand und offene Risiken berücksichtigen.
- Orders senden, ändern, stornieren und Ausführungen verarbeiten.
- offene Orders und Positionen regelmäßig mit dem Broker abgleichen.
- Stop, Ziel, Zeitlimit und Teilfüllungen verwalten.
- bis zu drei logische, gleichgerichtete MES-Trades einer aggregierten
  Brokerposition eindeutig zuordnen;
- gegenläufige MES-Signale bei bereits offenen Trades in V1 ablehnen;
- zwei Stunden vor täglichen Börsenpausen keine neuen Positionen eröffnen;
- alle Positionen vor dem wöchentlichen Börsenschluss am Freitag schließen;
- Backtest, Paper und Live über denselben fachlichen Ausführungskern unterstützen.
- Strategy Trades und Positionen eindeutig einer gemeinsamen
  Broker-Nettoposition zuordnen.

## Risiko und Betrieb

- jede Order vor Ausführung gegen feste Grenzen prüfen;
- Strategiegrenzen und gemeinsame Konto-/Portfoliogrenzen nacheinander prüfen;
- Risiko, Tradeplatz, tägliches Trade-Token und Richtung vor Orderübermittlung
  atomar reservieren;
- einzelne Trades begrenzen oder ablehnen;
- das gesamte Trading automatisch stoppen;
- einen kontrollierten Systemstopp ohne neue Arbeiten sowie einen Full-Stop mit
  vollständiger Positionsschließung anbieten;
- bei unklaren Daten-, Broker- oder Positionszuständen keine neuen Trades zulassen;
- definierte Notfallverfahren für offene Positionen ausführen;
- Warnungen und Systemzustände bereitstellen.
- Fehler auf Strategy-, Instrument-, Konto- oder Plattformebene isolieren und
  nur im erforderlichen Umfang eskalieren;
- Strategy Instances begrenzt und zustandsgeprüft automatisch neu starten;
- globale Handelsfehler bei möglichst weiterlaufendem Schutz, Brokerabgleich
  und Monitoring behandeln.

## Forschung und Training

- Rohdaten bereinigen, ohne sie zu überschreiben;
- versionierte Trainingsstände erzeugen;
- historische Long-, Short- und Kein-Trade-Varianten simulieren;
- Modelle zeitlich getrennt trainieren und testen;
- Kandidaten nach ONNX exportieren;
- Experimente, Ergebnisse und Abhängigkeiten versionieren;
- Champion und Challenger vergleichen.
- geschätzte und tatsächliche Haltedauer einschließlich Schließungsgrund
  vergleichen und für spätere Offline-Trainingsstände bereitstellen.
- Training und Forschung vollständig von Test und Produktion isolieren und
  ausschließlich kontrollierte unveränderliche Daten- und Modellpakete
  austauschen.

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
- zoneneigene Modellverzeichnisse nach neuen Paketen durchsuchen;
- kopierte Pakete vollständig und idempotent prüfen und registrieren;
- nur verfügbare, für die Zielstufe geeignete Pakete in der UI anbieten;
- Strategy Instances ausschließlich nach bewusster Auswahl und Bestätigung
  erzeugen.

## Dashboard

- Systemzustand, Verbindungen, Datenqualität und Warnungen anzeigen;
- Kontostand, Risiko, Positionen, Orders und Trades darstellen;
- aktive und frühere Modellversionen anzeigen;
- Backtest-, Shadow-, Paper- und Live-Ergebnisse vergleichen;
- Strategy Instances einzeln sowie in einer gemeinsamen Konto- und
  Portfolioansicht vergleichen;
- beide Not-Aus-Stufen und eine kontrollierte Wiederaufnahme anbieten;
- kritische Bedienhandlungen bestätigen und protokollieren.
