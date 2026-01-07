# Impulse Detection Engine

## Pipeline Position
Triggered on LTF candle close.

## Purpose
Detect the existence of a strong impulsive move.

## Inputs
- LTF candles
- ATR

## Outputs
ImpulseContext:
- Direction
- Start / end index
- High / low

## Constraints
- Rule-based only
- No quality assessment
