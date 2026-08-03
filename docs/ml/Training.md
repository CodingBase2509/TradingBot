# Training

## Lernaufgabe

Das Modell soll nicht den exakten zukünftigen Preis vorhersagen. Es soll die Qualität möglicher Handelsentscheidungen in der aktuellen Marktsituation einschätzen:

1. Handeln oder kein Trade?
2. Long oder Short?
3. Welche Stop- und Zielabstände?
4. Wie lange bleibt die Alternative voraussichtlich offen?

V1 verwendet ein gemeinsames Kandidatenmodell. Es erhält neben der
Marktsituation Richtung, Stop und Ziel einer adaptiv erzeugten Alternative. Es
schätzt deren erwartetes Nettoergebnis in `R`, erwartete aktive Haltedauer und
eine obere Haltedauerschätzung. Alle zulässigen Alternativen werden mit
demselben Scorer verglichen.

Das V1-Modell wählt keine Risikofraktion. Die Plattform plant einen akzeptierten
Trade mit höchstens 2 % Risiko und darf die Größe reduzieren oder den Trade
ablehnen.

Das Modell bewertet historische Alternativen anhand ihres Nettoergebnisses in
`R`. Nur Alternativen mit einem erwarteten Netto-Risk-to-Reward von mindestens
`1:1` sind zulässig.

Die verbindlichen Einstiegs-, Kosten-, Ausgangs-, Netto-`R`-, Haltedauer- und
Censoring-Regeln stehen im [Backtesting](./Backtesting.md).

## Trainingsablauf

```text
Rohdaten → Qualitätsprüfung → versionierter Datenstand
→ Features → historische Lernvorlagen → zeitliche Aufteilung
→ Training → Abstimmung → unbekannter Abschlusstest
→ ONNX-Export → vollständiges Modellartefakt
```

Die isolierte Python-Umgebung wird als ein schlankes Paket mit den fünf
Bereichen Contracts, Data, Research, Modeling und Jobs umgesetzt. Offizielle
Abläufe verwenden bekannte CLI-Jobs und eine zusammenhängende versionierte
Laufkonfiguration. Zusätzliche Dienste oder Schichten entstehen nur bei einem
konkreten fachlichen oder gemessenen Bedarf.

Originaldaten und kanonische Marktdaten bleiben von Features und Labels
getrennt. Schnelle Forschungsstände dürfen temporär sein. Offizielle
Evaluationen verwenden ausschließlich eingefrorene, geprüfte Trainingsstände
mit eindeutiger ID, Manifest und Prüfsummen.

## Zeitliche Trennung

Daten werden nicht zufällig gemischt. Vergangenheit dient zum Lernen; spätere Zeiträume dienen Abstimmung und Abschlusstest. Walk-Forward-Läufe wiederholen diesen Ablauf über mehrere Zeitfenster.

## Startmodelle

Begonnen wird mit einfachen, gut prüfbaren Verfahren, etwa Gradient Boosting. Neuronale Netze, Sequenzmodelle oder Reinforcement Learning folgen nur, wenn einfachere Modelle nachweislich nicht ausreichen.

## Reproduzierbarkeit

Jeder Lauf speichert Datenstand, Feature-Version, Codeversion, Konfiguration, Bibliotheksumgebung, Zufallsstartwerte, Modell und Ergebnisse.

MLflow zeichnet Experimente, Parameter, Metriken, Herkunft und
Forschungsartefakte auf. Es unterstützt Vergleich und Auswahl interessanter
Läufe, erteilt aber keine Freigabe für Shadow, Paper oder späteren
Echtgeldbetrieb.

## Verbesserung

Live- und Paper-Daten werden gesammelt und später in neue Offline-Trainingsstände aufgenommen. Ein neues Modell ersetzt das aktive Modell nur nach vollständiger Prüfung.

Für jeden eröffneten Trade werden die beim Einstieg geschätzte Haltedauer,
tatsächlich vergangene Zeit, aktive Marktzeit und der Schließungsgrund
gespeichert. TP- und SL-Ausgänge liefern vollständige Vergleichswerte.
Freitagsschließung, Full-Stop, manuelle und technische Eingriffe werden als
abgeschnittene beziehungsweise künstlich beendete Beobachtungen gekennzeichnet
und nicht unbesehen wie natürliche Ausgänge trainiert.
