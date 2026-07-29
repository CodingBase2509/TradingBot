# ADR-010: Datenqualität und Lückentoleranz der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Ein vollständiger Handelsstopp bei jeder fehlenden abgeleiteten Kerze wäre
unnötig streng, wenn die zugrunde liegenden Rohdaten vorhanden sind oder eine
kleine ältere Lücke die aktuelle Entscheidung nicht beeinflusst. Gleichzeitig
dürfen unbekannte Marktbewegungen weder interpoliert noch als sichere Daten
behandelt werden.

## Entscheidung

### Qualitätsstufen

Die V1 unterscheidet drei Stufen:

#### Grün – vollständig oder sicher rekonstruierbar

- Die benötigten Rohdaten sind vollständig und plausibel.
- Fehlt nur eine daraus abgeleitete 1-, 5-, 15- oder 60-Minuten-Kerze, wird sie
  deterministisch neu erzeugt.
- Nach erfolgreicher Rekonstruktion und erneuter Qualitätsprüfung darf normal
  weitergearbeitet werden.

#### Gelb – begrenzte, nicht entscheidungsrelevante Lücke

- Innerhalb des betrachteten historischen Datenbereichs fehlen höchstens drei
  5-Minuten-Intervalle.
- Die Lücken betreffen weder die aktuelle Signalkerze noch die letzten 60
  Minuten noch ein für die konkrete Entscheidung benötigtes Feature- oder
  Ausführungsfenster.
- Die Verarbeitung darf fortgesetzt werden, wenn Lückenanzahl, Position und
  Qualitätsstatus als explizite Eingaben beziehungsweise Metadaten verfügbar
  sind und das Modell mit derselben Behandlung trainiert wurde.
- Betroffene höhere Zeitintervalle werden nicht stillschweigend als vollständig
  markiert.

#### Rot – entscheidungsrelevante oder größere Lücke

Mindestens eine der folgenden Bedingungen blockiert neue Trades:

- die aktuelle Signalkerze ist unvollständig;
- innerhalb der letzten 60 Minuten fehlen benötigte Rohdaten oder Kerzen;
- ein benötigtes Feature kann nicht vollständig oder gemäß seinem versionierten
  Missing-Data-Vertrag berechnet werden;
- für Einstieg, Stop, Ziel oder Positionsverwaltung fehlen relevante
  Ausführungsdaten;
- mehr als drei 5-Minuten-Intervalle im betrachteten Bereich fehlen;
- Zeitreihenfolge, Preis, Bid/Ask oder Volumen sind widersprüchlich.

Die Sperre endet erst, nachdem Daten nachgeladen, betroffene Kerzen und Features
neu berechnet und alle Qualitätsprüfungen bestanden wurden.

### Historische Daten

- Ein Datenfehler verwirft nicht automatisch einen gesamten Handelstag.
- Nur Entscheidungszeitpunkte, deren Features oder Trade-Simulation von der
  Lücke betroffen sind, werden ausgeschlossen oder gemäß dem versionierten
  Missing-Data-Vertrag behandelt.
- Nachträglich beschaffte Daten überschreiben keine alten Datenstände. Sie
  erzeugen eine neue Rohdaten- und Datensatzversion.
- Trainings-, Validierungs- und Testergebnisse speichern den verwendeten
  Qualitätsstand.

### Allgemeine Prüfungen

- UTC ist die technische Zeitbasis.
- Geplante Börsenpausen und Feiertage gelten nicht als Datenlücke.
- Identische Duplikate werden bei der kanonischen Verarbeitung dedupliziert.
- Widersprüchliche Duplikate werden isoliert und gemeldet.
- Bid darf nicht größer als Ask sein.
- Preise, Mengen, Zeitstempel und Reihenfolge werden auf Plausibilität geprüft.
- Rohdaten bleiben unverändert erhalten.

## Begründung

Die Regel erlaubt eine begrenzte praktische Fehlertoleranz, ohne unbekannte
Preise zu erfinden. Sie unterscheidet einen reparierbaren Verarbeitungsfehler von
einem echten Informationsverlust. Entscheidungsrelevante Unsicherheit führt
weiterhin zuverlässig zu „kein Trade“.

## Folgen

- Jede Kerze und jeder Feature-Satz benötigt Qualitäts- und
  Vollständigkeitsmetadaten.
- Der Feature-Vertrag muss die erlaubte Missing-Data-Behandlung festlegen.
- Python und .NET wenden dieselben Stufen und Toleranzen an.
- Datenreparaturen erzeugen neue Versionen und lösen eine gezielte
  Neuberechnung aus.
- Die genaue technische Definition des „betrachteten Datenbereichs“ wird pro
  Feature-Satz versioniert.
