# Trade Data Storage & Dashboard Preparation

## Purpose
Prepare the system for a future monitoring and control dashboard
without adding complexity or latency to the live trading system.

---

## Core Principle

Live trading writes **events**.
Dashboards read **projections**.

The trading core is UI-agnostic.

---

## Data Categories

### 1. Trade Lifecycle Events
Mandatory from v1.

Examples:
- TradeOpened
- TradePartiallyFilled
- TradeClosed
- TradeStoppedOut
- TradeTakeProfitHit

Each event includes:
- TradeId
- ContextId
- Symbol
- Direction
- EntryPrice
- ExitPrice (if applicable)
- SL / TP
- Size
- Timestamp

---

### 2. Decision & Reason Events
Used for analysis and model improvement.

Examples:
- TradeApproved
- TradeRejected

Includes:
- Reason codes
- ML scores
- Risk/Reward
- Cost ratios

---

### 3. Account State Snapshots
Captured periodically (rate-limited).

Includes:
- Balance
- Equity
- Margin used
- Open positions count
- Timestamp

---

## Storage Strategy (v1)

### Append-Only Event Storage
- Asynchronous writer
- No reads in live trading
- File-based storage

Recommended formats:
- Parquet (preferred)
- JSON Lines (acceptable for early v1)

Example structure:
data/events/
├── trades_opened_YYYY-MM-DD.parquet
├── trades_closed_YYYY-MM-DD.parquet
├── trade_decisions_YYYY-MM-DD.parquet
├── account_snapshots_YYYY-MM-DD.parquet

yaml
Code kopieren

---

## What We Do NOT Do in v1

- No database
- No REST API
- No queries in live trading
- No aggregations
- No dashboard backend

---

## Dashboard (v2 / v3 Vision)

Future architecture:
Event Store
↓
Projection Builder
↓
Read Models (Trades, PnL, Equity)
↓
Dashboard API
↓
Web UI

yaml
Code kopieren

The trading core will not change.

---

## Preparation Checklist (v1)

- All trades have stable TradeId
- All decisions have ContextId
- All rejections include reason codes
- Trade lifecycle events emitted
- Account snapshots emitted
- Async event writer in place