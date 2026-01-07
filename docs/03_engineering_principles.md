# Engineering Principles & System Constraints

## Purpose
This document defines non-functional requirements and engineering rules
that guide all implementation decisions.

---

## Core Principles

### Performance First
- Live trading operates under millisecond-level latency constraints.
- No blocking I/O, database access, or serialization in the hot path.
- Allocations and GC pressure must be minimized.

### Determinism
- Backtests must be fully reproducible.
- No time-based randomness in core logic.
- Clear separation between live-time and simulated-time behavior.

### Separation of Concerns
- Trading logic is independent of UI, persistence, and monitoring.
- ML training is never part of the live trading application.
- Observability is asynchronous and non-intrusive.

---

## Architectural Style

- Modular monolith
- Phase-oriented pipeline
- Event-driven (not call-stack-driven)
- In-process communication for the hot path

---

## Live vs Offline Applications

### Live Trading
- Loads immutable ML model artifacts
- Executes real trades
- Maintains in-memory state only

### Backtesting
- Deterministic data replay
- Simulated execution
- Identical analysis & decision logic

### Training
- Offline only
- Feature & label generation
- Model training and export

---

## Performance Guardrails

- Use `Channel<T>` with bounded capacity
- Prefer structs for events and primitives
- Use pooling for buffers and feature vectors
- No synchronous logging in the hot path
