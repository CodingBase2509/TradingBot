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

Die vollständige Paketstruktur, Kompatibilitätsprüfung und Aktivierung regelt
[ADR-018](../decisions/ADR-018-Model-Package-And-Runtime-Contract.md).
Konkrete Schemas, Referenzfälle und Toleranzen regelt
[ADR-033](../decisions/ADR-033-Model-Package-Schemas-And-Parity.md).

## Grundregeln

- Python schreibt keine Live-Orders.
- Angular enthält keine Handelslogik.
- Ein inkompatibles Artefakt wird nicht geladen.
- Schnittstellenänderungen erhalten eine neue Version.
- Ein Message Broker wird erst eingeführt, wenn asynchrone Last oder Entkopplung ihn rechtfertigt.
- Die Forschungsoberfläche darf nur bekannte, versionierte Auftragstypen
  starten und keine freien Python-Befehle ausführen.
