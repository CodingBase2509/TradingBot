# Deployment und Betrieb

## Geplantes Startbild

- .NET-Anwendung als modularer Monolith;
- Angular als getrennt ausgelieferte Weboberfläche;
- Python-Umgebung nur für Import, Training und Tests;
- ONNX Runtime im .NET-Prozess;
- relationale Datenbank für betriebliche Zustände und Metadaten;
- Parquet oder vergleichbarer Dateispeicher für große Zeitreihen;
- Objektablage für Modelle, Trainingsstände und Berichte;
- IB Gateway oder TWS als Brokerzugang.

Die konkreten Produkte für Datenbank, Objektablage und Hosting sind offen.

## Betriebsgrundsätze

- Forschungsumgebung und Live-System werden getrennt.
- Nur signierte oder per Prüfsumme verifizierte, freigegebene Artefakte gelangen in Produktion.
- Konfiguration und Geheimnisse liegen außerhalb von Quellcode und Modellartefakten.
- Rollback auf die letzte stabile Modellversion muss ohne erneutes Training möglich sein.
- Backups und Wiederherstellung werden regelmäßig getestet.

## Ausfallverhalten

- Python-Ausfall beeinflusst laufende Inferenz nicht.
- Angular-Ausfall stoppt nicht automatisch den Kern, erzeugt aber Alarm.
- Daten-, Modell- oder Brokerunsicherheit blockiert neue Trades.
- Nach Neustart erfolgt zuerst der Abgleich mit dem Broker.
- Verhalten offener Positionen wird pro Ausfallart vorab definiert und getestet.

## Noch festzulegen

- lokale, Cloud- oder hybride Zielumgebung;
- Hochverfügbarkeit und Wiederanlaufziele;
- Observability-Stack;
- Zeitdienst und Zeitsynchronisation;
- Aufbewahrungs- und Backup-Ziele;
- Freigabeprozess und Rollen.
