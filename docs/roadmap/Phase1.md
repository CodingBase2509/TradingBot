# Phase 1 – Deterministischer Plattformkern

## Ziel

Beweisen, dass Daten, Simulation, Risiko und Orderzustände korrekt funktionieren. Profit ist noch kein Abnahmekriterium.

## Umfang

- MES und einen Datenanbieter als erstes Profil über den allgemeinen
  Instrumentvertrag anbinden, ohne Symbolkonstanten in Risiko-, Order- oder
  Positionslogik;
- Rohdaten speichern und prüfen;
- gemeinsamen Ausführungskern entwickeln;
- eventbasierten Backtest mit Kosten aufbauen;
- einfache feste Testlogik verwenden, bevor ein lernendes Modell integriert wird;
- Paper-Broker und Risk Guard integrieren;
- die neun Modulgrenzen und Trade-/Order-/Positionszustände implementieren;
- die Module als eigene Projekte eines gemeinsam ausgelieferten modularen
  Monolithen durch Projektverweise und bewusste öffentliche Schnittstellen
  trennen;
- typisierte und versionierte Platform-, Account-, Instrument-, Candidate-
  und Strategy-Konfigurationen mit eigenen Tabellen umsetzen;
- StrategyInstanceId, gemeinsame Kontosicht und Execution Router im
  deterministischen Kern vorsehen;
- Entscheidungen, Orders und Ausführungen vollständig aufzeichnen;
- minimales Angular-Dashboard bereitstellen.

## Abnahme

Die vollständige fachliche Abnahme folgt den
[V1-End-to-End-Abnahmeszenarien](./V1AcceptanceScenarios.md). Dazu gehören
insbesondere:

- wiederholbare historische Läufe;
- nachvollziehbare Orderzustände;
- getestete Teilfüllungs-, Schutz-, Stornierungs- und Neustartübergänge;
- getestete Risiko- und Ausfallregeln;
- Abgleich interner und simulierter Brokerpositionen;
- sichtbare Datenqualität und Warnungen;
- maximal drei gleichgerichtete offene Paper-Trades und keine Position außerhalb
  der beschlossenen Handelsgrenzen.
- Tests des instrumentneutralen Plattformkerns gegen MES-Sonderfälle.
