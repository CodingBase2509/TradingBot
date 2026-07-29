# ADR-031: Schlanke physische Struktur der .NET-Plattform

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

ADR-022 definiert neun fachliche Modulgrenzen. Diese Grenzen sind für
Sicherheit, Testbarkeit und Nachvollziehbarkeit notwendig, rechtfertigen aber
nicht automatisch neun Dienste, neun Deployments oder eine große
Clean-Architecture-Projektlandschaft.

Wie die Python-Umgebung soll auch die .NET-Plattform nur die technisch
notwendige Komplexität enthalten.

## Entscheidung

### Ein ausführbarer modularer Monolith

V1 startet als:

```text
eine .NET-Solution
+ ein ausführbarer Plattform-Host
+ ein kompaktes Testprojekt
+ ein Angular-Projekt
+ sprachneutrale Verträge
```

Die neun fachlichen Module laufen im selben .NET-Prozess. Sie werden durch
Ordner, Namespaces, interne Sichtbarkeit, Verträge und Architekturtests
getrennt, nicht durch Netzwerkaufrufe.

### Startstruktur

```text
dotnet/
├── TradingPlatform.sln
├── src/
│   └── TradingPlatform/
│       ├── Modules/
│       │   ├── Market/
│       │   ├── Intelligence/
│       │   ├── Decision/
│       │   ├── Risk/
│       │   ├── Trading/
│       │   ├── Execution/
│       │   ├── Reconciliation/
│       │   ├── Models/
│       │   └── Operations/
│       ├── Infrastructure/
│       ├── Api/
│       └── Program.cs
└── tests/
    └── TradingPlatform.Tests/
        ├── Unit/
        ├── Integration/
        ├── Architecture/
        └── Golden/
```

Die Namen spiegeln ADR-022 wider, ohne jede fachliche Unterkomponente in ein
eigenes Projekt zu verwandeln.

Angular und Python bleiben getrennte Build- und Laufzeitbereiche. Sie werden
nicht in das .NET-Projekt eingebettet.

### Aufbau eines Moduls

Ein Modul darf zunächst so klein sein wie:

```text
Modules/Risk/
├── Contracts/
├── RiskGuard.cs
├── AccountRiskCoordinator.cs
├── Entities/
└── Persistence/
```

Unterordner wie `Domain`, `Application`, `Commands`, `Queries`, `Handlers`,
`Services`, `Repositories` oder `Factories` werden nicht pauschal erzeugt.
Sie entstehen nur, wenn reale Inhalte und Verantwortlichkeiten sie
rechtfertigen.

### Host und Betriebsarten

Der gemeinsame Host enthält:

- REST und SignalR;
- Modulregistrierung;
- kontrollierte Hintergrundabläufe;
- Market-Data- und Execution-Adapter;
- Reconciliation und Health Checks;
- Backtest- beziehungsweise Simulationsstart.

Backtest, Shadow und Paper verwenden denselben fachlichen Kern. Sie benötigen
keine getrennten Anwendungen; Konfiguration sowie Zeit-, Daten- und
Execution-Adapter bestimmen den Modus.

Produktion wird später separat konfiguriert und ausgeliefert. Ein
Laufzeitschalter von Paper zu Live bleibt unzulässig.

### Infrastruktur

`Infrastructure` enthält nur konkrete Adapter:

- PostgreSQL/EF Core;
- ONNX Runtime;
- IBKR;
- Datei-/Artefaktzugriff;
- Systemzeit und externe Betriebsintegration.

Es wird kein allgemeines Broker-, Datenbank-, Storage- oder
Messaging-Framework gebaut. Austauschbarkeit entsteht durch kleine
Schnittstellen an tatsächlich benötigten Grenzen.

### Persistenz

V1 verwendet PostgreSQL und EF Core mit Git-versionierten Migrationen.
Fachliche Schemas und Eigentum aus ADR-026 bleiben bestehen.

Es werden nicht pauschal eingeführt:

- generisches Repository;
- Unit-of-Work-Abstraktion über EF Core;
- eigene ORM-Schicht;
- vollständiges Event Sourcing;
- getrennte Datenbank je Modul.

Module greifen trotzdem nicht direkt auf fremde Tabellen zu. Diese Regel wird
durch Codeorganisation, Reviews und Architekturtests geschützt. Eine
strengere physische Trennung folgt nur, wenn die einfache Grenzsicherung nicht
ausreicht.

### Kommunikation

Für unmittelbare fachliche Antworten werden direkte typisierte
Modulverträge verwendet. Bereits eingetretene Zustandsänderungen werden über
einen kleinen In-Process-Dispatcher verteilt und bei fachlicher Relevanz
persistiert.

V1 verwendet weder Kafka, RabbitMQ noch einen eigenen allgemeinen Service Bus.
Die Execution Outbox aus ADR-026 bleibt eine gezielte
Zuverlässigkeitskomponente für externe Brokeraktionen und kein universelles
Messaging-Framework.

### Keine pauschale Frameworkarchitektur

Nicht automatisch Bestandteil der V1 sind:

- Microservices;
- CQRS- oder Mediator-Framework;
- generisches Repository-/Unit-of-Work-Muster;
- vollständiges Event Sourcing;
- Plugin-Architektur;
- dynamische Modulbeladung;
- Kubernetes;
- externer Message Broker;
- mehrere API-Gateways;
- allgemeine Workflow Engine;
- verteilte Caches;
- eigene Observability-Plattform.

Eine kleine Bibliothek darf verwendet werden, wenn sie konkreten Code und
Fehlerfläche nachweislich reduziert. Sie wird nicht allein zur Einhaltung eines
Architekturmusters eingeführt.

### Tests

Das gemeinsame Testprojekt enthält:

- Unit Tests für fachliche Regeln und Zustandsübergänge;
- Integration Tests mit PostgreSQL, ONNX und simulierten Adaptern;
- Architecture Tests für Modulabhängigkeiten;
- Golden Tests für Python-/NET-Parität;
- deterministische End-to-End-Szenarien für Backtest und Paperkern.

Ein weiteres Testprojekt wird erst angelegt, wenn Laufzeit, Abhängigkeiten oder
Testisolation dies messbar erfordern.

### Regel für Wachstum

Ein Modul wird erst zu einem eigenen .NET-Projekt oder Dienst, wenn mindestens
ein realer Grund besteht:

- notwendige Sicherheits- oder Prozessisolation;
- unabhängiger Deployment- oder Versionszyklus;
- gemessenes Last- oder Skalierungsproblem;
- eigenständige Verfügbarkeitsanforderung;
- klare Team- oder Eigentumsgrenze;
- einfache Architekturtests können die benötigte Grenze nicht ausreichend
  sichern.

Ein weiterer Adapter oder eine gemeinsame Abstraktion entsteht grundsätzlich
erst bei einem realen zweiten Anwendungsfall oder einem klaren
Test-/Sicherheitsbedarf.

## Begründung

Ein einzelner Host reduziert Build-, Deployment-, Debugging- und
Betriebsaufwand. Fachliche Modulgrenzen bleiben trotzdem sichtbar und
testbar. Direkte typisierte Aufrufe sind für V1 einfacher und zuverlässiger als
interne Netzwerkkommunikation.

Der Verzicht auf pauschale Schichten und Frameworkmuster hält die Codebasis für
einen einzelnen Entwickler überschaubar. Spätere Trennung bleibt möglich, wenn
Messwerte oder Sicherheitsanforderungen sie begründen.

Auch die konkrete C#-Implementierung folgt den sprachübergreifenden
Einfachheits- und Vollständigkeitsregeln aus ADR-032.

## Folgen

- ADR-022 beschreibt fachliche Grenzen; ADR-031 beschreibt ihre schlanke
  physische Umsetzung.
- Die erste Solution enthält möglichst wenige Projekte.
- Neue Schichten, Projekte, Frameworks und Dienste benötigen eine dokumentierte
  Begründung.
- Architekturtests schützen die wichtigsten Abhängigkeits- und
  Persistenzgrenzen.
- Die konkrete .NET-, EF-Core- und Testbibliotheksauswahl erfolgt minimal und
  versioniert vor Implementierungsbeginn.
