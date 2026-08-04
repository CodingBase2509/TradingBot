import argparse
from collections.abc import Sequence

from trading_research import __version__


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        prog="trading-research",
        description="Reproducible jobs for trading research and model training.",
    )
    parser.add_argument("--version", action="version", version=__version__)

    subparsers = parser.add_subparsers(dest="job", metavar="JOB")
    for job in ("import", "dataset", "train", "backtest", "evaluate", "export"):
        subparsers.add_parser(job, help=f"Run the {job} job.")

    return parser


def main(argv: Sequence[str] | None = None) -> int:
    parser = build_parser()
    args = parser.parse_args(argv)

    if args.job is None:
        parser.print_help()
        return 0

    parser.error(f"The '{args.job}' job is not implemented yet.")
    return 2
