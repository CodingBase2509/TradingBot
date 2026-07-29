# ADR-027: Fehlerisolierung und vollständige Trennung der Trainingsumgebung

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Die Plattform hostet mehrere Strategy Instances und später gegebenenfalls
mehrere Märkte über gemeinsame Konto-, Risiko- und Brokerkomponenten. Ein
lokaler Modellfehler soll gesunde Strategien nicht unnötig stoppen. Ein Fehler
in gemeinsam genutzten sicherheitskritischen Komponenten darf dagegen keinen
weiteren Handel erlauben.

Python-Training und Forschung führen veränderlichen, experimentellen Code aus.
Sie dürfen weder eine Laufzeitabhängigkeit noch einen direkten Zugriffsweg in
Test oder Produktion bilden.

## Entscheidung

### Grundsatz der kleinsten sicheren Sperre

Fehler werden so eng wie möglich isoliert und so weit wie nötig eskaliert:

```text
Strategy-Fehler
→ betroffene Strategy Instance

Instrumentfehler
→ alle Strategy Instances dieses Instruments

Konto-/Brokerfehler
→ alle ausführenden Instanzen dieses Kontos

Plattformfehler
→ globale Handelssperre der betroffenen Umgebung
```

„System anhalten“ bedeutet zuerst, keine neuen Trades oder
bestätigungspflichtigen Änderungen zuzulassen. Die Anwendung bleibt soweit
sicher möglich aktiv, um offene Positionen, brokerseitigen Schutz,
Reconciliation, Alarme und Wiederherstellung zu betreiben.

Ein Prozess wird erst kontrolliert heruntergefahren, wenn sein Weiterbetrieb
nicht mehr sicher möglich ist. Ein Full-Stop bleibt eine gesonderte bewusste
beziehungsweise fest definierte Notfallaktion.

### Strategy-bezogene Fehler

Beispiele:

- Modellpaket oder ONNX-Inferenz nur dieser Instanz fehlerhaft;
- inkompatibler Feature- oder Entscheidungsvertrag;
- ungültige Kandidaten oder Modellausgaben;
- fehlerhafte Instanzkonfiguration;
- wiederholte Laufzeitüberschreitung.

Reaktion:

```text
Instanz blockieren
→ keine neuen Entscheidungen oder Orders
→ offene Trades unter gemeinsame Verwaltung stellen
→ Fehler und Zustand sichern
→ begrenzten kontrollierten Neustart versuchen
→ vollständig prüfen
→ freigeben oder Faulted lassen
```

Offene Trades bleiben Eigentum der Plattform und werden durch Trade
Management, Execution und Reconciliation weiter überwacht. Das Stoppen einer
Strategy Instance schließt sie nicht automatisch.

Verwenden mehrere Instanzen dieselbe fehlerhafte Modell- oder
Konfigurationsversion, werden alle betroffenen Instanzen gesperrt.

### Instrumentbezogene Fehler

Beispiele sind rote oder fehlende Marktdaten, unklare Vertragszuordnung,
fehlerhafter Rollover, ungültige Instrumentstammdaten oder ein unklarer
Handelskalender nur für dieses Instrument.

Alle Instanzen des Instruments werden für neue Entscheidungen und Orders
blockiert. Andere unabhängige Märkte dürfen weiterlaufen. Vor Wiederaufnahme
werden Datenfrische, Qualität, Vertrag, Kalender und offene Positionen geprüft.

### Konto- und Brokerfehler

Beispiele:

- Brokerverbindung oder Kontostand unklar;
- Broker- und interne Position weichen ab;
- Schutzorders sind nicht bestätigt;
- Account Risk Coordinator oder Risikoreservierungen sind inkonsistent;
- unbekannte Order oder Ausführung.

Alle ausführenden Instanzen des betroffenen Kontos werden blockiert.
Brokerzustand, Orders, Ausführungen, Positionen und Schutz werden abgeglichen.
Fehlender Schutz wird korrigiert oder die betroffene Position gemäß
Notfallregel geschlossen.

Unabhängige Shadow- und Simulated-Paper-Instanzen dürfen weiterlaufen, wenn
ihre Daten- und virtuellen Kontodienste gesund sind. Ergebnisse während einer
Kontostörung werden entsprechend markiert.

### Plattformweite Fehler

Eine globale Handelssperre der betroffenen Test- oder Produktionsumgebung gilt
mindestens bei:

- ausgefallenem oder inkonsistentem PostgreSQL;
- unzuverlässiger Systemzeit oder Zeitsynchronisation;
- fehlendem gemeinsamen Kalender;
- vollständigem Ausfall der Marktdatenverteilung;
- nicht funktionsfähigem globalem Risk Guard;
- nicht speicherbarem kritischem Audit- oder Operationszustand;
- beschädigtem oder widersprüchlichem Plattformzustand.

Es werden keine neuen Trades, Modellaktivierungen oder
Konfigurationsänderungen zugelassen. Offene Positionen bleiben soweit möglich
durch Brokerorders geschützt; Brokerzugang, Reconciliation, Monitoring und
Alarmierung bleiben aktiv.

Nach Wiederherstellung erfolgen vollständige Zustands-, Risiko-, Daten- und
Brokerprüfungen. Globale Zeit-, Datenbank-, Risiko- und Zustandsfehler
benötigen eine bewusste manuelle Handelsfreigabe.

### Zustände

Strategy Instances verwenden mindestens:

```text
Starting
Healthy
Degraded
Restarting
Blocked
Faulted
Stopped
```

Konto beziehungsweise Plattform verwenden mindestens:

```text
Healthy
TradingBlocked
Recovering
ManualApprovalRequired
ShuttingDown
FullStop
```

Die Zustände und Gründe sind stabile C#-Enums gemäß ADR-026.

### Kontrollierter Strategy-Neustart

```text
blockieren
→ Zustand und Ursache speichern
→ begrenzte Wartezeit
→ Konfiguration und Modellpaket prüfen
→ Feature- und Marktzustand neu aufbauen
→ Trades, Reservierungen und Zuordnungen laden
→ erforderlichen Brokerabgleich ausführen
→ Gesundheitsprüfung
→ kontrolliert freigeben
```

Wiederholungen sind begrenzt und verwenden wachsende Wartezeiten. Die konkreten
Fristen sind komponentenbezogene Konfiguration. Nach wiederholtem Fehler bleibt
die Instanz `Faulted` und benötigt manuelle Prüfung. Endlose
Neustartschleifen sind unzulässig.

Ein Neustart darf keine Order erneut senden, deren Zustand nicht eindeutig
geklärt ist.

### Wiederaufnahme

- Kurzfristige eindeutig behobene Strategy-Fehler dürfen nach vollständiger
  Prüfung automatisch freigegeben werden.
- Instrumentfehler dürfen nur nach ausreichend frischen und zulässigen Daten
  aufgehoben werden.
- Konto-/Brokerfehler benötigen erfolgreiche Reconciliation.
- PostgreSQL-, Zeit-, globaler Risiko- oder Plattformzustandsfehler benötigen
  manuelle Freigabe.
- Full-Stop benötigt immer manuelle Freigabe.

### Vollständige Isolation von Training und Forschung

Training/Forschung, Test und Produktion sind getrennte Laufzeit- und
Sicherheitszonen.

Die Python-Trainingsumgebung besitzt:

- eigene Prozesse beziehungsweise Hosts oder Container;
- eigenes Python, MLflow und Trainingswerkzeuge;
- eigene PostgreSQL-/MLflow-Bereiche;
- eigene Datei- und Artefaktwurzeln;
- eigene Benutzer, Rollen und Geheimnisse;
- keine Broker- oder Echtgeldzugänge;
- keinen direkten Schreibzugriff auf Test oder Produktion;
- keine Möglichkeit, Modelle dort selbst zu aktivieren.

Test und Produktion:

- führen keinen Python-Trainingscode aus;
- hängen zur Laufzeit nicht von Python, MLflow oder Training Orchestrator ab;
- greifen nicht direkt auf MLflow-Datenbank oder Forschungsartefakte zu;
- verwenden ausschließlich kontrolliert importierte und geprüfte
  Modellpakete.

Zwischen den Zonen gibt es keinen gemeinsamen beschreibbaren Datenspeicher und
keine direkte Datenbankkopplung. Gemeinsamer Quellcode darf aus demselben
Git-Repository gebaut werden, wird aber getrennt ausgeliefert und
konfiguriert.

### Kontrollierter Datenaustausch

Erlaubt sind ausschließlich:

```text
Test/Produktion
→ unveränderlicher, geprüfter Export
→ Training/Forschung

Training/Forschung
→ unveränderliches Modellpaket
→ Quarantäne und Prüfung
→ Test
→ später gesonderte Production-Promotion
```

Exporte enthalten keine Geheimnisse und werden über Manifest, Prüfsumme,
Quelle und Zeitpunkt registriert. Training darf Rückkopplungsdaten nur als
Kopie lesen und schreibt niemals in operative Zustände zurück.

Ein Modellpaket gelangt nicht direkt aus MLflow in eine aktive Umgebung. Es
wird zuerst in einem Import-/Quarantänebereich geprüft, registriert, durch
.NET validiert und manuell für Zielumgebung und Modus freigegeben.

Die Übertragung erfolgt gemäß ADR-034 manuell. Die Zielplattform scannt ihr
eigenes Modellverzeichnis; erst ein vollständig geprüftes Paket kann in der UI
bewusst zur Erzeugung einer Strategy Instance gewählt werden.

Produktion erhält später keine direkte Promotion aus Training. Das identische
Paket muss zuvor den beschlossenen Test-, Shadow- und Paper-Weg durchlaufen
haben.

### Ausfall der Trainingsumgebung

Ausfall von Python, MLflow, Dataset Builder, Training Orchestrator oder
Databento-Import:

- stoppt beziehungsweise markiert nur betroffene Forschungsaufträge;
- verhindert neue Modellpaketexporte;
- beeinflusst Test, Paper oder später Produktion nicht;
- verändert kein bereits freigegebenes ONNX-Modell.

Umgekehrt darf ein Fehler in Test oder Produktion keine Schreib- oder
Steueraktion in der Trainingsumgebung auslösen. Exporte erfolgen kontrolliert
und entkoppelt.

## Begründung

Gestufte Fehlerbereiche halten gesunde Strategien und Märkte verfügbar, ohne
gemeinsame Konto- oder Plattformrisiken zu ignorieren. Eine Handelssperre bei
weiterlaufender Überwachung ist sicherer als ein sofortiger Prozessabbruch.

Die harte Trennung der experimentellen Python-Zone verhindert, dass
Forschungscode, MLflow-Ausfall, Zugangsdaten oder ein Trainingsfehler in den
Handelspfad gelangen. Unveränderliche Einweg-Artefakte erhalten dennoch den
notwendigen Lern- und Promotionsfluss.

## Folgen

- Health-Zustände und Fehlergründe werden je Strategy, Instrument, Konto,
  Plattform und Umgebung geführt.
- Ein Supervisor koordiniert begrenzte Strategy-Neustarts.
- Gemeinsame Module müssen offene Trades unabhängig von der verursachenden
  Strategy Instance verwalten können.
- Deployment, Netzwerk, Datenbanken, Secrets und Speicherwurzeln erzwingen die
  Trennung der drei Zonen.
- Import-/Exportgrenzen benötigen Manifeste, Prüfsummen, Quarantäne und Audit.
- Konkrete Fristen, Retryzahlen und Health-Checks werden pro Komponente im
  Implementierungsdesign getestet.
