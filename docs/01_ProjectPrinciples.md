# Projektgrundsätze

1. **Sicherheit vor Profit:** Im Zweifel wird nicht gehandelt. Das Modell kann feste Limits und den Not-Aus nicht umgehen.
2. **Kontrolliertes Offline-Lernen:** Live-Ergebnisse fließen in spätere Trainingsstände ein, verändern das aktive Modell aber nie unmittelbar.
3. **Unbekannte Daten entscheiden:** Ein Modell wird auf zeitlich späteren Daten bewertet, die es beim Training nicht gesehen hat.
4. **Kein Trade ist eine echte Entscheidung:** Handelsaktivität ist kein Selbstzweck.
5. **Realismus vor schöner Statistik:** Gebühren, Spread, schlechtere Ausführung, Verzögerung und Liquiditätsgrenzen gehören in die Bewertung.
6. **Einfach beginnen:** Zusätzliche Daten und komplexere Modelle müssen einen messbaren Vorteil beweisen.
7. **Austauschbare Komponenten:** Broker, Datenquellen, Modelle und Oberfläche werden über klare Verträge angebunden.
8. **Reproduzierbarkeit:** Datenstand, Merkmale, Code, Einstellungen, Umgebung und Zufallsstartwerte werden versioniert.
9. **Messbarkeit:** Jede wesentliche Änderung wird gegen eine geeignete Vergleichsvariante getestet.
10. **Ein Entscheidungsweg:** Backtest, Paper und Live teilen möglichst dieselbe Order-, Positions- und Risikologik.
11. **Identische Merkmale:** Python und .NET müssen dieselben Modelleingaben bitgenau oder innerhalb definierter Toleranzen berechnen.
12. **Vollständige Beobachtbarkeit:** Entscheidung, Ablehnung, Order, Ausführung, Abweichung und Störung werden protokolliert.
13. **Anbieterunabhängigkeit:** Kein fachlicher Kern hängt unnötig von Broker oder Datenanbieter ab.
14. **Modell und Sicherung bleiben getrennt:** Das Modell optimiert Handelsentscheidungen; .NET prüft technische und finanzielle Zulässigkeit.
15. **Beschlüsse sind sichtbar:** Vorschläge, Arbeitswerte, Beschlüsse und verworfene Optionen werden eindeutig gekennzeichnet.
16. **Keine künstliche Komplexität:** Neue Komponenten, Abstraktionen und Infrastruktur benötigen einen realen zweiten Anwendungsfall, eine Sicherheitsgrenze oder einen gemessenen Nutzen.
17. **Einfacher, vollständiger Code:** Python und C# verwenden die kleinste verständliche Implementierung, die benötigte Funktionalität, Sicherheit, Fehlerbehandlung und Tests vollständig erhält.
