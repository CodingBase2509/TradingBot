# Glossary – TradingBot Ubiquitous Language

This glossary defines the **exact meaning** of key terms used throughout
the codebase and documentation.

LLMs and developers must use these terms consistently.

---

## Analysis & Trading Concepts

### Impulse
A strong directional price movement, typically consisting of
1–3 candles, detected by rule-based logic.
Used as the anchor for Fibonacci analysis.

---

### Fibonacci Context (Fib Context)
A retracement context created from an impulse leg.
Includes Fib levels (0, 0.382, 0.5, 0.618, 0.786) and lifecycle state.

---

### Reversal
A market reaction near Fib-0 that indicates potential continuation
in the direction of the original impulse.
Evaluated dynamically over multiple candles.

---

### HTF (Higher Timeframe)
Timeframes larger than the execution timeframe (e.g. 30m, 1h, 4h),
used for structural context only.

---

### LTF (Lower Timeframe)
Execution timeframes (5m, 15m) used for entries and trade management.

---

## Pipeline & Architecture

### Pipeline
The ordered, event-driven flow from market data ingestion
to trade execution.

---

### Context
A stateful analysis object that represents the current understanding
of the market for a specific purpose.

Examples:
- MarketContext
- FibContext
- ReversalContext
- TradeContext

Contexts are **thread-affine** and short-lived.

---

### Engine
Code responsible for **deterministic decisions or transformations**
within a specific pipeline step.

Characteristics:
- Rule-based or algorithmic
- No learning
- No training
- No side effects outside its responsibility

Examples:
- Impulse Detection Engine
- Fibonacci Context Engine
- Risk Engine
- Execution Engine

---

### Model
A **machine learning model** used to perform probabilistic classification
or scoring within the pipeline.

Characteristics:
- Trained offline
- Loaded as immutable artifacts in live trading
- Outputs scores or probabilities
- Never defines rules or boundaries

Examples:
- Reversal Quality Model
- Trade Decision Model
- Impulse Quality Model

---

### Trade Candidate
A potential trade definition consisting of:
- Direction
- Stop loss
- Take profit
Generated before final selection and validation.

---

### Decision
The act of selecting or rejecting a trade candidate based on:
- ML scores
- Rule-based constraints
- Risk limits

A decision may approve or reject a trade.

---

## Execution & Risk

### Risk Engine
The final authority that validates or rejects trades
based on capital exposure, RR, costs, and constraints.

---

### Execution Engine
The component responsible for placing, tracking, and managing
orders with the broker or execution venue.

---

## System & Performance

### Hot Path
The latency-critical execution path:
Market Data → Analysis → ML → Decision → Risk → Execution.

Must not include blocking I/O, logging, or persistence.

---

### Warm Path
Asynchronous processing path for:
- Logging
- Telemetry
- Persistence
- Debugging

---

### Event
An immutable message representing something that happened
in the system (e.g. candle closed, trade approved).

Events are typically implemented as structs.

---

### Projection
A read-optimized view built from stored events,
used for dashboards and analytics.

---

## ML & Data

### Feature
A numerical input used by an ML model.
Derived from market data, indicators, or context.

---

### Label
The ground truth outcome used for training ML models
(e.g. TP hit before SL).

---

### Inference
The process of applying a trained ML model
to features to produce a score or probability.

---

## Closing Note
All documentation, code, and AI-assisted contributions
must adhere to this glossary to avoid ambiguity.
