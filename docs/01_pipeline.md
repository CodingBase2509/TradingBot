# Trading Pipeline Overview

## High-Level Flow

[HTF Candle Close]
→ HTF Structure & Zone Engine
→ HTF Context Cache
--------------------------------
[LTF Candle Close]
→ Impulse Detection
→ Impulse Quality ML (optional)
→ Fib Context Engine
→ Fib-0 Gate
→ Reversal Quality ML
→ Trade Candidate Engine
→ Trade Decision ML
→ Risk Engine
→ Execution Engine

## Design Rules
- HTF never triggers entries
- ML never defines context
- One opportunity per impulse
- Risk engine can veto any trade
