# Impulse Quality ML (Model A)

## Pipeline Position
After impulse detection, before fib context.

## Purpose
Score impulse quality to filter weak setups.

## Feature Groups

### Impulse Geometry
- impulse_range_atr
- impulse_body_sum_atr
- candle_count

### Candle Quality
- avg_wick_ratio
- close_near_extreme

### Volatility Context
- atr_slope
- atr_percentile

## Output
impulse_quality_score ∈ [0,1]
