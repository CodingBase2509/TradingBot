# ADR-012: Modellevaluation und Promotion der V1

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Ein Modell darf nicht aufgrund weniger günstiger Trades oder eines einzelnen
Marktregimes freigegeben werden. V1 benötigt vorab festgelegte Mindestkriterien
für unbekannte historische Daten, Shadow Mode und Paper Trading.

## Entscheidung

### Statistische Zähleinheit

Mehrere eng zusammengehörige MES-Trades dürfen die Mindestzahl nicht künstlich
erhöhen. Alle gleichgerichteten Trades von der ersten Eröffnung bis zur
vollständigen Glattstellung bilden für die statistische Mindestzahl eine
**Signalgruppe**.

Jeder logische Trade bleibt zusätzlich einzeln auswertbar. Die Definition und
Version der Gruppierung wird in jedem Prüfbericht gespeichert.

### Unbekannter historischer Test

Ein Kandidat muss mindestens folgende Bedingungen erfüllen:

- mindestens 300 abgeschlossene Signalgruppen;
- mindestens 24 Monate zeitlich spätere, beim Training unbekannte Daten;
- mindestens fünf Walk-Forward-Testfenster von jeweils ungefähr vier bis sechs
  Monaten;
- positives Nettoergebnis in mindestens vier von fünf Fenstern;
- positives Nettoergebnis nach vollständigen Kosten;
- Profit Factor von mindestens `1,20` im Basisszenario;
- maximaler Drawdown von höchstens `20 %`;
- weiterhin positives Nettoergebnis und Profit Factor über `1,0` bei zwei Ticks
  Slippage je Orderseite;
- bei drei Ticks Slippage kein struktureller Zusammenbruch: Profit Factor
  mindestens `0,90` und Drawdown höchstens das 1,5-Fache des Drawdowns im
  Basisszenario.

Sowohl Mindestzeitraum als auch Mindestzahl müssen erfüllt sein. 300
Signalgruppen in einem kurzen Zeitraum genügen nicht. Sind nach 24 Monaten
weniger als 300 Signalgruppen vorhanden, ist die Evidenz noch nicht ausreichend.

Testfenster und Enddatum werden vor Ausführung des unbekannten Tests festgelegt.
Sie dürfen nicht nachträglich an einen günstigen Ergebnisverlauf angepasst
werden.

### Konzentrationsprüfung

- Kein einzelner logischer Trade und keine einzelne Signalgruppe darf für mehr
  als 10 % des gesamten Nettogewinns verantwortlich sein.
- Kein einzelnes Walk-Forward-Fenster darf mehr als 40 % der Summe aller
  positiven Fensterergebnisse liefern.
- Verlustserien, Tageszeiten, Sitzungen, Long/Short und Volatilitätsphasen
  werden getrennt ausgewiesen.

Eine Überschreitung blockiert die automatische Promotion und benötigt einen
neuen Kandidaten oder eine ausdrücklich dokumentierte Neubewertung mit zuvor
festgelegten Kriterien.

### Vergleichsvarianten

Jeder Kandidat wird unter denselben Daten-, Kosten- und Risikobedingungen
verglichen mit:

- keinem Handel;
- einer einfachen Trend-Baseline;
- einer einfachen Mean-Reversion-Baseline;
- einer Zufallsentscheidung mit gleichem Risikobudget;
- dem aktuellen Champion, sobald einer existiert.

Der Kandidat muss „kein Trade“ und die einfachen Baselines nach Kosten
übertreffen. Die konkrete Modellschwelle gegenüber „kein Trade“ wird auf
Validierungsdaten gewählt und vor dem unbekannten Test eingefroren.

### Shadow Mode

Nach bestandener historischer Validierung läuft der Kandidat mindestens vier
vollständige Wochen mit Live-Marktdaten:

- keine Orders an den Broker;
- Entscheidungen und intern simulierte Ausführungen werden vollständig
  aufgezeichnet;
- Daten-, Feature- und ONNX-Ausgaben müssen fehlerfrei und nachvollziehbar sein;
- wiederholte technische oder fachliche Abweichungen setzen die
  Beobachtungsdauer nach Behebung mit einer neuen Version zurück.

### Paper Trading

Nach bestandenem Shadow Mode handelt der Kandidat über die Paper-Umgebung des
Brokers. Für eine erfolgreiche V1-Paper-Phase müssen beide Bedingungen erfüllt
sein:

- mindestens acht vollständige Wochen;
- mindestens 100 abgeschlossene Signalgruppen.

Zusätzlich gelten:

- keine ungeklärten Abweichungen zwischen internen und Brokerpositionen;
- Stop Loss und Take Profit sind für jede offene Position brokerseitig
  bestätigt;
- alle Risiko-, Wochenend- und Not-Aus-Regeln funktionieren nachweislich;
- Paper-Ergebnis, Ausführungskosten und Slippage weichen nicht unerklärt vom
  erwarteten Bereich ab.

### Ablehnung und Rückstufung

- Ein nicht bestandenes Muss-Kriterium verhindert die Promotion.
- Kriterien werden nicht nachträglich gelockert, um einen vorhandenen Kandidaten
  freizugeben.
- Modell-, Feature-, Schwellen- oder wesentliche Konfigurationsänderungen
  erzeugen einen neuen Kandidaten und starten die betroffenen Prüfungen neu.
- Technische Störungen werden behoben und mit einer neuen Artefakt- oder
  Plattformversion erneut geprüft.

Canary und Live-Produktion gehören nicht zum V1-Umfang und erhalten vor einem
späteren Live-Pilot eine eigene Entscheidung.

## Begründung

Zeitdauer und Anzahl verhindern gemeinsam, dass hohe Handelsfrequenz in nur
einem Marktregime als ausreichende Evidenz gilt. Walk-Forward-Fenster,
Kostenstress und Konzentrationsgrenzen prüfen, ob Ergebnisse zeitlich stabil
und nicht von wenigen Glückstreffern abhängig sind.

Shadow trennt Live-Daten- und Modellprobleme von Brokerproblemen. Paper Trading
prüft anschließend den vollständigen Order- und Positionsablauf ohne echtes
Kapital.

## Folgen

- Prüfberichte müssen alle Muss-Kriterien maschinenlesbar ausweisen.
- Signalgruppen und logische Trades werden getrennt gespeichert.
- Ein Statuswechsel ist nur bei vollständig bestandenem Prüfbericht möglich.
- Schwellenwerte sind versionierte Freigabepolitik und keine frei änderbaren
  Dashboard-Einstellungen.
