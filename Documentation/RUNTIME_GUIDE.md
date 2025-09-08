# Runtime‑Guide (Live‑Betrieb)

## Streams
- **WebSocket:** `kline_15m` (liefert Bar‑Updates & *isFinal*, d. h. Bar geschlossen) + `bookTicker` (Bid/Ask → Spread)
- **REST:** Backfill (Startup), Resync bei Verbindungsabbrüchen

## Loop (vereinfacht, ohne Zeit‑Stop)
1) **Startup Backfill**: 30–45 Tage laden → Ring‑Buffer füllen
2) **WS on Closed Bar**: Feature‑Vektor berechnen (nur abgeschlossene Bar)
3) **Policy.Score(p)** laden (model.zip)
4) **Decision**: SL/TP aus Struktur, **R:R (costed) ≥ 1.5**, Kostenfilter, risk‑Sizing, Order‑Typ
5) **Order**: Market/Stop/Limit platzieren, nach Fill **OCO** (TP/SL) anfügen
6) **Intrabar**: Preis‑Trigger feuern, ggf. Cancel/Replace bei Strukturbruch, Spread‑Wächter
7) **Telemetry**: Decisions, Orders, Fills, Outcomes → Postgres; Logs/Metriken updaten

## Lookback / Warm‑up Gates
- Signale erst freigeben, wenn genug Historie für **alle** Features geladen ist (z. B. `HasWarmup==true`)
- Regressionskanal & S/R‑Clustering benötigen 30–45 Tage

## Resilienz & Recovery
- WS Reconnect mit Backoff; nach Verbindungsabbruch **fehlende Bars** per REST nachladen
- Heartbeats/Healthchecks: WS‑Status, DB‑Reachability, Modell geladen, Warm‑up erfüllt
- Kill‑Switch: bei Fehler‑Salven, Spread‑Extremen, Tages‑Drawdown
