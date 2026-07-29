# Deployment und Betrieb

## Geplantes Startbild

- .NET-Anwendung als modularer Monolith;
- ein ausführbarer .NET-Plattform-Host statt verteilter interner Dienste;
- Angular wird gebaut und durch den .NET-Plattformhost ausgeliefert;
- Python-Umgebung nur für Import, Training und Tests;
- ein installierbares Python-Forschungspaket mit einem schlanken CLI statt
  verteilter Forschungsdienste;
- MLflow für Experimentaufzeichnung und Forschungsartefakte;
- ONNX Runtime im .NET-Prozess;
- PostgreSQL für betriebliche Zustände, Metadaten und Audit;
- Parquet-Dateien für große Zeitreihen, Features, Labels und Backtestergebnisse;
- lokale, später austauschbare Datei-/Objektablage für Originaldaten, Modelle,
  Trainingsstände und Berichte;
- IB Gateway oder TWS als Brokerzugang.

Es werden zwei eigene Anwendungsimages gepflegt: `trading-research` und
`trading-platform`. Das Plattformimage enthält .NET-Backend und gebautes
Angular-Frontend.

## Umgebungsbild

- **Training:** Python, historische Daten, Trainingsspeicher und MLflow; eigene
  isolierte Zone auf dem Home-Server ohne Brokerzugänge.
- **Test:** .NET-Plattform, Angular, Backtestadapter, IBKR-Live-Daten und
  IBKR-Paper-Zugang; eigene isolierte Zone auf demselben Home-Server.
- **Produktion:** später getrennte .NET-/Angular-Auslieferung mit eigenem
  IBKR-Live-Zugang auf einem dedizierten Cloud-Server; in V1 nicht aktiviert.

Gemeinsame Quellkomponenten werden getrennt konfiguriert und ausgeliefert.
Datenbanken, Geheimnisse, Brokerbenutzer und Artefaktbereiche werden nicht
zwischen den Umgebungen geteilt.

Training/Forschung ist eine vollständig isolierte Laufzeit- und
Sicherheitszone. Test und Produktion benötigen weder Python noch MLflow zur
Laufzeit. Zwischen den Zonen bestehen keine gemeinsamen beschreibbaren
Speicher oder Datenbanken. Rückflussdaten und Modellpakete passieren
kontrollierte Export-, Quarantäne-, Prüf- und Freigabegrenzen.

PostgreSQL und Parquet sind beschlossen. Konkrete Datei-/Objektablage, Secret
Store und Hosting bleiben offen.

MLflow bleibt auf die Trainingsumgebung begrenzt. Test und Produktion erhalten
keinen direkten MLflow-Zugriff und laden ausschließlich durch die Plattform
geprüfte Modellpakete.

Training startet mit lokalen, reproduzierbaren CLI-Jobs. Forschungs-UI,
verteilter Orchestrator und Job Queue gehören nicht zur V1.

Auch die .NET-Plattform beginnt mit einer Solution, einem ausführbaren
Plattformprojekt und einem kompakten Testprojekt. Zusätzliche Dienste,
Assemblies und Frameworkschichten benötigen einen gemessenen oder
sicherheitsrelevanten Nutzen.

Die lokale Speicherwurzel verwendet die fünf Bereiche `raw`, `market`,
`datasets`, `models` und `temp`. Pfade bleiben flach; zusätzliche
Partitionierung wird nur aufgrund gemessener Dateigröße oder Laufzeit
eingeführt.

Modellpakete werden manuell zwischen den Stufen kopiert. Der Model Manager
scannt das zoneneigene Modellverzeichnis, prüft Pakete und registriert sie.
Erst eine bewusste UI-Auswahl eines verfügbaren Pakets erzeugt eine
versionierte Strategy Instance. Details regelt
[ADR-034](../decisions/ADR-034-Deployment-Zones-And-Manual-Model-Promotion.md).

## Betriebsgrundsätze

- Forschungsumgebung und Live-System werden getrennt.
- Nur signierte oder per Prüfsumme verifizierte, freigegebene Artefakte gelangen in Produktion.
- Konfiguration und Geheimnisse liegen außerhalb von Quellcode und Modellartefakten.
- Rollback auf die letzte stabile Modellversion muss ohne erneutes Training möglich sein.
- Backups und Wiederherstellung werden regelmäßig getestet.
- Automatische Bereinigung darf nur ausdrücklich temporäre, reproduzierbare und
  nicht referenzierte Daten entfernen.

## Ausfallverhalten

- Python-Ausfall beeinflusst laufende Inferenz nicht.
- Angular-Ausfall stoppt nicht automatisch den Kern, erzeugt aber Alarm.
- Daten-, Modell- oder Brokerunsicherheit blockiert neue Trades.
- Nach Neustart erfolgt zuerst der Abgleich mit dem Broker.
- Verhalten offener Positionen wird pro Ausfallart vorab definiert und getestet.

Fehler werden möglichst auf Strategy, Instrument oder Konto begrenzt.
PostgreSQL-, Zeit-, globaler Risiko- und Plattformzustandsfehler blockieren die
gesamte betroffene Handelsumgebung. Die Anwendung bleibt für Schutz,
Reconciliation, Wiederherstellung und Alarmierung möglichst aktiv.

## Noch festzulegen

- lokale, Cloud- oder hybride Zielumgebung;
- Hochverfügbarkeit und Wiederanlaufziele;
- Observability-Stack;
- Zeitdienst und Zeitsynchronisation;
- konkrete Backup-Frequenz und Wiederherstellungsziele;
- Freigabeprozess und Rollen.
