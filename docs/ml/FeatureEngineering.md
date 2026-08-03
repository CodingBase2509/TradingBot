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

## Candidate Generator der V1

Der Generator läuft nach jeder vollständig abgeschlossenen 5-Minuten-Kerze.
Seine Startfenster umfassen 120 Ein-Minuten-, 288 Fünf-Minuten-, 192
Fünfzehn-Minuten- und 120 Stundenkerzen. Swings werden kausal bestätigt: links
und rechts gelten je drei Kerzen auf 1 und 5 Minuten sowie je zwei auf 15 und
60 Minuten. Ein historischer Swing wird erst ab seiner damaligen Bestätigung
verwendet.

Strukturquellen sind bestätigte Swings, mehrfach berührte Zonen, aktuelles
Sitzungs- und vorheriges Tageshoch/-tief, Handelsspannen, bestätigte Ausbruchs-
und Rücklaufniveaus sowie Ausgangsbereiche starker Bewegungen. Nahe Niveaus
werden innerhalb von `max(2 Ticks, 0,15 × 5-Minuten-ATR)` zu Zonen verbunden.

```text
structureScore =
  reactionStrength      × 0,25
+ timeframeConfirmation × 0,25
+ recency               × 0,20
+ touches               × 0,20
+ relativeVolume        × 0,10
```

Long-Stops liegen unter, Short-Stops über passender Struktur. Der Startpuffer
ist `max(2 Ticks, Spread in Ticks, 0,10 × 5-Minuten-ATR)`. Ziele verwenden die
spiegelbildlich vor dem Einstieg liegenden Strukturen und bestätigte
Bewegungsprojektionen. Rundung erfolgt immer konservativ auf gültige Ticks.
ATR-basierte Niveaus sind nur als gekennzeichnete `VolatilityFallback`-Kandidaten
zulässig.

Je Richtung werden zunächst höchstens vier Stops mit vier Zielen kombiniert.
Ungültige Seiten, Instrumentgrenzen, unzureichende Datenqualität, Marktrauschen
und ein Netto-Risk-to-Reward unter `1:1` werden vor dem Modell verworfen.
Redundante Kandidaten innerhalb derselben Zonentoleranz werden nach
Strukturwert, Anzahl bestätigender Zeitrahmen, Aktualität, konservativerem Preis
und stabilem technischem Tie-Breaker dedupliziert. Es bleiben höchstens zwölf
Kandidaten je Richtung und 24 insgesamt.

Jeder persistierte Kandidat erhält eine UUID Version 7 und zusätzlich einen
deterministischen Fingerprint aus Generatorversion, Richtung, Preisen,
Quelltypen und Parametern. Zielzeit sind 100 ms je Strategy-Entscheidung, die
harte Startgrenze beträgt 500 ms. Eine Überschreitung erzeugt keine Order und
setzt die Strategy auf `Degraded`; Wiederholungen führen zur Sperre.

Lookbacks, Swingfenster, Toleranzen, Gewichte, Puffer, Limits und Laufzeitbudgets
sind versionierte Startparameter. Änderungen werden nur auf Trainings- und
Validierungsdaten gewählt und benötigen neue Golden- und Paritätstests.

## Spätere Erweiterungen

- Wirtschaftskalender;
- veröffentlichte Wirtschaftswerte und Erwartungsabweichung;
- verwandte Märkte;
- Orderbuch;
- strukturierte Nachrichtensignale.

Jede Erweiterung wird gegen eine ansonsten identische Variante ohne dieses Feature getestet.
