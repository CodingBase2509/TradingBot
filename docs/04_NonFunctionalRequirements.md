# Nichtfunktionale Anforderungen

## Sicherheit

- Fehlende entscheidungsrelevante, veraltete oder inkompatible Eingaben führen
  zu „kein Trade“.
- Begrenzte ältere Lücken sind nur gemäß einem versionierten
  Missing-Data-Vertrag zulässig; entscheidungsrelevante Lücken führen immer zu
  „kein Trade“.
- Modellfehler dürfen keine Ersatzentscheidung erzeugen.
- Geheimnisse und Brokerzugänge werden nicht in Code oder Modellartefakten gespeichert.
- Kritische Aktionen benötigen Authentifizierung, Berechtigung und Audit-Eintrag.

## Zuverlässigkeit

- Der Ausfall von Python oder Angular darf den stabilen .NET-Handelskern nicht unmittelbar stoppen.
- Unklarer Broker- oder Positionszustand blockiert neue Trades.
- Neustarts müssen offene Orders und Positionen sicher rekonstruieren.
- Zeitquelle, Zeitzone und Zeitstempel werden überwacht.

## Reproduzierbarkeit

Jedes Experiment verweist eindeutig auf:

- Roh- und Trainingsdatenstand;
- Feature-Version;
- Trainingscode und Konfiguration;
- Laufzeitumgebung und Zufallsstartwerte;
- Modellartefakt;
- Backtestversion und Kostenannahmen;
- Ergebnis- und Freigabebericht;
- Git-Commit und sauberen Arbeitsstand.

## Nachvollziehbarkeit

- Ereignisse werden unveränderlich beziehungsweise append-only aufgezeichnet.
- Jeder Trade ist auf Daten, Features, Modellversion, Prüfungen und Orders zurückführbar.
- Abgelehnte Entscheidungen enthalten maschinenlesbare Gründe.
- Große Daten und Artefakte werden über Prüfsummen und Manifeste mit ihren
  relationalen Metadaten verbunden.

## Leistung

- Die Modellentscheidung muss deutlich innerhalb des gewählten Handelsintervalls liegen.
- Datenaufnahme, Feature-Berechnung und Orderverarbeitung dürfen sich nicht gegenseitig blockieren.
- Konkrete Latenz- und Durchsatzbudgets werden im technischen Design festgelegt.

## Wartbarkeit

- Klare Verantwortungsgrenzen und versionierte Schnittstellen.
- Der Start als modularer Monolith ist beschlossen; Dienste werden erst bei betrieblichem Nutzen getrennt.
- Anbieteradapter werden isoliert getestet.
- Fachbegriffe und Annahmen werden in der Dokumentation gepflegt.
- Python- und C#-Code bevorzugt linearen, expliziten und fachlich benannten
  Kontrollfluss vor unnötigen Schichten oder cleveren Kurzformen.
- Abstraktionen, Frameworks und Konfigurierbarkeit benötigen einen realen
  Anwendungsfall oder messbaren Nutzen.
- Vereinfachung darf keine erforderliche Funktion, Sicherheitsregel,
  Fehlerbehandlung oder Nachvollziehbarkeit entfernen.

## Testbarkeit

- deterministische Tests für Features, Risiko und Positionsberechnung;
- Vergleichstests zwischen Python- und .NET-Features;
- Simulation von Verbindungsabbruch, Teilfüllung und fehlerhaften Daten;
- Wiederholung historischer Läufe mit gleichem Ergebnis;
- gestufte Tests für V1 von Backtest bis Paper, später bis Canary.

## Compliance und Betrieb

Vor Live-Handel sind Brokerbedingungen, Marktdatenlizenzen, Steuern, Aufzeichnungspflichten und anwendbare Regulierung fachkundig zu prüfen.
