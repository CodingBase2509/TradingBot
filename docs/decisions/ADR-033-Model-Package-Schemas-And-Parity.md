# ADR-033: Modellpaket-Schemas und Python-/NET-Parität

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

ADR-018 definiert das Modellpaket fachlich. Für eine sichere, einfache
Implementierung müssen Dateiumfang, Pflichtfelder, Referenzfälle,
Fließkommatoleranzen und fachliche Kompatibilitätsregeln konkret feststehen.

## Entscheidung

### Fünf Paketdateien

Ein offizielles V1-Modellpaket enthält:

```text
pkg-<uuid-v7>/
├── model.onnx
├── manifest.json
├── contracts.json
├── reference-data.parquet
└── evaluation.json
```

Weitere große Detailberichte werden nicht kopiert, sondern unveränderlich über
URI und SHA-256 referenziert.

### Manifest

`manifest.json` enthält mindestens:

- Schemaversion;
- Modellpaket-ID als UUID Version 7;
- Modellversion, Instrument und UTC-Erstellungszeit;
- Git-Commit und Kennzeichen eines sauberen Arbeitsstands;
- MLflow-Run-ID und Dataset-ID;
- Feature-, Candidate-, Output-, Generator-, Label- und
  Kostenmodellversion;
- Python-, Trainingsbibliotheks-, ONNX- und Runtimeinformationen;
- tatsächliche ONNX-Opset-Version;
- Name, Größe und SHA-256 jeder Paketdatei.

Die unterstützten Opset-/Runtimekombinationen werden durch die .NET-Runtime
explizit freigegeben. Die Architektur legt keine hypothetische zukünftige
Opset-Version fest.

Ein Paket ist unveränderlich. Jede inhaltliche Änderung erzeugt eine neue
Paket-ID. Namen wie `latest`, `best` oder `final` sind keine Paketidentitäten.

### Gebündelter Vertrag

`contracts.json` bündelt:

- Eingabetensorname, Datentyp und Form;
- feste Featureanzahl und -reihenfolge;
- Featureindex, Name, Einheit, Zeitrahmen, Wertebereich und
  Missing-Data-Regel;
- stabile Candidate-, Richtungs-, Struktur- und Quellcodes;
- maximale Kandidatenzahl von 24;
- Stop-/TP-Abstände und Strukturwerte;
- Namen, Datentypen und Wertebereiche der Outputs;
- Vertrags- und Schemaversionen.

Modellinput verwendet grundsätzlich `float32`, sofern der Vertrag nicht
ausdrücklich etwas anderes festlegt. Preise, Mengen, Enumcodes, Zeitpunkte und
Tickabstände bleiben außerhalb numerischer Modelltransformationen in
verlustfreien fachlichen Typen.

Pflichtoutputs sind:

- `expectedNetR`;
- `estimatedHoldingMinutes` als P50;
- `holdingTimeP90Minutes` als P90;
- technischer Gültigkeitsstatus.

Alle Modelloutputs müssen endlich sein. Haltedauern dürfen nicht negativ sein
und P90 darf nicht kleiner als P50 sein. `NaN`, Unendlich, fehlende oder
unerwartete Outputs lehnen das Paket beziehungsweise die Inferenz ab.

Unbekannte Features werden nicht ignoriert. Fehlende Features werden nicht
stillschweigend mit null oder einem Standardwert ersetzt.

### Referenzdaten

`reference-data.parquet` enthält mindestens 500 repräsentative und
deterministisch ausgewählte Fälle mit:

- Case-ID und Szenariotyp;
- vollständigem Inputvektor;
- erwarteten Python-/ONNX-Outputs;
- erwarteter Kandidatenrangfolge;
- erwarteter Schwellenentscheidung;
- erwarteter Auswahl beziehungsweise NoTrade.

Abgedeckt werden mindestens:

- Trend, Seitwärtsmarkt, Ausbruch und Rücklauf;
- niedrige und hohe Volatilität;
- niedriger und hoher Spread;
- Werte nahe der Entscheidungsschwelle;
- sehr ähnliche Kandidaten;
- minimale und maximale zulässige Featurewerte;
- Rollover und Freitag;
- ungültige und fehlende Werte als Ablehnungstest.

### Exakte Vergleiche

Exakt übereinstimmen müssen:

- Enumcodes;
- Tickpreise und Tickabstände;
- Mengen und Richtung;
- Kandidatenanzahl und -reihenfolge;
- Featureanzahl und -reihenfolge;
- Zeitstempel und Qualitätsstatus;
- Filter- und Ablehnungsgründe;
- Candidate Fingerprints;
- Schwellenentscheidung;
- Long, Short oder NoTrade;
- ausgewählter Kandidat;
- sicherheitsrelevante Gültigkeits- und Risk-to-Reward-Prüfungen.

### Numerische Feature-Toleranz

Für normalisierte Fließkommafeatures gilt:

```text
absolute Abweichung ≤ 1e-7
oder
relative Abweichung ≤ 1e-6
```

Preise, Ticks, Mengen, Enumcodes und Zeitintervalle verwenden keine
Fließkommatoleranz.

### Numerische Modelloutput-Toleranz

Für `expectedNetR`, P50 und P90 gilt als V1-Start:

```text
absolute Abweichung ≤ 1e-5
oder
relative Abweichung ≤ 1e-4
```

Diese Toleranz erlaubt keine fachliche Abweichung. Führt ein numerisch kleiner
Unterschied zu anderer Rangfolge, Schwellenentscheidung, Richtung,
Kandidatenauswahl oder Gültigkeit, wird das Paket abgelehnt.

### Evaluation

`evaluation.json` enthält kompakt:

- Schema, Status und UTC-Zeit;
- Anzahl Signalgruppen und Monate;
- Profit Factor und maximalen Drawdown;
- positive und gesamte Walk-Forward-Fenster;
- Ergebnisse der Slippage-/Kostenstressstufen;
- P50-Kalibrierfehler und P90-Abdeckung;
- URI und SHA-256 des vollständigen Berichts.

Die Datei ersetzt weder den vollständigen Prüfbericht noch die manuelle
Plattformfreigabe.

### Ladeprüfung in .NET

```text
Pflichtdateien
→ SHA-256 und Größen
→ Schema- und Vertragsversionen
→ Instrument- und Umgebungsfreigabe
→ ONNX-/Opset-/Runtimekompatibilität
→ Tensoren, Formen und Typen
→ mindestens 500 Referenzfälle
→ numerische Toleranzen
→ exakte fachliche Entscheidungen
→ Evaluation
→ manuelle Freigabe
```

Bei Fehler bleibt das vorherige gültige Paket aktiv. Ohne gültiges Paket
eröffnet die Strategy Instance keine neuen Trades.

### Einfache Versionsregel

V1 implementiert keine automatische Vertragskonvertierung:

- exakt unterstützte Vertragsversion: Prüfung zulässig;
- unbekannte oder abweichende Version: Paket ablehnen;
- Vertragsänderung: neue ganzzahlige Version;
- alte Runtime nur erhalten, wenn ein aktives oder rollbackfähiges Paket sie
  benötigt.

## Begründung

Fünf Dateien halten das Paket klein und vollständig. Exakte fachliche
Vergleiche verhindern, dass scheinbar unbedeutende Fließkommaabweichungen eine
andere Handelsentscheidung erzeugen.

Eine einfache exakte Versionsregel ist für V1 sicherer und verständlicher als
eine flexible Kompatibilitäts- und Konvertierungsschicht.

## Folgen

- Sprachneutrale JSON-/Parquet-Schemas werden in Git versioniert.
- Python-Exporter und .NET-Loader verwenden dieselben Golden- und
  Referenzfälle.
- Modellpaketexport scheitert bei fehlenden Pflichtdaten oder Paritätsfehlern.
- Toleranzen werden beim ersten echten Modell gemessen und dürfen nur
  versioniert sowie mit erneuter vollständiger Prüfung geändert werden.
