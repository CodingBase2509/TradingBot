# ADR-023: Mehrere Strategy Instances auf einer Plattform

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

In Test sollen mehrere Modelle gleichzeitig mit denselben Live-Marktdaten im
Shadow- und Paper-Verfahren verglichen werden. Später sollen mehrere
markt- oder strategiespezifische Modelle kontrolliert über dasselbe
Brokerkonto handeln können.

Mehrere vollständige Plattformprozesse mit unabhängiger Kontosicht würden
Risikolimits, Brokerzustand und Not-Aus vervielfachen. Mehrere Strategien auf
demselben Brokerkonto können sich außerdem gegenseitig beeinflussen, weil der
Broker pro Instrument eine Nettoposition führt.

## Entscheidung

### Strategy Instance

Eine Plattform kann mehrere eindeutig identifizierte `Strategy Instances`
gleichzeitig betreiben. Eine Instanz verbindet eine versionierte Konfiguration
aus mindestens:

- Markt, Instrument und zulässigen Verträgen;
- Marktdatenquelle und Zeitrahmen;
- Feature- und Candidate-Generator-Konfiguration;
- freigegebenem Modellpaket;
- Entscheidungsschwelle;
- Ausführungsmodus;
- strategiebezogenem Risikoprofil;
- aktiviertem Konfigurationsstand.

Jede Instanz besitzt eigene Feature-Zustände, Modellruntime, Kandidaten,
Entscheidungen, logische Trades, Performancekennzahlen,
Haltedauerschätzungen und Auditzuordnung.

Eine Änderung an Modell, Markt, Zeitrahmen, Schwelle, Ausführungsmodus oder
Risikoprofil erzeugt eine neue Konfigurationsversion. Der Wechsel in einen
höheren Ausführungsmodus benötigt eine gesonderte Freigabe.

### Ausführungsmodi

| Modus | Daten | Brokerorder | Zweck |
|---|---|---|---|
| Backtest | historisch | simuliert | historischer Test |
| Shadow | live | keine | parallele Beobachtung |
| Simulated Paper | live | intern simuliert | unabhängiger Modellvergleich |
| Broker Paper | live | IBKR-Paper | Brokerintegration und realitätsnaher Test |
| Live | live | IBKR-Live | späterer Echtgeldbetrieb |

Shadow und Simulated Paper dürfen mit vielen Strategy Instances parallel
laufen. Jede Simulated-Paper-Instanz besitzt ein eigenes virtuelles Ergebnis-
und Positionsbuch.

V1 begrenzt Broker Paper auf eine kontrollierte Ausführungsgruppe je
Instrument. Mehrere Broker-Paper- oder Live-Strategien auf demselben Instrument
benötigen zuvor eine geprüfte Netto- und Allokationslogik.

### Gemeinsame Plattformdienste

Die Plattform teilt:

- Marktdaten und Börsenkalender;
- Brokerverbindung und Account Snapshot;
- Account Risk Guard;
- Execution Router und Brokeradapter;
- Reconciliation;
- Operations, Audit und beide Not-Aus-Stufen;
- Model- und Konfigurationsverwaltung.

Feature & Intelligence sowie Decision laufen je Strategy Instance. Trade
Management besitzt strategiebezogene logische Zustände und eine gemeinsame
Sicht auf die Broker-Nettoposition.

### Zwei Risikoschichten

Jede ausführende Absicht durchläuft:

```text
Strategy Risk
→ Account/Portfolio Risk
→ Execution Router
```

Strategiebezogene Grenzen dürfen restriktiver sein. Die gemeinsamen
Kontogrenzen prüfen alle ausführenden Instanzen zusammen und können von keiner
Instanz überschrieben werden.

Die beschlossenen V1-Grenzen für Risiko, Tagesverlust, Parallelität,
Tradeanzahl und Sperren gelten auf Kontoebene. Eine Strategie darf zusätzlich
ein kleineres Budget erhalten. Tagesgewinn einer anderen Strategie erhöht die
globale Risikobasis nicht.

Gleichzeitige Freigaben werden durch die atomare Konto-Risikoreservierung aus
ADR-025 koordiniert.

Bei später mehreren Märkten kommen mindestens hinzu:

- aggregiertes Kontorisiko;
- Budget je Markt und Strategie;
- gleichzeitige Verlust- und Drawdownbegrenzung;
- Korrelations- und Konzentrationsgrenzen;
- Deckungs- und Marginprüfung.

Mehr Strategy Instances oder Märkte garantieren keinen höheren Gewinn. Ihre
Aufnahme muss einen nach Kosten robusten Vorteil oder einen messbaren
Diversifikationseffekt zeigen.

### Execution Router

Eine Strategy Instance erzeugt einen `ExecutionRequest`, besitzt aber keine
eigene unkoordinierte Brokerverbindung. Der gemeinsame Execution Router:

- erhält Strategy-, Trade- und Freigabe-ID;
- prüft die globale Risikofreigabe;
- vergibt eindeutige interne Order-IDs;
- ordnet Orders, Ausführungen und Kosten einer Instanz zu;
- berücksichtigt bestehende Brokerorders und Nettopositionen;
- verhindert widersprüchliche oder unzureichende Schutzorders;
- übergibt freigegebene Orders an den gemeinsamen Adapter.

### Positionsebenen

Die Plattform unterscheidet:

```text
Strategy Trade
→ Strategy Position Allocation
→ Broker Net Position
```

Reconciliation prüft, ob die Summe interner Allokationen mit Orders,
Ausführungen und Broker-Nettoposition übereinstimmt.

Gegenläufige ausführende Strategien auf demselben Instrument bleiben in V1
blockiert. Eine spätere Freigabe erfordert eine explizite Regel für interne
Verrechnung, Orderwirkung, Schutzorders, Kosten und eindeutige
Performancezuordnung.

### Vergleich und Promotion

Paralleltests verwenden möglichst dieselben Marktdaten, Kalenderstände, Kosten-
und Ausführungsannahmen. Verglichen werden nicht nur Gewinn, sondern mindestens:

- Profit Factor und Drawdown;
- Stabilität über Zeitfenster und Marktphasen;
- Kosten- und Slippageempfindlichkeit;
- Tradezahl und Fehlerverhalten;
- Haltedauerschätzung;
- zusätzlicher Nutzen und Korrelation im Gesamtportfolio.

Ein Ergebnis aktiviert keine Instanz automatisch. Modellpaket,
Instanzkonfiguration, Zielumgebung und Ausführungsmodus werden getrennt
freigegeben und auditiert.

## Begründung

Strategy Instances ermöglichen faire parallele Forschung und später mehrere
marktbezogene Handelsansätze, ohne Kontosicherheit und Brokerzustand zu
duplizieren. Gemeinsame Account-Grenzen verhindern, dass einzeln zulässige
Strategien zusammen ein unzulässiges Risiko erzeugen.

Simulated Paper isoliert Modellperformance. Die Begrenzung von Broker Paper je
Instrument verhindert in V1, dass Nettopositionen unabhängige Modellresultate
verfälschen.

## Folgen

- Konfiguration, Zustand, Entscheidung, Trade und Audit erhalten eine
  `StrategyInstanceId`.
- Risk Guard wird in Strategy Risk und Account/Portfolio Risk gegliedert.
- Execution erhält einen gemeinsamen Router vor den Brokeradaptern.
- Reconciliation verwaltet Strategieallokationen und Broker-Nettoposition
  getrennt.
- Dashboard und API benötigen Filter und Gesamtansichten je Instanz, Modus,
  Markt und Konto.
- Korrelations- und echtes marktübergreifendes Portfoliorisiko bleiben
  Schwerpunkt von Phase 4.
