# ADR-008: Marktdatenauflösung der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Das Modell entscheidet auf abgeschlossenen 5-Minuten-Kerzen und verwendet
Marktkontext aus 1, 5, 15 und 60 Minuten. Für realistische historische
Ausführung reichen aggregierte Modellfeatures allein jedoch nicht aus:
Reihenfolge von Stop und Ziel, Bid/Ask, Spread, Slippage und erreichbarer
Einstieg benötigen feinere Daten.

## Entscheidung

Die V1 verwendet eine mehrstufige Datenhaltung:

1. **Ereignisnahe Rohdaten**
   - vollständiges TBBO: einzelne Trades einschließlich Preis, Menge,
     Zeitstempel und unmittelbar zuvor gültigem bestem Bid und Ask;
   - MBP-1 mit sämtlichen Top-of-Book-Änderungen nur für kleine,
     repräsentative Testzeiträume;
   - konkrete Futures-Kontraktkennung;
   - unverändert und anbieterbezogen gespeichert.
2. **Kanonische 1-Minuten-Daten**
   - Open, High, Low, Close und Volumen;
   - abgeleitete Bid-/Ask- und Spread-Informationen;
   - Qualitätsstatus, Lückenkennzeichen und Herkunft;
   - deterministisch aus dem versionierten Rohdatenstand erzeugt.
3. **Modellsichten**
   - Features auf 1, 5, 15 und 60 Minuten;
   - ausschließlich aus zu diesem Zeitpunkt abgeschlossenen Daten;
   - identische, versionierte Berechnung in Python und .NET.

### Verwendung im Backtest

- Entscheidungen entstehen nach Abschluss einer 5-Minuten-Kerze.
- Der Einstieg wird zum ersten realistisch ausführbaren Preis danach simuliert.
- TBBO-Daten werden für Einstieg,
  Stop-/TP-Reihenfolge, Spread, Slippage und Liquiditätsprüfung verwendet.
- MBP-1-Testzeiträume prüfen, ob zusätzliche Bid-/Ask-Änderungen die
  Ausführungssimulation materiell verändern.
- Stehen für einen Zeitraum nur 1-Minuten-Daten zur Verfügung, darf er mit
  deutlich gekennzeichneter geringerer Ausführungsgenauigkeit verwendet werden.
- Werden Stop und Take Profit innerhalb derselben 1-Minuten-Kerze berührt und
  ist ihre Reihenfolge unbekannt, gilt konservativ der Stop Loss zuerst.
- Daten unterschiedlicher Genauigkeit werden in Evaluation und Berichten nicht
  unbemerkt vermischt.

## Begründung

Die 1-Minuten-Basis ist kompakt und ausreichend fein für die vorgesehenen
Feature-Zeiträume. Ereignisnahe Rohdaten reduzieren gleichzeitig unrealistische
Annahmen bei Orderausführung und Schutzorders. Die Trennung erlaubt eine
erneute, reproduzierbare Aggregation, ohne Rohdaten zu überschreiben.

## Folgen

- Der Datenanbieter muss vollständiges historisches TBBO und OHLCV-1m für MES
  anbieten.
- Speicher- und Verarbeitungskosten werden vor dem Kauf anhand eines
  repräsentativen Datenausschnitts gemessen.
- Rohdaten, Aggregationscode und erzeugte Datenstände erhalten eigene Versionen.
- Backtestergebnisse nennen die verwendete Ausführungsauflösung.
- Der konkrete Datenanbieter, Zeitraum, Rollover und Aufbewahrungsregeln bleiben
  separat festzulegen.
