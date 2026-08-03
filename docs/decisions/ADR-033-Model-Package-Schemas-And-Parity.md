# ADR-033: Modellpaket-Schemas und Python-/NET-Parität

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Das fachliche Modellpaket benötigte konkrete Dateien, Pflichtfelder, Referenzfälle und Vergleichstoleranzen.

## Entscheidung

- V1-Pakete enthalten model.onnx, manifest.json, contracts.json, reference-data.parquet und evaluation.json.
- Mindestens 500 Referenzfälle prüfen exakte fachliche Entscheidungen und eng tolerierte numerische Werte zwischen Python, ONNX und .NET.
- Nur exakt unterstützte Vertrags- und Runtimeversionen werden geladen; V1 besitzt keine automatische Konvertierung.

## Begründung

Ein kleines vollständiges Paket und exakte fachliche Parität verhindern stille Abweichungen im Handelsweg.

## Folgen

- Jede Paketänderung erzeugt eine neue UUID-v7-Paket-ID und neue Prüfsummen.

## Verbindliche Dokumentation

- [Communication](../architecture/Communication.md)
- [ModelLifecycle](../ml/ModelLifecycle.md)
