# ADR-023: Mehrere Strategy Instances auf einer Plattform

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Mehrere Modelle und Strategien sollen parallel vergleichbar laufen und später ein gemeinsames Brokerkonto sicher nutzen.

## Entscheidung

- Jede Strategy Instance besitzt eine eigene versionierte Markt-, Modell-, Schwellen-, Modus- und Risikokonfiguration.
- Marktdaten und Plattformdienste werden geteilt; Strategy Risk wird vor gemeinsamem Account Risk geprüft.
- Ein Execution Router verhindert unzulässige Vermischung. V1 erlaubt je Instrument nur eine Broker-Paper-Ausführungsgruppe.

## Begründung

Getrennte Instanzen erlauben faire Modellvergleiche; die gemeinsame Kontosicht verhindert vervielfachte Limits.

## Folgen

- Shadow, Simulated Paper, Broker Paper und später Live bleiben explizit getrennte Modi.

## Verbindliche Dokumentation

- [Overview](../architecture/Overview.md)
- [Components](../architecture/Components.md)
- [RiskManagement](../trading/RiskManagement.md)
