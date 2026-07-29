# ADR-011: Kosten- und Ausführungsmodell im V1-Backtest

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Historische Modellbewertungen sind nur aussagekräftig, wenn Einstieg,
Ausführungskosten und ungünstige Abweichungen realistisch simuliert werden.
Kerzenschlusskurse ohne Spread, Gebühren oder Slippage würden Ergebnisse
systematisch überschätzen.

## Entscheidung

### Preise und Kosten

- Ein Long-Einstieg verwendet den realistisch verfügbaren Ask-Preis, ein
  Short-Einstieg den Bid-Preis.
- Ausstiege verwenden entsprechend die für die jeweilige Transaktion
  erreichbare Marktseite.
- Broker-, Börsen-, Clearing- und regulatorische Gebühren werden pro Kontrakt
  und Orderseite berücksichtigt.
- Gebühren und Kostenparameter sind zeitabhängig, konfigurierbar und
  versioniert. Sie werden nicht dauerhaft als unveränderliche Werte im
  Fachcode hinterlegt.
- Wenn historische Bid-/Ask-Daten fehlen, wird ein konservativer,
  dokumentierter Spread verwendet.

### Slippage und Ausführung

- Das Basisszenario verwendet zusätzlich mindestens einen Tick Slippage je
  Orderseite.
- Robustheitstests verwenden mindestens zwei und drei Ticks Slippage je
  Orderseite.
- Verzögerte Ausführung, Teilfüllungen und nicht vollständig ausgeführte Orders
  werden im ereignisbasierten Backtest unterstützt.
- Die konkrete Baseline für Latenz und Teilfüllungswahrscheinlichkeit wird
  anhand beobachteter Paper-Ausführungen kalibriert und danach versioniert.
- Stop-Orders dürfen schlechter als ihr Auslösepreis ausgeführt werden.
- Take-Profit-Orders dürfen nicht automatisch als ausgeführt gelten, nur weil
  ein Kerzenpreis das Limit berührt.

### Bewertung

- Alle Ergebnisse werden nach vollständigen Kosten ausgewiesen.
- Basisszenario und verschlechterte Kostenszenarien werden getrennt berichtet.
- Ein Modell gilt nur als robust, wenn sein Vorteil nicht bereits bei moderat
  höheren Kosten, Spread oder Slippage zusammenbricht.
- Kostenmodell, Ausführungsauflösung und Parameter gehören zur Identität eines
  Backtestlaufs.

## Begründung

Bid-/Ask-Seite, Gebühren und Slippage bilden die wirtschaftlich erreichbare
Ausführung deutlich realistischer ab als reine Kerzenpreise. Mehrere
Stressszenarien zeigen, ob ein scheinbarer Modellvorteil nur aus optimistischen
Annahmen entsteht.

## Folgen

- Der Backtest benötigt versionierte Gebühren- und Kostenkonfigurationen.
- Paper-Ausführungen werden mit den simulierten Ausführungen verglichen.
- Abweichungen fließen erst in eine neue Kostenmodellversion ein und verändern
  abgeschlossene historische Berichte nicht rückwirkend.
- Konkrete Gebühren werden nach Broker- und Kontomodellauswahl aktuell erhoben.
