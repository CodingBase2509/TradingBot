# Logging & Telemetry Design

## Goals
- Explainability of decisions
- Operational visibility
- Post-mortem analysis
- Zero impact on trading latency

---

## Hot Path vs Warm Path

### Hot Path
- Market data ingestion
- Analysis & ML inference
- Trade decision & execution request

Rules:
- No synchronous logging
- No string formatting
- No I/O
- No blocking calls

---

### Warm Path (Asynchronous)
- Logging
- Metrics export
- Event persistence
- Debug traces

---

## Logging Strategy

### Logging Levels
- Trace: disabled in live
- Debug: development / backtest only
- Information: business events
- Warning: trade rejections, fallbacks
- Error: unexpected failures
- Critical: system not operable

---

### What Is Logged
- Impulse detected
- Reversal approved / rejected
- Trade approved / rejected
- Order state changes
- System errors

Logs are **decision-focused**, not data dumps.

---

### Structured Logging
All logs are structured and include:
- ContextId
- TradeId (if applicable)
- Symbol
- Timeframe
- Reason codes

No free-form text logging.

---

## Decision Reason Codes

Examples:
- IMPULSE_WEAK
- FIB_CONTEXT_EXPIRED
- REVERSAL_SCORE_TOO_LOW
- RR_TOO_LOW
- COST_TOO_HIGH
- HTF_CONFLICT
- ML_TIMEOUT
- EXECUTION_REJECTED

Reason codes are mandatory for all trade rejections.

---

## Metrics & Telemetry

### Required Metrics

#### Performance
- Candle-close to decision latency (ms)
- ML inference latency (p50/p95/p99)
- Channel queue depth
- ML timeout count

#### Trading KPIs
- Trades per symbol / timeframe
- Rejection count per reason
- Average RR
- Average SL distance

#### System Health
- Feed lag
- Reconnect count
- Memory usage
- GC pauses

---

## Implementation Notes
- Use `System.Diagnostics.Metrics`
- Counters, histograms, gauges only
- Export via OpenTelemetry or Prometheus
- Metrics collection must be allocation-free
