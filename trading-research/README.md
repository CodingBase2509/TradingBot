# Trading Research

Isolierte Python-Umgebung für Datenaufbereitung, Forschung, Training,
Backtesting, Evaluation und den Export geprüfter Modellpakete.

Die offizielle Projektlogik liegt im installierbaren Paket unter `src/`.
Notebooks dürfen später zur Erkundung verwendet werden, enthalten aber nie die
einzige Implementierung eines offiziellen Ablaufs.

## Lokale Einrichtung

Benötigt wird CPython 3.14. Das auf macOS vorinstallierte System-Python sollte
nicht verwendet oder verändert werden.

```bash
cd trading-research
python3.14 -m venv .venv
source .venv/bin/activate
python -m pip install --upgrade pip==26.1.2
python -m pip install --editable '.[dev]'
```

Windows PowerShell aktiviert die Umgebung stattdessen mit:

```powershell
.venv\Scripts\Activate.ps1
```

## Prüfung

```bash
python -m pytest
python -m ruff check .
python -m ruff format --check .
trading-research --help
```

## Bereiche

| Bereich | Verantwortung |
|---|---|
| `contracts` | gemeinsame Datenverträge, Manifeste und stabile Codes |
| `data` | Import, Qualität, kanonische Marktdaten und Datasets |
| `research` | Features, Kandidaten, Labels und historische Simulation |
| `modeling` | Training, Evaluation, MLflow, ONNX und Modellpakete |
| `jobs` | bekannte reproduzierbare CLI-Abläufe |

## Abhängigkeiten

- PyArrow liest und schreibt Parquet und hält die verbindlichen Datenschemas.
- pandas dient als V1-DataFrame für Forschung und ML-Vorbereitung.
- pytest führt Tests aus.
- Ruff formatiert und prüft Python-Code.

Exakte transitive Abhängigkeiten werden mit Python 3.14 vor der ersten
fachlichen Implementierung in einer Lockdatei fixiert.
