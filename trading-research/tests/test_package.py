from trading_research import __version__
from trading_research.cli import main


def test_package_has_version() -> None:
    assert __version__ == "0.1.0"


def test_cli_without_job_is_successful() -> None:
    assert main([]) == 0
