# ADR-028: Adaptiver Candidate Generator der V1

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Das Kandidatenmodell benötigt eine begrenzte, reproduzierbare Menge plausibler
Long- und Short-Alternativen. TP und SL sollen aus der vorherigen
Marktbewegung stammen, nicht primär aus einem starren ATR-Raster. Training,
Backtest, Shadow und Paper müssen bei identischen Eingaben dieselben Kandidaten
erzeugen, ohne zukünftige Informationen zu verwenden.

## Entscheidung

### Eingaben und Entscheidungstakt

Der versionierte Candidate Generator läuft nach einer vollständig
abgeschlossenen 5-Minuten-Entscheidungskerze. Er verwendet:

- abgeschlossene 1-, 5-, 15- und 60-Minuten-Kerzen;
- zum Entscheidungszeitpunkt bekannten Bid-/Ask-Zustand und Spread;
- Datenqualitätsstatus;
- konkreten Vertrag, Tickgröße und Tickwert;
- Börsenkalender und Handelssitzung;
- versionierte Instrument-, Kosten- und Generatorkonfiguration.

Startfenster:

| Zeitrahmen | Kerzen |
|---|---:|
| 1 Minute | 120 |
| 5 Minuten | 288 |
| 15 Minuten | 192 |
| 60 Minuten | 120 |

Fehlende oder noch nicht abgeschlossene Kerzen werden nicht mit zukünftigen
Werten ergänzt. Die beschlossenen Datenqualitätsregeln bleiben vorrangig.

### Kausale Swing-Erkennung

Ein Swing-Hoch beziehungsweise Swing-Tief wird erst verwendet, wenn alle
benötigten Kerzen auf beiden Seiten abgeschlossen und zum Entscheidungszeitpunkt
bekannt sind.

Startparameter:

| Zeitrahmen | Kerzen links | Kerzen rechts |
|---|---:|---:|
| 1 Minute | 3 | 3 |
| 5 Minuten | 3 | 3 |
| 15 Minuten | 2 | 2 |
| 60 Minuten | 2 | 2 |

Damit wird ein Wendepunkt bewusst verzögert bestätigt. Historische Verarbeitung
darf ihn nicht auf seinen ursprünglichen Zeitpunkt zurückdatieren.

### Strukturquellen

Der Generator berücksichtigt mindestens:

- bestätigte Swing-Hochs und Swing-Tiefs;
- mehrfach berührte Unterstützungs- und Widerstandszonen;
- aktuelles Sitzungs- und vorheriges Handelstagshoch/-tief;
- Grenzen größerer Handelsspannen;
- bestätigte Ausbruchs- und Rücklaufniveaus;
- Ausgangsbereiche starker Bewegungen;
- ergänzende volatilitätsbasierte Niveaus.

Alle Definitionen erhalten explizite Parameter und Golden Samples. Eine
Änderung erzeugt eine neue Generatorversion.

### Zonenbildung

Nahe Preisniveaus werden zusammengeführt, wenn ihr Abstand höchstens

```text
max(2 Ticks, 0,15 × aktuelle 5-Minuten-ATR)
```

beträgt.

Eine Zone speichert mindestens untere und obere Grenze, Mittelpunkt,
Berührungen, letzte Berührung, beteiligte Zeitrahmen, relatives Volumen,
Reaktionsstärke und Quelltyp.

### Strukturwert

Strukturelle Relevanz wird auf `0` bis `1` normalisiert:

```text
structureScore =
  reactionStrength      × 0,25
+ timeframeConfirmation × 0,25
+ recency               × 0,20
+ touches               × 0,20
+ relativeVolume        × 0,10
```

Der Wert sagt nicht unmittelbar Profitabilität voraus. Er dient nur der
deterministischen Auswahl relevanter, nicht redundanter Marktstrukturen vor der
Modellbewertung.

### Stop-Loss-Niveaus

Long-Stops entstehen unter relevanten Swing-Tiefs, Unterstützungen,
Ausbruchs-/Rücklaufzonen oder Handelsspannen. Short-Stops entstehen
spiegelbildlich oberhalb entsprechender Widerstände.

Der Sicherheitspuffer lautet als Startregel:

```text
max(2 Ticks, aktueller Spread in Ticks, 0,10 × 5-Minuten-ATR)
```

Long-Stops werden konservativ nach unten, Short-Stops nach oben auf gültige
Ticks gerundet.

### Take-Profit-Niveaus

Long-Ziele entstehen oberhalb der Referenz an Swing-Hochs,
Widerstandszonen, Sitzungs-/Vortageshochs, Handelsspannengrenzen und
bestätigten Bewegungsprojektionen. Short-Ziele entstehen spiegelbildlich
unterhalb der Referenz.

Long-TPs werden konservativ nach unten, Short-TPs nach oben auf gültige Ticks
gerundet.

Die Entscheidungsreferenz ist der zum Entscheidungspunkt bekannte,
ausführungsseitig geeignete Preis. Unmittelbar vor einer echten Order werden
Abstände, Kosten und Risk-to-Reward mit dem tatsächlich planbaren Einstieg
erneut geprüft.

### Volatilitätsbasierter Rückfall

ATR-basierte Niveaus dürfen nur ergänzende Vergleichs- oder Rückfallkandidaten
erzeugen und werden als `VolatilityFallback` markiert. Unzureichende
Marktstruktur oder Datenqualität darf zu `NoTrade` führen; der Generator muss
nicht künstlich die maximale Kandidatenzahl erreichen.

### Bildung und Filterung

Je Richtung werden zunächst höchstens:

- vier strukturell stärkste Stop-Niveaus;
- vier strukturell stärkste Ziel-Niveaus

kombiniert. Vor der Modellbewertung werden Kandidaten verworfen, wenn:

- TP oder SL auf der falschen Seite der Referenz liegen;
- Tick- oder instrumentspezifische Distanzgrenzen verletzt werden;
- das Netto-Risk-to-Reward nach Kosten unter `1:1` liegt;
- Ziel oder Stop innerhalb eines unzulässigen Markt-/Kostenrauschens liegen;
- die benötigte Datenqualität nicht zulässig ist.

Kontostand, Positionsgröße, freie Tradeplätze und Kontorisiko gehören nicht zum
Candidate Generator. Sie werden später durch Strategy und Account Risk Guard
geprüft.

Nach Filterung bleiben höchstens zwölf Kandidaten je Richtung und damit
höchstens 24 Modellbewertungen pro Entscheidung.

### Deduplizierung

Kandidaten mit Stop und Ziel innerhalb

```text
max(2 Ticks, 0,15 × 5-Minuten-ATR)
```

gelten als redundant.

Beibehalten wird in dieser Reihenfolge:

1. höherer Strukturwert;
2. Bestätigung in mehr Zeitrahmen;
3. aktuelleres Niveau;
4. konservativerer Preis;
5. stabil definierter technischer Tie-Breaker.

Sortierung und Tie-Breaker müssen in Python und .NET identisch sein.

### Modellübergabe

Jeder Kandidat enthält mindestens:

- Richtung;
- Stop- und Zielabstand in Ticks;
- Stop- und Zielquelltyp als stabile C#-Enumcodes;
- Strukturwerte;
- beteiligte Zeitrahmen;
- Rundungs- und Pufferinformationen;
- Generatorversion.

Das Modell liefert wie in ADR-018 beschlossen mindestens `expectedNetR`,
`estimatedHoldingMinutes`, `holdingTimeP90Minutes` und technische Gültigkeit.

### Identität und Nachvollziehbarkeit

Jeder persistierte Kandidat erhält eine UUID Version 7. Zusätzlich wird ein
deterministischer Fingerprint aus Generatorversion, Richtung, Preisen,
Quelltypen und relevanten Parametern gebildet. Der Fingerprint verbindet
identische Ergebnisse zwischen Python und .NET, ersetzt aber nicht die
fachliche ID.

Für Entscheidungen bleiben mindestens ausgewählter Kandidat, Quellen,
Strukturwerte, Filterzusammenfassung, Generatorversion und Fingerprint
nachvollziehbar. Vollständige verworfene Kandidatentabellen bleiben gemäß
ADR-020 und ADR-021 grundsätzlich temporär.

### Laufzeitbudget

- Zielzeit je Strategy-Entscheidung: höchstens 100 Millisekunden;
- harte Grenze: 500 Millisekunden;
- maximale Ausgabe: 24 Kandidaten.

Bei Überschreitung entsteht keine Order. Die Entscheidung lautet technisch
blockiert beziehungsweise `NoTrade`, und die Strategy Instance wird
`Degraded`. Wiederholte Überschreitung führt gemäß ADR-027 zur
Strategy-Sperre und kontrollierten Wiederherstellung.

Die Werte sind Startparameter und werden auf der Zielhardware gemessen.

### Python-/NET-Parität

Golden Samples decken mindestens ab:

- Trend und Seitwärtsmarkt;
- Ausbruch und Rücklauf;
- mehrere nahe Swings;
- fehlende Daten;
- niedrige und hohe Volatilität;
- niedrigen und hohen Spread;
- Rollover und verkürzten Handelstag;
- keine zulässige Struktur.

Verglichen werden erkannte Swings, Zonen, Strukturwerte, Preise,
Tickrundung, Puffer, Filtergründe, Reihenfolge und Fingerprints. Eine
Abweichung blockiert Modellpaket beziehungsweise Strategy-Aktivierung.

## Parameteränderungen

Lookbacks, Swingfenster, Zonentoleranz, Gewichte, Puffer, Anzahl und
Laufzeitbudgets sind versionierte V1-Startparameter. Änderungen werden nur
anhand vorab definierter Trainings-/Validierungsvergleiche ausgewählt. Der
unbekannte Abschlusstest sowie Shadow- und Paper-Ergebnisse dürfen nicht
nachträglich zur Optimierung derselben Version verwendet werden.

## Begründung

Kausale Swings und strukturbezogene Zonen leiten TP und SL aus tatsächlich
bekannten Marktbewegungen ab. Begrenzung, Deduplizierung und Laufzeitbudget
halten parallele Strategy Instances beherrschbar. Der gemeinsame
Kandidatenscorer kann plausible Alternativen vergleichen, während Konto- und
Positionsrisiko außerhalb des Modells bleiben.

## Folgen

- Candidate Generator, Parameter, Fingerprint und Quelltypen werden
  versioniert.
- Python und .NET benötigen gemeinsame Golden Samples und
  Kompatibilitätstests.
- Instrumentbezogene Tick- und Distanzgrenzen folgen in versionierten
  Instrumentkonfigurationen.
- Die historische Labeldefinition für Netto-`R` und Haltedauer ist der nächste
  ML-Planungsschritt.
