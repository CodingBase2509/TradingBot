# ADR-006: Handels- und Risikopolitik der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Der V1-Produktumfang legt MES und Paper Trading fest. Für Backtest,
Positionsverwaltung, Risk Guard und Not-Aus werden zusätzlich eindeutige
Handelszeiten, Verlustgrenzen und Ausfallregeln benötigt.

## Entscheidung

### Handelszeit und Einstieg

- Die Plattform darf während der gesamten regulär verfügbaren MES-Handelszeit
  handeln und umfasst damit asiatische, europäische und amerikanische
  Handelsphasen.
- Der offizielle Börsenkalender ist die maßgebliche Quelle für Sitzungsgrenzen,
  Wartungspausen, Feiertage und verkürzte Handelstage. Zeiten werden nicht als
  dauerhaft unveränderliche Uhrzeiten im Fachkern hinterlegt.
- Zwei Stunden vor jeder täglichen Börsen- beziehungsweise Wartungspause werden
  keine neuen Positionen mehr eröffnet.
- Bereits offene Positionen dürfen über eine tägliche Wartungspause hinweg
  weiterlaufen. Ihre unveränderten brokerseitigen Stop-Loss- und
  Take-Profit-Orders bleiben bestehen.
- Während einer Börsenunterbrechung kann eine Schutzorder nicht ausgeführt
  werden. Dieses Kurslückenrisiko wird in Backtest und Paper-Auswertung
  ausdrücklich erfasst.
- Vor dem wöchentlichen Börsenschluss am Freitag werden alle Positionen
  kontrolliert geschlossen. Der genaue Sicherheitspuffer und die
  Eskalationsschritte für nicht ausgeführte Schließungsorders werden im
  Ausführungskonzept festgelegt.

### Handelsrisiko

- Das geplante maximale Risiko pro Trade beträgt 2,0 % des für die
  Risikoberechnung maßgeblichen Kontostands.
- Ist bereits ein einzelner MES-Kontrakt einschließlich Kosten und
  Sicherheitsaufschlag größer als das erlaubte Risiko, wird nicht gehandelt.
- Erreicht der laufende Tagesverlust 8 % des maßgeblichen Kontostands, werden
  für den restlichen Handelstag keine neuen Trades zugelassen.
- Diese 8-%-Grenze löst keinen automatischen Full-Stop aus. Bereits offene
  Trades laufen mit ihren bestehenden Stop-Loss- und Take-Profit-Orders weiter.
- Die 8-%-Grenze ist deshalb eine harte Einstiegssperre, aber keine garantierte
  Obergrenze des endgültigen Tagesverlusts. Offene Trades, Slippage und
  Kurslücken können zu einem höheren Endverlust führen.
- Es dürfen höchstens drei MES-Trades gleichzeitig offen sein.
- Solange MES-Trades offen sind, dürfen neue Trades nur in derselben Richtung
  eröffnet werden. Gegenläufige Signale werden in V1 als „kein neuer Trade“
  abgelehnt und mit einem eigenen Grund protokolliert.
- Das geplante aggregierte Anfangsrisiko aller offenen Trades darf 6,0 % des
  maßgeblichen Kontostands nicht überschreiten.
- Pro Handelstag dürfen höchstens zehn neue Trades eröffnet werden.
- Nach drei Verlusttrades in Folge werden für den restlichen Handelstag keine
  neuen Trades zugelassen.
- Verlusttrades und technische Orderfehler sind unterschiedliche Ereignisse und
  besitzen getrennte Zähler.
- Nach drei technischen Orderfehlern in Folge wird der Handel unabhängig von
  der Verlustserie gestoppt und muss vor einer Wiederaufnahme geprüft werden.

Bei vollständig ausgeschöpftem Risiko entsprechen drei aufeinanderfolgende
Stop-Loss-Trades planmäßig bis zu 6 % Verlust. Die Verlustseriensperre greift
damit im Normalfall vor der 8-%-Einstiegssperre. Die 8-%-Grenze schützt
zusätzlich bei nicht aufeinanderfolgenden Verlusten, gleichzeitig offenen
Verlusten, Slippage oder Kurslücken.

Für Kontostand und Tagesverlust gilt:

- Risikobasis ist der niedrigere Wert aus dem aktuellen Kontowert und dem
  Kontowert zu Beginn des fachlichen Handelstags. Tagesgewinne erhöhen das
  Risikobudget nicht.
- Der fachliche Handelstag entspricht dem offiziellen CME-Handelstag und nicht
  einem lokalen Kalenderwechsel um Mitternacht.
- Für die 8-%-Einstiegssperre zählen realisierte Verluste, offene Verluste,
  Gebühren und Provisionen gemeinsam.

### Ausfälle

Bei unklaren oder ausgefallenen Markt-, Broker-, Zeit-, Speicher- oder
Systemkomponenten gilt:

- keine neuen Trades;
- keine automatisch erzeugte Ersatzentscheidung;
- offene Positionen bleiben durch zuvor platzierte brokerseitige Stop-Loss- und
  Take-Profit-Orders geschützt;
- der Zustand wird protokolliert und nach Wiederanlauf zuerst mit dem Broker
  abgeglichen;
- kann der Schutz einer offenen Position nicht bestätigt werden, greift der
  nachfolgend definierte Notfallablauf.

Eine offene Position gilt nur dann als geschützt, wenn der Broker für die
tatsächlich offene Menge sowohl Stop Loss als auch Take Profit bestätigt. Die
Schutzorders müssen zur richtigen Position, Richtung und Menge gehören und so
gekoppelt sein, dass die Ausführung einer Seite die andere Seite zuverlässig
storniert oder anpasst. Interne Orderabsichten oder gesendete, aber nicht
bestätigte Orders genügen nicht.

Fehlt dieser bestätigte Schutz:

1. werden sofort alle neuen Trades blockiert;
2. wird die fehlende oder falsche Schutzorder erneut platziert beziehungsweise
   korrigiert;
3. gelingt dies nicht innerhalb einer kurzen, konfigurierbaren Frist, wird nur
   die betroffene ungeschützte Position kontrolliert geschlossen;
4. Teilfüllungen und Mengenänderungen lösen unmittelbar eine erneute
   Schutzprüfung aus.

### Manueller Not-Aus

Die V1 bietet zwei Stufen:

1. **System herunterfahren**
   - keine neuen Trades;
   - keine neuen Datenimporte, Analysen, Backtests oder Trainingsläufe;
   - laufende Arbeiten werden kontrolliert beendet oder abgebrochen;
   - offene Positionen und ihre brokerseitigen Schutzorders bleiben bestehen;
   - der erreichte Zustand wird vor dem Herunterfahren protokolliert.
2. **Full-Stop**
   - zunächst gelten alle Maßnahmen von „System herunterfahren“;
   - offene Orders werden kontrolliert storniert, soweit sie nicht für die
     Schließung benötigt werden;
   - alle offenen Positionen werden geschlossen;
   - das Herunterfahren gilt erst als abgeschlossen, wenn der Broker den
     positionslosen Zustand bestätigt hat;
   - kann eine Position nicht geschlossen werden, bleibt der Zustand
     „Full-Stop läuft/gestört“ aktiv und erzeugt einen kritischen Alarm.

Beide Aktionen benötigen eine bewusste Bestätigung und einen Audit-Eintrag. Ein
Neustart hebt die Handelssperre nicht automatisch auf; zuerst erfolgen
Brokerabgleich, Datenprüfung und eine kontrollierte Freigabe.

### Wiederaufnahme

- Eine Sperre durch drei Verlusttrades oder die 8-%-Tagesverlustgrenze kann
  frühestens mit dem nächsten offiziellen CME-Handelstag aufgehoben werden.
- Die Aufhebung erfolgt nur nach erfolgreicher Daten-, System- und
  Brokerprüfung.
- Eine Sperre durch technische Orderfehler sowie beide manuellen
  Not-Aus-Stufen benötigt zusätzlich eine bewusste manuelle Freigabe.

### Freitagsschließung

Die Zeitpunkte werden relativ zum offiziellen wöchentlichen Börsenschluss
bestimmt, damit verkürzte Handelstage berücksichtigt werden:

- 60 Minuten vorher beginnt die kontrollierte Glattstellung;
- 30 Minuten vorher wird die Ausführung aggressiver;
- 15 Minuten vorher wird bei jeder noch offenen Position ein kritischer Alarm
  ausgelöst und der Eskalationsablauf fortgesetzt.

## Begründung

Die Nutzung aller MES-Handelsphasen ermöglicht spätere Vergleiche verschiedener
Liquiditäts- und Volatilitätsbedingungen. Ein täglicher Einstiegsschluss
reduziert neue Risiken unmittelbar vor der Wartungspause, ohne bereits
geschützte Paper-Positionen künstlich zu schließen. Die verpflichtende
Freitagsschließung verhindert Wochenendrisiken.

Getrennte Sperren für Handelsverluste und technische Fehler vermeiden, dass
fachliche Modellqualität und Betriebsstörungen miteinander vermischt werden.
Die zwei Not-Aus-Stufen erlauben sowohl ein kontrolliertes Beenden mit
geschützten Positionen als auch eine vollständige Glattstellung.

Gleichgerichtete Trades vermeiden in V1 Mehrdeutigkeit zwischen unabhängigen
Trades und der beim Broker geführten Nettoposition. Eine spätere Erweiterung
kann gegenläufige Teilstrategien zulassen, sofern ihre Ausführung bestehende
Trades nicht ungewollt reduziert oder schließt und Brokerzustand, Risiko sowie
Zuordnung eindeutig bleiben.

## Folgen

- Backtests müssen tägliche Unterbrechungen, Feiertage, verkürzte Handelstage und
  Kurslücken abbilden.
- Der Positionsmanager benötigt eine priorisierte Freitagsschließung mit
  Bestätigung und Eskalation.
- Der Risk Guard benötigt getrennte Zähler für Tradezahl, Verlustserie und
  technische Fehler.
- Trade Controller und Positionsmanager müssen bis zu drei logische Trades
  derselben Richtung eindeutig der aggregierten Brokerposition zuordnen.
- Gegenläufige Signale benötigen einen maschinenlesbaren V1-Ablehnungsgrund.
- Das Dashboard muss beide Not-Aus-Stufen eindeutig unterscheiden und ihren
  Fortschritt anzeigen.
- Schutzorders müssen vor einem kontrollierten Herunterfahren beim Broker
  bestätigt sein.

## Noch im technischen Design zu konkretisieren

- Anbieter, Aktualisierung und Versionierung des offiziellen Börsenkalenders;
- konkrete Ordertypen und Preisgrenzen der Freitagsschließung;
- Dauer der konfigurierbaren Schutzorder-Frist;
- Regeln für das Nachziehen des aggregierten Risikos nach Teil- oder
  Vollschließungen.

## Spätere Erweiterung

Nach V1 soll geprüft werden, ob eine kurzfristige Gegenbewegung innerhalb eines
größeren offenen Trades separat gehandelt werden kann. Voraussetzung ist ein
explizites Ausführungsmodell, das klärt:

- ob der Broker unabhängige gegenläufige Positionen unterstützt oder Positionen
  netto führt;
- wann ein Gegensignal als Teil- beziehungsweise Vollschließung gilt;
- wie Schutzorders, Trade-Zuordnung und aggregiertes Risiko erhalten bleiben;
- wie Backtest und Paper Trading dasselbe Netting-Verhalten abbilden.

Diese Erweiterung darf bestehende Trades nicht unbeabsichtigt schließen und
gehört nicht zum V1-Umfang.
