# ADR-017: Speicherarchitektur und Git-Versionierung

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Das Projekt erzeugt sehr unterschiedliche Daten: transaktionale
Betriebszustände, große historische Zeitreihen, unveränderliche Originaldateien,
Modelle, Konfigurationen und Geheimnisse. Diese Daten benötigen unterschiedliche
Speichereigenschaften. Gleichzeitig müssen Code, Verträge und Experimente
reproduzierbar zusammengeführt werden.

## Entscheidung

### PostgreSQL

PostgreSQL ist der relationale V1-Speicher. Plattform und MLflow verwenden
logisch getrennte Bereiche. Der Plattformbereich enthält:

- Datensatz-, Feature-, Label- und Experimentmetadaten;
- Modellstatus, Promotion und Rollback;
- Aufträge und Laufstatus;
- Entscheidungen und Ablehnungsgründe;
- Risk-Guard-Zustände und Sperren;
- Orders, Ausführungen und Positionsprojektionen;
- Brokerabgleiche;
- append-only Fach- und Auditereignisse;
- Benutzer, Rollen und bestätigungspflichtige Aktionen.

Der MLflow-Bereich speichert Forschungsmetadaten wie Experimente, Läufe,
Parameter, Metriken und Artefaktverweise. Plattform und MLflow greifen nicht
direkt auf die Tabellen des jeweils anderen Bereichs zu.

Test und spätere Produktion verwenden getrennte Datenbanken beziehungsweise
vollständig getrennte Instanzen. Training erhält keinen Schreibzugriff auf
operative Test- oder Produktionszustände.

Das operative Datenmodell verwendet fachliche Schemas, UUID Version 7 und
stabile C#-Enumcodes gemäß ADR-026.

### Parquet

Parquet-Dateien speichern große analytische Tabellen:

- TBBO und kanonische Kerzen;
- historische Features und Labels;
- unveränderliche Trainingsstände;
- große Backtest-Trade-, Equity- und Kandidatentabellen;
- exportierte analytische Snapshots.

Parquet ist kein Server. Dateien werden partitioniert, mit Manifesten und
Prüfsummen versehen und über Python sowie bei Bedarf DuckDB analysiert.
Die physische V1-Struktur bleibt gemäß ADR-021 bewusst flach; Instrument,
Vertrag und Handelstag werden bevorzugt als Spalten statt als
Verzeichnisebenen gespeichert.

### Datei- und Artefaktablage

Eine lokale, später austauschbare Datei- beziehungsweise Objektablage speichert:

- unveränderte Databento-Originaldownloads;
- ONNX-Modelle;
- vollständige Modellartefakte;
- Datensatzmanifeste;
- Qualitäts-, Evaluations- und Freigabeberichte;
- große Logs und Diagnosepakete;
- Kalender- und Anbieteroriginale.

Metadaten, Beziehungen, Status und Prüfsummen liegen in PostgreSQL. Die großen
Dateien selbst werden nicht als Datenbank-BLOB gespeichert.

### Secret Store

Geheimnisse werden getrennt von Git, PostgreSQL, Parquet und
Modellartefakten gespeichert:

- Databento-API-Schlüssel;
- IBKR-Zugänge;
- Datenbankpasswörter;
- Backup-Zugänge;
- Signatur- und Verschlüsselungsschlüssel.

Das konkrete Secret-Store-Produkt wird mit dem Hostingdesign festgelegt.

### Git als Versionsquelle

Git versioniert kleine, textbasierte und codeabhängige Projektbestandteile:

- Python-, .NET- und Angular-Quellcode;
- Feature-, Label-, Training-, Backtest- und Risikodefinitionen;
- Modell-, Daten-, Event- und API-Verträge;
- Datenbankmigrationen;
- Abhängigkeitsdateien und Lockfiles;
- Import-, Training-, Export- und Evaluationsskripte;
- Dokumentation und ADRs;
- kleine Golden Samples und erwartete Testergebnisse.

Golden Samples decken mindestens Features, Datenlücken, Rollover,
Kostenberechnung, Stop/TP, Python/.NET-Parität und Modellvertrag ab.

### Nicht in Git

Nicht in Git gelangen:

- vollständige Databento-Daten;
- große Parquet-Dateien;
- vollständige Trainingsstände;
- automatisch erzeugte Feature- und Labelbestände;
- umfangreiche Backtestergebnisse und Logs;
- ONNX-Modelle und große Binärartefakte;
- Geheimnisse und lokale Zugangsdaten;
- temporäre oder maschinenspezifische Dateien.

Git LFS und DVC werden in V1 nicht benötigt. Ihre Einführung benötigt später
einen nachgewiesenen Nutzen und eine neue Entscheidung.

### Verbindung von Code, Daten und Experiment

Jeder offizielle Daten-, Trainings-, Backtest- und Exportlauf speichert:

- Git-Commit;
- Kennzeichen eines sauberen Arbeitsverzeichnisses;
- Datensatz-, Feature-, Label- und Kalenderversion;
- vollständige Laufkonfiguration;
- Laufzeit- und Abhängigkeitsversionen;
- Zufallsstartwerte;
- erzeugte Artefakt- und Berichts-IDs.

Offizielle Validierungs- und Freigabeläufe werden nur aus einem sauberen
Git-Arbeitsstand gestartet. Nicht eingecheckte Codeänderungen sind dafür nicht
zulässig.

### Fachliche Datenhoheit

| Datenart | Maßgebliche Quelle |
|---|---|
| historische Rohdaten | unveränderte Databento-Dateien |
| Börsenzeiten | versionierter CME-Kalender |
| abgeleitete Marktdaten | reproduzierbarer Dataset Builder |
| Features und Labels | versionierte Berechnungsverträge |
| Modell | unveränderliches Modellartefakt |
| tatsächliche Ausführung | Interactive Brokers |
| Entscheidungsbegründung | append-only Auditjournal |
| aktueller interner Zustand | relationale Zustandsprojektion |
| Geheimnisse | Secret Store |

## Begründung

PostgreSQL eignet sich für konsistente, zusammenhängende und veränderliche
Zustände. Parquet ist effizient für große spaltenorientierte Analysen.
Dateiablagen bewahren große unveränderliche Originale und Artefakte ohne
unnötige Datenbanklast.

Git speichert die reproduzierbare Berechnungsvorschrift, nicht sämtliche großen
Eingaben und Ergebnisse. Die Verknüpfung über Commit, Versionen, Manifeste und
Prüfsummen stellt den vollständigen Zusammenhang trotzdem her.

## Folgen

- Eine spezielle Time-Series-Datenbank, Kafka und Elasticsearch werden in V1
  nicht benötigt.
- Operative Tabellen enthalten aktuellen Zustand und append-only Ereignisse,
  ohne eine vollständige Event-Sourcing-Architektur für jedes Modul zu
  erzwingen.
- Datei- und Tabellenformate erhalten versionierte Schemas.
- Feinabstimmung von Partitionierung, PostgreSQL-Indizes und Constraints,
  Secret Store und Backuptechnik folgt im Implementierungs- beziehungsweise
  Hostingdesign.
