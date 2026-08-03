# ADR-006: Handels- und Risikopolitik der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Der V1-Umfang benötigte eindeutige Handelszeiten, Risikogrenzen, Sperren und Notfallregeln.

## Entscheidung

- Handel ist über alle MES-Handelsphasen erlaubt; zwei Stunden vor der täglichen Pause werden keine neuen Trades eröffnet und freitags werden alle Positionen geschlossen.
- Pro Trade gelten höchstens 2 %, aggregiert 6 %, Tagesverlustsperre 8 %, maximal drei gleichgerichtete offene Trades und zehn neue Trades je CME-Handelstag.
- Nach drei Verlusttrades oder drei technischen Orderfehlern greift die jeweilige Sperre. Es gibt einen kontrollierten Systemstopp und einen Full-Stop.

## Begründung

Die Regeln begrenzen gleichzeitig Konto-, Wochenend- und Betriebsrisiken, ohne geschützte Positionen bei jeder Störung unnötig zu schließen.

## Folgen

- Der Risk Guard setzt die Grenzen außerhalb des Modells durch.
- Gegenläufige logische Trades bleiben eine spätere Erweiterung.

## Verbindliche Dokumentation

- [RiskManagement](../trading/RiskManagement.md)
- [Execution](../trading/Execution.md)
- [TradingConcept](../trading/TradingConcept.md)
