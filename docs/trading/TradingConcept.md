# Trading-Konzept

## Ziel

V1 zielt auf Long- und Short-Day-Trades mit einer typischen Haltedauer von 30 Minuten bis 8 Stunden und einer tolerierten Obergrenze von ungefähr 24 Stunden. Extremes Scalping ist ausgeschlossen.

## Modellentscheidung

Das Modell schlägt vor:

- kein Trade, Long oder Short;
- Vertrauen;
- gewünschten Anteil am freigegebenen Risikobudget;
- Stop- und Take-Profit-Abstand;
- maximale Haltedauer.

Die Plattform berechnet daraus die technisch gültige Vertragsanzahl und darf reduzieren oder ablehnen.

## Entscheidungsablauf

```text
Marktsituation → Modellentscheidung → Plausibilitätsprüfung
→ Risiko- und Kostenprüfung → Orderplanung → Broker
→ Ausführung → Positionsverwaltung → Abschluss und Auswertung
```

## V1-Vereinfachungen

- ein Paper-Markt, voraussichtlich MES;
- zunächst höchstens eine klar definierte Position je Instrument;
- Stop und Ziel nach Einstieg nur begrenzt oder gar nicht veränderbar;
- kein News- oder Orderbuchsignal;
- kein Overnight- oder Wochenendverhalten ohne vorher beschlossene Regel;
- keine autonome Modellfreigabe.

## Spätere Entwicklung

Weitere Futures, marktbezogene Anpassungen, Portfolio-Risiko, Wirtschaftsdaten, News, Orderbuch sowie vorzeitige und teilweise Schließungen werden einzeln ergänzt und getestet.
