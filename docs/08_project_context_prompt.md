# Project Context Prompt – TradingBot

## Purpose
This document is the **canonical entry point** for any LLM or AI assistant
working on this project.

It provides a **compact, authoritative overview** of:
- the trading strategy
- the processing pipeline
- the ML models
- the software architecture
- strict boundary rules

All other documentation **extends** this context.

---

## Project Goal
Build a **low-latency, event-driven trading system** that trades continuation
moves after strong impulses, using a combination of **rule-based analysis**
and **machine learning**.

Primary goals:
- Robust decision-making
- Deterministic backtesting
- Minimal latency in live trading
- Clean separation of concerns
- Full auditability and explainability

---

## Markets & Timeframes
- Primary market: **XAUUSD**
- Execution timeframes: **5m, 15m**
- Higher-timeframe analysis: **30m, 1h, 4h**

HTF provides **context**, never entry timing.

---

## Trading Strategy (High-Level)
1. Detect a **strong impulse** (1–3 candles, rule-based).
2. Apply **Fibonacci retracement** to the impulse leg.
3. Wait for price to reach **Fib-0 ± tolerance**.
4. Observe **reversal behavior** on LTF.
5. Enter **in the direction of the original impulse**.
6. Open **two trades**:
   - Trade 1: TP ~ 0.381–0.5, ≥ 1:1 RR, 60% size
   - Trade 2: TP ~ 0.5–0.786, 40% size
7. Shared, structurally valid stop loss.
8. Risk engine has final veto authority.

---

## Pipeline Overview
HTF Candle Close
→ HTF Structure & Zones
→ Context Cache

LTF Candle Close
→ Impulse Detection (rules)
→ Impulse Quality Model (optional)
→ Fibonacci Context Engine
→ Fib-0 Gate
→ Reversal Quality Model (streaming)
→ Trade Candidate Engine (SL/TP candidates)
→ Trade Decision Model
→ Risk Engine
→ Execution Engine


---

## ML Models (Conceptual)
The system uses **multiple specialized ML models**, not a single monolithic one.

- **Impulse Quality Model**
  - Scores impulse strength / tradability
  - Optional filter

- **Reversal Quality Model**
  - Evaluates reversal behavior near Fib-0
  - Streaming, candle-by-candle
  - Outputs probability score ∈ [0,1]

- **Trade Decision Model**
  - Scores SL/TP candidate combinations
  - Outputs probability of TP hit before SL

All models:
- Are trained **offline**
- Are loaded as immutable artifacts in live trading
- Never modify context or rules directly

---

## Architecture Principles

- Modular monolith
- In-process event-driven pipeline
- Channel<T> for concurrency
- Struct-based events and primitives
- No blocking I/O in hot path
- Asynchronous logging, telemetry, and persistence
- Separate apps for:
  - Live trading
  - Backtesting
  - Model training

---

## Absolute Boundary Rules (Very Important)

The following must **never** be suggested or implemented:

- ❌ Training ML models in the live trading app
- ❌ Database reads in the live decision path
- ❌ UI or REST APIs coupled to trading logic
- ❌ Microservices in the hot path
- ❌ Synchronous logging or persistence in analysis/decision code
- ❌ ML models defining market context or rules

ML models **score**, rules **define boundaries**.

---

## Mental Model for LLMs

- Think in **pipeline phases**, not call stacks.
- Think in **events and contexts**, not global state.
- Assume **performance constraints are strict**.
- Prefer clarity and determinism over abstraction.

This document defines the authoritative intent of the project.
