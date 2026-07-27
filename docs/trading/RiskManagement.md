# Risiko- und Sicherheitskonzept

## Grundsatz

Das Modell entscheidet, welchen Trade es für sinnvoll hält. Der Risk Guard entscheidet unveränderbar, ob der Trade sicher, technisch möglich und regelkonform ist.

## Schutzebenen

1. Eingabequalität und Datenalter;
2. Plausibilität der Modellausgabe;
3. Vorhandelsprüfung von Risiko, Kosten und Portfolio;
4. Gültigkeit und Schutz der Brokerorder;
5. laufender Abgleich von Order und Position;
6. Tages-, Drawdown- und Gesamtrisiko;
7. betrieblicher Zustand von Zeit, Verbindungen, Speicher und Überwachung.

## Ein Trade wird blockiert, wenn

- Daten fehlen, zu alt oder widersprüchlich sind;
- Feature- und Modellversion nicht zusammenpassen;
- Modellwerte ungültig oder unplausibel sind;
- Spread oder erwartete Kosten zu hoch sind;
- Broker- oder Kontozustand unklar ist;
- die kleinste gültige Order das Limit überschreitet;
- Einzeltrade-, Portfolio- oder Frequenzgrenzen verletzt würden;
- ein definierter Sperrzeitraum gilt.

## Gesamter Handelsstopp

- Tagesverlust oder maximaler Drawdown erreicht;
- interne und Brokerpositionen weichen ab;
- wiederholte Order- oder Verbindungsfehler;
- Marktdatenquelle oder Aufzeichnung fällt aus;
- ungewöhnliche Signal- oder Orderfrequenz;
- fehlerhafte Zeitstempel;
- reale Ergebnisse weichen stark vom erwarteten Bereich ab.

## Frühe Arbeitswerte

| Grenze | Vorschlag, noch nicht beschlossen |
|---|---:|
| Risiko pro Trade | maximal 0,25 % des Kontowerts |
| Tagesverlust | maximal 1,0 % |
| offene Positionen | maximal 2 |
| neue Trades pro Tag | maximal 10 |
| Orderfehler bis Stopp | 3 in Folge |

Ist ein Micro-Kontrakt bereits zu groß für das Risikolimit, wird nicht gehandelt.

## Kill Switch

Der Not-Aus ist manuell und automatisch auslösbar, verhindert sofort neue Orders und verwendet eine vorab getestete Regel für offene Positionen. „Sofort alles schließen“ ist nicht in jeder Ausfallart automatisch die sicherste Handlung.
