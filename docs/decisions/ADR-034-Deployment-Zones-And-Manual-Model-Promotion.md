# ADR-034: Deploymentzonen und manuelle Modellpromotion

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Entwicklung, Training, Test und später Live benötigen ein konkretes, aber
überschaubares Betriebsbild. Training und Test teilen zunächst einen
Home-Server, müssen aber vollständig getrennte Laufzeit- und Sicherheitszonen
bleiben.

Modellpakete sollen niemals automatisch aus Forschung nach Test oder Produktion
gelangen. Der Eigentümer kopiert Pakete manuell und ordnet sie in der
jeweiligen Plattformoberfläche bewusst einer Strategy Instance zu.

## Entscheidung

### Zielumgebungen

| Bereich | Ort | Zweck |
|---|---|---|
| Entwicklung | lokaler PC | .NET, Angular, Python, Verträge, Images und Tests |
| Training/Forschung | Home-Server, Trainingszone | Daten, Python, MLflow, Training und Modellpakete |
| Evaluierung/Test | Home-Server, Testzone | .NET, Shadow, Simulated Paper und IBKR Broker Paper |
| Live | dedizierter Cloud-Server | späterer Echtgeldbetrieb |

Datenbanken, Dateispeicher und Artefakte liegen jeweils bei der zugehörigen
Maschine und Zone. Es gibt keinen gemeinsam beschreibbaren Laufzeitspeicher.

### Zwei eigene Anwendungsimages

```text
trading-research
trading-platform
```

`trading-research` enthält Python-Paket, CLI und notwendige
Forschungs-/MLflow-Abhängigkeiten. MLflow darf aus demselben Image mit einem
anderen bekannten Startkommando laufen.

`trading-platform` enthält .NET-Backend und gebautes Angular-Frontend. Das
Frontend wird durch denselben Host ausgeliefert. Ein eigenes
Frontend-Runtime-Image wird in V1 nicht gepflegt.

PostgreSQL und IB Gateway verwenden fest versionierte Standardimages
beziehungsweise gleichwertig gekapselte Installationen. Images verwenden feste
Versionen und Digests, nicht `latest`. Secrets gelangen nicht in Image, Build
Argument oder Quellcode.

### Lokale Entwicklung

Der Entwicklungs-PC baut und testet beide Anwendungsimages sowie die
sprachneutralen Verträge. Kleine Testdaten und Golden Samples liegen lokal;
vollständige Trainingsbestände müssen nicht kopiert werden.

Lokales Debugging und Hot Reload bleiben möglich. Deploymentnahe Tests
verwenden dieselben Dockerfiles und Images wie Home- und Cloud-Server.

### Getrennte Home-Server-Zonen

Training und Test verwenden getrennte:

- Compose-Projekte beziehungsweise Containergruppen;
- Docker-Netzwerke;
- PostgreSQL-Instanzen oder vollständig getrennte Datenbanken und Benutzer;
- Volumes, Speicherwurzeln, Hostpfade und Dateirechte;
- Environment-Konfigurationen und Secrets;
- veröffentlichte Ports und Backupbereiche.

Training besitzt keine IBKR-Zugänge und kann Test-PostgreSQL oder Test-Volumes
nicht erreichen. Test kann MLflow-PostgreSQL, Trainingsspeicher und
Databento-Secrets nicht erreichen.

Die gemeinsame physische Maschine ist keine gemeinsame Laufzeit- oder
Anwendungszone.

### Trainingszone

Mindestens:

```text
trading-research
MLflow aus dem Research-Image
Training-PostgreSQL
Training-Dateispeicher
```

Research exportiert Modellpakete nur in den eigenen Trainingsbereich und kann
keine Test- oder Live-Strategy aktivieren.

### Testzone

Mindestens:

```text
trading-platform
Test-PostgreSQL
IB Gateway Paper
Test-Dateispeicher
```

Die Plattform hostet parallele Shadow- und Simulated-Paper-Instanzen sowie die
beschränkt zulässige Broker-Paper-Ausführungsgruppe.

### Livezone

Der dedizierte Cloud-Server enthält nur:

- `trading-platform`;
- Live-PostgreSQL und Live-Dateispeicher;
- IB Gateway Live;
- freigegebene Modellpakete;
- hostgesteuertes Backup und Zeitüberwachung.

Python, MLflow, Databento-Import, Notebooks, Dataset Builder und
Trainingsdaten gehören nicht auf den Live-Server. Home-Server oder
Entwicklungs-PC übernehmen bei Cloud-Ausfall nicht automatisch den
Livehandel.

### Modellverzeichnis

Test und Live besitzen jeweils ein eigenes Modellverzeichnis. Der Eigentümer
kopiert vollständige unveränderte Pakete manuell hinein:

```text
models/
├── .incoming/
└── available/
    └── pkg-<uuid-v7>/
        ├── model.onnx
        ├── manifest.json
        ├── contracts.json
        ├── reference-data.parquet
        └── evaluation.json
```

Beim Kopieren wird zuerst unter `.incoming` oder einem temporären Namen
geschrieben. Erst nach vollständiger Kopie verschiebt der Host den Paketordner
atomar nach `available`. Die Plattform ignoriert temporäre und unvollständige
Ordner und verändert Paketdateien nicht.

### Erkennung und Registrierung

Der Model Manager durchsucht beim Start und kontrolliert wiederkehrend
`available`:

```text
entdeckt
→ SHA-256 und Größen
→ Schema, Vertrag, ONNX und Runtime
→ mindestens 500 Referenzfälle
→ Evaluation und Zielstufe
→ PostgreSQL-Registrierung
```

Registrierungszustände:

```text
Discovered
Validating
Available
Invalid
Incompatible
```

Ein Ordner im Dateisystem ist keine Freigabe. Nur `Available`-Pakete erscheinen
in der UI als auswählbar. Ungültige Pakete werden nicht ausgeführt und mit
verständlich protokolliertem Grund angezeigt.

Wiederholtes Scannen ist idempotent. Gleiche Paket-ID mit anderer Prüfsumme ist
ein kritischer Konflikt und wird abgelehnt.

### Strategy-Erzeugung

Über die UI wählt der Eigentümer:

- verfügbares Modellpaket;
- Markt und Instrument;
- Datenquelle und Zeitrahmen;
- Candidate-/Feature-Konfiguration;
- Ausführungsmodus;
- Risikoprofil und Entscheidungsschwelle.

Vor Bestätigung zeigt die UI Paket-ID, Herkunft, Prüfergebnisse, Instrument,
Zielstufe, Modus und Konfigurationszusammenfassung. Die Bestätigung erzeugt
eine neue versionierte Strategy Instance beziehungsweise
Konfigurationsversion sowie einen Audit-Eintrag.

Entdeckung oder Validierung erzeugt niemals automatisch eine Strategy Instance
und aktiviert kein Trading.

### Manuelle Promotion

```text
Training
→ Paket manuell nach Test
→ technische Prüfung
→ Strategy bewusst für Shadow/Paper anlegen
→ Teststufen bestehen
→ exakt dasselbe Paket manuell nach Live
→ Production-Prüfung und gesonderte Livefreigabe
→ Live-Strategy bewusst anlegen
```

Das Paket wird zwischen Stufen nicht neu gebaut oder verändert. Paket-ID und
SHA-256-Werte bleiben identisch. Eine Testfreigabe ist keine Livefreigabe.

Live prüft zusätzlich den unveränderlichen Nachweis bestandener Test-, Shadow-
und Paper-Stufen. Seine genaue kryptographische Form wird vor Liveaktivierung
festgelegt; Live bleibt in V1 deaktiviert.

### Datenrückfluss

Paper- und spätere Live-Daten werden als unveränderliche Exporte mit Manifest
und Prüfsumme erzeugt. Der Eigentümer kopiert sie manuell in den Importbereich
der Trainingszone. Training liest nur die Kopie und schreibt niemals in
operative Zustände zurück.

### Secrets

Secrets werden als Environment Variables übergeben. Ihre Werte liegen:

- außerhalb von Git;
- nicht in Dockerfiles, Images oder Build Arguments;
- nicht als Klartext in eingecheckten Compose-Dateien;
- in hostgeschützten Dateien oder geschützter Runtime-Konfiguration;
- unter restriktiven Host-Dateirechten.

Secrets dürfen nicht protokolliert oder in Modellpakete und normale
Fehlerberichte übernommen werden.

Environment Variables sind für Hostadministratoren und Containerprozesse
sichtbar. Ihre Sicherheit hängt von Host-, Docker- und Dateirechten ab. Ein
dedizierter Secret Store wird nur bei zusätzlichem Sicherheitsbedarf
eingeführt.

### Backups

Backups werden ausschließlich vom Host gesteuert. Container können
Backupziele nicht verwalten oder Sicherungen löschen.

Hostgesteuert bedeutet:

- manuell auslösbar und zusätzlich zeitgesteuert;
- konsistente PostgreSQL-Sicherungen;
- verschlüsselte getrennte Dateikopien;
- `temp` und reproduzierbare Caches ausgeschlossen;
- regelmäßige Wiederherstellung in ein isoliertes Ziel.

Frequenz und Wiederherstellungsziele werden je Zone festgelegt. Spätestens
Live darf nicht ausschließlich von manuell ausgelösten Backups abhängen.

### Zeit

Hosts synchronisieren ihre Systemzeit. Container verwenden Hostzeit und UTC;
PostgreSQL verwendet `timestamptz`. Eine unzuverlässige Hostzeit blockiert die
betroffene Handelsumgebung. Container betreiben keinen eigenen konkurrierenden
Zeitdienst.

### Ausfallgrenzen

- Entwicklungs-PC aus: kein Einfluss auf Home oder Live.
- Trainingszone aus: kein Einfluss auf Test oder Live.
- Testzone aus: kein Einfluss auf Training oder Live.
- Home-Server aus: Training und Test aus, Live läuft unabhängig.
- Cloud-Server aus: Live betroffen, kein automatischer Ersatz durch Home.

## Begründung

Zwei gepflegte Anwendungsimages und ein gemeinsamer .NET-/Angular-Host halten
Build und Betrieb klein. Getrennte Home-Server-Zonen erhalten die beschlossene
Isolation trotz gemeinsamer Hardware.

Manuelles Kopieren und bewusste UI-Auswahl machen Promotion sichtbar und
kontrollierbar. Vollständige Paketprüfung und getrennte Stage-Freigaben
verhindern, dass eine Datei allein durch ihre Anwesenheit aktiv wird.

## Folgen

- Dockerfiles und lokale Compose-Konfigurationen werden Teil der
  Implementierungsvorbereitung.
- Hostpfade, Netzwerke, Benutzer und Volumes erzwingen die Zonengrenzen.
- Model Manager benötigt Scanner, idempotente Registrierung und UI-Status.
- Strategy-Erzeugung benötigt eine bestätigungspflichtige UI-Aktion.
- Cloudanbieter, konkrete Container Runtime, Backupfrequenzen und
  Wiederherstellungsziele bleiben vor Live festzulegen.
