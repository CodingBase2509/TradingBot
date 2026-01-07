# Architecture Decisions & Rationale

## Architectural Style
The system is implemented as a **modular monolith** with clear domain boundaries.

Reasoning:
- Millisecond-level latency requirements
- Stateful trading contexts
- Deterministic execution
- Minimal overhead in hot paths

Microservices are intentionally avoided in the live trading path.

## Execution Model
- Single process
- Phase-oriented pipeline
- Thread-affinity per symbol where possible
- Event-driven, not call-stack-driven

## Live vs Offline Separation
Three separate applications exist:
- Live Trading
- Backtesting
- Model Training

Live trading:
- Loads immutable, pre-trained ML models
- No training or data mutation

Backtest / Training:
- Deterministic execution
- Synchronous data flow
- No live threads or timers

## Event Transport
- In-process event dispatch
- No serialization
- No external brokers
- Bounded queues for backpressure

## Data Storage Strategy
- Hot state: in-memory only
- Warm storage: async append-only (Parquet)
- Cold storage: offline analytics & training

Live trading never reads from databases.

## ML Architecture
- ML inference only in live trading
- Training happens offline
- Models are versioned artifacts
- ML is always gated by rule-based context

## Scalability Path
If scaling becomes necessary:
- ML inference → separate process
- Execution → separate process
- Market data → separate process

All without changing core domain logic.
