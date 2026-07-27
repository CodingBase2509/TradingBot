# Architekturübersicht

## Logische Ebenen

```text
┌───────────────────────────────────────────────┐
│ Angular: Dashboard, Kontrolle, Warnungen      │
└──────────────────────┬────────────────────────┘
                       │ REST + SignalR
┌──────────────────────▼────────────────────────┐
│ .NET: Daten, ONNX, Risiko, Orders, Portfolio │
│ Broker, Modellverwaltung, Audit, Monitoring  │
└──────────────┬─────────────────┬──────────────┘
               │                 │ Artefakte
        offizielle API           │
               │          ┌──────▼──────────────┐
┌──────────────▼──────┐   │ Python-Forschung   │
│ Broker / Börse      │   │ Training und Tests │
└─────────────────────┘   └──────┬──────────────┘
                                 │
                         ┌───────▼──────────────┐
                         │ Daten und Registry  │
                         └──────────────────────┘
```

## Verantwortungsgrenzen

- **.NET** ist Produktions-, Ausführungs- und Sicherheitsebene.
- **Python** ist Forschungs-, Trainings- und Evaluationsumgebung und sendet keine Live-Orders.
- **ONNX** ist das bevorzugte Übergabeformat für freigegebene Modelle.
- **Angular** ist Bedien- und Beobachtungsebene ohne Handelslogik.

## Live-Datenfluss

```text
Datenanbieter → Prüfung → Features → ONNX-Modell
→ Risk Guard → Trade Controller → Broker
→ Ausführung und Position → Recorder → Dashboard
```

## Trainings- und Freigabefluss

```text
Rohdaten → geprüfter Trainingsstand → Training
→ unbekannter historischer Test → Kandidat
→ Shadow → Paper → Canary → Production
```

## Architekturhaltung

Für den Start wird ein modularer .NET-Monolith bevorzugt. Klare Modulgrenzen bleiben erhalten, ohne frühzeitig den Betriebsaufwand verteilter Dienste einzuführen.
