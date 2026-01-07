# Trading Strategy – Impulse, Fib Retrace & Reversal (MTF)

## Markets & Timeframes
- Execution Timeframes: 5m, 15m
- Analysis Timeframes: 30m, 1h, 4h
- Primary Market: XAUUSD

## Strategy Objective
Trade continuation moves after strong impulses by entering on validated
reversals at structurally relevant Fibonacci levels.

## Core Principles
- Trade only in impulse direction
- Entries only on LTF
- HTF provides context, not timing
- Rules define boundaries, ML evaluates quality
- Risk is deterministic and centralized

## Impulse & Fibonacci
- Impulse: 1–3 strong candles
- Fibonacci applied to impulse leg
- Decision zone: Fib-0 ± ATR tolerance

## Entry Logic
- Entry on candle close
- Direction follows impulse
- No fixed candle pattern
- ML evaluates reversal dynamically

## Trade Structure
Two trades per opportunity:
- Trade 1: TP 0.381–0.5, RR ≥ 1.0, 60%
- Trade 2: TP 0.5–0.786, 40%
- Shared structural stop loss

## Risk Philosophy
- Max exposure per opportunity: ~8–10%
- Multiple hard risk constraints
- Risk engine has final authority
