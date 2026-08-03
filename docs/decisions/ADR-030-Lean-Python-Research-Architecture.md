# ADR-030: Schlanke Python-Trainings- und Forschungsarchitektur

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Die Forschungsumgebung benötigt reproduzierbare Abläufe, soll aber nicht durch viele Dienste und Schichten aufgebläht werden.

## Entscheidung

- V1 verwendet ein installierbares Python-Paket mit Contracts, Data, Research, Modeling und Jobs.
- Offizielle Abläufe laufen über eine kleine CLI und eine früh validierte versionierte Konfiguration.
- Notebooks dienen nur der Erkundung; MLflow zeichnet Läufe auf. Queue, verteilter Orchestrator und eigene Forschungs-UI sind nicht V1.

## Begründung

Ein modularer Python-Monolith hält Logik auffindbar, testbar und später gezielt erweiterbar.

## Folgen

- Nur fachlich notwendige und referenzierte Ergebnisse werden dauerhaft gespeichert.

## Verbindliche Dokumentation

- [Components](../architecture/Components.md)
- [Training](../ml/Training.md)
