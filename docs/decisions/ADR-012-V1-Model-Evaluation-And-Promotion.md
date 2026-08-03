# ADR-012: Modellevaluation und Promotion der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Ein Kandidat benötigt vorab festgelegte historische, Shadow- und Paper-Hürden.

## Entscheidung

- Die historische Mindestprüfung verwendet 300 Signalgruppen über mindestens 24 unbekannte Monate, fünf Walk-Forward-Fenster und die festgelegten Profit-, Drawdown-, Konzentrations- und Kostenstressgrenzen.
- Danach folgen mindestens vier Wochen Shadow sowie acht Wochen und 100 Paper-Signalgruppen.
- Freigabe und Rückstufung sind manuell, explizit und auditierbar.

## Begründung

Zeitliche, statistische und operative Prüfungen reduzieren Overfitting und messen die reale Entscheidungs- und Ausführungskette.

## Folgen

- Canary und Production liegen außerhalb von V1.

## Verbindliche Dokumentation

- [Evaluation](../ml/Evaluation.md)
- [ModelLifecycle](../ml/ModelLifecycle.md)
