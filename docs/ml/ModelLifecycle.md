# Modelllebenszyklus

## Zustände

```text
Candidate
→ Backtested
→ Validated
→ Shadow
→ Paper
→ Canary
→ Production
→ Retired
```

- **Candidate:** trainiertes, noch ungeprüftes Modell.
- **Backtested:** technische historische Tests abgeschlossen.
- **Validated:** fachliche Mindestkriterien auf unbekannten Daten bestanden.
- **Shadow:** läuft mit Live-Daten, sendet keine Orders.
- **Paper:** handelt mit virtuellem Kapital.
- **Canary:** handelt live mit einem kleinen Teil des erlaubten Risikos.
- **Production:** aktiver Champion.
- **Retired:** nicht mehr aktiv, aber weiterhin nachvollziehbar.

## Promotion-Regeln

- Zustandswechsel sind explizit und auditierbar.
- Ein Modell aktiviert sich nie selbst.
- Das unveränderliche Modellpaket enthält Modell, Verträge, Herkunft,
  Prüfsummen, Referenzfälle und Evaluationsbericht.
- Artefakt, Feature-Vertrag, Entscheidungsvertrag und Runtime müssen kompatibel
  und in .NET technisch geprüft sein.
- Mindestens 500 Paket-Referenzfälle müssen die numerischen Toleranzen und
  exakt dieselbe fachliche Entscheidung in Python und .NET bestehen.
- Modellpakete werden manuell zwischen den Stufen kopiert. Das bloße
  Vorhandensein im Modellverzeichnis aktiviert nichts.
- Eine Strategy Instance entsteht erst nach vollständiger Paketregistrierung
  sowie bewusster Auswahl und Bestätigung in der UI.
- Ein Kandidat muss den Champion anhand vorher festgelegter Kriterien schlagen.
- Bei auffälliger Abweichung wird begrenzt, gestoppt oder zurückgerollt.

Für V1 folgen auf die historische Validierung mindestens vier Wochen Shadow
Mode. Paper Trading dauert mindestens acht Wochen und bis wenigstens 100
Signalgruppen abgeschlossen sind. Beide Bedingungen müssen erfüllt sein.
Canary und Production gehören nicht zum V1-Umfang.

## Champion und Challenger

Der Champion bleibt aktiv, während ein Challenger zunächst Shadow- und Paper-Entscheidungen erzeugt. Vergleichbar sind nur Ergebnisse unter denselben Daten-, Kosten- und Risikobedingungen.

## Rollback

Die letzte stabile Version bleibt vollständig verfügbar. Rollback verändert keine historischen Daten und überschreibt keine Artefakte. Jede Aktivierung und Rücknahme wird mit Grund und Zeitpunkt gespeichert.

Kann ein neues Paket nicht vollständig geprüft oder geladen werden, bleibt das
letzte gültige Modell aktiv. Ohne gültiges Modell werden keine neuen Trades
eröffnet.

## Kein Online-Umlernen in V1

Abgeschlossene Trades sind neue Beobachtungen, aber kein unmittelbares Trainingssignal für das aktive Modell. Verbesserung geschieht durch neue, versionierte Offline-Läufe.
