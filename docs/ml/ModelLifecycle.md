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
- Artefakt, Feature-Vertrag und Runtime müssen kompatibel sein.
- Ein Kandidat muss den Champion anhand vorher festgelegter Kriterien schlagen.
- Bei auffälliger Abweichung wird begrenzt, gestoppt oder zurückgerollt.

## Champion und Challenger

Der Champion bleibt aktiv, während ein Challenger zunächst Shadow- und Paper-Entscheidungen erzeugt. Vergleichbar sind nur Ergebnisse unter denselben Daten-, Kosten- und Risikobedingungen.

## Rollback

Die letzte stabile Version bleibt vollständig verfügbar. Rollback verändert keine historischen Daten und überschreibt keine Artefakte. Jede Aktivierung und Rücknahme wird mit Grund und Zeitpunkt gespeichert.

## Kein Online-Umlernen in V1

Abgeschlossene Trades sind neue Beobachtungen, aber kein unmittelbares Trainingssignal für das aktive Modell. Verbesserung geschieht durch neue, versionierte Offline-Läufe.
