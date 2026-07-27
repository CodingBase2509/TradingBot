# Trading Intelligence Platform – Vision

**Stand:** 27. Juli 2026  
**Status:** Planungsgrundlage

## Vision

Wir entwickeln eine modulare, KI-gestützte Trading-Plattform, die historische und aktuelle Marktdaten nutzt, um wiederkehrende Handelsmuster zu lernen, Handelsentscheidungen objektiv zu prüfen und freigegebene Modelle kontrolliert einzusetzen.

Die Plattform soll langfristig neue Modellversionen erzeugen, gegen die aktive Version vergleichen und nur nachweislich bessere Kandidaten schrittweise freigeben. Datenquellen, Broker, Merkmalsberechnung und Modelle bleiben austauschbar.

## Produktverständnis

Das Vorhaben besteht aus zwei verbundenen Produkten:

1. **Trading-Plattform:** stabile Infrastruktur für Daten, Simulation, Broker, Orders, Risiko, Überwachung und Bedienung.
2. **Forschungsplattform:** wiederholbare Experimente mit Datensätzen, Merkmalen, Lernverfahren, Modellen und Backtests.

Das Modell ist ein austauschbares Modul. Die Trading-Plattform muss auch mit einer einfachen Testentscheidung ohne lernende KI funktionieren.

## Langfristige Ziele

- robuste Muster über mehrere Marktarten und Marktphasen lernen;
- Long-, Short- und Kein-Trade-Entscheidungen treffen;
- gewünschtes Risiko, Stop Loss, Take Profit und Haltedauer vorschlagen;
- alle Ergebnisse nach Kosten und Risiko bewerten;
- neue Modellkandidaten kontrolliert trainieren, prüfen und freigeben;
- zunächst Paper Trading, später begrenzter Live-Betrieb;
- später Wirtschaftstermine, Nachrichten, Orderbuch und verwandte Märkte ergänzen;
- sämtliche Entscheidungen und Experimente nachvollziehbar speichern.

## Rahmen

| Bereich | Aktueller Stand |
|---|---|
| Handelsstil | Day-Trading, kein extremes Scalping |
| Haltedauer | typisch 30 Minuten bis 8 Stunden, maximal ungefähr 24 Stunden |
| Richtung | Long und Short |
| Zielmarkt | Futures |
| erster Markt | MES als vorgeschlagener Paper-Markt |
| weitere Kandidaten | MNQ, MGC und M6E |
| Produktion | .NET |
| Training und Forschung | Python |
| Modellausführung | ONNX in .NET |
| Bedienung | Angular |
| Broker | Interactive Brokers als vorgeschlagene Wahl |
| Sicherheit | feste, nicht vom Modell veränderbare Grenzen in .NET |

## Nicht-Ziele der ersten Version

- garantierte Profitabilität;
- Millisekunden- oder Hochfrequenzhandel;
- sofortiger Einsatz echten Kapitals;
- Umlernen nach jedem einzelnen Trade;
- Reinforcement Learning als Startpunkt;
- ungeprüft vom Modell erzeugter Programmcode;
- viele Broker, Märkte und Datenanbieter gleichzeitig;
- News-, Sprachmodell- und Orderbuchauswertung von Beginn an.

## Frühe Erfolgsdefinition

Die erste Version ist erfolgreich, wenn Daten korrekt verarbeitet werden, Backtest und Paper Trading denselben Entscheidungsweg verwenden, Kosten realistisch berücksichtigt werden, Ergebnisse reproduzierbar sind und unsichere Trades zuverlässig blockiert werden.

Profitabilität ist eine zu prüfende Hypothese, kein Versprechen.
