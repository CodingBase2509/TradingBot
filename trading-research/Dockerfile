FROM python:3.14-slim

ENV PYTHONDONTWRITEBYTECODE=1 \
    PYTHONUNBUFFERED=1 \
    PIP_DISABLE_PIP_VERSION_CHECK=1

WORKDIR /app

RUN python -m pip install --no-cache-dir --upgrade pip==26.1.2

COPY pyproject.toml README.md ./
COPY src ./src

RUN python -m pip install --no-cache-dir .

ENTRYPOINT ["trading-research"]
CMD ["--help"]
