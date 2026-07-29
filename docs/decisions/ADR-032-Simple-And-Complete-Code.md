# ADR-032: Einfacher, verständlicher und vollständiger Code

- **Status:** beschlossen
- **Datum:** 29. Juli 2026

## Kontext

Die Plattform enthält notwendige fachliche Komplexität durch Trading,
Risikoregeln, Brokerzustände, Datenqualität und Machine Learning. Diese
Komplexität darf nicht durch unnötige Schichten, Muster, Abstraktionen oder
clevere Kurzformen vergrößert werden.

Gleichzeitig darf „einfacher Code“ nicht als Begründung dienen, benötigte
Sicherheitsregeln, Fehlerfälle, Auditdaten oder Tests wegzulassen.

## Entscheidung

### Leitregel

Für Python und C# gilt:

> Implementiere die kleinste verständliche Lösung, die die benötigte
> Funktionalität, Sicherheit, Fehlerbehandlung und Nachvollziehbarkeit
> vollständig erfüllt.

Lesbarkeit, explizites Verhalten und einfache Änderung haben Vorrang vor
technischer Cleverness oder maximaler Wiederverwendung.

### Verständlichkeit

- Namen beschreiben fachliche Bedeutung statt technische Abkürzungen.
- Kontrollfluss bleibt möglichst linear und lokal nachvollziehbar.
- Sicherheitsrelevante Regeln werden ausdrücklich dargestellt und nicht in
  generischen Helfern versteckt.
- Funktionen und Klassen besitzen eine zusammenhängende Verantwortung.
- Methoden werden geteilt, wenn dies Verständnis oder Testbarkeit verbessert,
  nicht um künstlich eine geringe Zeilenzahl zu erreichen.
- Kommentare erklären Gründe, Annahmen und nicht offensichtliche
  Sicherheitsentscheidungen; sie wiederholen nicht den Code.
- Einfache Datentransformationen bleiben einfache Datentransformationen.

### Keine vorschnelle Abstraktion

Nicht pauschal eingeführt werden:

- Interface für jede Klasse;
- Basisklasse für jeden gemeinsamen Methodennamen;
- generische Repository-, Service-, Manager-, Factory- oder Helper-Schichten;
- universelle Pipelines für einen einzelnen realen Ablauf;
- Reflection, dynamische Modulbeladung oder Metaprogrammierung;
- eigene Frameworks um verwendete Bibliotheken;
- Konfigurierbarkeit ohne fachlichen Anwendungsfall;
- Wiederverwendung um jeden Preis.

Eine Abstraktion entsteht, wenn ein realer zweiter Anwendungsfall,
Test-/Sicherheitsbedarf oder nachgewiesene wiederkehrende Änderung vorliegt.

Kleine, offensichtliche Duplikation darf vorübergehend verständlicher sein als
eine falsche gemeinsame Abstraktion. Sicherheits- und Berechnungslogik darf
aber nicht unkontrolliert auseinanderlaufen; dafür werden gemeinsame Verträge
und Golden Tests verwendet.

### Komplexität sichtbar halten

Notwendige Komplexität wird in fachlich benannten Typen und Abläufen
ausgedrückt:

```text
TradeIntent
→ RiskReservation
→ Order
→ Fill
→ ProtectedPosition
```

Sie wird nicht hinter allgemeinen Begriffen wie `Processor`, `Engine`,
`Context`, `Manager` oder verschachtelten Callback-/Handlerketten verborgen,
wenn ein konkreter fachlicher Name möglich ist.

Verschachtelung wird durch frühe Validierung und klare Rückgaben begrenzt.
Fehler werden nicht als normale Kontrollflussabkürzung missbraucht und nicht
stillschweigend verschluckt.

### C#

Für C# gilt zusätzlich:

- konkrete Klassen werden verwendet, solange keine echte austauschbare Grenze
  oder Testisolation ein Interface benötigt;
- Komposition wird tiefer Vererbung vorgezogen;
- `record`, Value Objects und Enums werden genutzt, wenn sie fachliche
  Unveränderlichkeit oder erlaubte Zustände klarer machen;
- `async` wird für echte asynchrone I/O- und Wartevorgänge verwendet, nicht
  pauschal für jede Methode;
- LINQ wird verwendet, wenn es lesbarer ist als eine Schleife, nicht für
  schwer nachvollziehbare verschachtelte Abfragen;
- EF Core wird direkt und gezielt eingesetzt, ohne generische
  Repository-/Unit-of-Work-Hülle;
- UUID Version 7, stabile Enums, Dezimal- und Tickdarstellung folgen ADR-026;
- keine Mediator-, Result-, Mapping- oder Validation-Schicht für jede einzelne
  Operation ohne nachgewiesenen Nutzen.

### Python

Für Python gilt zusätzlich:

- öffentliche und fachlich wichtige Funktionen erhalten Typangaben;
- einfache Funktionen und Datenklassen werden komplexen Klassenhierarchien
  vorgezogen;
- Dataframes werden für tabellarische Verarbeitung genutzt, aber fachliche
  Regeln erhalten benannte Funktionen und Tests statt schwer lesbarer
  Methodenkaskaden;
- keine dynamischen Monkeypatch-, Metaklassen- oder Magie-Lösungen im
  offiziellen Daten- und Trainingsweg;
- Notebooks enthalten keine alleinige offizielle Logik;
- Vektorisierung wird verwendet, wenn sie korrekt, messbar schneller und noch
  verständlich ist; unleserliche Optimierung benötigt einen gemessenen Grund;
- Konfiguration und Datenverträge werden früh validiert und nicht als
  unstrukturierte Dictionaries durch die gesamte Anwendung gereicht.

### Fehlerbehandlung

- Erwartbare Ablehnungen verwenden klare fachliche Resultate und Reason Codes.
- Unerwartete technische Fehler bleiben als Fehler sichtbar und werden
  protokolliert.
- Ein Fehler wird nur dort abgefangen, wo er sinnvoll behandelt, ergänzt oder
  in eine definierte Sperre übersetzt werden kann.
- Leere `catch`-/`except`-Blöcke und stilles Ersetzen ungültiger Werte sind
  unzulässig.
- Im Zweifel wird der kleinstmögliche sichere Bereich blockiert gemäß ADR-027.

### Tests

- Tests beschreiben beobachtbares fachliches Verhalten.
- Nicht jede private Methode benötigt einen eigenen Test.
- Tests dürfen Implementierungsdetails nicht so stark fixieren, dass einfache
  interne Verbesserungen unnötig schwer werden.
- Sicherheitsregeln, Zustandsübergänge, Grenzwerte und Fehlerfälle werden
  ausdrücklich getestet.
- Golden Tests sichern kritische Python-/NET-Parität.
- Ein Test-Helfer wird erst abstrahiert, wenn Wiederholung die Lesbarkeit
  tatsächlich verschlechtert.

### Änderungen und Reviews

Bei jeder Umsetzung wird geprüft:

1. Ist die Funktion fachlich vollständig?
2. Ist die einfachste sichere Lösung gewählt?
3. Kann ein neuer Leser den Hauptablauf ohne unnötige Sprünge verstehen?
4. Ist jede Abstraktion oder Bibliothek konkret begründet?
5. Sind Fehler- und Grenzfälle sichtbar?
6. Kann Code, Konfiguration oder Persistenz entfallen, ohne Funktion zu
   verlieren?
7. Sind Tests proportional zum Risiko?

Wenn zwei Lösungen fachlich gleichwertig sind, wird die mit weniger Code,
weniger Zuständen, weniger Abhängigkeiten und einfacherem Kontrollfluss
bevorzugt.

### Vereinfachung ohne Funktionsverlust

Nicht entfernt oder abgeschwächt werden dürfen:

- feste Risikogrenzen;
- brokerseitiger TP-/SL-Schutz;
- Reconciliation und Idempotenz;
- Audit und wichtige Herkunftsinformationen;
- Datenqualitäts- und Leakage-Schutz;
- Isolation der Umgebungen;
- Modellkompatibilitäts- und Paritätstests;
- definierte Fehler- und Wiederherstellungswege.

Diese Funktionen sind notwendige fachliche Komplexität. Ihre Implementierung
soll klar sein, nicht fehlen.

## Begründung

Eine verständliche Codebasis reduziert Fehler, Einarbeitungszeit und
Wartungsaufwand. Das ist bei finanz- und sicherheitsrelevanten Abläufen
wichtiger als die Demonstration möglichst vieler Architekturmuster.

Die explizite Grenze gegen falsche Vereinfachung schützt davor, notwendige
Sicherheitsfunktionen im Namen geringer Codegröße auszulassen.

## Folgen

- Code Reviews bewerten Verständlichkeit und unnötige Komplexität ausdrücklich.
- Neue Patterns, Frameworks und allgemeine Abstraktionen benötigen einen
  konkreten dokumentierbaren Nutzen.
- Refactoring darf Code vereinfachen, solange Verträge, Verhalten und
  Sicherheit durch Tests erhalten bleiben.
- Python und C# folgen denselben Grundsätzen, auch wenn sprachtypische einfache
  Lösungen unterschiedlich aussehen.
