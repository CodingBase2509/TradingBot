# ADR-013: Databento als historischer Datenanbieter

- **Status:** vorläufig beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Die V1 benötigt für MES die vollständige Historie ab Mai 2019, konkrete
Futures-Kontrakte, einzelne Trades, bestes Bid/Ask und kanonische
1-Minuten-Daten. Interactive Brokers ist der Broker, kann die benötigte
vollständige Historie abgelaufener Futures aber nicht als alleinige
Trainingsdatenquelle bereitstellen.

Verglichen wurden insbesondere Databento, CME DataMine und Tick Data.

## Vorläufige Entscheidung

Databento wird als bevorzugter historischer Datenanbieter für V1 ausgewählt.
Verwendet wird der CME-Globex-Datensatz `GLBX.MDP3`.

Zu prüfen sind insbesondere:

- `OHLCV-1m` für kanonische 1-Minuten-Kerzen;
- `Trades` für einzelne Ausführungen;
- `TBBO` für Trades mit zugehörigem bestem Bid und Ask;
- `MBP-1` für sämtliche Änderungen am besten Bid und Ask;
- Instrumentdefinitionen und konkrete MES-Kontraktkennungen.

Die Entscheidung ist vorläufig, bis Datenqualität, Kosten und Nutzungsrechte
praktisch bestätigt wurden. CME DataMine bleibt erster Ersatzanbieter, Tick
Data zweiter Ersatzanbieter.

## Vorgehen vor dem vollständigen Datenkauf

### 1. Konto und Lizenzrahmen

- Databento-Konto für private, interne Forschung einrichten.
- Nutzungszweck im Lizenzprozess korrekt angeben.
- Schriftlich beziehungsweise anhand der Vertragsunterlagen bestätigen:
  - dauerhafte lokale Speicherung bereits bezahlter Downloads;
  - interne Verarbeitung und Erzeugung abgeleiteter Features;
  - Nutzung für Training, Backtesting und Paper Trading;
  - zulässige Backups;
  - Einschränkungen bei Weitergabe und Veröffentlichung.

### 2. Kosten getrennt schätzen

Vor einer Bestellung werden für MES vom 6. Mai 2019 bis zum aktuellen
Stichtag getrennte Preisangebote beziehungsweise API-Schätzungen erstellt:

1. vollständiges `OHLCV-1m`;
2. vollständige `Trades`;
3. vollständiges `TBBO`;
4. vollständiges `MBP-1`.

Die Schätzungen werden mit Datenmenge, Zeitraum, Schema, Produktfilter,
Lizenzgebühr und Downloadkosten dokumentiert. Ein Kostenlimit wird vor dem
Vollkauf festgelegt.

#### Erste Schätzung vom 28. Juli 2026

Databento schätzte für MES im Zeitraum 28. Juni bis 27. Juli 2026:

| Schema | Preis je GB | Datenmenge | Monatskosten |
|---|---:|---:|---:|
| MBP-1 | 1,80 USD | ca. 13 GB | ca. 23 USD |
| TBBO | 28 USD | ca. 450 KB | ca. 0,01 USD |
| OHLCV-1m | ca. 70 USD | ca. 280 KB | ca. 0,02 USD |

Eine rein lineare Hochrechnung über ungefähr 87 Monate von Mai 2019 bis Juli
2026 ergäbe grob:

| Schema | Hochgerechnete Datenmenge | Hochgerechnete Kosten |
|---|---:|---:|
| MBP-1 | ca. 1,1 TB | ca. 2.000 USD |
| TBBO | ca. 39 MB | ca. 1 USD |
| OHLCV-1m | ca. 24 MB | ca. 2 USD |

Diese Hochrechnung ist keine Kaufpreisgarantie. Nachrichtenaufkommen,
Handelsaktivität, Kompression, Produktfilter und Databento-Preise unterscheiden
sich über die Jahre. Vor dem Kauf wird deshalb immer der vollständige konkrete
Zeitraum im Preisrechner beziehungsweise über die API geschätzt.

### 3. Repräsentativen Testumfang beschaffen

Zunächst wird kein vollständiger Bestand gekauft. Der Test enthält mindestens:

- einen normalen Handelsmonat;
- einen Monat mit hoher Volatilität;
- einen Zeitraum um einen Futures-Rollover;
- einen Feiertag beziehungsweise verkürzten Handelstag;
- OHLCV-1m, TBBO und MBP-1 für dieselben Zeiträume.

Soweit möglich wird dafür vorhandenes Startguthaben verwendet.

### 4. Technische und fachliche Prüfung

Der Testbestand wird geprüft auf:

- vollständige MES-Kontrakte und korrekte Instrumentdefinitionen;
- UTC-Zeitstempel, Reihenfolge und Eindeutigkeit;
- Trades, Bid/Ask, Mengen und Spread;
- geplante und ungeplante Datenlücken;
- Rollover und parallele Kontrakte;
- deterministische Erzeugung der 1-Minuten-Kerzen;
- Übereinstimmung von Anbieterkerzen und eigener Aggregation;
- Speicherbedarf, Downloadzeit und Verarbeitungsgeschwindigkeit;
- Eignung für Stop-/TP-Reihenfolge und Backtest-Kostenmodell.

### 5. Minimal ausreichendes Schema wählen

- Die vollständige MES-Historie der V1 wird auf `TBBO + OHLCV-1m`
  beschränkt.
- Ein separater `Trades`-Download ist nicht vorgesehen, da TBBO bereits jedes
  Trade-Ereignis zusammen mit dem unmittelbar zuvor gültigen besten Bid und Ask
  enthält.
- `OHLCV-1m` allein genügt nicht für die endgültige Ausführungssimulation.
- `MBP-1` wird nur für kleine, fachlich begründete Testzeiträume beschafft.
- Relevante MBP-1-Zeiträume umfassen insbesondere hohe Volatilität, Rollover,
  typische ruhige Sitzungen und besondere Ausführungsfälle.
- Derselbe Backtest-Ausschnitt wird mit TBBO und MBP-1 ausgewertet, um den
  Zusatznutzen zu messen.
- Ein vollständiger MBP-1-Kauf gehört nicht zum anfänglichen V1-Budget.

TBBO enthält bereits jedes Trade-Ereignis sowie das unmittelbar davor gültige
beste Bid und Ask. Aufgrund des sehr großen Kosten- und
Datenmengenunterschieds ist `TBBO + OHLCV-1m` die beschlossene
V1-Kombination.

### 6. Kostenprinzip und spätere Erweiterung

- Der Aufbau beginnt mit dem kleinsten fachlich ausreichenden Datenumfang.
- Startguthaben und kostenlose Stichproben werden zuerst genutzt.
- Jeder kostenpflichtige Download benötigt eine bewusste manuelle Freigabe.
- Modell, Trading-Plattform und automatisierte Trainingsprozesse dürfen weder
  Käufe auslösen noch Budgets verändern.
- Zusätzliche historische Daten werden nur gekauft, wenn ein vorher
  formulierter Vergleich ihren erwarteten Nutzen begründet.
- Falls ein späterer Echtgeldbetrieb nach allen Kosten tatsächlich realisierte
  Gewinne erzielt, kann ein manuell festgelegter Teil davon in zusätzliche
  Trainingsdaten reinvestiert werden.
- Gewinne führen nicht automatisch zu einem Datenkauf. Umfang, Budget und
  Datenschema werden jeweils als neue dokumentierte Entscheidung freigegeben.

### 7. Vollkauf und unveränderliche Ablage

Erst nach bestandener Prüfung und genehmigter Kostenübersicht wird der
vollständige Zeitraum bestellt. Danach:

- Originaldownloads unverändert und schreibgeschützt ablegen;
- Prüfsummen, Downloadauftrag, Lizenzstand und Anbieter-Metadaten speichern;
- mindestens eine getrennte Sicherung erstellen;
- kanonische Daten ausschließlich als reproduzierbare Ableitung erzeugen;
- zukünftige Aktualisierungen als neue Datenversion ergänzen.

## Abnahmekriterien für die endgültige Bestätigung

Databento wird endgültig bestätigt, wenn:

- MES ab Mai 2019 vollständig und vertragsbezogen verfügbar ist;
- die gewählte Schema-Kombination ADR-008 bis ADR-011 erfüllt;
- Qualitätsprüfungen und Rollover mit dem Testbestand funktionieren;
- dauerhafte interne Speicherung und ML-Nutzung vertraglich zulässig sind;
- Gesamtkosten innerhalb des vorher festgelegten Budgets liegen;
- ein erneuter Download und die Wiederherstellung getestet wurden.

Andernfalls wird derselbe Prüfprozess mit CME DataMine und danach Tick Data
wiederholt.

## Begründung

Databento deckt CME-Futures mit Trades, Top-of-Book, Markttiefe,
Instrumentdefinitionen und aggregierten Kerzen über eine einheitliche
historische API ab. Nutzungsabhängige historische Preise erlauben einen kleinen
Test, bevor ein größerer Bestand gekauft wird.

Die gestufte Beschaffung trennt die günstige technische Erprobung von der
endgültigen Dateninvestition und verhindert, dass unnötig umfangreiche
Markttiefedaten gekauft werden.

Das Kostenprinzip hält die V1 für einen privaten Projektstart finanzierbar, ohne
die spätere Erweiterbarkeit einzuschränken.

## Folgen

- Python übernimmt Download, Prüfung und Aufbereitung der historischen Daten.
- Der fachliche Datenvertrag bleibt anbieterneutral.
- Zugangsdaten werden außerhalb des Quellcodes gespeichert.
- Preise und Lizenzbedingungen werden unmittelbar vor jedem Kauf erneut
  geprüft.
- Diese ADR wird nach dem Test mit „beschlossen“ oder „verworfen“ aktualisiert.
