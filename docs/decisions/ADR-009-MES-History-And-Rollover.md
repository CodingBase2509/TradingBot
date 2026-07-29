# ADR-009: MES-Historie und Rollover

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

MES-Futures bestehen aus einzelnen, zeitlich begrenzten Verträgen. Vor dem
Auslaufen verlagert sich der Handel üblicherweise auf den nächsten Vertrag.
Training, Backtest und Paper Trading benötigen deshalb eine nachvollziehbare
Regel, welcher konkrete Vertrag zu welchem Zeitpunkt verwendet wird.

## Entscheidung

### Historischer Zeitraum

- Die V1 verwendet die vollständige verfügbare MES-Historie ab dem offiziellen
  Handelsstart am 6. Mai 2019.
- Später hinzukommende Daten werden unveränderlich ergänzt und in neuen
  Datensatzversionen berücksichtigt.
- Daten anderer, älterer S&P-500-Futures werden nicht stillschweigend als
  MES-Historie behandelt.

### Speicherung

- Rohdaten werden immer getrennt nach konkretem Futures-Kontrakt gespeichert.
- Kontraktkennung, Laufzeit, Tick-Größe, Multiplikator, Handelskalender und
  Datenquelle bleiben für jeden Datensatz nachvollziehbar.
- Historische Ausführung und Kosten werden ausschließlich auf dem damals
  tatsächlich ausgewählten Vertrag simuliert.

### Auswahl des handelbaren Vertrags

- Für jeden abgeschlossenen Handelstag wird das gehandelte Volumen des
  bisherigen Frontkontrakts mit dem nächsten geeigneten Kontrakt verglichen.
- Übersteigt das Volumen des Folgekontrakts das Volumen des bisherigen
  Frontkontrakts, wird ab dem folgenden Handelstag auf den Folgekontrakt
  gewechselt.
- Die Auswahl verwendet nur zu diesem Zeitpunkt vollständig abgeschlossene
  Daten. Spätere Volumeninformationen dürfen den historischen Wechselzeitpunkt
  nicht rückwirkend verändern.
- Der konkrete Wechsel wird mit Datum, altem Vertrag, neuem Vertrag,
  Vergleichsvolumen und Regelversion gespeichert.
- Sonderfälle nahe Fälligkeit oder bei fehlerhaften Volumendaten benötigen eine
  konservative Ersatzregel im technischen Design.

### Kontinuierliche Analysereihe

- Eine zusammenhängende, gegebenenfalls rückwärts angepasste Kursreihe darf für
  langfristige Analyse und geeignete Features erzeugt werden.
- Diese Reihe wird separat versioniert und niemals für historische
  Orderausführung oder die Ermittlung tatsächlich erreichbarer Preise verwendet.
- Features, die durch eine Preisanpassung verändert werden, kennzeichnen die
  verwendete Rollover- und Anpassungsversion.

## Begründung

Die volumenbasierte Regel folgt der tatsächlichen Verlagerung der
Marktaktivität. Die Anwendung erst am folgenden Handelstag verhindert, dass
Informationen aus der Zukunft in historische Entscheidungen einfließen.
Getrennte Vertragsdaten vermeiden künstliche Kurssprünge und erhalten
realistische Ausführungspreise.

## Folgen

- Der Datenimport benötigt Stammdaten und Volumen für jeden relevanten
  MES-Kontrakt.
- Dataset Builder und Backtest verwenden dieselbe versionierte Vertragsauswahl.
- Rollover-Tage werden in Evaluation und Berichten gesondert auswertbar.
- Die Ersatzregel für fehlende Volumendaten wird vor Implementierung
  konkretisiert und getestet.
