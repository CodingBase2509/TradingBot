# Training

## Lernaufgabe

Das Modell soll nicht den exakten zukünftigen Preis vorhersagen. Es soll die Qualität möglicher Handelsentscheidungen in der aktuellen Marktsituation einschätzen:

1. Handeln oder kein Trade?
2. Long oder Short?
3. Welcher Anteil des erlaubten Risikobudgets?
4. Welche Stop- und Zielabstände?
5. Welche maximale Haltedauer?

Diese Fragen können intern durch mehrere Teilmodelle beantwortet werden, solange der externe Vertrag stabil bleibt.

## Trainingsablauf

```text
Rohdaten → Qualitätsprüfung → versionierter Datenstand
→ Features → historische Lernvorlagen → zeitliche Aufteilung
→ Training → Abstimmung → unbekannter Abschlusstest
→ ONNX-Export → vollständiges Modellartefakt
```

## Zeitliche Trennung

Daten werden nicht zufällig gemischt. Vergangenheit dient zum Lernen; spätere Zeiträume dienen Abstimmung und Abschlusstest. Walk-Forward-Läufe wiederholen diesen Ablauf über mehrere Zeitfenster.

## Startmodelle

Begonnen wird mit einfachen, gut prüfbaren Verfahren, etwa Gradient Boosting. Neuronale Netze, Sequenzmodelle oder Reinforcement Learning folgen nur, wenn einfachere Modelle nachweislich nicht ausreichen.

## Reproduzierbarkeit

Jeder Lauf speichert Datenstand, Feature-Version, Codeversion, Konfiguration, Bibliotheksumgebung, Zufallsstartwerte, Modell und Ergebnisse.

## Verbesserung

Live- und Paper-Daten werden gesammelt und später in neue Offline-Trainingsstände aufgenommen. Ein neues Modell ersetzt das aktive Modell nur nach vollständiger Prüfung.
