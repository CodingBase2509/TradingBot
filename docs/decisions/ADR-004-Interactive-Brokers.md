# ADR-004: Interactive Brokers als bevorzugter Broker

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Benötigt wird ein Broker mit breiter Marktabdeckung, Long/Short-Unterstützung, Paper-Umgebung und brauchbarer .NET-Anbindung.

## Entscheidung

Interactive Brokers wird Broker für V1-Paper-Trading und ist der bevorzugte
Broker für einen späteren Echtgeldbetrieb. Der erste Brokeradapter wird gegen
die TWS-API beziehungsweise den IB Gateway entwickelt.

Historische Trainingsdaten werden ausdrücklich von einem spezialisierten,
separat auszuwählenden Datenanbieter bezogen. Interactive Brokers ist dafür
nicht die maßgebliche Quelle.

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
- Vor Implementierung werden Kontoeröffnung für einen deutschen Privatkunden,
  MES-Handelsberechtigung, aktuelle Gebühren, Marktdatenabonnements,
  Paper-Verhalten und API-Funktionen praktisch verifiziert.
- Vor einem späteren Echtgeldbetrieb werden Ein- und Auszahlung,
  Notfallzugriff, Kundengeldschutz und die dann geltenden Vertragsbedingungen
  erneut geprüft.
