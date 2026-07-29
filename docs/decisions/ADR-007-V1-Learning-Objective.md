# ADR-007: Lernziel und historische Handelsalternativen der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Das erste Modell benötigt ein klar definiertes Lernziel. Es soll nicht den
exakten zukünftigen Preis vorhersagen, sondern konkrete Handelsalternativen
unter realistischen Kosten bewerten. Die Risikosteuerung bleibt vom Modell
getrennt und wird durch die .NET-Plattform erzwungen.

## Entscheidung

### Lernaufgabe

Nach jeder abgeschlossenen 5-Minuten-Kerze bewertet das Modell:

- kein Trade;
- Long mit verschiedenen adaptiven Stop-/Zielkombinationen;
- Short mit verschiedenen adaptiven Stop-/Zielkombinationen.

Das Modell lernt den erwarteten Nettoertrag einer Alternative in `R` sowie eine
für die Entscheidung geeignete Wahrscheinlichkeit beziehungsweise
Vertrauensangabe. `1 R` entspricht dem geplanten Verlust bei Erreichen des Stop
Loss einschließlich der angenommenen Ausführungskosten.

„Kein Trade“ besitzt den Referenzwert `0 R`. Eine Handelsalternative wird nur
gewählt, wenn sie den später festzulegenden Mindestvorteil gegenüber „kein
Trade“ erreicht.

Der Zahlenwert dieser Schwelle wird nicht in Phase 0 fest vorgegeben. Während
der Modellvalidierung werden ausschließlich vorab definierte Kandidatenwerte
auf den Validierungsdaten verglichen. Nach der Auswahl wird die Schwelle als
Teil der Modellkonfiguration eingefroren. Sie darf weder anhand des unbekannten
Abschlusstests noch während Shadow- oder Paper-Betrieb nachträglich angepasst
werden. Jede spätere Änderung erzeugt eine neue Modell- und
Konfigurationsversion und durchläuft erneut den vollständigen Prüfprozess.

### Modellstruktur

V1 verwendet ein gemeinsames Kandidatenmodell statt mehrerer unabhängiger
Teilmodelle. Ein Kandidat besteht aus:

- Marktsituation und versionierten Features;
- Richtung Long oder Short;
- Stop-Abstand;
- Take-Profit-Abstand;

Das Modell bewertet jede zulässige Kombination mit derselben Bewertungslogik,
schätzt mindestens ihren erwarteten Nettoertrag in `R` und prognostiziert die
erwartete Zeit bis zur Schließung. Eine
Vertrauens- beziehungsweise Unsicherheitsangabe wird aus demselben
Modellartefakt oder einer fest zugeordneten, gemeinsam versionierten
Kalibrierung erzeugt.

Zur Laufzeit werden alle zulässigen Kandidaten bewertet. Die Plattform wählt den
besten Kandidaten nur dann aus, wenn er sämtliche fachlichen Mindestkriterien
erfüllt. Andernfalls lautet die Entscheidung „kein Trade“.

Die konkrete Modellfamilie ist damit nicht festgelegt. Für V1 werden zunächst
einfache, ONNX-kompatible Verfahren geprüft.

### Risk-to-Reward

Jeder neu eröffnete Trade muss zum Zeitpunkt der Orderplanung ein erwartetes
Netto-Risk-to-Reward-Verhältnis von mindestens `1:1` besitzen.

```text
erwarteter Nettogewinn am Take Profit
───────────────────────────────────── ≥ 1,0
 erwarteter Nettoverlust am Stop Loss
```

Der erwartete Nettogewinn berücksichtigt Preisgewinn abzüglich Gebühren,
Spread und Slippage. Der erwartete Nettoverlust berücksichtigt Preisverlust
zuzüglich dieser Kosten. Unterschreitet das Verhältnis nach Rundung auf gültige
Ticks oder bei geänderten Kostenannahmen den Wert `1,0`, wird der Trade
abgelehnt.

### Adaptive Kandidatenerzeugung

Stop-Loss und Take-Profit stammen nicht primär aus einem starren
Volatilitätsraster. Ein deterministischer, versionierter Candidate Generator
leitet situationsbezogene Preisniveaus ausschließlich aus den zum
Entscheidungszeitpunkt bekannten Marktbewegungen ab.

Er berücksichtigt mindestens:

- jüngste bestätigte Swing-Hochs und Swing-Tiefs;
- Unterstützungs-, Widerstands- und Ausbruchsniveaus;
- aktuelle und zurückliegende Handelsspannen;
- Volatilität, Kerzengrößen und Dochte;
- Trend- und Rücklaufstruktur;
- Spread, Liquidität und Ausführungskosten.

ATR beziehungsweise eine vergleichbare Volatilitätsmessung ist dabei ein
Eingangswert, aber nicht die alleinige Quelle der Abstände. Volatilitätsbasierte
Alternativen dürfen als zusätzliche Vergleichs- oder Rückfallkandidaten
entstehen.

Die konkreten V1-Lookbacks, Swingfenster, Zonen, Puffer, Filter,
Kandidatenlimits und Laufzeitbudgets regelt ADR-028.

Der Generator erzeugt für Long und Short mehrere fachlich plausible
Stop-/Zielpaare. Alle Preisniveaus werden anhand der versionierten
Instrumentdaten auf gültige Ticks gerundet und gegen instrumentspezifische
Mindest- und Höchstgrenzen geprüft. Tickgröße, Tickwert und Grenzen werden
nicht im Modellcode fest eingebaut.

Das Modell bewertet die verbleibenden adaptiven Kandidaten anhand derselben
vorherigen Marktbewegung. Es wählt damit situationsabhängig TP und SL und
schätzt die erwartete Haltedauer, ohne freie, ungeprüfte Preiswerte erzeugen zu
dürfen. Kombinationen, die das Netto-Risk-to-Reward von mindestens `1:1` nicht
erreichen, werden verworfen. Existiert keine sinnvolle Kombination, lautet das
Ergebnis „kein Trade“.

### Historische Simulation

- Features verwenden ausschließlich Informationen, die am
  Entscheidungszeitpunkt vorlagen.
- Der Einstieg wird zum ersten realistisch ausführbaren Preis nach der
  abgeschlossenen Signalkerze simuliert.
- Gebühren, Spread, Slippage, Tick-Größe und Verzögerung werden berücksichtigt.
- Werden Stop und Ziel innerhalb derselben verfügbaren Kerze berührt und ist die
  Reihenfolge unbekannt, wird konservativ der Stop Loss angenommen.
- TP, SL, Freitagsschließung und andere feste Plattformregeln beenden den Trade
  unabhängig von der Haltedauerprognose.
- Tages-, Positions- und Gesamtrisikogrenzen werden im vollständigen
  ereignisbasierten Backtest angewendet. Das Einzellabel bewertet zunächst die
  jeweilige Handelsalternative.
- Zeitlich überlappende Beispiele dürfen keine Informationen zwischen
  Trainings-, Abstimmungs- und Testzeiträumen übertragen.

### Risikobudget

Das V1-Modell wählt kein Risikobudget. Ein akzeptierter Trade wird anhand des
festen maximalen Risikos von 2 % geplant. Risk Guard und Trade Controller dürfen
die Größe aufgrund von Vertragsgröße, Kosten, offenen Risiken und Limits
reduzieren oder den Trade ablehnen.

Eine vom Modell gewählte Risikofraktion ist eine spätere, getrennt zu
validierende Erweiterung.

## Begründung

Die Bewertung konkreter Alternativen entspricht direkt der späteren
Handelsentscheidung. Die Darstellung in `R` macht Trades mit unterschiedlichen
Stop-Abständen vergleichbar. Das Netto-Mindestverhältnis verhindert Trades,
deren nominelles Chance-Risiko-Verhältnis durch Kosten unter `1:1` fällt.

Die feste Risikopolitik hält das erste Lernproblem überschaubar und verhindert,
dass Modellqualität und Positionsgrößenoptimierung gleichzeitig verändert
werden.

## Folgen

- Label Generator und Backtest benötigen dieselben Kosten- und
  Ausführungsannahmen.
- Feature-, Label-, Kosten-, Instrument- und
  Kandidatengeneratorversion werden gemeinsam gespeichert.
- Die Plattform prüft das Netto-Risk-to-Reward erneut unmittelbar vor der
  Orderfreigabe.
- Der Modellvertrag der V1 benötigt keine variable Risikofraktion.
- Alle Kandidaten werden durch denselben versionierten Scorer vergleichbar
  bewertet.
- Candidate Generator und Scorer werden getrennt versioniert und mit
  historischen sowie Golden-Sample-Fällen geprüft.
- Der Zahlenwert des Mindestvorteils gegenüber „kein Trade“ und die genaue
  Vertrauenskalibrierung werden im Validierungsprozess bestimmt.
- Historische Ausführung, Netto-`R`, Ausgangsklassen und Haltedauerlabels
  richten sich nach ADR-029.
