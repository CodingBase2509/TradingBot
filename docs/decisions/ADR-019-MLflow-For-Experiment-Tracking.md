# ADR-019: MLflow für Experimente und Forschungsmodelle

- **Status:** beschlossen
- **Datum:** 28. Juli 2026

## Kontext

Trainingsläufe benötigen nachvollziehbare Parameter, Metriken, Herkunft und Artefakte, ohne eine eigene Forschungsverwaltung zu bauen.

## Entscheidung

- MLflow wird ausschließlich in der isolierten Trainingszone für Experimente und Forschungsartefakte verwendet.
- MLflow erteilt keine Test-, Paper- oder Livefreigabe.
- Nur ein vollständig exportiertes und geprüftes Modellpaket verlässt die Trainingszone.

## Begründung

MLflow deckt Experimenttracking mit wenig Eigenentwicklung ab, bleibt aber außerhalb des sicherheitskritischen Handelswegs.

## Folgen

- Test und Produktion benötigen keinen MLflow-Zugriff.

## Verbindliche Dokumentation

- [Training](../ml/Training.md)
- [Deployment](../architecture/Deployment.md)
