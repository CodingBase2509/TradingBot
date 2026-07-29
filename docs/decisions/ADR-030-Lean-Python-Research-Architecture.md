# ADR-030: Schlanke Python-Trainings- und Forschungsarchitektur

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Die isolierte Trainingsumgebung muss Import, Datenqualität, Features,
Kandidaten, Labels, Training, Evaluation, MLflow und Modellpaketexport
abdecken. Fachliche Verantwortlichkeiten rechtfertigen jedoch nicht
automatisch eigene Dienste, Projekte oder Frameworkschichten.

Übersichtlichkeit und reproduzierbare einfache Abläufe haben in V1 Vorrang vor
hypothetischer Erweiterbarkeit.

## Entscheidung

### Ein modularer Python-Monolith

V1 verwendet ein installierbares Python-Paket, eine isolierte
Python-Umgebung, ein Kommandozeilenprogramm, versionierte Konfigurationen,
Parquet, MLflow und automatisierte Tests. Es gibt keine Microservices oder
verteilte Forschungsplattform.

### Fünf Quellbereiche

```text
python/
├── pyproject.toml
├── src/
│   └── trading_research/
│       ├── contracts/
│       ├── data/
│       ├── research/
│       ├── modeling/
│       └── jobs/
├── configs/
├── tests/
└── notebooks/
```

- `contracts`: Features, Kandidaten, Labels, Modelloutputs, Manifeste und
  stabile Enumcodes;
- `data`: Databento-Import, Qualität, kanonische Kerzen, Rollover,
  Dataset Builder und Parquet;
- `research`: Feature Engineering, Candidate Generator, Label Generator,
  historische Simulation, aktive Marktzeit und Censoring;
- `modeling`: zeitliche Aufteilung, Training, Walk-Forward, Evaluation,
  MLflow, ONNX-Parität und Modellpaket;
- `jobs`: bekannte reproduzierbare CLI-Abläufe.

Training, Evaluation und Export bleiben fachlich getrennte Abläufe, benötigen
aber keine eigenen Dienste oder Python-Projekte.

### CLI

Das CLI bietet zunächst:

```text
import
build-dataset
train
evaluate
build-package
run
```

`run` führt den reproduzierbaren Standardablauf aus einer versionierten
Konfiguration aus. Einzeljobs dienen Wiederholung und Fehlersuche.

Ein späterer Training Orchestrator darf nur bekannte Jobtypen aufrufen und
keine beliebigen Python-Befehle entgegennehmen.

### Konfiguration

Ein Lauf verwendet zunächst eine zusammenhängende YAML- oder
JSON-Konfiguration mit Laufname, Zufallsstartwert, Datenstand, Zeitraum,
Feature-, Candidate-, Label- und Kostenmodellversion, Modellparametern sowie
Evaluations-/Walk-Forward-Konfiguration.

Die vollständige Konfiguration wird in MLflow und im Manifest gespeichert. Sie
wird erst bei realer Unübersichtlichkeit oder Wiederverwendung aufgeteilt.

### Notebooks

Notebooks dienen Exploration, Visualisierung und Hypothesenbildung. Bestätigte
Logik wird in normale Python-Module übertragen, getestet und über einen
offiziellen Job ausgeführt.

Notebooks dürfen weder einzige Quelle offizieller Berechnung sein noch Modelle
freigeben oder direkt nach Test beziehungsweise Produktion exportieren.

### Offizielle und parallele Läufe

Ein offizieller Lauf prüft Konfiguration und Eingaben, erfasst Git-, Daten- und
Umgebungsversion, legt einen MLflow-Lauf an, führt einen bekannten Job aus und
speichert Ergebnisse sowie Prüfsummen.

Parallele Läufe besitzen eigene Arbeitsverzeichnisse, MLflow-Run-IDs,
Zufallsstartwerte und Ressourcenlimits. Kanonische Eingangsdaten werden
gemeinsam nur lesend verwendet.

### Tests

V1 verwendet:

- Unit Tests;
- Golden Tests für Python-/NET-Parität;
- Leakage Tests gegen zukünftige Informationen;
- Integration Tests der kleinen vollständigen Pipeline;
- Reproduction Tests offizieller Läufe.

Ein Golden-Test-Unterschied bei Feature, Kandidat, Label oder Modellvertrag
blockiert Export beziehungsweise Aktivierung.

### Minimale dauerhafte Ergebnisse

Ein normaler Forschungsversuch speichert dauerhaft nur Konfiguration,
Herkunft, Metriken, wesentliche Diagnoseinformationen und
Entscheidung/Ablehnungsgrund.

Große Features, Kandidaten, Labels, Zwischenmodelle und Backtestergebnisse
bleiben reproduzierbar und temporär. Dauerhafte Übernahme erfolgt nur bei hohem
Neuerzeugungsaufwand oder Freigaberelevanz gemäß ADR-020 und ADR-021.

### Bewusst nicht Bestandteil der V1

- Forschungs-Weboberfläche;
- verteilter Training Orchestrator;
- eigene Job Queue;
- Microservices und Plugin-Architektur;
- allgemeines Workflow-Framework;
- eigene Experimentdatenbank neben MLflow;
- Kubernetes, Kafka oder verteiltes Rechnen;
- universelles Datenanbieter- oder Modellframework.

### Regel für neue Komplexität

Eine neue Komponente, Abstraktion oder Infrastruktur wird nur eingeführt, wenn
mindestens ein nachgewiesener Grund besteht:

- eigene Sicherheitsgrenze;
- unabhängiger Lebenszyklus;
- gemessenes Leistungsproblem;
- notwendige Fehlerisolierung;
- realer zweiter Anwendungsfall;
- die bestehende einfache Lösung erfüllt die Anforderung nicht vertretbar.

„Könnte später nützlich sein“ genügt nicht. Eine gemeinsame Abstraktion
entsteht grundsätzlich erst bei einem realen zweiten Anwendungsfall oder einem
klaren Test-/Sicherheitsbedarf.

## Abhängigkeitsrichtung

```text
CLI-Jobs
→ Anwendungsabläufe
→ fachliche Berechnungen
→ typisierte Verträge
→ einfache Parquet-/MLflow-/Dateiadapter
```

Fachliche Berechnungen kennen keine Databento-Pfade, MLflow-Datenbank oder
Test-/Produktionssysteme. Adapter bleiben klein und werden nicht durch ein
allgemeines Providerframework ersetzt.

## Isolation

ADR-027 bleibt uneingeschränkt gültig. Das Python-Paket läuft ausschließlich
in der Trainingszone, ohne Brokerzugang oder direkten Schreib-/Laufzeitzugriff
auf Test und Produktion. Austausch erfolgt nur über geprüfte unveränderliche
Exporte und Modellpakete.

## Begründung

Fünf Quellbereiche halten zusammengehörige Funktionen auffindbar, ohne jede
Verantwortung in ein eigenes Projekt zu verwandeln. Ein Paket und ein CLI
reichen für reproduzierbare lokale V1-Abläufe. MLflow deckt
Experimentverwaltung ab, sodass keine eigene Plattform nachgebaut wird.

Die bewusste Nicht-Einführung hypothetischer Abstraktionen reduziert Code,
Fehlerflächen, Wartung und Einarbeitungsaufwand.

Auch die konkrete Python-Implementierung folgt den sprachübergreifenden
Einfachheits- und Vollständigkeitsregeln aus ADR-032.

## Folgen

- Die Python-Struktur bleibt klein und wächst nur aufgrund realer
  Anforderungen.
- Offizielle Logik lebt in getesteten Modulen, nicht in Notebooks.
- Training Orchestrator und Forschungs-UI werden erst nach stabilen CLI-Jobs
  erneut bewertet.
- Bibliotheken und Paketverwaltung werden im
  Implementierungsvorbereitungsschritt minimal ausgewählt und versioniert.
