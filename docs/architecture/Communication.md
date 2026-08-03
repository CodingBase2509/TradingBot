# Kommunikation

| Verbindung | Weg | Zweck |
|---|---|---|
| Angular → .NET | REST | Abfragen, Einstellungen und kontrollierte Aktionen |
| .NET → Angular | SignalR | Live-Zustände, Trades und Warnungen |
| .NET ↔ Broker | offizielle Broker-API | Marktdaten, Orders, Konto und Positionen |
| .NET → ONNX | direkt im Prozess | robuste Modellentscheidung ohne Python im Live-Pfad |
| Python → Plattform | versionierte Artefakte | Modell, Vertrag, Metadaten und Testergebnisse |
| .NET → Python | unveränderliche Datenkopien | Training ohne Schreibzugriff auf Live-Zustände |
| Angular → Training Orchestrator | REST + SignalR, später | Aufträge, Status, Logs und Ergebnisse |

## Modellvertrag

Eingaben sind durch Name, Reihenfolge, Datentyp, Einheit, Zeitbezug und
Feature-Version festgelegt. Das gemeinsame Kandidatenmodell bewertet jeweils
eine zulässige Kombination aus Richtung, Stop-Loss und Take-Profit:

```json
{
  "candidate": {
    "direction": "Long",
    "stopTicks": 12,
    "takeProfitTicks": 18
  },
  "result": {
    "expectedNetR": 0.34,
    "estimatedHoldingMinutes": 95,
    "holdingTimeP90Minutes": 210,
    "valid": true
  }
}
```

`NoTrade` ist die sichere Vergleichsoption mit `0 R`. Die V1-Modellausgabe
enthält keine variable Risikofraktion. Die .NET-Plattform bestimmt die
zulässige Größe und prüft alle Schutzregeln unabhängig vom Modell. Die
Haltedauerschätzung dient Bewertung und Überwachung und löst keine automatische
Schließung aus.

## Modellpaket

Ein offizielles V1-Paket ist unveränderlich und enthält genau fünf Dateien:

```text
pkg-<uuid-v7>/
├── model.onnx
├── manifest.json
├── contracts.json
├── reference-data.parquet
└── evaluation.json
```

Das Manifest verbindet Paket-ID, Modellversion, Instrument, UTC-Zeit,
Git-Commit, MLflow-Run, Dataset-ID, alle Vertragsversionen, verwendete
Laufzeiten, ONNX-Opset sowie Name, Größe und SHA-256 jeder Datei. Änderungen
erzeugen immer eine neue Paket-ID; `latest`, `best` oder `final` sind keine
Identitäten.

`contracts.json` legt Tensoren, Typen, Formen, Featureanzahl und -reihenfolge,
Einheiten, Wertebereiche, Missing-Data-Regeln, stabile Enumcodes, maximal 24
Kandidaten und alle Outputs fest. V1 verwendet für Modellinputs grundsätzlich
`float32`; Preise, Ticks, Mengen, Codes und Zeitpunkte bleiben außerhalb der
Modelltransformation verlustfrei.

Pflichtoutputs sind `expectedNetR`, `estimatedHoldingMinutes` als P50,
`holdingTimeP90Minutes` als P90 und ein technischer Gültigkeitsstatus. Alle
Werte müssen endlich sein, Haltedauern dürfen nicht negativ und P90 nicht
kleiner als P50 sein. Fehlende oder unbekannte Features und Outputs werden
nicht stillschweigend ersetzt oder ignoriert.

## Paritätsprüfung

`reference-data.parquet` enthält mindestens 500 deterministische,
repräsentative Fälle einschließlich verschiedener Marktphasen, Grenzwerte,
Rollover, Freitag und ungültiger Daten.

Exakt übereinstimmen müssen insbesondere Enumcodes, Tickwerte, Mengen,
Kandidatenreihenfolge, Featurefolge, Qualitätsstatus, Ablehnungsgründe,
Candidate Fingerprints, Schwellenentscheidung, Richtung und Kandidatenauswahl.
Für normalisierte Features gilt als Startwert eine absolute Toleranz von
`1e-7` oder relative Toleranz von `1e-6`; für Modelloutputs `1e-5`
beziehungsweise `1e-4`. Eine andere fachliche Entscheidung lehnt das Paket
unabhängig von der kleinen numerischen Abweichung ab.

Die .NET-Ladeprüfung kontrolliert Pflichtdateien, Prüfsummen,
Vertragsversionen, Zielstufe, ONNX-Kompatibilität, Tensoren, Referenzfälle und
Evaluation. V1 konvertiert keine Verträge automatisch. Bei einem Fehler bleibt
das vorherige gültige Paket aktiv; ohne gültiges Paket gibt es keine neuen
Trades.

## Grundregeln

- Python schreibt keine Live-Orders.
- Angular enthält keine Handelslogik.
- Ein inkompatibles Artefakt wird nicht geladen.
- Schnittstellenänderungen erhalten eine neue Version.
- Ein Message Broker wird erst eingeführt, wenn asynchrone Last oder Entkopplung ihn rechtfertigt.
- Die Forschungsoberfläche darf nur bekannte, versionierte Auftragstypen
  starten und keine freien Python-Befehle ausführen.

## Interne Kommunikation

Module eines Prozesses verwenden direkte, typisierte Aufrufe und fachlich
benannte Ereignisse. Ein interner Message Broker oder Mediator wird erst bei
gemessenem asynchronem oder betrieblichem Bedarf eingeführt. Wiederholbare
externe Vorgänge tragen Idempotenzschlüssel; Reihenfolge und Zustand werden
dauerhaft nachvollziehbar gespeichert.
