# Risiko- und Sicherheitskonzept

## Grundsatz

Das Modell entscheidet, welchen Trade es für sinnvoll hält. Der Risk Guard entscheidet unveränderbar, ob der Trade sicher, technisch möglich und regelkonform ist.

## Schutzebenen

1. Eingabequalität und Datenalter;
2. Plausibilität der Modellausgabe;
3. Vorhandelsprüfung von Risiko, Kosten und Portfolio;
4. Gültigkeit und Schutz der Brokerorder;
5. laufender Abgleich von Order und Position;
6. Tages-, Drawdown- und Gesamtrisiko;
7. betrieblicher Zustand von Zeit, Verbindungen, Speicher und Überwachung.

Bei mehreren Strategy Instances erfolgt die Vorhandelsprüfung zweistufig:

```text
strategiebezogene Grenzen
→ gemeinsame Konto- und Portfoliogrenzen
```

Die globalen Grenzen berücksichtigen alle ausführenden Instanzen und sind
immer maßgeblich. Mehrere Modelle dürfen das gemeinsame Konto- oder
Tagesrisiko nicht vervielfachen.

Der Account Risk Coordinator reserviert Risikobetrag, parallelen Tradeplatz,
tägliches Trade-Token und Instrumentrichtung atomar, bevor eine Order gesendet
werden darf. Dabei müssen Risiko offener Trades, freigegebener noch nicht
abgeschlossener Orders und der neuen Absicht gemeinsam innerhalb des Limits
bleiben.

Reservierungen durchlaufen `Requested`, `Rejected`, `Reserved`,
`PartiallyConsumed`, `Consumed`, `Released`, `Expired` oder `Unknown`.
`Unknown` bleibt vollständig gebunden. Die Freigabe ist vor dem Senden nur kurz
und konfigurierbar gültig; unmittelbar davor werden Preis, Stop, Kosten,
Kontostand, globale Sperren sowie Broker- und Datenzustand erneut geprüft.

Bei Teilfüllung wandert der gefüllte Anteil in das gebundene Positionsrisiko,
der offene Anteil bleibt reserviert. `CancelRequested`, Timeout oder fehlende
Antwort reichen nicht zur Freigabe. Ursprüngliches Stop-Risiko bleibt für die
offene Menge konservativ gebunden und sinkt nicht durch unrealisierte Gewinne.
Trade-Token wird erst mit der ersten Füllung endgültig verbraucht.

Nach einem Neustart bleiben alle Reservierungen erhalten. Neue Trades sind
blockiert, bis Orders, Fills, Positionen, Risikobeträge, Plätze, Tokens und
Richtung mit dem Broker abgeglichen wurden. V1 benötigt dafür keinen
verteilten Lock-Dienst; der Coordinator verarbeitet Entscheidungen je Konto
geordnet und konsistent in PostgreSQL.

## Ein Trade wird blockiert, wenn

- Daten fehlen, zu alt oder widersprüchlich sind;
- Feature- und Modellversion nicht zusammenpassen;
- Modellwerte ungültig oder unplausibel sind;
- Spread oder erwartete Kosten zu hoch sind;
- Broker- oder Kontozustand unklar ist;
- die kleinste gültige Order das Limit überschreitet;
- Einzeltrade-, Portfolio- oder Frequenzgrenzen verletzt würden;
- ein definierter Sperrzeitraum gilt.
- das erwartete Netto-Risk-to-Reward nach Gebühren, Spread, Slippage und
  Tick-Rundung unter `1:1` liegt.

## Gesamter Handelsstopp

- Tagesverlust oder maximaler Drawdown erreicht;
- interne und Brokerpositionen weichen ab;
- wiederholte Order- oder Verbindungsfehler;
- Marktdatenquelle oder Aufzeichnung fällt aus;
- ungewöhnliche Signal- oder Orderfrequenz;
- fehlerhafte Zeitstempel;
- reale Ergebnisse weichen stark vom erwarteten Bereich ab.

## Beschlossene V1-Grenzen

| Grenze | V1-Regel |
|---|---:|
| Risiko pro Trade | maximal 2,0 % des maßgeblichen Kontostands |
| Tagesverlust bis Einstiegssperre | 8,0 % |
| offene Trades | maximal 3, nur gleichgerichtet |
| aggregiertes geplantes Anfangsrisiko | maximal 6,0 % |
| neue Trades pro Handelstag | maximal 10 |
| Verlusttrades bis Tagessperre | 3 in Folge |
| technische Orderfehler bis Stopp | 3 in Folge |

Ist ein Micro-Kontrakt bereits zu groß für das Risikolimit, wird nicht gehandelt.

Verlustserien und technische Orderfehler werden getrennt gezählt. Risikobasis
ist der niedrigere Wert aus aktuellem Kontowert und Kontowert zu Beginn des
offiziellen CME-Handelstags. Realisierte Verluste, offene Verluste, Gebühren und
Provisionen zählen gemeinsam zum Tagesverlust. Tagesgewinne erhöhen das
Risikobudget nicht.

Die 8-%-Grenze blockiert neue Trades, schließt bestehende Trades aber nicht
automatisch. Sie ist daher keine garantierte Obergrenze des endgültigen
Tagesverlusts.

Für V1 gelten diese Grenzen auf Kontoebene für alle ausführenden
Strategy Instances zusammen. Einzelne Instanzen dürfen zusätzlich kleinere
Budgets erhalten. Korrelations- und marktübergreifendes Portfoliorisiko werden
vor mehreren Live-Märkten ergänzt und validiert.

## Kill Switch

Der manuelle Not-Aus besitzt zwei Stufen:

1. **System herunterfahren:** neue Trades und neue Hintergrundarbeiten stoppen;
   offene Positionen mit bestätigten brokerseitigen Schutzorders bestehen
   lassen.
2. **Full-Stop:** zusätzlich alle offenen Positionen kontrolliert schließen und
   den positionslosen Zustand beim Broker bestätigen.

Bei automatischen Ausfallstopps werden zunächst neue Trades blockiert. Offene
Positionen bleiben durch bestätigte brokerseitige Stop- und Zielorders geschützt.
Ist dieser Schutz unklar, greift ein gesonderter Notfallablauf.

Strategy-bezogene Fehler sperren nur die betroffene Instanz. Instrumentfehler
sperren alle Instanzen dieses Instruments, Konto-/Brokerfehler alle ausführenden
Instanzen des Kontos und plattformweite Fehler die gesamte betroffene
Handelsumgebung. Offene Trades bleiben unter gemeinsamer Schutz-, Schließungs-
und Abgleichverwaltung. Strategy-Neustarts sind begrenzt, setzen einen
bekannten Brokerzustand voraus und heben eine manuell notwendige Freigabe nicht
auf.

Eine Position gilt erst als geschützt, wenn der Broker Stop Loss und Take Profit
für die tatsächlich offene Richtung und Menge bestätigt hat. Fehlt dieser
Schutz, werden neue Trades blockiert und die Schutzorders werden korrigiert.
Kann der Schutz nicht innerhalb einer kurzen konfigurierbaren Frist hergestellt
werden, wird ausschließlich die betroffene Position geschlossen.
