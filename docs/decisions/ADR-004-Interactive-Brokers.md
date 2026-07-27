# ADR-004: Interactive Brokers als bevorzugter Broker

- **Status:** vorgeschlagen, vor Implementierung aktuell zu verifizieren
- **Datum:** 27. Juli 2026

## Kontext

Benötigt wird ein Broker mit breiter Marktabdeckung, Long/Short-Unterstützung, Paper-Umgebung und brauchbarer .NET-Anbindung.

## Entscheidung

Interactive Brokers ist die bevorzugte Brokerwahl für Planung und ersten Adapter. Die endgültige Bestätigung erfolgt nach aktueller Prüfung von API, Gebühren, Marktdaten, Kontoberechtigungen und Paper-Verhalten.

## Begründung

- breite Markt- und Produktabdeckung;
- API-Nutzung aus C# möglich;
- Paper Trading;
- langfristig weniger Brokerwechsel bei Erweiterung auf weitere Märkte.

## Folgen

- IB Gateway oder TWS wird als betriebliche Abhängigkeit berücksichtigt.
- Marktdatenabonnements und deren Rechte werden separat geprüft.
- Trainingsdaten stammen nicht ausschließlich vom Broker.
- Der Brokeradapter verhindert, dass IBKR-spezifische Details in den fachlichen Kern gelangen.
