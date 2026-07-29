# ADR-019: MLflow für Experimente und Forschungsmodelle

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Training und Forschung erzeugen viele Läufe mit unterschiedlichen Datenständen,
Features, Parametern, Modellen und Ergebnissen. Eine eigene vollständige
Experimentverwaltung würde erheblichen Entwicklungsaufwand verursachen.
Gleichzeitig dürfen veränderliche Forschungsmetadaten keine Modelle im
sicherheitskritischen Handelskern aktivieren.

## Entscheidung

### MLflow in der Trainingsumgebung

MLflow wird für V1 als Experiment- und Forschungsverwaltung eingesetzt. Es
zeichnet mindestens auf:

- Experimente, Trainings- und Evaluationsläufe;
- Parameter, Metriken, Tags und Notizen;
- Git-Commit, Daten-, Feature-, Label- und Konfigurationsversionen;
- Zufallsstartwerte und Laufzeitumgebung;
- Diagramme, Berichte und Forschungsartefakte;
- Herkunft und Vergleich trainierter Modelle.

MLflow gehört ausschließlich zur Trainingsumgebung und besitzt keine
Brokerzugänge. Training, Feature-Berechnung, Label-Erzeugung, Backtesting und
ONNX-Export bleiben Aufgaben unserer eigenen Python-Komponenten und der
gemeinsamen Plattformverträge.

### Speicher

MLflow verwendet einen eigenen logisch getrennten PostgreSQL-Bereich als
Backend Store. Große Lauf- und Modellartefakte liegen in der beschlossenen
Datei-/Objektablage und nicht als Datenbank-BLOB.

Für den lokalen Start darf MLflow ohne getrennten Serverprozess betrieben
werden. Sobald mehrere Prozesse, die Forschungsoberfläche oder parallele
Aufträge darauf zugreifen, wird ein nur in der Trainingsumgebung erreichbarer
MLflow Tracking Server eingesetzt.

### Sicherheitsgrenze

MLflow ist nicht die maßgebliche Quelle für:

- technische Kompatibilitätsprüfung eines Modellpakets;
- fachliche Freigabe;
- Shadow-, Paper-, Canary- oder Production-Status;
- aktive Modellversion einer Handelsumgebung;
- Rollback und bestätigungspflichtige Aktionen.

Diese Zustände liegen in der Plattformdatenbank und werden durch Model Manager,
Audit und die Regeln aus ADR-018 verwaltet.

MLflow-Tags oder veränderliche Aliasse wie `champion` dürfen niemals direkt ein
Modell in Test oder Produktion aktivieren. Die Handelsplattform lädt nur eine
explizit freigegebene, unveränderliche Paket-ID mit geprüften Prüfsummen.

### Übergabe

```text
Python-Lauf
→ Aufzeichnung und Vergleich in MLflow
→ ausgewählten Lauf als Modellpaket exportieren
→ technische Prüfung in .NET
→ manuelle fachliche Freigabe
→ Shadow und Paper
```

MLflow-Run-ID und gegebenenfalls Modellversion werden im Manifest des
Modellpakets gespeichert. Die Plattform-Paket-ID bleibt dennoch die
maßgebliche Identität für Aktivierung und Rollback.

### Spätere Forschungsoberfläche

Der Training Orchestrator verwendet die MLflow-Schnittstellen, um bekannte
Aufträge zu protokollieren und Ergebnisse bereitzustellen. Eine spätere
Angular-Oberfläche kann diese Informationen kontrolliert anzeigen und
vergleichen. Sie greift nicht direkt auf die MLflow-Datenbank zu und kann keine
beliebigen Python-Befehle oder Handelsfreigaben ausführen.

## Begründung

MLflow stellt vorhandene Funktionen für Laufaufzeichnung, Vergleich, Herkunft
und Artefaktverwaltung bereit. Dadurch kann sich das Projekt auf Datenqualität,
Handelslogik und Modellbewertung konzentrieren.

Die getrennte Plattformfreigabe verhindert, dass eine Forschungsaktion, ein
veränderlicher Alias oder ein Bedienfehler unmittelbar das gehandelte Modell
austauscht. Forschung und sicherheitskritischer Betrieb bleiben klar getrennt.

## Folgen

- MLflow wird Bestandteil der Python-Trainingsabhängigkeiten.
- MLflow-Schema und Plattform-Schema bleiben logisch getrennt und werden nicht
  durch direkte Tabellenzugriffe gekoppelt.
- Eigene Experimenttabellen werden nur für plattformspezifische Aufträge,
  Referenzen und Freigaben angelegt; MLflow-Funktionen werden nicht
  nachgebaut.
- Backup und Wiederherstellung umfassen MLflow-Metadaten und referenzierte
  Artefakte.
- Die konkrete Bereitstellungsart wird mit dem lokalen Hostingdesign
  festgelegt.
