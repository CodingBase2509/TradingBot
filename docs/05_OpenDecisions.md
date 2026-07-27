# Offene Entscheidungen

## Priorität A – vor dem Detaildesign

- V1 ausschließlich auf MES begrenzen?
- genaue minimale, typische und maximale Haltedauer;
- Stop und Ziel nach Einstieg unveränderlich oder begrenzt anpassbar;
- Positionen über Handelsschluss oder Wochenende;
- genaue Formel für historische Lernvorlagen;
- gemeinsame Ausgabe oder mehrere Teilmodelle;
- Mindestvorteil gegenüber „kein Trade“;
- Mindestzahl unabhängiger Trades und zulässiger Drawdown;
- Dauer und Abnahmekriterien von Shadow, Paper und Canary;
- konkrete Risiko-, Tagesverlust- und Gesamtexposure-Grenzen.

## Priorität B – Daten und Anbieter

- Databento und Alternativen nach Tiefe, Bid/Ask, Lizenz und Preis vergleichen;
- benötigte Grundauflösung festlegen;
- Rollover-Methode für kontinuierliche Analyse-Reihen;
- Handelszeiten und relevante Sitzungen;
- Datenbudget, Aufbewahrung und Backup;
- Broker-Marktdaten gegen unabhängige Daten validieren.

## Priorität C – Technik und Betrieb

- modularer Monolith verbindlich bestätigen;
- relationale Datenbank, Parquet- und Objektablage auswählen;
- MLflow oder schlanke eigene Experimentverwaltung;
- Artefaktübergabe von Python an .NET;
- Hosting, IB Gateway, Zeitabgleich und Geheimnisverwaltung;
- exakter Modell-Ein- und Ausgabevertrag;
- Notfallverhalten je Ausfallart.

## Empfohlene Entscheidungsreihenfolge

1. V1-Produktumfang.
2. Handels- und Risikopolitik.
3. Lernziel und Bewertungsformel.
4. Datenbedarf und Backtestannahmen.
5. Modellfreigabekriterien.
6. Anbieterwahl.
7. technische Architektur und Schnittstellen.
8. Datenmodelle und Umsetzung.

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
