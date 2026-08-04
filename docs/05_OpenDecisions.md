# Offene Entscheidungen

## Priorität B – Daten und Anbieter

- Databento-Testbestand prüfen und Anbieter endgültig bestätigen;
- Broker-Marktdaten gegen unabhängige Daten validieren.

Für das technische Design bleibt eine konservative Rollover-Ersatzregel bei
fehlenden oder fehlerhaften Volumendaten festzulegen.

## Priorität C – Technik und Betrieb

- PostgreSQL-Major-Version sowie exakte initiale Versionen von Npgsql,
  PyArrow, pandas, Parquet.Net, Docker Engine und Compose Plugin festlegen;
- konkreter Cloudanbieter, Container Runtime und IB-Gateway-Betriebsdetails;
- praktische Bestätigung der beschlossenen Modellpaket-Toleranzen am ersten
  echten Modell;
- praktische Kalibrierung der beschlossenen P50-/P90-Haltedauerschätzung;
- konkrete Health-Checks, Retryfristen und Eskalationsschwellen je Komponente;
- konkrete Hostpfade, Dateirechte und Environment-Secret-Dateien;
- konkrete PostgreSQL-Spalten, Indizes und Constraints je
  Implementierungsphase sowie Messung und Feinabstimmung der beschlossenen
  Parquet-Dateiaufteilung;
- technische Umsetzung der beschlossenen Aufbewahrung;
- Backupfrequenzen und Wiederherstellungsziele je Zone.

## Empfohlene Entscheidungsreihenfolge

1. Phase 1 beginnen und die exakten initialen Paketversionen im Projekt fixieren.
2. Parquet-Dateiprofil mit einem gemeinsamen Golden-Datensatz bestätigen.
3. Databento und Brokerdaten während der Datenanbindung praktisch validieren.
4. Rollover-Ersatzregel vor der Implementierung des Vertragswechsels festlegen.

## Entscheidungsformat

```text
Entscheidung:
Status: offen | vorgeschlagen | beschlossen | verworfen
Datum:
Kontext:
Optionen:
Gewählte Option:
Begründung:
Folgen:
Zu überprüfende Annahmen:
```
