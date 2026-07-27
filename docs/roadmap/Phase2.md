# Phase 2 – Erstes lernendes Modell

## Ziel

Ein reproduzierbarer Weg von Rohdaten zu identischen Python- und .NET-Modelleingaben sowie nachvollziehbaren Shadow-Entscheidungen.

## Umfang

- versionierten Trainingsstand erstellen;
- erste Features in Python und .NET implementieren;
- Übereinstimmung automatisiert testen;
- Lernvorlagen aus simulierten Entscheidungen erzeugen;
- einfaches Modell trainieren;
- zeitlich getrennte und Walk-Forward-Tests ausführen;
- Modell als ONNX exportieren;
- vollständiges Artefakt erzeugen;
- in .NET im Shadow Mode ausführen.

## Abnahme

- Training ist reproduzierbar;
- unbekannter Testzeitraum blieb bis zum Abschluss unberührt;
- ONNX-Ausgaben entsprechen der Python-Referenz;
- Shadow-Entscheidungen sind vollständig auditierbar;
- keine Modellentscheidung kann den Risk Guard umgehen.
