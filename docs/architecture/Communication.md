# Kommunikation

| Verbindung | Weg | Zweck |
|---|---|---|
| Angular → .NET | REST | Abfragen, Einstellungen und kontrollierte Aktionen |
| .NET → Angular | SignalR | Live-Zustände, Trades und Warnungen |
| .NET ↔ Broker | offizielle Broker-API | Marktdaten, Orders, Konto und Positionen |
| .NET → ONNX | direkt im Prozess | robuste Modellentscheidung ohne Python im Live-Pfad |
| Python → Plattform | versionierte Artefakte | Modell, Vertrag, Metadaten und Testergebnisse |
| .NET → Python | unveränderliche Datenkopien | Training ohne Schreibzugriff auf Live-Zustände |

## Modellvertrag

Eingaben sind durch Name, Reihenfolge, Datentyp, Einheit, Zeitbezug und Feature-Version festgelegt. Ausgaben verwenden einen versionierten Vertrag, beispielsweise:

```json
{
  "action": "Long",
  "confidence": 0.76,
  "riskBudgetFraction": 0.15,
  "stopVolatilityFactor": 1.3,
  "takeProfitVolatilityFactor": 2.1,
  "maxHoldingMinutes": 240
}
```

`riskBudgetFraction` bezieht sich auf das von .NET erlaubte Risikobudget, nicht direkt auf den Kontowert.

## Grundregeln

- Python schreibt keine Live-Orders.
- Angular enthält keine Handelslogik.
- Ein inkompatibles Artefakt wird nicht geladen.
- Schnittstellenänderungen erhalten eine neue Version.
- Ein Message Broker wird erst eingeführt, wenn asynchrone Last oder Entkopplung ihn rechtfertigt.
