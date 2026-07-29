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

Decision, Trade, Order und Position besitzen getrennte Zustandsmaschinen. Die
vollständigen Zustände, Teilfüllungsregeln, Idempotenz und Wiederherstellung
regelt
[ADR-024](../decisions/ADR-024-Trade-Order-And-Position-State-Machines.md).

Die normale Benutzersicht fasst die technischen Zustände als geplant,
freigegeben, Einstieg läuft, eröffnet und geschützt, Schließung läuft sowie
geschlossen und abgeglichen zusammen.

## Schutzorders

Stop Loss und Take Profit müssen so brokerseitig abgesichert werden, dass ein Ausfall der Anwendung nicht unnötig eine ungeschützte Position hinterlässt. Brokerabhängige Möglichkeiten und deren Fehlerfälle werden vor Live-Betrieb getestet.

„Brokerseitig geschützt“ bedeutet, dass Stop Loss und Take Profit für die
tatsächlich offene Richtung und Menge bestätigt wurden. Beide Schutzorders
müssen so gekoppelt sein, dass eine Ausführung oder Teilfüllung keine verwaiste
oder übergroße Gegenorder hinterlässt. Nach jeder Teilfüllung und Mengenänderung
wird der Schutz erneut geprüft.

Fehlt eine Bestätigung, werden neue Trades blockiert und der Schutz wird
korrigiert. Gelingt das nicht innerhalb einer kurzen konfigurierbaren Frist,
wird nur die betroffene ungeschützte Position geschlossen.

Während einer Börsenunterbrechung können Schutzorders nicht ausgeführt werden.
Dieses Kurslückenrisiko wird aufgezeichnet und in Backtests berücksichtigt.

## Sitzungs- und Wochenendgrenzen

Zwei Stunden vor der täglichen Börsenpause werden keine neuen Positionen
eröffnet. Bestehende, brokerseitig geschützte Positionen dürfen die tägliche
Pause überschreiten.

Vor dem wöchentlichen Börsenschluss am Freitag werden alle Positionen
kontrolliert geschlossen. Der Positionsmanager bestätigt die Glattstellung mit
dem Broker. Nicht oder nur teilweise ausgeführte Schließungsorders lösen einen
definierten Eskalationsablauf und eine kritische Warnung aus.

Die Glattstellung beginnt 60 Minuten vor dem offiziellen Wochenschluss. Ab 30
Minuten vor Schluss wird aggressiver geschlossen. Sind 15 Minuten vor Schluss
noch Positionen offen, wird ein kritischer Alarm ausgelöst.

## Reconciliation

Verglichen werden offene Orders, Mengen, Durchschnittspreise, Positionen, Kontowerte und Schutzorders. Bei Abweichung werden neue Trades blockiert und ein definierter Notfallablauf gestartet.

## Paper und Live

Paper und Live verwenden dieselben internen Verträge. Abweichungen der Broker-Simulation von echter Ausführung werden gemessen und in konservative Backtestannahmen zurückgeführt.
