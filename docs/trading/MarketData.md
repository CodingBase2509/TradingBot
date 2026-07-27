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

- **Broker:** Interactive Brokers als vorgeschlagene langfristige Anbindung.
- **Historische Daten:** unabhängiger Anbieter; Databento ist ein Kandidat.
- Broker und Trainingsdatenanbieter bleiben getrennt austauschbar.

Die Anbieterwahl wird vor Kauf anhand aktueller API, Datenabdeckung, Lizenz, Speicherrecht, Preis und Paper-Fähigkeit verifiziert.

## Benötigte V1-Daten

- Open, High, Low, Close;
- Volumen;
- Bid/Ask oder Spread;
- Trade-Anzahl, sofern verfügbar;
- exakte Zeitstempel und Instrumentkennung;
- Vertragsstammdaten, Tick-Größe, Multiplikator und Handelszeiten.

## Datenqualität

- UTC als technische Zeitbasis;
- Duplikate, Lücken und Ausreißer erkennen;
- Datenalter und Reihenfolge prüfen;
- Korrekturen versionieren;
- Rohdaten unverändert bewahren;
- Trainingsstände als unveränderliche Momentaufnahme erzeugen.

## Futures-Rollover

Rohdaten werden pro konkretem Vertrag gespeichert. Kontinuierliche Reihen dürfen für Features genutzt werden, müssen aber versionierte Rollover-Regeln besitzen. Ausführung und Kosten werden immer am tatsächlich handelbaren Vertrag simuliert.

## Spätere Daten

Wirtschaftskalender, tatsächliche Veröffentlichungen, verwandte Märkte, Orderbuch und strukturierte News werden in getrennten Ausbaustufen ergänzt.
