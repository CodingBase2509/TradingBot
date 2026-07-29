# Markt- und Datenkonzept

## Zielmärkte

Futures sind der langfristige Zielmarkt, weil Long und Short gleichwertig möglich sind und sehr unterschiedliche Basiswerte über ein gemeinsames technisches Modell abgedeckt werden können.

Vorgeschlagene Startgruppe:

| Symbol | Marktrolle |
|---|---|
| MES | breiter US-Aktienindex, erster Paper-Kandidat |
| MNQ | bewegungsstärkerer US-Technologieindex |
| MGC | Gold und anderer Marktcharakter |
| M6E | Währungsmarkt EUR/USD |

Ein universelles Modell wird nicht vorausgesetzt. Geprüft wird ein gemeinsames Grundmodell mit marktbezogenen Eingaben oder Anpassungen.

## Datenquellen

- **Broker:** Interactive Brokers für V1-Paper-Trading und als bevorzugte
  spätere Echtgeldanbindung.
- **Historische Daten:** Databento ist vorläufig ausgewählt; CME DataMine und
  Tick Data bleiben Ersatzanbieter.
- Broker und Trainingsdatenanbieter bleiben getrennt austauschbar.

Die Anbieterwahl wird vor Kauf anhand aktueller API, Datenabdeckung, Lizenz, Speicherrecht, Preis und Paper-Fähigkeit verifiziert.

Vor dem Kauf werden Databento-Preise für OHLCV-1m, TBBO und MBP-1 getrennt
geschätzt. Der vollständige V1-Zeitraum wird nur als TBBO und OHLCV-1m gekauft.
Repräsentative Normal-, Volatilitäts-, Rollover- und Feiertagszeiträume erhalten
ergänzend MBP-1. Der Kauf erfolgt erst nach Qualitäts-, Lizenz- und
Kostenfreigabe.

Der anfängliche Datenumfang bleibt so klein und kostengünstig wie fachlich
vertretbar. Zusätzliche Datenkäufe sind manuelle Entscheidungen. Spätere
realisierte Handelsgewinne können nach gesonderter Freigabe teilweise in
Trainingsdaten reinvestiert werden.

Freigegebene Databento-Downloads werden manuell in die Testumgebung übernommen.
Dort werden Originaldateien unverändert registriert, geprüft und in kanonische
Datenstände überführt. V1 verwendet lokalen Primärspeicher und ein
verschlüsseltes externes Backup; konkrete Produkte und Pfade folgen im
technischen Design.

## Benötigte V1-Daten

- vollständiges TBBO mit jedem Trade und dem unmittelbar zuvor gültigen besten
  Bid und Ask;
- MBP-1 nur für kleine, repräsentative Vergleichszeiträume;
- daraus erzeugte 1-Minuten-Daten mit Open, High, Low, Close und Volumen;
- daraus erzeugte 5-, 15- und 60-Minuten-Sichten;
- Bid/Ask, Spread und Trade-Anzahl in den kanonischen Aggregationen;
- exakte Zeitstempel und Instrumentkennung;
- Vertragsstammdaten, Tick-Größe, Multiplikator und Handelszeiten.

Ereignisnahe Rohdaten dienen der Ausführungssimulation. Modellfeatures basieren
auf den kanonischen 1-, 5-, 15- und 60-Minuten-Sichten. Ist historisch nur eine
1-Minuten-Auflösung verfügbar, wird die geringere Ausführungsgenauigkeit
gekennzeichnet und bei unklarer Stop-/TP-Reihenfolge konservativ bewertet.

## Datenqualität

- UTC als technische Zeitbasis;
- Duplikate, Lücken und Ausreißer erkennen;
- Datenalter und Reihenfolge prüfen;
- Korrekturen versionieren;
- Rohdaten unverändert bewahren;
- Trainingsstände als unveränderliche Momentaufnahme erzeugen.

Der Datenfluss trennt Originaldaten, kanonische Marktdaten, temporäre oder
eingefrorene Trainingsstände sowie Trainingsläufe und Modellpakete. Nur
ausdrücklich reproduzierbare, nicht referenzierte temporäre Daten dürfen gemäß
[ADR-020](../decisions/ADR-020-Data-Stages-And-Retention.md) bereinigt werden.

Der versionierte CME-Börsenkalender ist die maßgebliche Quelle für Sitzungen,
Feiertage und Sonderzeiten. IBKR-Angaben dienen als operative Gegenprüfung.

Die Datenqualität verwendet drei Stufen:

- **Grün:** vollständig oder aus vollständigen Rohdaten sicher rekonstruierbar;
- **Gelb:** höchstens drei ältere 5-Minuten-Lücken außerhalb aller für die
  aktuelle Entscheidung benötigten Fenster;
- **Rot:** aktuelle, innerhalb der letzten 60 Minuten liegende,
  entscheidungsrelevante oder größere Lücke.

Nur Rot blockiert zwingend neue Trades. Gelb ist nur zulässig, wenn die Lücke
explizit markiert ist und Feature-Berechnung sowie Modell mit derselben
Missing-Data-Regel entwickelt wurden. Unbekannte Marktpreise werden niemals
interpoliert.

## Futures-Rollover

Rohdaten werden pro konkretem Vertrag gespeichert. Die V1 verwendet die
vollständige MES-Historie ab dem offiziellen Handelsstart am 6. Mai 2019.

Der Folgekontrakt wird ab dem nächsten Handelstag verwendet, nachdem sein
Volumen in einem vollständig abgeschlossenen Handelstag das Volumen des
bisherigen Frontkontrakts überstiegen hat. Der Wechsel wird nicht rückwirkend
mit später bekannt gewordenen Daten verändert.

Kontinuierliche Reihen dürfen für Analyse und geeignete Features genutzt werden,
müssen aber versionierte Rollover- und Anpassungsregeln besitzen. Ausführung und
Kosten werden immer am damals tatsächlich ausgewählten Vertrag simuliert.

## Spätere Daten

Wirtschaftskalender, tatsächliche Veröffentlichungen, verwandte Märkte, Orderbuch und strukturierte News werden in getrennten Ausbaustufen ergänzt.
