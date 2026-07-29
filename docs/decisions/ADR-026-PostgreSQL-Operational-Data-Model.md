# ADR-026: Operatives PostgreSQL-Datenmodell

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Die beschlossenen Strategy Instances, Risikoreservierungen, Trade- und
Orderzustände, Modellfreigaben sowie Auditvorgänge benötigen ein konsistentes
operatives Datenmodell. Große Markt-, Feature- und Backtestdaten bleiben
außerhalb von PostgreSQL.

IDs und Zustände müssen in C# stark typisiert, sicher persistiert und auch bei
parallelen Abläufen eindeutig sowie nachvollziehbar sein.

## Entscheidung

### Fachliche PostgreSQL-Schemas

Die operative Datenbank wird logisch gegliedert:

```text
market
strategy
model
risk
trading
execution
operations
```

Alle Bereiche liegen für V1 in derselben PostgreSQL-Instanz der jeweiligen
Umgebung. Test und spätere Produktion verwenden vollständig getrennte
Datenbanken.

Jedes Modul schreibt ausschließlich in sein eigenes Schema. Andere Module
greifen über versionierte Verträge und Ereignisse zu, nicht über fremde
Tabellen. Innerhalb eines Moduls werden normale Fremdschlüssel und
Konsistenzregeln verwendet. Modulübergreifend werden stabile fachliche IDs
übergeben.

### IDs mit UUID Version 7

Fachliche und technische Identitäten werden in C# als `Guid` mit UUID Version 7
erzeugt und in PostgreSQL als `uuid` gespeichert.

Dies gilt insbesondere für:

- Strategy Instances und Konfigurationen;
- Decisions, Trades und Position Allocations;
- Risikoreservierungen und Sperren;
- Orders, Ausführungen und Reconciliation-Läufe;
- Modellpakete, Freigaben, Alerts und Auditereignisse;
- Correlation-, Causation- und Idempotenz-IDs.

Neue IDs werden in der Anwendung erzeugt, bevor ein Objekt persistiert oder
eine externe Aktion ausgelöst wird. Zufällige UUID-v4-Werte werden nicht als
Standard für neue fachliche Datensätze verwendet. Extern vorgegebene
Broker-IDs bleiben zusätzliche Felder und ersetzen die interne ID nicht.

UUID Version 7 verbessert zeitliche Sortierbarkeit und Indexlokalität. Die
tatsächliche Ereignisreihenfolge wird trotzdem durch Zeitstempel,
Sequenznummern und Brokerereignisse bestimmt, nicht allein durch die UUID.

### Zustände und Ereignistypen als C#-Enums

Zustände, Modi, Richtungen, Rollen, Sperrgründe und Ereignistypen werden im
Fachcode als ausdrücklich definierte C#-Enums dargestellt, beispielsweise:

- `ExecutionMode`;
- `TradeStatus`;
- `OrderStatus`;
- `ReservationStatus`;
- `Direction`;
- `OrderRole`;
- `ExitReason`;
- `RiskLockType`;
- `TradeEventType`;
- `OrderEventType`;
- `AlertSeverity`.

Jeder Enumwert erhält einen expliziten stabilen Code. Werte dürfen niemals
umgeordnet, umgedeutet oder für eine neue Bedeutung wiederverwendet werden.
Entfernte Werte bleiben reserviert.

Die Datenbank speichert die stabilen Codes als `smallint` oder bei fachlich
notwendigem größerem Wertebereich als `integer`. Check Constraints begrenzen
zulässige Werte. PostgreSQL-native Enums werden nicht verwendet, um
Schemaänderungen und Rückwärtskompatibilität kontrolliert über normale
Migrationen zu verwalten.

`Unknown = 0` darf für noch nicht klassifizierte externe Eingaben vorgesehen
werden, gilt im sicherheitskritischen Entscheidungs- und Ausführungspfad aber
nicht als zulässiger Fachzustand. Ein unbekannter oder nicht unterstützter Code
führt zu Fehler, Alarm und gegebenenfalls Handelssperre statt zu einem stillen
Fallback.

Externe IBKR-Zustände werden explizit auf interne Enums abgebildet. Der
ursprüngliche Anbieterwert beziehungsweise relevante Rohinhalt bleibt für
Diagnose und Audit erhalten.

### Zeit, Mengen und Zahlen

- Zeitpunkte werden als `timestamptz` gespeichert und technisch in UTC
  verarbeitet.
- Fachlicher Handelstag und Kalenderbezug werden zusätzlich ausdrücklich
  gespeichert.
- Vertragsmengen sind Ganzzahlen.
- Geld, Risiko und Prozente verwenden feste Dezimaltypen, keine binären
  Fließkommazahlen.
- Handelbare Preise werden bevorzugt als ganzzahlige Ticks gespeichert;
  Tickgröße und abgeleiteter Dezimalpreis sind nachvollziehbar.
- Veränderliche Projektionen erhalten eine Versionsnummer für konkurrierende
  Aktualisierungen.
- Flexible Zusatzinformationen dürfen `jsonb` verwenden, ersetzen aber keine
  sicherheitskritischen eigenen Spalten.

### Schema `market`

Mindestens:

| Tabelle | Zweck |
|---|---|
| `instrument` | Markt, Börse, Währung, Tickgröße und Tickwert |
| `contract` | konkrete Futures-Kontrakte und Broker-/Anbieterkennung |
| `calendar_version` | registrierte Kalenderquelle, Version und Prüfsumme |
| `session` | Öffnung, Wartung, Schluss und verkürzte Handelstage |
| `data_quality_state` | aktueller operativer Qualitätszustand |

Große Markt- und Qualitätsreihen liegen in Parquet.

### Schema `strategy`

Mindestens:

| Tabelle | Zweck |
|---|---|
| `instance` | stabile Strategy-Identität |
| `config_version` | unveränderliche Markt-, Modell-, Modus- und Risikokonfiguration |
| `activation` | aktive Konfiguration je Umgebung und Modus |
| `decision` | NoTrade, Long-/Short-Absicht oder Blockierung |
| `decision_summary` | Kandidatenanzahl, Ablehnungsgründe und Laufzeit |

Nicht jeder verworfene Kandidat wird dauerhaft relational gespeichert.
Vollständige Kandidaten liegen nur temporär oder bei begründetem
Diagnose-/Freigabebedarf in Parquet.

### Schema `model`

Mindestens:

| Tabelle | Zweck |
|---|---|
| `package` | Paket-ID, URI, Prüfsumme, MLflow- und Dataset-Herkunft |
| `validation` | technische, historische, Shadow- und Paper-Prüfungen |
| `approval` | manuelle Freigabe je Zielumgebung und Modus |

ONNX und große Modellartefakte liegen in der Datei-/Artefaktablage.

### Schema `risk`

Mindestens:

| Tabelle | Zweck |
|---|---|
| `profile` | restriktive Strategiegrenzen |
| `account_state` | aktuelle Konto-, Verlust- und Risikoprojektion |
| `reservation` | atomare Risiko-, Slot-, Token- und Richtungsreservierung |
| `lock` | globale, Konto-, Instrument- oder Strategy-Sperre |
| `daily_counter` | Trades, Verlustserie und technische Fehler |

`account_state` besitzt eine Versionsnummer. Reservierung und Aktualisierung
des Kontozustands erfolgen in einer Transaktion beziehungsweise durch einen
vergleichbar strengen Serialisierungsmechanismus des Account Risk Coordinators.

### Schema `trading`

Mindestens:

| Tabelle | Zweck |
|---|---|
| `trade` | aktuelle Projektion des logischen Trades |
| `trade_event` | unveränderliche fachliche Zustandsereignisse |
| `position_allocation` | Zuordnung von Strategy Trade zu Broker-Nettoposition |

`trade_event` enthält Trade-ID, Ereignistyp, Sequenznummer, Auftretens- und
Speicherzeit, Correlation-/Causation-ID und versionierten Ereignisinhalt.

Die Plattform verwendet damit aktuelle Projektionen plus append-only Ereignisse,
aber kein vollständiges Event Sourcing für jedes Modul.

### Schema `execution`

Mindestens:

| Tabelle | Zweck |
|---|---|
| `order` | aktuelle interne Broker-/Simulationsorder |
| `order_event` | unveränderliche Broker- und Orderereignisse |
| `fill` | bestätigte Teil- und Vollausführungen |
| `broker_position` | zuletzt bestätigte Brokersicht |
| `reconciliation_run` | durchgeführter Abgleich |
| `reconciliation_issue` | gefundene und gelöste Abweichung |
| `outbox` | dauerhaft registrierte, noch zu sendende Brokeroperation |

Interne Order-ID, stabiler Idempotenzschlüssel und externe Brokerorder-ID sind
getrennte Felder. Broker-Ausführungs- und Ereigniskennungen besitzen
Eindeutigkeitsregeln gegen doppelte Verarbeitung.

Order und zugehöriger Outbox-Eintrag werden gemeinsam persistiert. Ein Worker
sendet die Operation anschließend. Bei unklarer Antwort erfolgt zuerst
Reconciliation und kein blindes Neusenden.

### Schema `operations`

Mindestens:

| Tabelle | Zweck |
|---|---|
| `system_state` | aktueller Betriebs-, Handels- und Shutdown-Zustand |
| `alert` | Warnungen, Bestätigung und Lösung |
| `audit_event` | append-only Bedien- und Sicherheitsjournal |
| `approval` | bestätigungspflichtige Aktionen |
| `idempotency` | Schutz vor doppelten API- und Bedienaktionen |

Audit erfasst insbesondere Modell- und Konfigurationsfreigaben, Sperren,
Systemstopp, Full-Stop, Wiederaufnahme, Rollback und kritische manuelle
Aktionen.

### Persistenzregeln

- Eine aktuelle Projektion darf aktualisiert werden; das zugehörige
  Fachereignis bleibt unveränderlich.
- Ereignisse besitzen eine eindeutige Sequenz je Aggregate beziehungsweise
  fachlichem Objekt.
- Wiederholte Nachrichten und Brokerereignisse werden idempotent verarbeitet.
- Kritische Zustandsänderung und zugehöriges ausgehendes Ereignis werden
  atomar gespeichert.
- Unmögliche Enumwerte, Sequenzsprünge oder Versionskonflikte führen nicht zu
  automatischer Korrektur im Handelsweg.
- Migrationen werden mit Git versioniert und vor Anwendung in Test geprüft.

### Nicht in PostgreSQL

Nicht relational gespeichert werden:

- TBBO-, MBP-1- und vollständige historische Kerzenbestände;
- große Feature-, Kandidaten- und Labeltabellen;
- große Backtest- und Equity-Zeitreihen;
- ONNX-Dateien und vollständige Modellpakete;
- große MLflow-, Log- und Diagnoseartefakte.

PostgreSQL speichert deren IDs, URIs, Status, Prüfsummen, Größe und Beziehungen.

### Partitionierung

V1 startet ohne vorzeitige komplexe Tabellenpartitionierung. Nach Messung
können insbesondere `strategy.decision`, `trade_event`, `order_event`, `fill`
und `audit_event` zeitlich partitioniert werden.

Operative Projektionen bleiben klein. Analytische Exporte dürfen nach Parquet
erfolgen, ohne die beschlossene relationale Audit- und Aufbewahrungspflicht zu
umgehen.

### Umsetzungsreihenfolge

Phase 1 benötigt zuerst:

- Instrument, Vertrag und Sitzungen;
- Strategy Instance und Konfigurationsversion;
- Decision;
- Account State, Reservation und Risk Lock;
- Trade, Trade Event und Position Allocation;
- Order, Order Event, Fill und Outbox;
- Reconciliation Run und Issue;
- System State, Alert und Audit Event.

Erweiterte Modellvalidierungs-, Freigabe- und Bedienabläufe folgen mit den
entsprechenden Roadmap-Phasen.

## Begründung

UUID Version 7 verbindet globale Eindeutigkeit mit besserer zeitlicher
Sortierbarkeit. C#-Enums verhindern ungültige Zustände im Fachcode; explizite
Codes und Datenbank-Constraints sichern ihre Persistenz.

Getrennte Schemas spiegeln die Modulgrenzen wider. Aktuelle Projektionen
ermöglichen schnelle operative Abfragen, während append-only Ereignisse
Nachvollziehbarkeit bieten. Große analytische Daten verbleiben in Parquet und
belasten die operative Datenbank nicht.

## Folgen

- C#-Enumwerte erhalten explizite Codes und Kompatibilitätstests.
- EF-Core-Konfigurationen prüfen UUID-v7-Erzeugung, Enum-Konvertierung,
  Dezimalpräzision, Tickdarstellung und Versionsspalten.
- Architekturtests verhindern direkte Persistenzzugriffe auf fremde
  Modulschemas.
- Konkrete Spalten, Indizes und Constraints werden bei Implementierung pro
  Phase als Git-versionierte Migrationen ergänzt.
- Reale Datenmengen bestimmen, ob und wann Tabellenpartitionierung notwendig
  wird.
