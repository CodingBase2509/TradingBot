# Decision & Execution (ohne Zeit‑Stop)

## Grundsatz
Trade **nur**, wenn **Struktur** (SL=Invalidation, TP realistisch) **+ Kosten** **+** **R:R (nach Kosten) ≥ 1.5** passen. **SL nicht zu eng**, **TP nicht utopisch**.

## Kosten & R:R (Long; Short gespiegelt)
- Entry‑Preis: `entry = mid + 0.5*spread + slip_in`
- Roundtrip‑Kosten: `cost_round = spread + slip_in + slip_out + fee_in + fee_out`
- **Risk_costed:** `risk = (entry − SL) + cost_round/2`
- **Reward_costed:** `reward = (TP − entry) − cost_round/2`
- **R:R_costed:** `rr = reward / risk` → **≥ 1.5** erforderlich

## Sanity‑Checks
- **SL‑Mindestabstand:** `(entry − SL)/ATR ≥ 1.0` (Noise‑Floor)  
- **TP‑Machbarkeit:** `TP` vor starkem R‑Cluster; `(TP − entry)/ATR ≤ 3–4`  
- **Kanal‑Platz:** z. B. `channel_r2 ≥ 0.6`, `channel_pos` nicht „oben“ (für Long)  
- **Kostenfilter:** `cost_ratio = cost_round / ATR ≤ 0.15`

## Position‑Sizing (risk‑basiert)
```
risk_amount   = equity * risk_pct          // z. B. 0.5–1.0 %
risk_per_unit = (entry - SL) + cost_round/2
qty           = floor( risk_amount / risk_per_unit )    // LotSize/MinNotional beachten
```

## Order‑Typen & OCO
- **Market:** sofort bei starker Bestätigung
- **Stop‑(Limit):** Breakout über/unter Level (z. B. SR + 0.3×ATR)
- **Limit:** Retest/Pullback an SR/Fibo/VWAP
- **OCO‑Bracket:** Nach Fill TP & SL als verknüpfte Child‑Orders

## Playbooks (Beispiele)
**A) Breakout → Retest → Bestätigung (Long)**
- Breakout: `Close > SR + 0.3–0.5×ATR` + Volumen‑Spike
- Retest (≤ 8 Bars): Berührung des Levels ±0.3×ATR, Schluss wieder darüber
- Entry: Stop‑Limit oberhalb SR; SL: `SR − 2×ATR`; TP: nächstes R oder `+3×ATR`/Kanal‑Oberband

**B) Trend‑Pullback (Fibo 50/61.8 + VWAP + Kanal‑Mitte)**
- Trend up (EMA21 > EMA50, ADX>20); Korrektur in 50/61.8 nahe Anchored‑VWAP ±0.3×ATR
- Entry: Limit; SL: unter Swing‑Low −1.8×ATR; TP: 1.272/1.618‑Extension oder Kanal‑Oberband

## Live‑Loop (vereinfacht)
1) Auf **Candle‑Close** Features berechnen (stabil)  
2) **Score p** vom Modell  
3) **Decision:** SL/TP aus Struktur, R:R≥1.5, Kostenfilter, Position‑Sizing, Order‑Typ  
4) **Orders** platzieren (ggf. Pending Stop/Limit) + **OCO** anhängen  
5) **Intrabar**: Trigger/Amends/Cancel per Preis‑Events; Telemetry schreiben
