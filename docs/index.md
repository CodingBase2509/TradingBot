# Trading Intelligence Platform – Inhaltsverzeichnis

Zentraler Einstieg in die Projektdokumentation der Trading-KI-Plattform.

**Stand:** 27. Juli 2026  
**Aktuelle Phase:** Phase 0 – Planung und fachliche Spezifikation  
**Implementierungsstatus:** Noch keine produktive Implementierung

Für eine Einführung und Hinweise zur Nutzung der Dokumentation siehe
[Dokumentationsübersicht](./README.md).

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
4. [Deployment und Betrieb](./architecture/Deployment.md)  
   Geplantes Startbild, Betriebsgrundsätze und Ausfallverhalten.

## Trading

1. [Trading-Konzept](./trading/TradingConcept.md)  
   Handelsstil, Modellentscheidungen, Ablauf und V1-Vereinfachungen.
2. [Risiko- und Sicherheitskonzept](./trading/RiskManagement.md)  
   Risk Guard, Schutzebenen, Handelsstopps und vorläufige Grenzwerte.
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

Die späteren Phasen 3 bis 7 sind derzeit in der
[Roadmap-Übersicht](./roadmap/Overview.md) beschrieben und besitzen noch keine
eigenen Dokumente.

## Architekturentscheidungen

1. [ADR-001 – Python und .NET kombinieren](./decisions/ADR-001-Python-And-DotNet.md)  
   **Beschlossen:** Trennung von ML-Forschung und Produktionskern.
2. [ADR-002 – ONNX als bevorzugtes Modellformat](./decisions/ADR-002-ONNX.md)  
   **Beschlossen mit Vorbehalt:** Modellausführung direkt in .NET.
3. [ADR-003 – Futures als Zielmärkte](./decisions/ADR-003-Futures-Target-Markets.md)  
   **Teilweise beschlossen:** Futures als Zielrichtung, konkrete V1-Auswahl vorgeschlagen.
4. [ADR-004 – Interactive Brokers](./decisions/ADR-004-Interactive-Brokers.md)  
   **Vorgeschlagen:** Bevorzugter Broker, vor Implementierung zu verifizieren.

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
betroffenen Architekturentscheidungen berücksichtigt werden.
