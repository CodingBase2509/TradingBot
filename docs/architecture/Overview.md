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

## Instrumentneutraler Kern

Der Plattformkern kennt kein fest eingebautes MES-Symbol. Er arbeitet mit einer
stabilen `InstrumentId` und bezieht alle marktspezifischen Eigenschaften aus
versionierten Stammdaten, Konfigurationen und Adaptern:

```text
InstrumentId
→ Datenquellen-Symbol und Brokerkontrakt
→ Börse, Kalender und Sitzungen
→ Tickgröße, Tickwert, Multiplikator und Mindestmenge
→ Kosten-, Rollover- und Handelsrichtlinien
→ Strategy Instance
```

Ein Instrument ist handelbar, wenn Datenquelle und Broker es unterstützen und
eine vollständige, validierte Instrumentkonfiguration vorhanden ist. Futures-
spezifische Funktionen wie Kontraktablauf und Rollover werden als
Instrumentfähigkeit modelliert; Instrumente ohne Ablauf verwenden diese
Funktion nicht.

MES ist ausschließlich das erste V1-Profil, mit dem dieser allgemeine Weg
implementiert und abgenommen wird. Ein weiteres Symbol soll durch Stammdaten,
Konfiguration und Zuordnung in den vorhandenen Adaptern ergänzt werden können,
ohne Risiko-, Entscheidungs-, Order- oder Positionskern zu verändern.

## Umgebungen

```text
Training
→ Daten, Features, Labels, Training, Evaluation, ONNX

Test
→ Backtest, Shadow mit Live-Daten, IBKR Paper Trading

Produktion
→ später IBKR Echtgeld; in V1 nicht aktiviert
```

Die Umgebungen besitzen getrennte Konfigurationen, Zustände, Geheimnisse,
Artefaktbereiche und Brokerzugänge. Ein einfacher Laufzeitwechsel von Paper zu
Echtgeld ist nicht zulässig.

## Live-Datenfluss

```text
Market → Feature & Intelligence → Decision → Risk Guard
→ Trade Management → Execution → Broker
→ Reconciliation → Operations & Audit → Dashboard
```

Model Management stellt Feature & Intelligence ausschließlich ein geprüftes,
für die jeweilige Umgebung freigegebenes Modellpaket bereit.

## Mehrere Strategy Instances

Eine Plattform kann mehrere versionierte Strategy Instances parallel hosten.
Jede verbindet Markt, Zeitrahmen, Features, Candidate Generator, Modell,
Schwelle, Ausführungsmodus und Risikoprofil.

```text
gemeinsame Marktdaten
→ mehrere Strategy Instances
→ Strategy Risk
→ gemeinsames Account/Portfolio Risk
→ Execution Router
→ gemeinsamer Brokeradapter und Brokerkonto
```

Shadow und Simulated Paper sind unabhängig parallel möglich. Broker Paper ist
in V1 auf eine Ausführungsgruppe je Instrument begrenzt, damit die
Broker-Nettoposition Modellresultate nicht vermischt.

## Trainings- und Freigabefluss

```text
Rohdaten → geprüfter Trainingsstand → Training
→ unbekannter historischer Test → Kandidat
→ Shadow → Paper → Canary → Production
```

## Architekturhaltung

Für den Start ist ein modularer .NET-Monolith beschlossen. Klare Modulgrenzen
bleiben erhalten, ohne frühzeitig den Betriebsaufwand verteilter Dienste
einzuführen.

Physisch besteht die Plattform aus einem ASP.NET-Core-Host, einem kleinen
Shared Kernel, neun fachlichen Modulprojekten, einer gemeinsamen
Testbibliothek sowie getrennten Unit- und Integrationstestprojekten. Die
verbindliche Zuordnung aller Funktionen und Abhängigkeitsregeln beschreibt die
[.NET-Solution- und Projektarchitektur](./SolutionStructure.md).
