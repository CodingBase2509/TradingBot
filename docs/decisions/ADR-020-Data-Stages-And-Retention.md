# ADR-020: Datenstufen und Aufbewahrung

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Die Trainingsumgebung erzeugt neben kostenpflichtigen Originaldaten viele
reproduzierbare Zwischenstände, offizielle Trainingsdaten, Experimente und
Modelle. Würde alles dauerhaft gespeichert, wüchsen Kosten und
Verwaltungsaufwand unnötig. Würden wichtige Stände gelöscht oder
überschrieben, wären Evaluation und Modellfreigabe nicht mehr nachvollziehbar.

## Entscheidung

### Vier Datenstufen

Der Trainingsdatenfluss besitzt vier klar getrennte Stufen:

```text
Originaldaten
→ kanonische Marktdaten
→ Trainingsstand
→ Trainingslauf und Modellpaket
```

### Stufe 1: Originaldaten

Unveränderte Databento-Downloads, Anbieteroriginale, CME-Kalenderquellen und
spätere unveränderte Paper-/Live-Exporte werden dauerhaft aufbewahrt. Sie
werden niemals überschrieben oder inhaltlich korrigiert.

Jede Lieferung erhält mindestens Anbieter, Instrument, Vertrag, Datenart,
Zeitraum, Downloadzeit, Auftragsreferenz, Dateigröße, Prüfsumme sowie Lizenz-
und Nutzungshinweise.

### Stufe 2: Kanonische Marktdaten

Aus den Originalen werden versionierte, vereinheitlichte Parquet-Stände
erzeugt. Dazu gehören insbesondere:

- geprüfte und zugeordnete Ereignisdaten;
- kanonische 1-, 5-, 15- und 60-Minuten-Sichten;
- Sitzungs-, Kalender-, Qualitäts- und Rolloverinformationen;
- Vertrags- und Instrumentzuordnung.

Eine Änderung an Import, Qualität, Kalender, Aggregation oder Rollover erzeugt
eine neue Version. Bestehende Versionen werden nicht unter derselben ID
ersetzt. Kanonische Stände werden gespeichert, obwohl sie reproduzierbar sind,
weil eine vollständige Neuerzeugung aufwendig sein kann.

### Stufe 3: Trainingsstände

Ein Trainingsstand verbindet einen ausgewählten Zeitraum und Vertragssatz mit:

- zeitlicher Trainings-, Validierungs- und Testaufteilung;
- Features;
- adaptiven TP-/SL-Kandidaten und Labels;
- Kosten- und Slippageannahmen;
- aktiver sowie normal vergangener Haltedauer;
- Schließungsgrund und Datenqualität.

Es gibt zwei Klassen:

1. **temporärer Forschungsstand**
   - reproduzierbar;
   - darf bereinigt werden;
   - nicht für offizielle Evaluation oder Freigabe zulässig.
2. **offiziell eingefrorener Trainingsstand**
   - eindeutige ID und unveränderliches Manifest mit Prüfsummen der
     referenzierten Eingaben;
   - unveränderlich und extern gesichert;
   - für offizielle Evaluation und Modellfreigabe zulässig.

Ein temporärer Stand muss ausdrücklich eingefroren werden, bevor ein
freigaberelevanter Lauf darauf aufbauen darf.

Ein eingefrorener Stand muss nicht automatisch alle reproduzierbaren Features,
Kandidaten und Labels als Kopie enthalten. Das Manifest kann die vollständige
Berechnung aus unveränderlichen kanonischen Daten, Code und Konfiguration
festlegen. Große abgeleitete Tabellen werden nur eingefroren, wenn
Neuerzeugungsaufwand oder Beweiswert dies rechtfertigen oder das Modell für
Shadow, Paper beziehungsweise spätere Freigabe vorgesehen ist.

### Stufe 4: Trainingsläufe und Modellpakete

MLflow speichert Metadaten, Parameter, Metriken, Herkunft und
Artefaktverweise. Für die Aufbewahrung wird unterschieden:

- normale Forschungsversuche: Metadaten dauerhaft, große reproduzierbare
  Artefakte zeitlich begrenzt;
- interessante Kandidaten: Ergebnisse und Artefakte bis zur abschließenden
  Entscheidung;
- validierte, in Shadow oder Paper verwendete und freigegebene Modelle:
  vollständige Modellpakete und Prüfnachweise dauerhaft.

Ablehnung, Gründe und wesentliche Kennzahlen eines Versuchs bleiben erhalten,
auch wenn große Zwischenartefakte später entfernt werden.

### Status eines Datenstands

Versionierte kanonische Daten und Trainingsstände verwenden:

```text
Draft → Frozen → Approved → Retired
```

- `Draft`: veränderlich beziehungsweise neu erzeugbar, nicht freigabefähig;
- `Frozen`: Inhalt, Manifest und Prüfsummen stehen unveränderlich fest;
- `Approved`: für offizielle Läufe zugelassen;
- `Retired`: nicht für neue offizielle Läufe, aber weiterhin nachvollziehbar.

Statusänderungen ersetzen oder verändern keine Dateien. Sie werden als
auditierbare Metadaten gespeichert.

### Aufbewahrungsregeln der V1

| Datenart | Aufbewahrung |
|---|---|
| Originaldaten und Anbieteroriginale | dauerhaft |
| freigegebene kanonische Datenstände | dauerhaft |
| Manifeste offizieller Trainingsstände | dauerhaft |
| eingefrorene abgeleitete Trainingsdateien | bei hohem Neuerzeugungsaufwand oder Freigaberelevanz dauerhaft |
| MLflow-Metadaten | dauerhaft |
| validierte und in Shadow/Paper verwendete Modellpakete | dauerhaft |
| Paper-/Live-Ausführungs- und Auditdaten | dauerhaft |
| große Artefakte erfolgloser Forschungsversuche | zunächst 90 Tage |
| temporäre Features, Kandidaten, Labels und Caches | zunächst 30 Tage |

Die Zeiträume für temporäre Daten sind versionierte Startwerte und können nach
Messung von Speicherverbrauch und Wiederherstellungsaufwand angepasst werden.

Automatische Bereinigung ist nur zulässig, wenn eine Datei:

- ausdrücklich als temporär und reproduzierbar markiert ist;
- von keinem eingefrorenen oder freigegebenen Stand referenziert wird;
- von keinem relevanten MLflow-Lauf, Prüfbericht oder Modellpaket benötigt
  wird;
- nicht unter eine gesetzte Aufbewahrungs- oder Untersuchungssperre fällt.

Im Zweifel wird nicht gelöscht. Jeder Bereinigungslauf erzeugt ein
nachvollziehbares Protokoll.

### Haltedauerbeobachtungen

Für eröffnete Trades werden mindestens gespeichert:

- Prognose beim Einstieg;
- Öffnungs- und Schließungszeit;
- normale vergangene Minuten;
- aktive Marktminuten ohne Börsenpause;
- Schließungsgrund;
- Modell-, Candidate- und Datenversion.

TP- und SL-Ausgänge sind vollständige Laufzeitbeobachtungen.
Freitagsschließung, Full-Stop, manuelle oder technische Eingriffe werden als
abgeschnittene beziehungsweise künstlich beendete Beobachtungen markiert und
nicht unbesehen als natürliche Zielwerte verwendet.

### Backup

Das externe Backup umfasst mindestens Originaldaten, Manifeste, freigegebene
kanonische Daten, freigaberelevante eingefrorene Trainingsdateien, MLflow-Metadaten,
freigaberelevante Berichte, dauerhaft aufzubewahrende Modellpakete sowie
Paper-/Live- und Auditdaten.

Reproduzierbare temporäre Caches benötigen kein externes Backup.

## Begründung

Originale, Entscheidungen und freigaberelevante Ergebnisse müssen dauerhaft
nachvollziehbar bleiben. Temporäre Berechnungen können dagegen Speicher
verbrauchen, ohne zusätzlichen Beweiswert zu liefern.

Die Trennung zwischen temporären und eingefrorenen Trainingsständen verbindet
kostengünstige Forschung mit reproduzierbarer Evaluation. Status, Manifeste und
Prüfsummen verhindern stilles Überschreiben.

## Folgen

- Der Dataset Builder erzeugt Manifeste, Prüfsummen und Datenstandsstatus.
- Offizielle Läufe lehnen nicht freigegebene Trainingsstände ab.
- Referenzen und Sperren müssen vor jeder automatischen Bereinigung geprüft
  werden.
- Die konkrete Verzeichnisstruktur und Parquet-Partitionierung folgen im
  Detaildesign.
- Speicherverbrauch, Löschvolumen und Wiederherstellbarkeit werden überwacht.
