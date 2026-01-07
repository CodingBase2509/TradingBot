# HTF Structure & Zone Engine

## Pipeline Position
Asynchronous background processing on 30m / 1h / 4h candle close.

## Purpose
Detect major market structure and zones used to validate LTF trades.

## Inputs
- HTF OHLCV candles

## Outputs
- Imbalance zones
- Supply / demand zones
- Major swing highs / lows
- Range boundaries
- Zone strength and timeframe

## Notes
- No trade decisions
- Read-only context for LTF
