# ADR-021: Schlanke physische Speicherstruktur

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Die logischen Datenstufen aus ADR-020 benötigen eine physische Ablage. Eine
tiefe Partitionierung nach Anbieter, Datensatz, Schema, Instrument, Vertrag,
Jahr, Monat und Tag würde viele Verzeichnisse und kleine Dateien erzeugen.
Ebenso würden dauerhaft gespeicherte Features, Kandidaten und
Backtest-Zwischenstände unnötig Speicher belegen.

## Entscheidung

### Fünf Hauptbereiche

Die V1 verwendet unter einer konfigurierbaren, nicht in Git liegenden
Speicherwurzel:

```text
trading-storage/
├── raw/
├── market/
├── datasets/
├── models/
└── temp/
```

- `raw`: unveränderte Anbieteroriginale;
- `market`: versionierte kanonische Marktdaten;
- `datasets`: Manifeste und nur bei Bedarf eingefrorene Trainingsdaten;
- `models`: offizielle Modellpakete;
- `temp`: reproduzierbare Zwischenberechnungen.

Die tatsächliche Speicherwurzel wird je Umgebung konfiguriert. Training, Test
und spätere Produktion verwenden keine gemeinsame beschreibbare Wurzel.

### Originaldaten

Originale werden flach nach Anbieter und Bezugsmonat abgelegt:

```text
raw/
└── databento/
    └── 2026-07/
        ├── mes-tbbo-2026-07.dbn.zst
        ├── mes-ohlcv-1m-2026-07.dbn.zst
        └── manifest.json
```

Anbieterdateinamen werden möglichst beibehalten. Schema, Instrument,
Verträge, Zeitraum, Job-ID, Lizenzinformationen und Prüfsummen stehen im
Manifest und in PostgreSQL statt in zusätzlichen Verzeichnisebenen.

Ungeprüfte oder unvollständige Importe liegen vorübergehend unter
`temp/imports/` und werden erst nach bestandener Prüfung als Original
registriert.

### Kanonische Marktdaten

```text
market/
└── mes-v0001/
    ├── manifest.json
    ├── tbbo/
    │   └── 2026-07-28.parquet
    ├── bars-1m/
    │   └── 2026-07.parquet
    ├── bars-5m/
    │   └── 2026.parquet
    ├── bars-15m/
    │   └── 2026.parquet
    └── bars-60m/
        └── 2026.parquet
```

Instrument, konkreter Vertrag und fachlicher Handelstag sind Spalten in
Parquet und keine eigenen Ordner. Als Startaufteilung gilt:

| Datenart | Dateiaufteilung |
|---|---|
| TBBO und MBP-1 | Handelstag |
| 1-Minuten-Kerzen | Monat |
| 5-, 15- und 60-Minuten-Kerzen | Jahr |

Die Aufteilung darf anhand echter Dateigröße und Messwerte angepasst werden.
Ziel sind ausreichend große Dateien ohne unnötiges vollständiges Einlesen.
Zstandard ist die bevorzugte Startkomprimierung und wird praktisch geprüft.

### Trainingsstände

Ein Trainingsstand besteht zunächst nur aus einem unveränderlichen Manifest:

```text
datasets/
└── ds-000001/
    └── manifest.json
```

Das Manifest verweist mindestens auf kanonische Daten, Zeitraum,
Trainingsaufteilung, Feature-, Label-, Candidate-Generator-,
Kostenmodell- und Codeversion.

Reproduzierbare Features, Kandidaten und Labels werden standardmäßig unter
`temp` erzeugt. Vollständige Trainingsdateien werden nur in den Dataset-Ordner
übernommen, wenn:

- ihre Neuerzeugung unverhältnismäßig aufwendig ist;
- sie für einen offiziellen Vergleich als exakter Beleg benötigt werden;
- das zugehörige Modell für Shadow, Paper oder eine spätere Freigabe vorgesehen
  ist.

Wenn sie eingefroren werden, wird die Struktur flach gehalten:

```text
datasets/
└── ds-000001/
    ├── manifest.json
    ├── train.parquet
    ├── validation.parquet
    └── test.parquet
```

Weitere Partitionen werden erst eingeführt, wenn gemessene Dateigröße oder
Verarbeitungszeit sie erfordern.

### Modellpakete

Nur offizielle Pakete liegen unter `models`:

```text
models/
└── pkg-000001/
    ├── model.onnx
    ├── manifest.json
    ├── contracts.json
    ├── reference-data.parquet
    └── evaluation.json
```

Für V1 dürfen Feature-, Entscheidungs- und Vorverarbeitungsvertrag in
`contracts.json` sowie Referenzeingaben und -ausgaben in einer Parquet-Datei
gebündelt werden. Eine spätere Trennung ändert den fachlichen Vertrag nicht.

MLflow verwaltet Forschungsartefakte in seinem konfigurierten
Artefaktbereich. Ein offizielles Modellpaket wird kontrolliert nach `models`
exportiert; die Handelsplattform liest nicht direkt aus dem
MLflow-Forschungsbereich.

### Temporäre Daten

`temp` enthält beispielsweise:

```text
temp/
├── imports/
├── features/
├── candidates/
├── training/
└── backtests/
```

Der gesamte Bereich ist reproduzierbar, darf nach den Schutzprüfungen aus
ADR-020 bereinigt werden und benötigt kein externes Backup.

Vollständige abgelehnte Kandidatentabellen und alle Zwischenmodelle werden
nicht standardmäßig aufbewahrt. Stattdessen bleiben zusammengefasste Anzahlen,
Ablehnungsgründe, Metriken und relevante Fehlerbeispiele erhalten.

### PostgreSQL und IDs

PostgreSQL registriert IDs, Status, Speicher-URI, Größe, Prüfsumme, Herkunft
und Beziehungen. Große Dateien werden nicht als BLOB gespeichert.

Lesbare IDs verwenden Typ und fortlaufende Identität, beispielsweise:

```text
mes-v0001
ds-000001
pkg-000001
run-000001
```

Bezeichnungen wie `latest`, `best`, `final` oder `new` sind für unveränderliche
Dateien und Verzeichnisse unzulässig. Aktives Modell und Champion stehen als
auditierbarer Zustand in PostgreSQL.

### Sicheres Veröffentlichen

```text
temporär schreiben
→ Inhalt prüfen
→ Prüfsummen und Manifest erstellen
→ unveränderlich veröffentlichen
→ in PostgreSQL registrieren
```

Veröffentlichte Versionen werden niemals überschrieben. Unvollständige
temporäre Ausgaben dürfen nach Prüfung entfernt werden.

## Begründung

Die fachlich wichtigen Informationen gehören in Manifeste, Parquet-Spalten und
PostgreSQL, nicht in eine tiefe Ordnerhierarchie. Wenige stabile Bereiche
erleichtern lokalen Betrieb, Backup und eine spätere Umstellung auf NAS oder
Object Storage.

Originale, tatsächliche Handelsereignisse und freigaberelevante Modellpakete
bleiben erhalten. Reproduzierbare Zwischenberechnungen werden nur dauerhaft
gespeichert, wenn Neuerzeugung oder Beweiswert ihre Aufbewahrung rechtfertigen.

## Folgen

- Die ersten Databento-Testdateien bestimmen die endgültigen
  Parquet-Dateigrößen und Komprimierungseinstellungen.
- Manifeste und PostgreSQL-Registrierung tragen mehr Metadaten, um flache Pfade
  zu ermöglichen.
- Dataset Builder und Training müssen temporäre Daten reproduzierbar
  neuerzeugen können.
- Backups schließen `temp` aus.
- Eine komplexere Partitionierung benötigt später einen gemessenen Nutzen.
