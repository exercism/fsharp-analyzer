# Exercism F# analyzer

An [analyzer][analyzer-introduction] can automatically detect issues with submissions and comment on them.

The F# analyzer implements the [analyzer interface][analyzer-interface]. It uses the [F# Compiler service][fsharp-compiler-service] to parse the submission's source code into syntax trees, which are then analyzed for known patterns.

## Analyzing a solution

To analyze a solution, follow these steps:

1. Open a command prompt in the root directory.
1. Run `./bin/run.sh <exercise> <source-directory> <output-directory>`. This script will run the analyzer on the specified directory.
1. Once the script has completed, the analysis results will be written to `<output-directory>/analysis.json`.

## Analyzing a solution using Docker

To analyze a solution using a Docker container, follow these steps:

1. Open a command prompt in the root directory.
1. Run `./bin/run-in-docker.sh <exercise> <source-directory> <output-directory>`. This script will:
   1. Build the analyzer Docker image (if necessary).
   1. Run the analyzer Docker image (as a container), passing the specified `exercise`, `source-directory` and `output-directory` arguments.
1. Once the script has completed, the analysis result can be found at `<output-directory>/analysis.json`.

Note that the Docker image is built using the [.NET IL Linker](https://github.com/dotnet/core/blob/master/samples/linker-instructions.md#using-the-net-il-linker), which is why building can be quite slow.

[analyzer-introduction]: https://github.com/exercism/automated-analysis/blob/master/docs/analyzers/introduction.md
[analyzer-interface]: https://github.com/exercism/automated-analysis/blob/master/docs/analyzers/interface.md
[fsharp-compiler-service]: https://fsharp.github.io/FSharp.Compiler.Service/
