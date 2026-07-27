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
- Definition, Einheit, Reihenfolge und Zeitbezug versionieren.
- Berechnung in Python und .NET mit gemeinsamen Beispieldaten vergleichen.
- Zusätzliche Indikatoren nur aufnehmen, wenn sie messbaren Nutzen bringen.

## Mehrere Zeithorizonte

Das Modell sieht kurzfristige Ausführung und größeren Zusammenhang:

- 1 Minute für genaue Bewegung und spätere Ausführung;
- 5 und 15 Minuten für kurzfristige Muster;
- 60 Minuten für das Marktumfeld.

## Spätere Erweiterungen

- Wirtschaftskalender;
- veröffentlichte Wirtschaftswerte und Erwartungsabweichung;
- verwandte Märkte;
- Orderbuch;
- strukturierte Nachrichtensignale.

Jede Erweiterung wird gegen eine ansonsten identische Variante ohne dieses Feature getestet.
