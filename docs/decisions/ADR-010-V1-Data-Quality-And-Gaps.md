# ADR-010: Datenqualität und Lückentoleranz der V1

- **Status:** beschlossen
- **Datum:** 27. Juli 2026

## Kontext

Kleine ältere Lücken sollen nicht jeden Lauf unbrauchbar machen; entscheidungsrelevante Unsicherheit darf aber keine Order erzeugen.

## Entscheidung

- Grün ist vollständig oder sicher rekonstruierbar, Gelb erlaubt höchstens drei ältere 5-Minuten-Lücken außerhalb benötigter Fenster, Rot bezeichnet aktuelle, relevante oder größere Lücken.
- Rot blockiert neue Trades. Gelb ist nur mit explizitem Missing-Data-Vertrag zulässig.
- Unbekannte Marktpreise werden nicht interpoliert.

## Begründung

Die abgestufte Regel erhält nutzbare Historie, ohne fehlende Information als bekannte Preisbewegung auszugeben.

## Folgen

- Qualitätsstatus und Lückenlage sind Bestandteil von Features, Audit und Golden Tests.

## Verbindliche Dokumentation

- [MarketData](../trading/MarketData.md)
- [FeatureEngineering](../ml/FeatureEngineering.md)
