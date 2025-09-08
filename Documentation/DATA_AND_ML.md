# Daten, Features & ML‑Training

## Datenquellen
- **Historie (REST):** 15‑Min‑Klines pro Symbol (BTC/USDT, ADA/USDT, XRP/USDT). Zeitraum 2017–2024/25. (SOL erst ab 2020 relevant.)
- **Live (WebSocket):** `kline_15m` (inkl. isFinal), `bookTicker` (Bid/Ask → Spread)
- **Perps (optional):** Funding‑Rate, Open Interest

## Lookback/Warm‑up (Live)
- **Standard‑Indikatoren:** 1k Bars (~10 Tage) genügen;  
- **Kanal & S/R‑Clustering:** **30–45 Tage** (≈ 3k–4k Bars) empfohlen

## Features (pro abgeschlossener Bar)
**Trend/Kanal**
- Linearer Regressionskanal über 30 Tage → `channel_slope`, `channel_r2`, `channel_pos`, `channel_width/ATR`
- EMA‑Ribbon (EMA21/EMA50), ADX (+DI/−DI)

**Momentum & Volatilität**
- RSI(14/5), MACD(12/26/9) & Δ‑Histogramm, ROC, Stoch %K
- ATR(14), Bollinger‑Breite, Squeeze (BB<Keltner), Donchian(55)

**Lage/Struktur**
- VWAP/Anchored‑VWAP‑Distanz, Bollinger‑Position, Donchian‑Kanten
- **Support/Resistance** über Pivot/ZigZag‑Clustering (Bins ~0.25×ATR, minTouch≥3, Recency‑Gewichtung)
- **Fibonacci**: 38.2/50/61.8 Retracements, 1.272/1.618 Extensions (Swing‑Definition via ZigZag‑Schwelle ~2×ATR)

**Kosten/Liquidität**
- `spread/ATR`, `cost_ratio` (Roundtrip/ATR), simple Slippage‑Heuristik, Gebührenmodell

## Labeling (automatisch, Training)
Für Zeitpunkt *t* (Entry konservativ inkl. halbem Spread):
- **SL (Invalidation):** unter Swing‑Low/S‑Level mit ATR‑Puffer (z. B. 1.8×ATR)
- **TP (realistisch):** nächstes R‑Level/ Kanal‑Oberband **oder** ≤ 3×ATR
- **Vorwärtsprüfung (N Bars, z. B. 48):** TP zuerst → **Label=1**, SL zuerst → **0**, Tie‑Break konservativ (SL zuerst)
- **Nach‑Kosten‑Denke:** nur Kandidaten mit **R:R (costed) ≥ 1.5** zulassen

Optional fokussiert: **Playbook‑Labeling** (Breakout→Retest→Bestätigung etc.)

## Training & Evaluation
- **Modell:** ML.NET **LightGBM** (Binary) → Score *p ∈ [0,1]*
- **Splits:** Zeitlich! Walk‑Forward (z. B. 2017–20 train, 2021 val, 2022 test; final 2023/24)
- **Metriken:** AUC/LogLoss + **Backtest‑PnL/Sharpe/MaxDD** (nach Kosten) zur Schwellenwahl
- **Artefakte:** `model.zip`, `config.json` (Feature/Decision‑Config), `metrics.json`

## Model‑Lifecycle
- **Offline** trainieren (separates EXE), Artefakte versionieren (z. B. `policy-YYYY-MM-DD`)
- **Live** nur inferieren (Modell/Config laden); **kein** Online‑Training
- **Retrain** regelmäßig (wöchentlich/monatlich); Rollout/Rollback per Version

## Beispiel `config.json`
```json
{
  "model_version": "policy-2025-09-08",
  "feature_config": {
    "rsi": [5, 14],
    "atr": 14,
    "macd": { "fast": 12, "slow": 26, "signal": 9 },
    "bollinger": { "period": 20, "stdev": 2.0 },
    "donchian": 55,
    "ema": [21, 50],
    "channel_days": 30,
    "sr": { "pivot_left": 3, "pivot_right": 3, "bin_atr": 0.25, "min_touches": 3, "half_life_days": 10 },
    "fibo": { "zigzag_atr": 2.0 }
  },
  "labeling": { "horizon_bars": 48, "sl_atr": 1.8, "tp_atr_cap": 3.0, "tie_break": "SL_FIRST" },
  "decision": { "threshold": 0.58, "rr_min_costed": 1.5, "cost_ratio_max": 0.15, "risk_pct": 0.0075 }
}
```
