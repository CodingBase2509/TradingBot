# ADR-014: Datenimport, Aufbewahrung und Backup

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Historische Databento-Daten werden kostenpflichtig beschafft und bilden die
Grundlage reproduzierbarer Datensätze, Backtests und Modelle. Für V1 muss
festgelegt werden, wie Daten in das Projekt gelangen und welche
Aufbewahrungsgrundsätze gelten. Konkrete Speicherprodukte und Verzeichnisse
werden erst im technischen Design ausgewählt.

## Entscheidung

### Manueller Import

- Der Eigentümer lädt freigegebene Datenbestände manuell von Databento herunter.
- Modell, Trading-Plattform und automatisierte Trainingsprozesse besitzen keine
  Berechtigung, Daten zu kaufen oder Kosten auszulösen.
- Jeder Download wird mit Auftrag beziehungsweise Databento-Job-ID, Zeitraum,
  Produkten, Schema, Symboltyp, Lizenzstand, Erstellungszeit und Dateiprüfsumme
  registriert.
- Zugangsdaten und API-Schlüssel werden nicht in Datendateien oder Quellcode
  gespeichert.

### Testumgebung

- Neue Downloads werden zuerst ausschließlich in die getrennte Test- und
  Forschungsumgebung geladen.
- Dort erfolgen Entpacken, Formatprüfung, Qualitätsprüfung, Vertragszuordnung,
  Rollover-Berechnung und Erzeugung der kanonischen Daten.
- Ungeprüfte Dateien gelangen nicht in einen freigegebenen Trainingsstand und
  nicht in den Paper-Live-Datenpfad.
- Erst ein bestandener Importlauf erzeugt eine unveränderliche,
  referenzierbare Datenversion.

### Rohdaten und Ableitungen

- Originaldownloads werden unverändert als Rohdaten aufbewahrt.
- Anbieterdateien werden niemals durch bereinigte oder aggregierte Daten
  überschrieben.
- Kanonische Kerzen, Features, Qualitätsberichte und Trainingsstände werden
  getrennt gespeichert.
- Jede Ableitung verweist auf Rohdatenversion, Transformationscode,
  Konfiguration und Prüfsummen.
- Aus Rohdaten reproduzierbare temporäre Zwischenergebnisse dürfen später nach
  definierten Regeln entfernt werden.
- Datenstände, die von einem Experiment, Prüfbericht oder Modellartefakt
  referenziert werden, dürfen nicht gelöscht oder verändert werden.

### Kopien und Backup

Für V1 genügen zwei getrennte Kopien:

1. lokaler Primärspeicher für Rohdaten und freigegebene Ableitungen;
2. verschlüsseltes externes Backup außerhalb dieses Primärspeichers.

Zusätzlich gilt:

- das Backup umfasst Rohdaten, Metadaten, Prüfsummen, relevante Ableitungen und
  Wiederherstellungsinformationen;
- Backup-Zugangsdaten werden getrennt von den Daten verwahrt;
- Übertragung und Ablage sind verschlüsselt;
- Backup-Erfolg wird überwacht und protokolliert;
- Wiederherstellung wird regelmäßig stichprobenartig und vor wichtigen
  Freigaben vollständig getestet.

### Noch im technischen Design festzulegen

- lokales Speichermedium und Verzeichnisstruktur;
- Format und Partitionierung der Roh- und Parquet-Daten;
- externer Backup-Anbieter beziehungsweise externes Medium;
- Backup-Frequenz;
- Verschlüsselungs- und Schlüsselverwaltung;
- technische Umsetzung der in ADR-020 beschlossenen Aufbewahrungs- und
  Bereinigungsregeln.

## Begründung

Der manuelle Kauf verhindert unbeabsichtigte Kosten. Die Testumgebung isoliert
ungeprüfte Daten vom Handels- und Freigabepfad. Unveränderte Rohdaten und
referenzierte Versionen sichern Reproduzierbarkeit. Ein lokaler Primärspeicher
plus verschlüsseltes externes Backup bietet für den privaten V1-Start einen
angemessenen, kostengünstigen Schutz.

## Folgen

- Der Import benötigt ein Manifest und einen reproduzierbaren Prüfbericht.
- Datenversionen werden erst nach bestandener Qualitätsprüfung freigegeben.
- Speicher- und Backupadapter bleiben austauschbar.
- Eine spätere Produktion mit echtem Kapital kann strengere
  Wiederherstellungs- und Redundanzanforderungen erhalten.
