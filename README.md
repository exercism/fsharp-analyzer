# Exercism F# analyzer

An [analyzer][analyzer-introduction] can automatically detect issues with submissions and comment on them.

The F# analyzer implements the [analyzer interface][analyzer-interface]. It uses the [F# Compiler service][fsharp-compiler-service] to parse the submission's source code into syntax trees, which are then analyzed for known patterns.

## Analyzing a solution

To analyze a solution, follow these steps:

1. Open a command prompt in the root directory.
1. Run `./analyze.ps1 <exercise> <directory>`. This script will run the analyzer on the specified directory.
1. Once the script has completed, the analysis results will be written to `<directory>/analysis.json`.

## Analyzing a solution using Docker

To analyze a solution using a Docker container, follow these steps:

1. Open a command prompt in the root directory.
1. Run `./run-in-docker.ps1 <exercise> <directory>`. This script will:
   1. Build the analyzer Docker image (if necessary).
   1. Run the analyzer Docker image (as a container), passing the specified `exercise` and `directory` arguments.
1. Once the script has completed, the analysis result can be found at `<directory>/analysis.json`.

TODO: update this section
Note that the Docker image is built using the [.NET IL Linker](https://github.com/dotnet/core/blob/master/samples/linker-instructions.md#using-the-net-il-linker), which is why building can be quite slow.

## Source code formatting

TODO: add source code formatting

### Scripts

The scripts in this repository are written in PowerShell. As PowerShell is cross-platform nowadays, you can also install it on [Linux](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-linux?view=powershell-6) and [macOS](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-macos?view=powershell-6).

[analyzer-introduction]: https://github.com/exercism/automated-analysis/blob/master/docs/analyzers/introduction.md
[analyzer-interface]: https://github.com/exercism/automated-analysis/blob/master/docs/analyzers/interface.md
[fsharp-compiler-service]: https://fsharp.github.io/FSharp.Compiler.Service/
