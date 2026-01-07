# Project Responsibilities – Module Overview

## Purpose
This document provides a **concise overview of project responsibilities**
within the trading system.  
It is intended as a **developer-facing reference** and complements the
detailed architecture documentation.

---

## Trading.Core
**Shared domain foundation**

Responsibilities:
- Domain primitives (SymbolId, Timeframe, Price, Quantity)
- Market data structures (Candle)
- Identifiers (ContextId, TradeId, OrderId)
- Event contracts (struct-based)
- Decision reason codes
- Shared configuration contracts

Rules:
- No I/O
- No ML
- No logging implementation
- No broker-specific APIs

---

## Trading.Market
**Market data ingestion and normalization**

Responsibilities:
- Broker / feed adapters behind unified interfaces
- Live data ingestion (candles, bid/ask, spread)
- Candle aggregation (5m, 15m, 30m, 1h, 4h)
- Reconnect, retry, backpressure handling

Rules:
- No trading decisions
- No ML scoring
- No order execution

---

## Trading.Analysis
**Market and setup analysis (Pipeline Phases 1–3)**

Responsibilities:
- HTF structure and zone detection
- HTF context caching (read-only for LTF)
- Impulse detection (rule-based)
- Fibonacci context creation
- Fib-0 gate logic
- Reversal context lifecycle (streaming)
- SL/TP candidate generation (no final decision)

Characteristics:
- Stateful
- Thread-affine
- No blocking I/O
- No training logic

---

## Trading.ML
**Inference-only machine learning**

Responsibilities:
- Feature schema definitions (FeatureRow contracts)
- Shared feature builders (live, backtest, training)
- Loading immutable ML.NET model artifacts (`.zip`)
- Model scoring (Reversal, Trade Decision, optional Impulse Quality)

Rules:
- Inference only
- No training pipelines
- No data loading
- No persistence

---

## Trading.Execution
**Risk management and order execution**

Responsibilities:
- Risk engine (final veto authority)
- Trade construction and validation
- Order lifecycle state machine
- Execution adapter interfaces
- Idempotency and retry handling

Notes:
- Live: real broker execution
- Backtest: simulated execution

---

## Trading.Infrastructure
**Cross-cutting, non-functional concerns**

Responsibilities:
- Structured logging (async)
- Metrics and telemetry
- Async event persistence (append-only)
- Parquet / JSONL writers
- Configuration and hosting utilities

Rules:
- No trading logic
- Must not block hot paths
- Accessed via interfaces only

---

## Application Hosts

### Trading.App.Live
Responsibilities:
- Load configuration and ML models (read-only)
- Wire modules together
- Start channels and workers
- Connect to live market data
- Execute real trades
- Emit telemetry and trade events asynchronously

---

### Trading.App.Backtest
Responsibilities:
- Deterministic replay of historical data (LTF + HTF)
- Use identical analysis, ML, and risk logic
- Simulated execution (fills, spread, slippage, fees)
- Produce reports and trade events

---

### Trading.App.Train
Responsibilities:
- Offline feature and label generation
- ML.NET LightGBM training pipelines
- Walk-forward validation
- Model export as versioned artifacts (`.zip`)

---

## Dependency Rules (Short)
- Trading.Core → no dependencies
- Trading.Market → Core
- Trading.Analysis → Core
- Trading.ML → Core
- Trading.Execution → Core
- Trading.Infrastructure → Core
- Application hosts → wire everything together only
