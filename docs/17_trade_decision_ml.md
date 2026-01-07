# Trade Decision ML (Model C)

## Pipeline Position
After candidate generation.

## Purpose
Score SL/TP combinations and select optimal trade geometry.

## Feature Groups

### Context
- reversal_score
- impulse_quality_score
- timeframe

### SL Geometry
- sl_distance_atr
- sl_to_htf_zone_atr

### TP Geometry
- tp_distance_atr
- tp_fib_level
- tp_to_htf_zone_atr

### Risk / Reward
- risk_reward_ratio
- reward_atr_over_risk_atr

### Market Regime
- atr_percentile
- spread_atr

### Trade Leg
- trade_leg (1 or 2)

## Output
P(TP_hit_before_SL | candidate_pair)
