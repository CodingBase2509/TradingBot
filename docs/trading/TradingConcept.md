# Trading-Konzept

## Ziel

V1 zielt auf Long- und Short-Trades im MES. Die Haltedauer wird vom Modell
geschätzt; eine Position darf über die tägliche Börsenpause laufen, aber niemals
über das Börsenwochenende. Extremes Scalping ist ausgeschlossen. V1 verwendet
ausschließlich Paper Trading.

## Modellentscheidung

Ein versionierter Candidate Generator leitet aus der bisherigen Marktbewegung
mehrere plausible Long- und Short-Alternativen mit situationsbezogenen
Stop-Loss- und Take-Profit-Werten ab. Das Modell bewertet diese Alternativen,
schätzt ihre voraussichtliche Haltedauer und vergleicht sie mit der sicheren
Option „kein Trade“.

TP und SL werden damit anhand von Marktstruktur, Volatilität und
Ausführungsbedingungen ausgewählt. Sie stammen nicht primär aus einem starren
ATR-Raster und werden nicht als ungeprüfte freie Orderpreise ausgegeben.

Die Plattform berechnet daraus die technisch gültige Vertragsanzahl und darf reduzieren oder ablehnen.

In V1 wählt das Modell keine Risikofraktion. Die Plattform verwendet höchstens
2 % Risiko pro Trade. Ein Trade wird nur eröffnet, wenn sein erwartetes
Netto-Risk-to-Reward nach Kosten mindestens `1:1` beträgt.

Eine variable, vom Modell vorgeschlagene Risikofraktion ist erst als spätere
Erweiterung vorgesehen.

## Entscheidungsablauf

```text
Marktsituation → adaptive Kandidaten → Modellbewertung → Plausibilitätsprüfung
→ Risiko- und Kostenprüfung → Orderplanung → Broker
→ Ausführung → Positionsverwaltung → Abschluss und Auswertung
```

## V1-Vereinfachungen

- ausschließlich MES als Paper-Markt;
- höchstens drei offene MES-Trades gleichzeitig;
- offene MES-Trades müssen dieselbe Richtung besitzen;
- Stop und Ziel bleiben nach Einstieg unverändert;
- neue Entscheidungen auf abgeschlossenen 5-Minuten-Kerzen;
- Marktkontext aus 1-, 5-, 15- und 60-Minuten-Daten;
- kein News- oder Orderbuchsignal;
- keine neuen Positionen ab zwei Stunden vor der täglichen Börsenpause;
- bestehende Positionen dürfen über die tägliche Pause weiterlaufen;
- alle Positionen werden vor dem wöchentlichen Börsenschluss am Freitag
  geschlossen;
- keine Position über das Wochenende;
- keine autonome Modellfreigabe.
- kein Live-Handel.

## Spätere Entwicklung

Weitere Futures, marktbezogene Anpassungen, Portfolio-Risiko, Wirtschaftsdaten, News, Orderbuch sowie vorzeitige und teilweise Schließungen werden einzeln ergänzt und getestet.

Später kann geprüft werden, ob kurzfristige Gegenbewegungen innerhalb eines
größeren Trades separat gehandelt werden. Dafür muss die Ausführung
gegenläufige logische Trades eindeutig von einer Reduzierung der beim Broker
geführten Nettoposition unterscheiden.
