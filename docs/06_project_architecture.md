# Project Architecture – Trading System (Engineering View)

## Purpose
This document describes the complete **project and solution architecture** from a **software engineering perspective**, focusing on:

- Low-latency live trading
- Deterministic backtesting
- ML.NET + LightGBM (C# only) for training and inference
- Minimal allocations / minimal GC pressure in hot paths
- Clean separation of concerns
- Logging & telemetry designed for performance
- Preparation for a future dashboard / control UI (v2/v3) without extra work in v1

---

## Architectural Style
- **Modular Monolith**
- **Single process in v1** for the trading core
- **Event-driven pipeline** (not call-stack driven)
- **In-process communication** (no network / no serialization in the hot path)
- **No microservices in the hot path**

Microservices are intentionally avoided for decision-making to keep latency predictable and reduce operational complexity. Isolation is achieved via internal modules, channels, and dedicated workers.

---

## Core Engineering Decisions (Summary)

### Language, ML, and Apps
- The entire system is built in **C#/.NET**.
- ML models are trained in C# using **ML.NET + LightGBM**.
- Three separate application hosts:
  - **Live Trading**: loads immutable models, performs inference only
  - **Backtesting**: deterministic replay + simulated execution
  - **Training**: offline feature/label generation + training + model export

### Concurrency and Communication
- Use `System.Threading.Channels` (`Channel<T>`) for in-process eventing.
- Prefer **bounded channels** with explicit backpressure.
- Use **struct-based events** and small DTOs to minimize allocations.
- Use **thread-affinity** for stateful analysis (per symbol or partition).
- ML inference and execution are isolated via separate channels and workers.

### Performance Guardrails
- No synchronous I/O in hot path (no DB, no file writes, no HTTP calls).
- No synchronous logging in hot path.
- Minimal allocations:
  - `readonly struct` / `record struct` events and primitives
  - `ArrayPool<T>` / object pooling for buffers
- Avoid strings in hot path. Use IDs (e.g., `SymbolId`) instead.

---

## Solution Structure (with Solution Folders)

```text
TradingBot.sln
│
├── src
│   ├── Core
│   │   └── Trading.Core
│   │
│   ├── Trading
│   │   ├── Trading.Market
│   │   ├── Trading.Analysis
│   │   ├── Trading.ML
│   │   ├── Trading.Execution
│   │   └── Trading.Infrastructure
│   │
│   └── Applications
│       ├── Trading.App.Live
│       ├── Trading.App.Backtest
│       └── Trading.App.Train
│
├── tests
│   ├── Trading.Core.Tests
│   ├── Trading.Analysis.Tests
│   ├── Trading.ML.Tests
│   └── Trading.Execution.Tests
│
└── docs
````
