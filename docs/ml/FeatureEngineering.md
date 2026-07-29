# Feature Engineering

Ein Feature ist ein vorbereiteter Eingangswert des Modells.

## V1-Eingaben

- Open, High, Low und Close;
- Handelsvolumen;
- Bid/Ask oder Spread, soweit historisch verfügbar;
- Zusammenhänge auf 1, 5, 15 und 60 Minuten;
- relative Preisänderungen und Schwankungsstärke;
- Volumen im Verhältnis zum bisherigen Durchschnitt;
- Abstand zu Tageshoch und Tagestief;
- Uhrzeit, Wochentag und Handelssitzung;
- Markt- und konkrete Futures-Kontraktkennung.

## Gestaltungsregeln

- Werte möglichst relativ zur typischen Schwankung ausdrücken.
- Nur zu diesem Zeitpunkt verfügbare Informationen verwenden.
- Fehlende Werte ausdrücklich behandeln, nicht still auffüllen.
- Lückenanzahl, Position und Qualitätsstatus gemäß dem versionierten
  Missing-Data-Vertrag abbilden.
- Definition, Einheit, Reihenfolge und Zeitbezug versionieren.
- Berechnung in Python und .NET mit gemeinsamen Beispieldaten vergleichen.
- Zusätzliche Indikatoren nur aufnehmen, wenn sie messbaren Nutzen bringen.

## Mehrere Zeithorizonte

Das Modell sieht kurzfristige Ausführung und größeren Zusammenhang:

- 1 Minute für genaue Bewegung und spätere Ausführung;
- 5 und 15 Minuten für kurzfristige Muster;
- 60 Minuten für das Marktumfeld.

Kanonische 1-Minuten-Daten werden deterministisch aus dem versionierten
Rohdatenstand erzeugt. Die größeren Zeithorizonte werden ausschließlich aus
vollständig abgeschlossenen 1-Minuten-Intervallen aggregiert.

## Adaptive Handelsalternativen

Ein versionierter Candidate Generator leitet mögliche Stop-Loss- und
Take-Profit-Niveaus aus der vorherigen Marktbewegung ab. Dafür verwendet er
unter anderem bestätigte Swings, Unterstützungs- und Widerstandsniveaus,
Handelsspannen, Trend- und Rücklaufstruktur, Volatilität, Dochte, Spread und
Liquidität.

ATR ist ein Eingangsmerkmal, aber kein starres primäres TP-/SL-Raster. Die
erzeugten Preisniveaus werden auf die Tickgröße des jeweiligen Instruments
gerundet, gegen instrumentspezifische Grenzen geprüft und anschließend
gemeinsam mit den Marktfeatures vom Kandidatenmodell bewertet.

Die Erzeugung muss in Training, Backtest und Laufzeit identisch und ohne
zukünftige Informationen erfolgen.

ADR-028 legt die kausale Swing-Erkennung, Zonenbildung, Strukturwerte,
Puffer, Filter, Deduplizierung und V1-Kandidatenlimits fest.

## Spätere Erweiterungen

- Wirtschaftskalender;
- veröffentlichte Wirtschaftswerte und Erwartungsabweichung;
- verwandte Märkte;
- Orderbuch;
- strukturierte Nachrichtensignale.

Jede Erweiterung wird gegen eine ansonsten identische Variante ohne dieses Feature getestet.
