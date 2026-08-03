# ADR-028: Adaptiver Candidate Generator der V1

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

TP und SL sollen aus vorheriger Marktstruktur entstehen und in Python sowie .NET kausal identisch sein.

## Entscheidung

- Ein versionierter Generator bildet nach abgeschlossenen 5-Minuten-Kerzen begrenzte Long- und Short-Kandidaten aus Swings, Zonen, Sitzungsniveaus, Spannen und Bewegungen.
- Puffer, Tickrundung, Strukturwertung, Filterung und Deduplizierung sind deterministisch; ATR bleibt nur ergänzender Fallback.
- Höchstens 24 Kandidaten werden bewertet; Fingerprints und Golden Samples sichern Herkunft und Parität.

## Begründung

Strukturbezogene Kandidaten verbinden adaptive Ausstiege mit reproduzierbarer, begrenzter Laufzeit.

## Folgen

- Parameteränderungen benötigen eine neue Version und Validierung ohne Nutzung des unbekannten Abschlusstests.

## Verbindliche Dokumentation

- [FeatureEngineering](../ml/FeatureEngineering.md)
- [Training](../ml/Training.md)
