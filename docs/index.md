# Trading-KI-Plattform – Inhaltsverzeichnis

Vollständige Navigation durch die Projektdokumentation.

**Stand:** 3. August 2026
**Aktuelle Phase:** Phase 0 – Planung und fachliche Spezifikation  
**Implementierungsstatus:** Noch keine produktive Implementierung

Eine kurze Einführung in das Projekt bietet die
[Startseite der Dokumentation](./README.md).

## Aufbau und Verbindlichkeit

Die Fach-, Architektur-, Trading- und ML-Dokumente beschreiben den aktuellen
verbindlichen Planungsstand. Die Roadmap ordnet die Umsetzung zeitlich und nach
Ergebnissen. [Offene Entscheidungen](./05_OpenDecisions.md) kennzeichnet
Punkte, die noch nicht abschließend festgelegt wurden.

Architekturentscheidungen (ADRs) dokumentieren knapp, warum eine grundlegende
Entscheidung getroffen wurde und welche Folgen sie hatte. Die vollständige
aktuelle Regel steht im jeweils verlinkten Fachdokument. ADRs werden bei
späteren Änderungen nicht gelöscht oder neu nummeriert; ihr Status wird
angepasst oder eine neue ADR ergänzt.

Damit konkrete Regeln nicht widersprüchlich doppelt gepflegt werden, gilt:

```text
Fachdokumente = aktueller verbindlicher Stand
ADRs          = Entscheidungsverlauf und Begründung
Roadmap       = Reihenfolge und Abnahme der Umsetzung
```

## Grundlagen

1. [Projektvision](./00_ProjectVision.md)  
   Vision, Produktverständnis, Ziele, Rahmen und Nicht-Ziele.
2. [Projektgrundsätze](./01_ProjectPrinciples.md)  
   Verbindliche Leitlinien für Sicherheit, Entwicklung und Betrieb.
3. [Glossar](./02_Glossary.md)  
   Zentrale Begriffe aus Trading, Machine Learning und Plattformbetrieb.
4. [Funktionale Anforderungen](./03_FunctionalRequirements.md)  
   Anforderungen an Daten, Modelle, Handel, Risiko, Forschung und Dashboard.
5. [Nichtfunktionale Anforderungen](./04_NonFunctionalRequirements.md)  
   Anforderungen an Sicherheit, Zuverlässigkeit, Reproduzierbarkeit und Wartbarkeit.
6. [Offene Entscheidungen](./05_OpenDecisions.md)  
   Noch zu klärende fachliche, technische und betriebliche Fragen.

## Architektur

1. [Architekturübersicht](./architecture/Overview.md)  
   Logische Ebenen, Verantwortungsgrenzen und zentrale Datenflüsse.
2. [Komponenten](./architecture/Components.md)  
   Verantwortlichkeiten der .NET-, Python- und Angular-Komponenten.
3. [Kommunikation](./architecture/Communication.md)  
   Schnittstellen, Kommunikationswege und Modellvertrag.
4. [Speicher und Datenhaltung](./architecture/Storage.md)
   Speicherarten, Datenstufen, Git-Grenzen und Aufbewahrung.
5. [Deployment und Betrieb](./architecture/Deployment.md)
   Geplantes Startbild, Betriebsgrundsätze und Ausfallverhalten.
6. [Technologie-Baseline](./architecture/TechnologyStack.md)
   Beschlossene Plattformversionen, Versionsregeln und offene Bibliotheksauswahl.
7. [Konfigurationsvertrag](./architecture/Configuration.md)
   Typisierte Konfigurationen, Tabellen, Versionierung und Aktivierungsregeln.

## Trading

1. [Trading-Konzept](./trading/TradingConcept.md)  
   Handelsstil, Modellentscheidungen, Ablauf und V1-Vereinfachungen.
2. [Risiko- und Sicherheitskonzept](./trading/RiskManagement.md)  
   Risk Guard, Schutzebenen, Handelsstopps und beschlossene V1-Grenzwerte.
3. [Markt- und Datenkonzept](./trading/MarketData.md)  
   Zielmärkte, Datenbedarf, Datenqualität und Futures-Rollover.
4. [Orderausführung und Positionsverwaltung](./trading/Execution.md)  
   Orderübersetzung, Brokeradapter, Positionslebenszyklus und Abgleich.

## Machine Learning

1. [Training](./ml/Training.md)  
   Lernaufgabe, Trainingsablauf, zeitliche Trennung und Startmodelle.
2. [Feature Engineering](./ml/FeatureEngineering.md)  
   Modelleingaben, Zeithorizonte und Regeln für die Feature-Berechnung.
3. [Backtesting](./ml/Backtesting.md)  
   Historische Simulation, Kostenannahmen und Robustheitstests.
4. [Evaluation](./ml/Evaluation.md)  
   Bewertung nach Profitabilität, Risiko, Stabilität und Robustheit.
5. [Modelllebenszyklus](./ml/ModelLifecycle.md)  
   Zustände, Promotion, Champion/Challenger und Rollback.

## Roadmap

1. [Roadmap-Übersicht](./roadmap/Overview.md)  
   Alle geplanten Entwicklungsphasen und durchgängigen Arbeitsstränge.
2. [Phase 0 – Planung und fachliche Spezifikation](./roadmap/Phase0.md)  
   Entscheidungen und Spezifikationen vor Beginn der Implementierung.
3. [Phase 1 – Deterministischer Plattformkern](./roadmap/Phase1.md)  
   Daten, Backtest, Risiko, Paper Broker und minimales Dashboard.
4. [Phase 2 – Erstes lernendes Modell](./roadmap/Phase2.md)  
   Reproduzierbares Training, ONNX-Export und Shadow Mode.
5. [V1-End-to-End-Abnahmeszenarien](./roadmap/V1AcceptanceScenarios.md)
   Verbindliche fachliche Abläufe und Nachweise für den Plattformkern.

Die späteren Phasen 3 bis 7 sind derzeit in der
[Roadmap-Übersicht](./roadmap/Overview.md) beschrieben und besitzen noch keine
eigenen Dokumente.

## Architekturentscheidungen

1. [ADR-001 – Python und .NET kombinieren](./decisions/ADR-001-Python-And-DotNet.md)  
   **Beschlossen:** Trennung von ML-Forschung und Produktionskern.
2. [ADR-002 – ONNX als bevorzugtes Modellformat](./decisions/ADR-002-ONNX.md)  
   **Beschlossen mit Vorbehalt:** Modellausführung direkt in .NET.
3. [ADR-003 – Futures als Zielmärkte](./decisions/ADR-003-Futures-Target-Markets.md)  
   **Beschlossen:** Futures als Zielrichtung und MES als V1-Markt.
4. [ADR-004 – Interactive Brokers](./decisions/ADR-004-Interactive-Brokers.md)  
   **Beschlossen:** V1-Paper-Broker und bevorzugter späterer Echtgeldbroker.
5. [ADR-005 – Produktumfang der V1](./decisions/ADR-005-V1-Product-Scope.md)
   **Beschlossen:** MES, Paper Trading und begrenzter Funktionsumfang.
6. [ADR-006 – Handels- und Risikopolitik der V1](./decisions/ADR-006-V1-Trading-And-Risk-Policy.md)
   **Beschlossen:** Handelszeiten, Risikogrenzen, Sperren und Not-Aus.
7. [ADR-007 – Lernziel und historische Handelsalternativen](./decisions/ADR-007-V1-Learning-Objective.md)
   **Beschlossen:** Netto-`R`, adaptive Kandidaten und Mindest-Risk-to-Reward.
8. [ADR-008 – Marktdatenauflösung der V1](./decisions/ADR-008-V1-Market-Data-Resolution.md)
   **Beschlossen:** Ereignisnahe Rohdaten und kanonische Zeitsichten.
9. [ADR-009 – MES-Historie und Rollover](./decisions/ADR-009-MES-History-And-Rollover.md)
   **Beschlossen:** Historischer Zeitraum und volumenbasierter Vertragswechsel.
10. [ADR-010 – Datenqualität und Lückentoleranz](./decisions/ADR-010-V1-Data-Quality-And-Gaps.md)
    **Beschlossen:** Qualitätsstufen und sichere Behandlung fehlender Daten.
11. [ADR-011 – Kostenmodell im V1-Backtest](./decisions/ADR-011-V1-Backtest-Cost-Model.md)
    **Beschlossen:** Bid/Ask, Gebühren, Slippage und Ausführungsstress.
12. [ADR-012 – Modellevaluation und Promotion](./decisions/ADR-012-V1-Model-Evaluation-And-Promotion.md)
    **Beschlossen:** Historische, Shadow- und Paper-Mindestkriterien.
13. [ADR-013 – Databento für historische Daten](./decisions/ADR-013-Databento-Historical-Data.md)
    **Vorläufig beschlossen:** Anbieterwahl und gestufter Beschaffungsprozess.
14. [ADR-014 – Datenimport, Aufbewahrung und Backup](./decisions/ADR-014-Data-Import-Retention-And-Backup.md)
    **Beschlossen:** Manueller Import, Testumgebung und zwei getrennte Kopien.
15. [ADR-015 – Börsenkalender und Handelszeiten](./decisions/ADR-015-Exchange-Calendar.md)
    **Beschlossen:** CME als maßgebliche, versionierte Kalenderquelle.
16. [ADR-016 – Umgebungen und modularer Monolith](./decisions/ADR-016-Environments-And-Modular-Monolith.md)
    **Beschlossen:** Training, Test, Produktion und gemeinsamer Plattformkern.
17. [ADR-017 – Speicherarchitektur und Git-Versionierung](./decisions/ADR-017-Storage-And-Version-Control.md)
    **Beschlossen:** PostgreSQL, Parquet, Artefakte, Secrets und Git-Grenzen.
18. [ADR-018 – Modellpaket und Laufzeitvertrag](./decisions/ADR-018-Model-Package-And-Runtime-Contract.md)
    **Beschlossen:** Sichere Übergabe, Prüfung und Aktivierung von Python-Modellen in .NET.
19. [ADR-019 – MLflow für Experiment Tracking](./decisions/ADR-019-MLflow-For-Experiment-Tracking.md)
    **Beschlossen:** Forschungsverwaltung mit MLflow bei getrennter Plattformfreigabe.
20. [ADR-020 – Datenstufen und Aufbewahrung](./decisions/ADR-020-Data-Stages-And-Retention.md)
    **Beschlossen:** Originale, kanonische Daten, Trainingsstände und Artefakte getrennt verwalten.
21. [ADR-021 – Schlanke physische Speicherstruktur](./decisions/ADR-021-Lean-Physical-Storage-Layout.md)
    **Beschlossen:** Flache Verzeichnisse und bedarfsgerechte Speicherung reproduzierbarer Daten.
22. [ADR-022 – Modulgrenzen des .NET-Plattformkerns](./decisions/ADR-022-DotNet-Module-Boundaries.md)
    **Beschlossen:** Neun fachliche Module im gemeinsamen modularen Monolithen.
23. [ADR-023 – Mehrere Strategy Instances](./decisions/ADR-023-Multi-Strategy-Runtime.md)
    **Beschlossen:** Parallele Modelle mit gemeinsamen Konto-, Risiko- und Brokergrenzen.
24. [ADR-024 – Trade-, Order- und Positionszustände](./decisions/ADR-024-Trade-Order-And-Position-State-Machines.md)
    **Beschlossen:** Nachvollziehbare Zustandsmaschinen für Ausführung und Wiederherstellung.
25. [ADR-025 – Atomare Konto-Risikoreservierung](./decisions/ADR-025-Atomic-Account-Risk-Reservations.md)
    **Beschlossen:** Konsistente Reservierungen für parallele Strategy Instances.
26. [ADR-026 – Operatives PostgreSQL-Datenmodell](./decisions/ADR-026-PostgreSQL-Operational-Data-Model.md)
    **Beschlossen:** Fachschemas, UUID Version 7, C#-Enums, Projektionen und Ereignisse.
27. [ADR-027 – Fehlerisolierung und Trainingsgrenze](./decisions/ADR-027-Failure-Containment-And-Training-Isolation.md)
    **Beschlossen:** Gestufte Sperren und vollständig isolierte Python-Forschungsumgebung.
28. [ADR-028 – Adaptiver Candidate Generator](./decisions/ADR-028-Adaptive-Candidate-Generator.md)
    **Beschlossen:** Kausale Marktstrukturen, adaptive TP-/SL-Kandidaten und feste V1-Grenzen.
29. [ADR-029 – Historische Trainingslabels](./decisions/ADR-029-V1-Training-Labels.md)
    **Beschlossen:** Netto-R, Ausführungskosten, Haltedauer und Censoring der V1.
30. [ADR-030 – Schlanke Python-Forschungsarchitektur](./decisions/ADR-030-Lean-Python-Research-Architecture.md)
    **Beschlossen:** Ein Paket, fünf Bereiche, ein CLI und keine künstliche Infrastruktur.
31. [ADR-031 – Schlanke .NET-Plattformstruktur](./decisions/ADR-031-Lean-DotNet-Platform-Structure.md)
    **Beschlossen:** Ein Host, wenige Projekte und fachliche Modulgrenzen ohne Frameworkballast.
32. [ADR-032 – Einfacher und vollständiger Code](./decisions/ADR-032-Simple-And-Complete-Code.md)
    **Beschlossen:** Verständliche Python- und C#-Implementierungen ohne Funktionsverlust.
33. [ADR-033 – Modellpaket-Schemas und Parität](./decisions/ADR-033-Model-Package-Schemas-And-Parity.md)
    **Beschlossen:** Fünf Paketdateien, 500 Referenzfälle und fachlich exakte Python-/NET-Prüfung.
34. [ADR-034 – Deploymentzonen und manuelle Modellpromotion](./decisions/ADR-034-Deployment-Zones-And-Manual-Model-Promotion.md)
    **Beschlossen:** Zwei Images, isolierte Zonen und bewusste Paketpromotion über die UI.
35. [ADR-035 – Technologie-Baseline](./decisions/ADR-035-Technology-Baseline.md)
    **Beschlossen mit offenen Details:** Hauptversionen und reproduzierbare Versionsführung.
36. [ADR-036 – Versionierte und typisierte Konfiguration](./decisions/ADR-036-Versioned-Typed-Configuration.md)
    **Beschlossen:** Eigene Verträge und Tabellen je fachlichem Konfigurationstyp.
37. [ADR-037 – Instrumentneutraler Plattformkern](./decisions/ADR-037-Instrument-Agnostic-Platform-Core.md)
    **Beschlossen:** MES als V1-Profil auf einem allgemein nutzbaren Instrumentvertrag.
38. [ADR-038 – Parquet- und DataFrame-Bibliotheken](./decisions/ADR-038-Parquet-And-DataFrame-Libraries.md)
    **Beschlossen:** PyArrow und pandas in Python, Parquet.Net und typisierte Verarbeitung in .NET.

## Empfohlene Lesereihenfolge

Für einen vollständigen Einstieg:

```text
Projektvision
→ Projektgrundsätze
→ Funktionale und nichtfunktionale Anforderungen
→ Trading- und Risikokonzept
→ Architektur
→ Machine Learning
→ Roadmap
→ offene Entscheidungen und ADRs
```

Bei Detailarbeiten sollte zusätzlich immer das zugehörige Fachdokument sowie die
zugehörige ADR für den Entscheidungskontext berücksichtigt werden.

## Pflege der Dokumentation

- Neue oder geänderte Regeln werden zuerst in das zuständige Fachdokument
  übernommen.
- Grundlegende Architektur- oder Technologieentscheidungen erhalten zusätzlich
  eine neue oder aktualisierte ADR.
- Vorschläge werden ausdrücklich als offen oder vorläufig gekennzeichnet.
- Erledigte Planungspunkte werden in der Roadmap aktualisiert.
- Neue Seiten und ADRs werden in diesem Inhaltsverzeichnis ergänzt.
