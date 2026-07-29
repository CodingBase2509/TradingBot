# ADR-025: Atomare Risikoreservierung je Konto

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Mehrere Strategy Instances können nahezu gleichzeitig zulässige Trades
vorschlagen. Würde jede Instanz nur den zuletzt gelesenen freien Kontobetrag
prüfen, könnten mehrere Freigaben dasselbe Risiko, denselben Tradeplatz oder
denselben täglichen Trade gleichzeitig verwenden.

Orders können außerdem teilweise gefüllt, während einer Stornierung ausgeführt
oder nach einer unklaren Brokerantwort erst beim Abgleich gefunden werden.

## Entscheidung

### Account Risk Coordinator

Jedes ausführende Broker- oder virtuelle Konto besitzt einen zentralen
`Account Risk Coordinator`. Jede Handelsabsicht durchläuft:

```text
Strategy Risk
→ Account Risk Coordinator
→ Execution Router
```

Der Coordinator verarbeitet Reservierungsentscheidungen je Konto geordnet und
speichert sie konsistent in PostgreSQL. Es wird für V1 kein verteilter
Lock-Dienst benötigt.

### Gemeinsame Rechnung

Für eine Freigabe gilt:

```text
Risiko offener Trades
+ Risiko freigegebener, noch nicht abschließend ausgeführter Orders
+ gewünschte neue Reservierung
≤ globales Kontorisikolimit
```

Die beschlossenen Konto-, Tages-, Parallelitäts-, Tradezahl- und
Richtungsgrenzen werden gemeinsam mit der Risikosumme geprüft.

### Umfang einer Reservierung

Eine Reservierung enthält mindestens:

- `ReservationId`;
- `StrategyInstanceId`;
- `TradeIntentId`;
- Broker- beziehungsweise virtuelles Konto;
- Instrument und Richtung;
- Vertragsanzahl;
- reservierter Risikobetrag;
- reservierter paralleler Tradeplatz;
- vorläufig reserviertes tägliches Trade-Token;
- verwendete Kontostands- und Risikozustandsversion;
- Erstellungs- und Gültigkeitszeit;
- Status.

Risiko, Tradeplatz, Tages-Token und Richtung werden in einem unteilbaren Vorgang
geprüft und reserviert. Bei einem Fehler wird nichts davon freigegeben.

### Reservierungszustände

Mindestens vorgesehen sind:

```text
Requested
├─ Rejected
└─ Reserved
   ├─ PartiallyConsumed
   ├─ Consumed
   ├─ Released
   ├─ Expired
   └─ Unknown
```

`Unknown` bleibt vollständig gebunden, bis Brokerabgleich oder Simulation den
tatsächlichen Zustand eindeutig klärt.

### Freigabegültigkeit

Eine Freigabe ist vor der Orderübermittlung nur kurz und konfigurierbar gültig.
Unmittelbar vor dem Senden werden mindestens Preis, Stop-Abstand, Kosten,
Kontostand, globale Sperren, Broker- und Datenzustand erneut geprüft.

Die konkrete Frist wird anhand gemessener Laufzeiten im IBKR-Paper-Test
festgelegt. Eine abgelaufene, noch nicht gesendete Reservierung darf
freigegeben werden. Eine bereits möglicherweise gesendete Order fällt nicht
unter diese automatische Freigabe.

### Teilfüllungen

Bei Teilfüllung wird die Reservierung aufgeteilt:

```text
gefüllte Menge
→ gebundenes Risiko der offenen Position

noch offene Einstiegsmenge
→ verbleibende Reservierung
```

Wird die restliche Order eindeutig storniert oder abgelehnt, wird nur der
ungenutzte Anteil freigegeben.

### Freigaberegeln

Reserviertes Risiko darf freigegeben werden, wenn:

- eine noch nicht gesendete Freigabe sicher abgelaufen ist;
- der Broker eine Order ohne Füllung eindeutig abgelehnt hat;
- Stornierung, ausgeführte Menge und Brokerposition eindeutig abgeglichen sind;
- eine Position anteilig oder vollständig geschlossen und der neue Zustand
  bestätigt wurde.

`CancelRequested`, Timeout oder fehlende Antwort genügen nicht. Bei unklarem
Status bleibt die Reservierung bestehen und neue Trades können global blockiert
werden.

### Risiko offener Positionen

Für V1 bleibt das ursprünglich geplante Stop-Risiko der tatsächlich offenen
Menge konservativ gebunden. Ein momentaner unrealisierter Gewinn reduziert
dieses gebundene Risiko nicht.

Bestätigte Teilschließungen reduzieren es anteilig. Vollständige Freigabe
erfolgt erst nach Schließung und Reconciliation. Tatsächliche offene und
realisierte Verluste werden unabhängig davon für die Tagesverlustgrenze
berücksichtigt.

Übersteigen Slippage, Kurslücke oder Kosten die Reservierung, wird der höhere
tatsächliche Wert verwendet. Neue Trades werden bei Bedarf blockiert;
bestehende geschützte Trades werden dadurch nicht automatisch geschlossen.

### Richtung und Parallelität

Die erste zulässige Reservierung kann die vorläufige Instrumentrichtung
festlegen. Solange offene oder reservierte MES-Trades Long sind, wird eine
gegenläufige ausführende MES-Absicht in V1 abgelehnt.

Einer der drei parallelen Tradeplätze wird bereits mit `RiskApproved`
reserviert. Dadurch können gleichzeitige Entscheidungen die Grenze nicht
überschreiten.

Das tägliche Trade-Token wird vorläufig reserviert und mit der ersten Füllung
endgültig verbraucht. Wird eine Order sicher ohne Füllung beendet, wird das
Token zurückgegeben.

### Neustart

Reservierungen werden dauerhaft gespeichert. Nach Neustart gilt:

```text
global blockieren
→ Reservierungen laden
→ Brokerorders, Ausführungen und Positionen abrufen
→ Beträge, Plätze, Tokens und Richtung abgleichen
→ kontrolliert freigeben
```

Alte Reservierungen mit unklarem Brokerstatus werden nicht automatisch
gelöscht.

### Ausführungsmodi

- Shadow berechnet Reservierungen nur hypothetisch für Vergleich und
  Ablehnungsanalyse.
- Jede Simulated-Paper-Instanz verwendet ihr eigenes virtuelles Konto und
  eigene Reservierungen.
- Broker Paper verwendet gemeinsame Reservierungen des IBKR-Paperkontos.
- Live verwendet später dieselbe fachliche Logik mit gesonderter Freigabe.

### Anzeige

Die Oberfläche zeigt je Konto mindestens:

- globales Limit;
- Risiko offener Positionen;
- Risiko reservierter Orders;
- noch verfügbares Risiko;
- verwendete und reservierte Tradeplätze;
- ausgeführte und reservierte Trades des Handelstags;
- aktuell gebundene Richtung je Instrument.

## Begründung

Atomare Reservierung verhindert Überbuchung durch gleichzeitige Strategy
Instances. Die Bindung unklarer und teilweise verwendeter Reservierungen ist
konservativ, aber notwendig, weil eine Brokerorder trotz fehlender lokaler
Bestätigung ausgeführt worden sein kann.

Gemeinsame Reservierung von Geldrisiko, Tradeplatz, Tages-Token und Richtung
stellt sicher, dass alle globalen Grenzen denselben konsistenten Kontozustand
verwenden.

## Folgen

- PostgreSQL benötigt Reservierungs-, Status- und Zuordnungstabellen mit
  Konsistenzregeln gemäß ADR-026.
- Zustandswechsel aus ADR-024 aktualisieren Reservierung und Positionsrisiko
  atomar beziehungsweise idempotent.
- Backtest- und Brokeradapter simulieren Teilfüllung, Stornierungsrennen und
  unbekannte Zustände.
- Gültigkeits- und Brokerfristen werden erst nach praktischen Messungen
  konkretisiert.
- Die UI benötigt eine kompakte Konto-Risikoübersicht.
