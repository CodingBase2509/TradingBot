# Reversal Quality ML (Model B)

## Pipeline Position
After Fib-0 gate.

## Purpose
Evaluate reversal structures dynamically in streaming mode.

## Feature Groups

### Fib Context
- fib_retrace_level
- dist_to_fib_levels_atr

### HTF Structure
- dist_to_htf_zone_atr
- htf_zone_strength
- is_inside_htf_zone

### Candle Reaction
- body_atr
- wick_ratios
- close_position
- directional_consistency

### Momentum
- rsi_14
- rsi_slope

### Volatility
- atr_percentile
- range_compression_ratio

### Time / Session
- hour_utc
- session_flags

## Output
reversal_score ∈ [0,1]
