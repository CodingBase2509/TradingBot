# ADR-002: ONNX als bevorzugtes Modellformat

- **Status:** beschlossen mit Kompatibilitätsvorbehalt
- **Datum:** 27. Juli 2026

## Kontext

In Python trainierte Modelle sollen ohne dauerhaften Python-Dienst im Live-Pfad in .NET ausgeführt werden.

## Entscheidung

Freigegebene V1-Modelle werden nach ONNX exportiert und mit ONNX Runtime direkt in .NET ausgeführt.

## Begründung

- geringe betriebliche Kopplung;
- keine Netzwerkabhängigkeit für Inferenz;
- versionierbares Modellartefakt;
- Python-Ausfall stoppt laufendes Trading nicht;
- passende Trennung zwischen Forschung und Produktion.

## Folgen

- Nur zuverlässig exportierbare Modelle kommen für den V1-Live-Weg infrage.
- Python- und ONNX-Ausgaben werden mit Referenzdaten verglichen.
- Custom Operators und nicht unterstützte Modellarten benötigen später eine neue ADR.
- Das Artefakt enthält neben ONNX den Feature-Vertrag, Prüfsumme und Prüfergebnisse.
