# ADR-014: Datenimport, Aufbewahrung und Backup

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Manuell beschaffte Anbieterdaten benötigen einen nachvollziehbaren Weg von der Originaldatei zum nutzbaren Datenstand.

## Entscheidung

- Downloads werden manuell übernommen, unverändert registriert und erst danach geprüft und verarbeitet.
- Originaldaten und Ableitungen bleiben getrennt; jede Ableitung verweist auf Herkunft, Version und Prüfsumme.
- Lokaler Primärspeicher und externes Backup reichen für V1; Backups werden hostseitig gesteuert.

## Begründung

Der manuelle, manifestbasierte Prozess ist für das anfängliche Volumen einfacher und ausreichend kontrollierbar.

## Folgen

- Konkrete Pfade, Rechte, Frequenzen und Wiederherstellungsziele folgen im Betriebskonzept.

## Verbindliche Dokumentation

- [MarketData](../trading/MarketData.md)
- [Storage](../architecture/Storage.md)
- [Deployment](../architecture/Deployment.md)
