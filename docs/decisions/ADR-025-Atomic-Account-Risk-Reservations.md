# ADR-025: Atomare Risikoreservierung je Konto

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Gleichzeitige Strategy-Entscheidungen dürfen dasselbe Kontorisiko, denselben Tradeplatz oder dasselbe Tages-Token nicht mehrfach verbrauchen.

## Entscheidung

- Ein zentraler Account Risk Coordinator reserviert Risiko, Tradeplatz, Tages-Token und Instrumentrichtung in einem unteilbaren PostgreSQL-Vorgang.
- Teilfüllungen teilen Reservierung und Positionsrisiko; unklare Brokerzustände bleiben vollständig gebunden.
- Nach Neustart werden Reservierungen erst nach vollständigem Brokerabgleich freigegeben.

## Begründung

Atomare, konservative Reservierung verhindert Überbuchung und berücksichtigt Orders, die trotz fehlender lokaler Bestätigung ausgeführt sein könnten.

## Folgen

- V1 benötigt keinen verteilten Lock-Dienst.

## Verbindliche Dokumentation

- [RiskManagement](../trading/RiskManagement.md)
- [Storage](../architecture/Storage.md)
