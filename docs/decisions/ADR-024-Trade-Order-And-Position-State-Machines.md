# ADR-024: Zustandsmaschinen für Trade, Order und Position

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Eine Modellentscheidung, eine fachliche Handelsidee, Brokerorders und die
tatsächlich ausgeführte Position sind unterschiedliche Dinge. Teilfüllungen,
verzögerte Bestätigungen, gleichzeitige Stornierung und Füllung,
Anwendungsneustarts sowie mehrere Strategy Instances machen eine einzelne
Statusspalte unzuverlässig.

Der interne Ablauf muss vollständig nachvollziehbar sein, ohne die
Benutzeroberfläche unnötig kompliziert zu machen.

## Entscheidung

### Getrennte Lebenszyklen

Die Plattform unterscheidet:

```text
Decision
→ Modell- beziehungsweise NoTrade-Entscheidung

Trade
→ fachliche Handelsidee einer Strategy Instance

Order
→ einzelner Auftrag an Broker oder Simulation

Position
→ aus bestätigten Ausführungen berechnete Menge
```

Ein Trade kann mehrere Einstiegs-, Schutz-, Korrektur- und Schließungsorders
besitzen. Eine Order kann durch mehrere Ausführungen teilweise oder vollständig
gefüllt werden.

### Vereinfachte Benutzersicht

Dashboard und normale Bedienung zeigen primär:

```text
geplant
→ freigegeben
→ Einstieg läuft
→ eröffnet und geschützt
→ Schließung läuft
→ geschlossen und abgeglichen
```

Feinere technische Zustände, Gründe und Ereignisse bleiben für Diagnose und
Audit einsehbar.

### Trade-Zustände

Der fachliche Trade verwendet mindestens:

```text
IntentCreated
├─ Rejected
└─ RiskApproved
   └─ EntryPending
      ├─ Cancelled
      ├─ Failed
      ├─ EntryPartial
      └─ Protecting
         ├─ ActiveProtected
         │  └─ ExitPending
         │     ├─ PartiallyClosed
         │     └─ Closed
         │        └─ Reconciled
         └─ EmergencyClosing
            ├─ Closed
            └─ EmergencyFailed
```

`StrategyInstanceId`, Konfigurations-, Modell-, Kandidaten- und
Freigabeversion begleiten den Trade durchgehend.

### Absicht und Risikofreigabe

`IntentCreated` enthält noch keine Brokerorder. Ablehnungen erhalten einen
maschinenlesbaren Grund.

`RiskApproved` enthält konkrete Menge, Preise, geplantes Risiko,
Kostenannahmen, Risikoreservierung und eine kurze Gültigkeit. Wesentliche
Änderungen vor dem Senden erfordern eine erneute Prüfung.

Risikobetrag, Tradeplatz, tägliches Trade-Token und Richtung werden gemäß
ADR-025 atomar reserviert. Zustandswechsel durch Teilfüllung, Stornierung und
Schließung verbrauchen, teilen oder lösen diese Reservierung nachvollziehbar.

### Einstieg und Teilfüllung

Vor dem Senden werden Orderabsicht und stabile Idempotenz-ID dauerhaft
registriert. Eine unbekannte Brokerantwort darf nicht als nicht ausgeführt
interpretiert werden.

Jede Teilfüllung erzeugt sofort eine tatsächliche Position. Die gefüllte Menge
muss unmittelbar durch passenden TP und SL geschützt werden. Die noch offene
Einstiegsmenge wird unabhängig behandelt und darf nicht ohne neue Kosten- und
Risikoprüfung aggressiver ausgeführt werden.

### Schutz

`Protecting` bedeutet, dass TP und SL gesendet oder korrigiert werden, aber
noch nicht vollständig bestätigt sind. `ActiveProtected` wird erst erreicht,
wenn der Broker für die tatsächlich offene Menge bestätigt:

- richtige Richtung und Menge;
- richtigen Stop und Take Profit;
- richtige Trade- beziehungsweise Allokationszuordnung;
- zuverlässige Kopplung beziehungsweise Mengenpflege.

Interne Absicht oder bloß gesendete Orders gelten nicht als Schutz.

Kann der Schutz nicht innerhalb einer kurzen konfigurierbaren Frist bestätigt
werden, wechselt der Trade zu `EmergencyClosing`. Neue Trades bleiben
mindestens für die betroffene Instanz blockiert; bei unklarem Konto- oder
Brokerzustand gilt die globale Sperre.

### Schließung

Schließung kann mindestens ausgelöst werden durch:

- Take Profit;
- Stop Loss;
- Freitagsschließung;
- Full-Stop;
- fehlenden Schutz;
- technische Notfallregel;
- später freigegebene manuelle Aktion.

Die prognostizierte Haltedauer ist kein Schließungsauslöser.

Teilweise Schließungen führen zur sofortigen Neuberechnung der Restposition und
Anpassung der Schutzordermengen. Vollständig geschlossene Positionen dürfen
keine verwaisten Schutzorders hinterlassen, die eine neue Gegenposition
eröffnen könnten.

### Abschluss und Reconciliation

`Closed` bedeutet intern erwartete Menge null. Erst `Reconciled` bestätigt:

- interne Strategy Position Allocation ist null;
- Brokerzuordnung und erwartete Nettoposition stimmen;
- keine zugehörigen oder verwaisten Orders sind offen;
- Ausführungen, Gebühren, Ergebnis und Schließungsgrund sind vollständig.

Erst danach fließt der Trade endgültig in Verlustserie, Performancevergleich
und spätere Trainingsdaten ein.

### Order-Zustände

Jede Order verwendet mindestens:

```text
Created
→ Submitted
→ Acknowledged
→ PartiallyFilled
→ Filled
```

Weitere Zustände:

```text
CancelRequested
Cancelled
Rejected
Expired
Unknown
```

`CancelRequested` ist nicht gleich `Cancelled`. Eine Füllung während einer
laufenden Stornierung muss korrekt und idempotent verarbeitet werden.

### Positionsberechnung

Positionen entstehen ausschließlich aus bestätigten Ausführungen:

```text
Long-Ausführungen - Short-Ausführungen = Broker-Nettoposition
```

Zusätzlich muss gelten:

```text
Summe Strategy Position Allocations = erwartete Broker-Nettoposition
```

Eine Abweichung blockiert neue Trades global, bis sie geklärt oder kontrolliert
behandelt ist.

### Neustart und Wiederherstellung

Nach einem Neustart gilt:

```text
global blockieren
→ internen Zustand laden
→ Brokerorders, Ausführungen und Positionen abrufen
→ Schutz prüfen
→ Abweichungen behandeln
→ kontrolliert freigeben
```

Eine möglicherweise bereits gesendete Order wird nicht blind erneut gesendet.
Zuerst wird sie über interne und Broker-IDs gesucht und abgeglichen.

### Modi

- Shadow speichert Decision und Trade Intent, sendet keine Order.
- Ein virtueller Shadow-Tracker darf den hypothetischen Verlauf getrennt
  beobachten.
- Simulated Paper durchläuft die vollständige Order-, Ausführungs-, Schutz- und
  Positionslogik.
- Broker Paper verwendet dieselbe Logik mit dem IBKR-Paper-Adapter.

## Grundregeln

- Keine aktive Position ohne bestätigten TP und SL.
- Jede Teilfüllung löst eine Schutzprüfung aus.
- Brokerereignisse werden idempotent verarbeitet.
- Unbekannter Orderstatus gilt nicht als abgelehnt oder storniert.
- Unmögliche Übergänge blockieren neue Trades und erzeugen Alarm.
- Jeder Übergang ist dauerhaft, zeitlich geordnet und auditierbar.
- Der Broker ist maßgeblich für tatsächliche Orders, Ausführungen und
  Positionen.

## Begründung

Getrennte Zustandsmaschinen verhindern, dass eine fachliche Absicht mit einer
tatsächlichen Brokerposition verwechselt wird. Teilfüllungen und asynchrone
Brokerereignisse können sicher verarbeitet werden. Idempotenz und
Reconciliation verhindern Doppelorders nach Neustarts.

Die vereinfachte Benutzersicht hält die Anwendung verständlich, während die
feine interne Zustandsfolge Diagnose, Audit und spätere Echtgeldsicherheit
ermöglicht.

## Folgen

- Zustandsübergänge werden als explizite Regeln und Tests implementiert.
- Broker- und Simulationsadapter müssen dieselben Orderereignisse liefern.
- Persistenz und Ereignisverarbeitung benötigen Idempotenzschlüssel.
- Die zeitlichen Fristen für Bestätigung, Schutz und Stornierung werden nach
  praktischen IBKR-Paper-Tests festgelegt.
- Konkrete PostgreSQL-Tabellen und Konsistenzregeln folgen im Datenmodell.
