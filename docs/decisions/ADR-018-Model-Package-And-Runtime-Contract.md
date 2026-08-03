# ADR-018: Modellpaket und Laufzeitvertrag

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Python-Modelle müssen ohne Python-Dienst sicher und reproduzierbar in .NET laufen.

## Entscheidung

- Freigegebene Modelle werden als unveränderliches ONNX-Paket mit Manifest, Verträgen, Referenzdaten und Evaluation übergeben.
- Input-, Output-, Feature-, Candidate- und Generatorverträge werden explizit versioniert.
- Die .NET-Runtime prüft Kompatibilität und Referenzfälle vollständig vor einer Aktivierung.

## Begründung

Ein selbstbeschreibendes Paket löst Forschung und Laufzeit voneinander und erlaubt sicheren Rollback.

## Folgen

- Inkompatible Pakete werden abgelehnt; das letzte gültige Paket bleibt aktiv.

## Verbindliche Dokumentation

- [Communication](../architecture/Communication.md)
- [ModelLifecycle](../ml/ModelLifecycle.md)
