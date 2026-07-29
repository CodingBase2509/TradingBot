# ADR-005: Produktumfang der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Vor der Festlegung von Handelsregeln, Datenbedarf, Risikogrenzen und technischem
Detaildesign benötigt die erste Version klare Produktgrenzen. Die V1 soll zuerst
die korrekte und reproduzierbare Verarbeitung von Daten, Entscheidungen, Risiken,
Orders und Positionen beweisen. Profitabilität und Live-Handel sind keine
Abnahmekriterien dieser Version.

## Entscheidung

Die V1 wird wie folgt begrenzt:

- ausschließlich MES als Handelsinstrument;
- ausschließlich Paper Trading;
- mehrere parallele MES-Strategy-Instances in Shadow und Simulated Paper;
- höchstens eine Broker-Paper-Ausführungsgruppe für MES;
- Long, Short und kein Trade als mögliche Entscheidungen;
- maximal drei gleichzeitig offene, gleichgerichtete Trades im MES;
- Positionen dürfen über tägliche Wartungspausen weiterlaufen;
- keine Positionen über das Wochenende;
- vom Modell geschätzte Haltedauer ohne automatische Schließung bei Erreichen
  der Schätzung;
- Stop Loss und Take Profit bleiben nach dem Einstieg unverändert;
- neue Modellentscheidungen entstehen auf Basis abgeschlossener
  5-Minuten-Kerzen;
- 1-, 5-, 15- und 60-Minuten-Daten liefern den Marktkontext;
- keine News-, Wirtschaftsdaten- oder Orderbuchsignale;
- vor dem ersten lernenden Modell wird der Plattformkern mit einer einfachen,
  festen Teststrategie geprüft;
- Modellfreigaben erfolgen manuell;
- Entscheidungen, Ablehnungen, Orders, Ausführungen und Zustandsänderungen
  werden vollständig aufgezeichnet;
- ein manueller und automatischer Not-Aus ist Bestandteil der V1;
- Interactive Brokers wird zunächst ausschließlich über die Paper-Umgebung
  angebunden;
- Live-Handel gehört nicht zum Umfang der V1.

## Begründung

Ein einzelner Markt und eine reine Paper-Umgebung begrenzen fachliche und
betriebliche Komplexität. Feste Positions- und Ausstiegsregeln erleichtern
deterministische Tests und den Vergleich zwischen Backtest und Paper Trading.
Die feste Teststrategie entkoppelt den Nachweis eines korrekten Plattformkerns
von der später zu prüfenden Modellqualität.

## Folgen

- V1 benötigt noch kein Portfoliorisiko über mehrere Instrumente, aber eine
  Begrenzung des aggregierten Risikos der bis zu drei MES-Trades.
- Handelszeiten, Einstiegsschluss und Freitagsschließung werden durch ADR-006
  konkretisiert.
- Geschätzte und tatsächliche Haltedauer werden getrennt gespeichert. TP, Stop
  und Sicherheitsregeln können einen Trade jederzeit beenden.
- Der Trade Controller muss Stop und Ziel nach Einstieg gegen Änderungen
  schützen.
- Die Architektur darf spätere Märkte und Live-Betrieb ermöglichen, muss sie in
  V1 aber nicht implementieren.
- Canary und Production bleiben Teil des langfristigen Modelllebenszyklus, sind
  jedoch keine V1-Freigabestufen.

## Zu überprüfende Annahmen

- MES ist mit den später beschlossenen Risikogrenzen sinnvoll im Paper Trading
  abbildbar.
- Die gewählte 5-Minuten-Entscheidungsfrequenz ist mit Datenqualität,
  Ausführungskosten und gewünschter Haltedauer vereinbar.
- Die IBKR-Paper-Umgebung bildet die für V1 benötigten Orderzustände ausreichend
  ab.
