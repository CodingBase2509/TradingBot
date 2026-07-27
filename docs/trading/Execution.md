# Orderausführung und Positionsverwaltung

## Orderübersetzung

Der Trade Controller übersetzt relative Modellausgaben in eine konkrete Order. Berücksichtigt werden Kontowert, erlaubtes Risiko, Stop-Abstand, Vertragsmultiplikator, Tick-Größe, Gebühren, Mindestmenge und bestehende Positionen.

## Brokeradapter

Der Adapter kapselt:

- Verbindung und Sitzungszustand;
- Order senden, ändern und stornieren;
- Bestätigungen, Ablehnungen und Ausführungen;
- Teilfüllungen;
- Kontowerte, offene Orders und Positionen;
- regelmäßigen Zustandsabgleich.

Der Broker ist die maßgebliche Quelle für tatsächlich ausgeführte Orders und Positionen.

## Positionslebenszyklus

```text
Entscheidung → Vorprüfung → Order gesendet → bestätigt
→ teilweise/vollständig ausgeführt → geschützt
→ aktiv verwaltet → geschlossen → abgeglichen
```

Jeder Übergang wird als Ereignis protokolliert. Unerwartete oder unmögliche Übergänge blockieren neue Trades.

## Schutzorders

Stop Loss und Take Profit müssen so brokerseitig abgesichert werden, dass ein Ausfall der Anwendung nicht unnötig eine ungeschützte Position hinterlässt. Brokerabhängige Möglichkeiten und deren Fehlerfälle werden vor Live-Betrieb getestet.

## Reconciliation

Verglichen werden offene Orders, Mengen, Durchschnittspreise, Positionen, Kontowerte und Schutzorders. Bei Abweichung werden neue Trades blockiert und ein definierter Notfallablauf gestartet.

## Paper und Live

Paper und Live verwenden dieselben internen Verträge. Abweichungen der Broker-Simulation von echter Ausführung werden gemessen und in konservative Backtestannahmen zurückgeführt.
