# Phase 0 – Planung und fachliche Spezifikation

## Ziel

Aus der Vision entsteht eine prüfbare, implementierbare Spezifikation.

## Umfang

- [x] V1-Produktgrenzen;
- [x] Handels- und Risikopolitik vollständig spezifizieren;
- [x] Lernziel und historische Bewertungsformel;
- [x] Datenbedarf und Qualitätsregeln;
- [x] Backtest- und Kostenannahmen;
- [x] Modellbewertung und Freigabeschwellen;
- [ ] Anbieterentscheidungen;
- [x] Systemarchitektur und Modellvertrag;
- [x] Speicher-, Betriebs- und Umgebungsarchitektur;
- [ ] V1-End-to-End-Abnahmeszenarien;
- [ ] minimaler Technologie- und Konfigurationsvertrag.

Die V1-Produktgrenzen und die Handels- und Risikopolitik sind mit ADR-005 und
ADR-006 beschlossen. Parameter des technischen Designs bleiben konfigurierbar,
ohne die fachliche Entscheidung erneut zu öffnen.

Das V1-Lernziel, adaptive Kandidaten und die historische Bewertungslogik sind
beschlossen. Der konkrete Mindestvorteil gegenüber „kein Trade“ wird als
versionierter Modellparameter erst auf Validierungsdaten ausgewählt.

Marktdatenauflösung, MES-Historie, Rollover, Lückentoleranz und das
Backtest-Kostenmodell sind mit ADR-008 bis ADR-011 beschlossen. Konkrete
Anbieterpreise und technisch kalibrierte Ausführungsparameter folgen in den
dafür vorgesehenen Entscheidungsblöcken.

Historische Mindestkriterien sowie Shadow- und Paper-Abnahme sind mit ADR-012
beschlossen. Canary- und Live-Kriterien bleiben außerhalb des V1-Umfangs.

## Abnahme

Alle offenen Punkte mit Einfluss auf Datenbeschaffung oder Kerndesign sind beschlossen und als ADR oder Fachentscheidung dokumentiert. Erst danach folgen Datenmodelle und Produktionscode.

## Nächstes zentrales Ergebnis

Konkrete V1-End-to-End-Abnahmeszenarien beschreiben den Weg vom bekannten
Datenstand über Modellentscheidung und Risikoprüfung bis zu Order,
Positionsende, Neustart und identischer Backtestsimulation.
