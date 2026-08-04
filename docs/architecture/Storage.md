# Speicher und Datenhaltung

## Grundsatz

Jede Information wird nur so lange und in dem Speicher gehalten, der zu ihrer
Nutzung passt. Training, Test und Produktion besitzen vollständig getrennte
Speicherbereiche. Große Daten, Modellpakete und operative Zustände werden nicht
in Git abgelegt, sondern über IDs, Manifeste und SHA-256-Prüfsummen mit Code und
Konfiguration verbunden.

## Speicherarten

| Speicher | Verbindlicher Zweck |
|---|---|
| PostgreSQL | operative Zustände, Metadaten, Konfiguration, Risiko, Trades, Orders, Reconciliation und Audit |
| Parquet | große unveränderliche Zeitreihen, Features, Labels, Trainingsstände und Backtestergebnisse |
| Dateisystem | Originaldownloads, Modellpakete, Manifeste, Berichte und temporäre Arbeitsdaten |
| Git | Code, Migrationen, Verträge, kleine Konfigurationen und Golden Samples |
| Environment Variables | Übergabe von Secrets an Container; Werte bleiben außerhalb von Git und Images |

Parquet ist ein Dateiformat und kein Datenbankserver. PostgreSQL beantwortet
gezielte Abfragen und schützt veränderliche Zustände; Parquet verarbeitet große
analytische Datenmengen effizient und reproduzierbar.

## Datenstufen

1. **Originaldaten:** unveränderte Anbieterdateien mit Herkunft, Lizenzbezug,
   Importzeit und Prüfsumme.
2. **Kanonische Marktdaten:** geprüfte, vereinheitlichte Daten je konkretem
   Vertrag einschließlich Qualitäts- und Rolloverinformationen.
3. **Trainingsstände:** eingefrorene Features, Kandidaten, Labels und zeitliche
   Aufteilungen mit eigener Dataset-ID.
4. **Läufe und Modellpakete:** Konfigurationen, Metriken, Berichte und
   unveränderliche auslieferbare Modellpakete.

Ein offizieller Datenstand wechselt nachvollziehbar durch `Building`,
`Validated`, `Frozen`, `Rejected` oder `Retired`. Nur `Frozen` darf für eine
offizielle Evaluation oder ein auslieferbares Modellpaket verwendet werden.

## Physische Ablage

Jede Zone beginnt mit derselben flachen Grundstruktur:

```text
storage/
├── raw/       # unveränderte Anbieterdateien
├── market/    # kanonische Marktdaten
├── datasets/  # eingefrorene Trainingsstände
├── models/    # unveränderliche Modellpakete
└── temp/      # jederzeit reproduzierbare Arbeitsdaten
```

Zusätzliche Unterteilungen entstehen nur bei fachlichem Bedarf oder gemessenen
Größen- beziehungsweise Laufzeitproblemen. Dateien werden zunächst temporär
vollständig geschrieben, geprüft und anschließend atomar veröffentlicht.
Unvollständige Dateien oder Paketordner sind niemals gültige Eingaben.

## PostgreSQL

Die Trading-Plattform gliedert ihre Tabellen fachlich in:

| Schema | Inhalt |
|---|---|
| `market` | Instrumente, Verträge, Kalender, Datenqualität und Importmetadaten |
| `strategy` | Strategy Instances und versionierte Konfigurationen |
| `model` | Modellpakete, Prüfungen, Freigaben und Aktivierungen |
| `risk` | Risikoreservierungen, Kontostände, Limits und Sperren |
| `trading` | Entscheidungen, logische Trades und Positionen |
| `execution` | Orders, Ausführungen und Brokerabgleiche |
| `operations` | Systemzustände, Alarme, Not-Aus und Auditereignisse |

Fachliche Konfigurationen werden nicht in einer allgemeinen Key-Value-Tabelle
gesammelt. Platform Runtime, Account, Instrument, Strategy und Candidate
Generator besitzen jeweils eigene versionierte Tabellen. Research Runs liegen
in einer eigenen Tabelle der isolierten Trainingsdatenbank. Die genaue
Zuordnung beschreibt der [Konfigurationsvertrag](./Configuration.md).

MLflow verwendet in der Trainingszone eine eigene Datenbank und einen eigenen
Dateibereich. Es ist keine Registry für Test- oder Produktionsfreigaben.

Für Identitäten verwendet C# `Guid` mit UUID Version 7. Fachliche Zustände und
Ereignistypen sind stabile C#-Enums, deren persistierte Codes niemals durch
nachträgliches Umsortieren ihre Bedeutung ändern. Zeitpunkte werden als UTC,
Geldwerte als Dezimalwerte und Preise beziehungsweise Abstände zusätzlich in
verlustfreien Ticks gespeichert.

Ereignisse und Auditdaten sind append-only. Veränderliche Projektionen dürfen
aus ihnen und dem Brokerzustand wieder aufgebaut werden. Fremdschlüssel,
Eindeutigkeitsregeln und Idempotenzschlüssel verhindern doppelte fachliche
Vorgänge. Partitionierung wird erst nach Messung eingeführt.

## Aufbewahrung

- Originaldaten und tatsächlich verwendete kanonische Daten bleiben erhalten.
- Eingefrorene Trainingsstände bleiben erhalten, solange ein Lauf, Bericht
  oder Modellpaket darauf verweist.
- Freigegebene, aktive und rollbackfähige Modellpakete werden nicht automatisch
  gelöscht.
- Audit-, Trade-, Order-, Fill- und Reconciliation-Daten werden dauerhaft
  nachvollziehbar aufbewahrt.
- Nur eindeutig temporäre, reproduzierbare und nicht referenzierte Daten dürfen
  automatisiert bereinigt werden.

Haltedauerschätzungen, tatsächliche Haltedauer, aktive Marktzeit und
Schließungsgrund gehören zu den dauerhaften Tradebeobachtungen. Künstlich
beendete beziehungsweise abgeschnittene Beobachtungen werden ausdrücklich
gekennzeichnet.

## Git und Herkunft

Mit Git versioniert werden Code, Datenverträge, Feature- und Labeldefinitionen,
Candidate-Generator-Regeln, Schemas, Migrationen, reproduzierbare
Konfigurationen sowie kleine Testsamples. Nicht in Git gehören Marktdaten,
Datenbanken, große Parquet-Dateien, MLflow-Artefakte, Modellpakete, Logs,
Backups und Secrets.

Jeder offizielle Lauf verbindet mindestens Git-Commit und sauberen
Arbeitsstand, Dataset-ID, Konfigurationsversion, Laufzeitumgebung, Zufallswerte,
MLflow-Run-ID und erzeugte Paket-ID.

## Backups und Wiederherstellung

Backups werden je Zone vom Host gesteuert und niemals zwischen den laufenden
Zonen gemeinsam beschrieben. Gesichert werden PostgreSQL, unveränderliche
Originaldaten, referenzierte Trainingsstände, Modellpakete, Manifeste und
notwendige Konfigurationen. Temporäre Daten benötigen kein Backup.

Ein Backup gilt erst nach einer Wiederherstellungsprobe als belastbar.
Frequenzen sowie RPO und RTO werden vor dem jeweiligen produktiven Betrieb
konkret festgelegt.
