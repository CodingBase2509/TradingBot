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

PostgreSQL, Parquet und lokale zoneneigene Dateiablagen sind beschlossen. Der
konkrete Cloudanbieter sowie spätere Hostpfade bleiben offen.

MLflow bleibt auf die Trainingsumgebung begrenzt. Test und Produktion erhalten
keinen direkten MLflow-Zugriff und laden ausschließlich durch die Plattform
geprüfte Modellpakete.

Training startet mit lokalen, reproduzierbaren CLI-Jobs. Forschungs-UI,
verteilter Orchestrator und Job Queue gehören nicht zur V1.

Auch die .NET-Plattform bleibt eine gemeinsam ausgelieferte Anwendung. Der Host,
der kleine Shared Kernel und die neun fachlichen Modulprojekte bilden einen
modularen Monolithen und keine getrennten Dienste. Zusätzliche Schichtprojekte,
Dienste und Frameworkschichten benötigen einen gemessenen oder
sicherheitsrelevanten Nutzen.

Die lokale Speicherwurzel verwendet die fünf Bereiche `raw`, `market`,
`datasets`, `models` und `temp`. Pfade bleiben flach; zusätzliche
Partitionierung wird nur aufgrund gemessener Dateigröße oder Laufzeit
eingeführt.

## Zonen und Container

Entwicklung findet lokal auf dem PC statt. Training und Test laufen zwar auf
demselben Home-Server, verwenden aber getrennte Containergruppen, Netzwerke,
PostgreSQL-Zugänge, Volumes, Hostpfade, Secrets, Ports und Backupbereiche.
Training besitzt keine Brokerzugänge; Test kann weder MLflow noch
Trainingsspeicher oder Databento-Secrets erreichen. Live läuft später auf einem
dedizierten Cloud-Server und enthält weder Python, MLflow, Databento-Import,
Notebooks noch Trainingsdaten.

`trading-research` enthält Python-Paket, CLI und MLflow-Abhängigkeiten.
`trading-platform` enthält .NET-Backend und gebautes Angular-Frontend.
PostgreSQL und IB Gateway verwenden fest versionierte Standardimages oder
gleichwertig gekapselte Installationen. Images verwenden feste Versionen oder
Digests statt `latest`.

## Manuelle Modellpromotion

Test und Live besitzen jeweils ein eigenes Modellverzeichnis:

```text
models/
├── .incoming/
└── available/
    └── pkg-<uuid-v7>/
```

Pakete werden manuell zuerst vollständig nach `.incoming` kopiert und danach
atomar nach `available` verschoben. Der Model Manager scannt wiederholbar,
prüft das unveränderte Paket und registriert es als `Discovered`, `Validating`,
`Available`, `Invalid` oder `Incompatible`. Gleiche Paket-ID mit abweichender
Prüfsumme ist ein kritischer Konflikt.

Nur `Available` erscheint in der UI. Erst die bewusste Auswahl von Paket,
Instrument, Datenquelle, Zeitrahmen, Candidate-/Feature-Konfiguration,
Ausführungsmodus, Risikoprofil und Schwelle erzeugt eine versionierte Strategy
Instance. Entdeckung oder Registrierung aktiviert niemals selbstständig
Trading.

```text
Training → manuelle Kopie nach Test → Shadow/Paper-Prüfung
→ exakt dasselbe Paket manuell nach Live → eigene Livefreigabe
```

Paket-ID und Prüfsummen bleiben zwischen den Stufen identisch. Eine
Testfreigabe ist keine Livefreigabe. Paper- und spätere Live-Daten fließen nur
als unveränderliche, manuell kopierte Exporte mit Manifest und Prüfsumme zurück
in die Trainingszone.

## Betriebsgrundsätze

- Forschungsumgebung und Live-System werden getrennt.
- Nur signierte oder per Prüfsumme verifizierte, freigegebene Artefakte gelangen in Produktion.
- Konfiguration und Geheimnisse liegen außerhalb von Quellcode und Modellartefakten.
- Rollback auf die letzte stabile Modellversion muss ohne erneutes Training möglich sein.
- Backups und Wiederherstellung werden regelmäßig getestet.
- Automatische Bereinigung darf nur ausdrücklich temporäre, reproduzierbare und
  nicht referenzierte Daten entfernen.
- Secrets werden als Environment Variables an Container übergeben. Ihre Werte
  liegen außerhalb von Git, Images, Build Arguments und normalen Logs in
  hostgeschützten Dateien oder Runtime-Konfigurationen.
- Backups werden ausschließlich vom jeweiligen Host gesteuert und durch
  Wiederherstellungsproben geprüft.
- Alle technischen Zeitpunkte verwenden UTC; Zeitsynchronisation und Drift
  werden überwacht.

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

Strategy-bezogene Fehler stoppen nur die betroffene Instanz und erlauben nach
Prüfung begrenzte Neustartversuche. Instrumentfehler stoppen alle zugehörigen
Instanzen. Konto-, Broker-, Zeit-, PostgreSQL-, globaler Risiko- oder
Plattformzustandsfehler stoppen alle betroffenen ausführenden Instanzen. Offene
Positionen bleiben unter zentraler Schutz- und Abgleichverwaltung. Eine
Wiederaufnahme erfolgt nie allein durch einen Prozessneustart, sondern erst
nach erfolgreicher Zustandsprüfung und – wo erforderlich – manueller Freigabe.

## Noch festzulegen

- konkreter Cloudanbieter, Container Runtime und IB-Gateway-Betrieb;
- Hochverfügbarkeit und konkrete Wiederanlaufziele;
- Observability-Stack;
- Zeitdienst und Zeitsynchronisation;
- konkrete Backup-Frequenz und Wiederherstellungsziele;
- Freigabeprozess und Rollen.
