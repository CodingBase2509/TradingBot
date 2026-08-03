# ADR-027: Fehlerisolierung und vollständige Trennung der Trainingsumgebung

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Ein einzelner Strategy-Fehler soll nicht unnötig alle Modelle stoppen; globale Unsicherheit darf dagegen nicht lokal behandelt werden.

## Entscheidung

- Fehler sperren den kleinsten sicheren Bereich: Strategy, Instrument, Konto oder gesamte Plattform.
- Strategy-Neustarts sind begrenzt und zustandsgeprüft; Wiederaufnahme verlangt je nach Ursache technische Prüfung und manuelle Freigabe.
- Training ist vollständig von Test und Produktion isoliert und tauscht ausschließlich unveränderliche geprüfte Pakete und Datenkopien aus.

## Begründung

Begrenzte Fehlerdomänen erhöhen Verfügbarkeit, ohne gemeinsame Risiko-, Broker- oder Datenunsicherheit zu verharmlosen.

## Folgen

- Offene Positionen bleiben unter zentraler Schutz- und Reconciliation-Verantwortung.

## Verbindliche Dokumentation

- [Deployment](../architecture/Deployment.md)
- [RiskManagement](../trading/RiskManagement.md)
