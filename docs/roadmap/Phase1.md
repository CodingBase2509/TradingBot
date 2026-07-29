# Phase 1 – Deterministischer Plattformkern

## Ziel

Beweisen, dass Daten, Simulation, Risiko und Orderzustände korrekt funktionieren. Profit ist noch kein Abnahmekriterium.

## Umfang

- MES und einen Datenanbieter anbinden;
- Rohdaten speichern und prüfen;
- gemeinsamen Ausführungskern entwickeln;
- eventbasierten Backtest mit Kosten aufbauen;
- einfache feste Testlogik verwenden, bevor ein lernendes Modell integriert wird;
- Paper-Broker und Risk Guard integrieren;
- die neun Modulgrenzen und Trade-/Order-/Positionszustände implementieren;
- die Module zunächst in einem schlanken ausführbaren Plattformprojekt durch
  Namespaces und Architekturtests trennen;
- StrategyInstanceId, gemeinsame Kontosicht und Execution Router im
  deterministischen Kern vorsehen;
- Entscheidungen, Orders und Ausführungen vollständig aufzeichnen;
- minimales Angular-Dashboard bereitstellen.

## Abnahme

- wiederholbare historische Läufe;
- nachvollziehbare Orderzustände;
- getestete Teilfüllungs-, Schutz-, Stornierungs- und Neustartübergänge;
- getestete Risiko- und Ausfallregeln;
- Abgleich interner und simulierter Brokerpositionen;
- sichtbare Datenqualität und Warnungen;
- maximal drei gleichgerichtete offene Paper-Trades und keine Position außerhalb
  der beschlossenen Handelsgrenzen.
