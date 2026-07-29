# Offene Entscheidungen

## Priorität B – Daten und Anbieter

- Databento-Testbestand prüfen und Anbieter endgültig bestätigen;
- Broker-Marktdaten gegen unabhängige Daten validieren.

Für das technische Design bleibt eine konservative Rollover-Ersatzregel bei
fehlenden oder fehlerhaften Volumendaten festzulegen.

## Priorität C – Technik und Betrieb

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

1. Databento-Test und Brokerdatenvergleich praktisch durchführen.
2. technische Architektur und Schnittstellen festlegen.
3. Datenmodelle und Umsetzung planen.

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
