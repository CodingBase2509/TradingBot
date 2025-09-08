# Speicherung & Telemetrie

## Parquet (History/Features)
- **Warum:** klein, schnell, typ‑sicher (Schema), ideal für Training/Backtests
- **Typische Schemata**
  - Bars: `timestamp_utc, symbol, open, high, low, close, volume`
  - Features: `timestamp_utc, symbol, <features...>, label`

## PostgreSQL (OLTP für Live‑Telemetry)
**Tabellen (vereinfachter Entwurf)**
```sql
create table strategy_decision (
  id uuid primary key,
  ts timestamptz not null,
  symbol text not null,
  timeframe text not null,
  model_version text not null,
  config_version text not null,
  score double precision not null,
  reason_codes text[] not null,
  entry_plan text not null,
  side text not null,
  entry_price numeric(38,9) not null,
  sl_price numeric(38,9) not null,
  tp_price numeric(38,9) not null,
  risk_per_unit numeric(38,9) not null,
  reward_per_unit numeric(38,9) not null,
  rr_costed double precision not null,
  cost_round numeric(38,9) not null,
  equity numeric(38,9) not null,
  risk_pct double precision not null,
  qty numeric(38,9) not null,
  features jsonb
);

create table orders (
  id uuid primary key,
  trade_id uuid not null references strategy_decision(id),
  ts timestamptz not null,
  symbol text not null,
  side text not null,
  type text not null,
  status text not null,
  price numeric(38,9),
  stop_price numeric(38,9),
  avg_fill_price numeric(38,9),
  qty numeric(38,9) not null,
  filled_qty numeric(38,9) default 0,
  exchange_order_id text,
  tag text
);

create table fills (
  id bigserial primary key,
  order_id uuid not null references orders(id),
  ts timestamptz not null,
  price numeric(38,9) not null,
  qty numeric(38,9) not null,
  fee numeric(38,9) not null,
  fee_asset text not null
);

create table trades_outcome (
  trade_id uuid primary key references strategy_decision(id),
  open_ts timestamptz not null,
  close_ts timestamptz,
  entry_price numeric(38,9) not null,
  exit_price numeric(38,9),
  exit_reason text,
  gross_pnl numeric(38,9),
  fees_total numeric(38,9),
  net_pnl numeric(38,9),
  mae_atr double precision,
  mfe_atr double precision
);
```

**EF Core Tipps**
- `timestamptz` für UTC‑Zeit; `numeric(38,9)` für Preise/Mengen
- Arrays (`text[]`) & `jsonb` via Npgsql‑Provider
- Indizes: `(symbol, ts)`; optional GIN auf `features`

## Dashboards (Grafana, Postgres‑Datasource)
**Daily Net PnL**
```sql
SELECT date_trunc('day', close_ts) AS day,
       SUM(COALESCE(net_pnl,0))    AS net_pnl
FROM trades_outcome
WHERE close_ts IS NOT NULL
GROUP BY day
ORDER BY day;
```
**Trades Table**
```sql
SELECT d.ts AS decided_at, d.symbol, d.side, d.entry_plan,
       d.entry_price, d.sl_price, d.tp_price,
       d.rr_costed, o.status, o.avg_fill_price,
       t.exit_reason, t.net_pnl, d.model_version
FROM strategy_decision d
LEFT JOIN orders o ON o.trade_id = d.id AND o.tag = 'PARENT'
LEFT JOIN trades_outcome t ON t.trade_id = d.id
ORDER BY d.ts DESC
LIMIT 200;
```

## Observability
- **Serilog** (strukturierte JSON‑Logs; Enricher für `trade_id`, `model_version`)
- **OpenTelemetry**: Metriken (Counters/Histograms), Traces (Decide→Place→Fill)
- **Alerts:** Kill‑Switch, Tagesverlust, Spread‑Ausreißer, Reconnects, Score‑Drift

## Retention
- OLTP: 6–12 Monate hot; ältere Events nach **Parquet** abrollen
- Backups: DB‑Snapshots + Parquet im Objektspeicher
