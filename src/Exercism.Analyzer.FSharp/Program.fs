module Exercism.Analyzer.FSharp.Program

open System
open System.IO
open CommandLine
open Humanizer
open Exercism.Analyzer.FSharp.Core
open Exercism.Analyzer.FSharp.Analyzer
open Exercism.Analyzer.FSharp.Output

type Options =
    { [<Value(0, Required = true, HelpText = "The solution's exercise")>]
      Slug: string
      [<Value(1, Required = true, HelpText = "The directory containing the solution")>]
      InputDirectory: string
      [<Value(2, Required = true, HelpText = "The directory to which the results will be written")>]
      OutputDirectory: string }

let private parseOptions argv =
    match Parser.Default.ParseArguments<Options>(argv) with
    | :? (Parsed<Options>) as options -> Some options.Value
    | _ -> None

let private createAnalysisContext options =
    let exercise = options.Slug.Dehumanize().Pascalize()
    let (</>) left right = Path.Combine(left, right)

    // TODO: read solution files from .meta/config.json
    { SolutionFiles = [| options.InputDirectory </> sprintf "%s.fs" exercise |]
      AnalysisJsonFile = options.OutputDirectory </> "analysis.json" }

let private runAnalyzer options =
    let currentDate () = DateTimeOffset.UtcNow.ToString("u")

    printfn "[%s] Running analyzer for '%s' solution..." (currentDate ()) options.Slug

    let context = createAnalysisContext options
    let analysis = analyze context
    writeAnalysisJsonFile context analysis

    printfn "[%s] Ran analyzer for '%s' solution" (currentDate ()) options.Slug

[<EntryPoint>]
let main argv =
    match parseOptions argv with
    | Some options ->
        runAnalyzer options
        0
    | None -> 1