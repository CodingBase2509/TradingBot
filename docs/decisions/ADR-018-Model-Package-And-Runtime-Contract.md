# ADR-018: Modellpaket und Laufzeitvertrag

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Python trainiert und exportiert Modelle, während der sicherheitskritische
Handelskern sie direkt in .NET ausführt. Eine einzelne ONNX-Datei reicht dafür
nicht aus: Feature-Reihenfolge, Vorverarbeitung, Kandidatenparameter,
Ausgabebedeutung, Herkunft und technische Vergleichstests müssen gemeinsam
versioniert und geprüft werden.

## Entscheidung

### Unveränderliches Modellpaket

Ein freigabefähiges Modell wird als unveränderliches Paket mit eindeutiger
Paket-ID übergeben. Es enthält mindestens:

```text
model-package/
├── model.onnx
├── manifest.json
├── contracts.json
├── reference-data.parquet
└── evaluation.json
```

Das Manifest verknüpft das Paket mindestens mit:

- Modell-, Paket- und Vertragsversion;
- Git-Commit und sauberem Arbeitsstand;
- Datensatz-, Feature-, Label- und Kalenderversion;
- Trainings- und Evaluationslauf;
- Python-, ONNX- und ONNX-Runtime-Version;
- Erstellungszeitpunkt;
- Prüfsummen aller Paketdateien.

Große Detailberichte dürfen außerhalb des Pakets liegen, müssen dann jedoch
über unveränderliche IDs und Prüfsummen referenziert werden.

Die konkrete V1-Bündelung in fünf Dateien, Pflichtschemas, 500 Referenzfälle
und Python-/NET-Toleranzen regelt ADR-033.

### Eingabevertrag

Die Inferenz bewertet eine Menge zulässiger Handelskandidaten zu einer
abgeschlossenen 5-Minuten-Entscheidungskerze. Jeder Kandidat besteht aus:

- demselben Markt- und Kontext-Feature-Vektor;
- Richtung `Long` oder `Short`;
- Stop-Loss-Abstand;
- Take-Profit-Abstand.

Die Stop- und Zielwerte werden vor der Inferenz durch den versionierten
Candidate Generator aus der vorherigen Marktstruktur abgeleitet. Sie sind keine
primär festen ATR-Stufen. Das Modell bewertet diese adaptiven Alternativen und
erzeugt keine ungeprüften freien Orderpreise.

Generatorversion, Parameter, Quelltypen und Fingerprint sind Bestandteil des
Modell- beziehungsweise Entscheidungskontexts gemäß ADR-028.

Für jedes Feature legt der Vertrag mindestens Name, feste Reihenfolge,
Datentyp, Einheit, Zeitbezug, Berechnungsversion, Behandlung fehlender Werte
und zulässigen Wertebereich fest.

Eine semantische Änderung, Umordnung, Ergänzung oder Entfernung eines Features
erzeugt eine neue Vertragsversion und erfordert Training und Prüfung eines
neuen Modells. Automatische Feldzuordnung oder stillschweigende Ersatzwerte
sind im Laufzeitpfad nicht zulässig.

### Ausgabevertrag

Das gemeinsame V1-Kandidatenmodell liefert für jeden Kandidaten mindestens:

- `expectedNetR`: erwartetes Ergebnis nach Kosten in `R`;
- `estimatedHoldingMinutes`: erwartete aktive Marktzeit bis zur Schließung;
- `holdingTimeP90Minutes`: obere Haltedauerschätzung;
- einen technischen Gültigkeitsindikator.

Weitere Diagnosewerte wie Wahrscheinlichkeit eines positiven Ergebnisses oder
Unsicherheit dürfen später ergänzt werden, bestimmen aber nicht eigenständig
die Positionsgröße.

Die Haltedauerschätzung ist keine Schließungsorder. Die Plattform kann daraus
unter Beachtung des Börsenkalenders einen erwarteten Schließungszeitpunkt für
Anzeige und Überwachung ableiten. TP, SL, Freitagsschließung und Notfallregeln
bleiben vorrangig.

`NoTrade` ist keine künstliche Order, sondern die sichere Vergleichsoption mit
dem Basiswert `0 R`. Die Decision Engine vergleicht alle gültigen Kandidaten,
wendet die noch zu validierende Entscheidungsschwelle an und darf stets
`NoTrade` wählen.

Vor einer Order prüft der .NET-Kern unabhängig vom Modell unter anderem:

- Netto-Risk-to-Reward von mindestens `1:1`;
- Handelszeit, Freitagsregel und Datenqualität;
- Risiko-, Verlust-, Trade- und Parallelitätsgrenzen;
- verpflichtenden und bestätigten Stop-Loss und Take-Profit.

Das Modell bestimmt weder Risikoprozent noch Positionsgröße und kann keine
Schutzregel umgehen.

### Vorverarbeitung

Fachliche Marktfeatures werden außerhalb des ONNX-Modells durch die
versionierte Feature Engine berechnet. Rein mathematische
Modelltransformationen, beispielsweise Skalierung, werden bevorzugt in den
ONNX-Graphen eingebettet.

Ist das technisch nicht möglich, beschreibt `contracts.json` die
Transformation vollständig. Python und .NET müssen sie dann mit denselben
Golden Samples auf Parität prüfen. Nicht deklarierte Vorverarbeitung ist
unzulässig.

### Export- und Ladeprüfung

Der Übergabeweg lautet:

```text
Python-Training
→ ONNX-Export
→ Vergleich Python gegen ONNX Runtime
→ Paketbildung und Prüfsummen
→ technische Prüfung in .NET
→ fachliche Freigabe
→ kontrollierte Promotion nach Test
```

.NET lädt ein Paket nur, wenn:

- alle Pflichtdateien vorhanden und ihre Prüfsummen korrekt sind;
- Paket-, Feature-, Entscheidungs- und Runtime-Versionen unterstützt werden;
- Feature-Anzahl, Reihenfolge, Typen und Wertebereiche passen;
- die Referenzfälle innerhalb festgelegter numerischer Toleranzen dieselben
  Ergebnisse wie der geprüfte Python-/ONNX-Export liefern;
- Ausgaben endlich und fachlich gültig sind;
- Evaluation und manuelle Freigabe für die Zielumgebung vorliegen.

Bei einem Fehler bleibt das zuletzt gültige Modell aktiv. Gibt es kein gültiges
Modell, werden keine neuen Trades eröffnet.

### Status und Aktivierung

Artefakte sind unveränderlich. Status, Freigaben, Aktivierungen und Rücknahmen
werden in PostgreSQL gespeichert und auditiert. Ein Modell darf sich niemals
selbst freigeben oder aktivieren.

Die letzte stabile Paketversion bleibt für einen kontrollierten Rollback
verfügbar. Test und spätere Produktion besitzen getrennte Freigaben; eine
Testfreigabe berechtigt nicht zum Echtgeldbetrieb.

## Begründung

Das Paket macht aus Modell, Berechnungsvorschrift und Prüfnachweisen eine
eindeutige Einheit. Ein versionierter Vertrag verhindert, dass ein technisch
ladbares Modell mit falscher Feature-Reihenfolge oder anderer Bedeutung
ausgeführt wird.

Der gemeinsame Kandidatenscorer entspricht der beschlossenen Lernaufgabe und
hält Handels- und Risikoregeln außerhalb des Modells. Referenzfälle prüfen die
kritische Grenze zwischen Python und .NET vor jeder Aktivierung.

## Folgen

- Paket- und Vertrags-Schemas sowie Starttoleranzen sind mit ADR-033
  spezifiziert und werden am ersten echten Modell praktisch bestätigt.
- Label und Kalibrierung der Netto-`R`- und Haltedauerschätzung werden im
  nächsten ML-Detaildesign festgelegt.
- ONNX-Modelle und große Pakete liegen gemäß ADR-017 außerhalb von Git;
  Verträge, Schemas und kleine Golden Samples werden mit Git versioniert.
