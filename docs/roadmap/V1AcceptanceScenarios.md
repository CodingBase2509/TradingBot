# V1-End-to-End-Abnahmeszenarien

## Zweck

Diese Szenarien definieren, wann der deterministische Plattformkern der V1 als
fachlich funktionsfähig gilt. Sie prüfen den vollständigen Weg über reale
Modulgrenzen hinweg. Profitabilität des späteren ML-Modells ist kein
Abnahmekriterium.

Jedes Szenario wird mindestens mit der festen Teststrategie und dem simulierten
Broker ausgeführt. Brokerabhängige Szenarien werden zusätzlich in der
IBKR-Paper-Umgebung geprüft. Zeit, Marktdaten und Brokerereignisse müssen für
automatisierte Wiederholungen kontrollierbar sein.

## Gemeinsame Nachweise

Für jedes Szenario müssen, soweit anwendbar, nachvollziehbar sein:

- verwendeter Datenstand und Datenqualitätsstatus;
- Strategy-, Konfigurations- und Modellpaket-ID;
- Decision, Candidate und maschinenlesbarer Reason Code;
- Risikoprüfung und Risikoreservierung;
- Order-, Fill-, Trade- und Positionsereignisse;
- Broker- beziehungsweise Simulationsabgleich;
- Audit-, Warn- und Systemzustände;
- eindeutiger Endzustand ohne verwaiste Orders oder Reservierungen.

Ein automatisierter Lauf ist nur bestanden, wenn sein fachliches Ergebnis bei
identischen Eingaben reproduzierbar ist.

## 1. Erfolgreicher geschützter Trade

**Ausgangslage:** Daten, Kalender, Modell, Broker und Konto sind gültig. Es
existiert kein offener MES-Trade und alle Grenzen besitzen ausreichend Budget.

**Ablauf:** Eine abgeschlossene 5-Minuten-Kerze erzeugt einen gültigen
Kandidaten. Decision und Risk Guard akzeptieren ihn. Die Einstiegsorder wird
vollständig ausgeführt und anschließend durch bestätigten TP und SL geschützt.
Eine Schutzorder schließt den Trade.

**Abnahme:** Der Trade erreicht `Reconciled`; Position und zugehörige Orders
sind beim Broker und intern geschlossen, Risiko und Tradeplatz sind korrekt
freigegeben und Ergebnis, Kosten, Haltedauern sowie Schließungsgrund sind
vollständig gespeichert.

## 2. Fachlich abgelehnter Trade

**Varianten:** Risiko über dem Limit, Netto-Risk-to-Reward unter `1:1`, falsche
Richtung bei offenen MES-Trades, drei belegte Tradeplätze, zehn verbrauchte
Tagestokens, Verlustseriensperre oder Tagesverlustsperre.

**Abnahme:** Es wird keine Brokerorder erzeugt. Die Ablehnung enthält den
richtigen Reason Code. Eine nur vorläufig angelegte Reservierung hinterlässt
kein verbrauchtes Risiko, keinen Tradeplatz und kein Tagestoken.

## 3. Teilfüllung des Einstiegs

**Ablauf:** Der Broker füllt nur einen Teil der Einstiegsorder. Später wird die
Restmenge ausgeführt, eindeutig storniert oder abgelehnt.

**Abnahme:** Bereits die erste gefüllte Menge erhält passenden brokerseitig
bestätigten TP und SL. Positionsrisiko und verbleibende Reservierung entsprechen
zu jeder Zeit den Mengen. Die Restorder darf nicht ohne erneute Prüfung
aggressiver ausgeführt werden.

## 4. Fehlender oder falscher Schutz

**Varianten:** TP oder SL wird abgelehnt, besitzt eine falsche Menge oder seine
Kopplung kann nicht bestätigt werden.

**Abnahme:** Neue Trades werden im kleinsten sicheren Bereich blockiert. Die
Plattform versucht den Schutz innerhalb der konfigurierten Frist zu korrigieren.
Scheitert dies, wird nur die betroffene Position kontrolliert geschlossen und
der Vorgang als kritischer Fehler auditiert.

## 5. Datenlücke

**Variante Gelb:** Höchstens drei ältere 5-Minuten-Lücken liegen außerhalb aller
für die Entscheidung benötigten Fenster und entsprechen dem Missing-Data-Vertrag.

**Variante Rot:** Eine Lücke ist aktuell, liegt innerhalb der letzten 60
Minuten, ist entscheidungsrelevant oder überschreitet die Toleranz.

**Abnahme:** Gelb darf ausschließlich mit Qualitätskennzeichen verarbeitet
werden. Rot erzeugt keine neue Order und einen eindeutigen Ablehnungs- und
Warnzustand. Unbekannte Marktpreise werden niemals interpoliert.

## 6. Broker- oder Verbindungsstörung

**Ablauf:** Die Verbindung fällt vor dem Senden, nach dem Senden mit unbekannter
Antwort oder während eines offenen Trades aus.

**Abnahme:** Es entstehen keine neuen Trades. Möglicherweise gesendete Orders
und ihre Reservierungen bleiben `Unknown` und gebunden. Nach Wiederverbindung
werden Brokerorders, Fills, Positionen und Schutz abgeglichen, bevor eine
kontrollierte Wiederaufnahme möglich ist.

## 7. Neustart mit offenem Zustand

**Varianten:** Neustart während ausstehender Einstiegsorder, Teilfüllung,
geschützter Position, Schließung oder unbekannter Brokerantwort.

**Abnahme:** Der Start erfolgt global blockiert. Persistierte Zustände und
Reservierungen werden geladen und mit dem Broker abgeglichen. Eine
möglicherweise gesendete Order wird nicht doppelt gesendet. Freigabe erfolgt
erst bei eindeutigem Order-, Positions-, Schutz- und Risikozustand.

## 8. Gleichzeitige Strategy-Entscheidungen

**Ablauf:** Mehrere Strategy Instances beantragen nahezu gleichzeitig den
letzten verfügbaren Tradeplatz, dasselbe Risikobudget oder die gebundene
Instrumentrichtung.

**Abnahme:** Der Account Risk Coordinator akzeptiert nur die zulässige Menge an
Reservierungen. Risiko, Tradeplatz, Tagestoken und Richtung werden atomar
behandelt; die globalen Grenzen werden durch Parallelität nie überschritten.

## 9. Tagesgrenzen und neuer Handelstag

**Varianten:** zehn eröffnete Trades, drei Verlusttrades in Folge, drei
technische Orderfehler in Folge und Tagesverlust von mindestens 8 %.

**Abnahme:** Jede Grenze blockiert neue Trades zum festgelegten Zeitpunkt.
Verlust- und Technikserien bleiben getrennt. Offene geschützte Trades werden
nicht allein wegen der 8-%-Sperre geschlossen. Tagesgrenzen wechseln erst mit
dem offiziellen CME-Handelstag und nur nach erfolgreicher Zustandsprüfung.

## 10. Handelszeit, Pause und Freitag

**Varianten:** Signal kurz vor der täglichen Pause, geschützter Trade über die
Pause sowie offene Position vor dem wöchentlichen Schluss.

**Abnahme:** Ab zwei Stunden vor der täglichen Pause gibt es keinen neuen
Einstieg. Geschützte Positionen dürfen die Pause überstehen. Freitags beginnt
die Glattstellung 60 Minuten vor Schluss, wird ab 30 Minuten aggressiver und
erzeugt 15 Minuten vor Schluss bei Restpositionen einen kritischen Alarm. Keine
Position bleibt wissentlich über das Wochenende offen.

## 11. Kontrollierter Systemstopp

**Ablauf:** Der Benutzer bestätigt „System herunterfahren“.

**Abnahme:** Neue Trades, Datenimporte, Analysen, Backtests und Trainingsläufe
werden blockiert beziehungsweise kontrolliert beendet. Offene Positionen
bleiben nur mit bestätigtem brokerseitigem TP und SL bestehen. Zustand und
Auslöser werden auditiert; ein Neustart hebt die Sperre nicht automatisch auf.

## 12. Full-Stop

**Ablauf:** Der Benutzer bestätigt den Full-Stop bei offenen Orders und
Positionen.

**Abnahme:** Nicht benötigte Orders werden storniert und alle Positionen
kontrolliert geschlossen. Erfolg wird erst angezeigt, wenn der Broker den
positionslosen Zustand und das Fehlen verwaister Orders bestätigt. Andernfalls
bleibt `Full-Stop läuft/gestört` mit kritischem Alarm aktiv.

## 13. Modellpaket und Strategy-Erstellung

**Varianten:** vollständiges gültiges Paket, unvollständiger Ordner, falsche
Prüfsumme, unbekannte Vertragsversion sowie gleiche Paket-ID mit anderem Inhalt.

**Abnahme:** Nur ein vollständig geprüftes Paket erreicht `Available` und wird
in der UI angeboten. Ein ungültiges Paket aktiviert weder Modell noch Strategy.
Erst die bestätigte UI-Auswahl erzeugt eine versionierte Strategy Instance und
einen Audit-Eintrag.

## 14. Python-/ONNX-/NET-Parität

**Ablauf:** Mindestens 500 Referenzfälle werden in Python, ONNX und .NET
ausgeführt.

**Abnahme:** Enumcodes, Ticks, Mengen, Kandidatenfolge, Fingerprints,
Ablehnungsgründe und endgültige Entscheidung stimmen exakt überein. Numerische
Features und Outputs liegen innerhalb der festgelegten Toleranzen. Jede andere
Rangfolge, Schwellenentscheidung, Richtung oder Kandidatenauswahl lehnt das
Paket ab.

## Abschluss der V1-Abnahme

Der Plattformkern ist erst abgenommen, wenn alle für Phase 1 relevanten
Szenarien automatisiert bestanden sind, die IBKR-abhängigen Varianten im
Paper-Konto praktisch bestätigt wurden und keine ungeklärte kritische
Abweichung offen ist. Spätere ML-, Shadow- und Paper-Mindestzeiträume bleiben
zusätzliche Abnahmen der folgenden Phasen.
