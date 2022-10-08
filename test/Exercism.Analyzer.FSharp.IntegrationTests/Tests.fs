module Exercism.Analyzer.FSharp.IntegrationTests

open System.Collections.Generic
open System.IO
open FsUnit.Xunit

open System.Text.Json
open System.Text.Json.Serialization
open Xunit

type TestAnalysis = { Expected: string; Actual: string }

type TestSolution =
    { Slug: string
      Directory: string
      DirectoryName: string }

type JsonAnalysisComment =
    { [<JsonPropertyName("comment")>]
      Name: string
      [<JsonPropertyName("params")>]
      Params: Dictionary<string, string>
      [<JsonPropertyName("type")>]
      Type: string }

type JsonAnalysis =
    { [<JsonPropertyName("summary")>]
      Summary: string
      [<JsonPropertyName("comments")>]
      Comments: JsonAnalysisComment[] }

let (</>) left right = Path.Combine(left, right)

let private jsonSerializerOptions = JsonSerializerOptions()
jsonSerializerOptions.DefaultIgnoreCondition <- JsonIgnoreCondition.WhenWritingNull

let normalizeAnalysisJson (json: string) =
    let jsonAnalysis =
        JsonSerializer.Deserialize<JsonAnalysis>(json, jsonSerializerOptions)

    let normalizedJsonTestRun =
        { jsonAnalysis with
              Comments =
                  jsonAnalysis.Comments
                  |> Array.sortBy (fun test -> test.Name) }

    let normalizeWhitespace (str: string) = str.Replace("\r\n", "\n")

    JsonSerializer.Serialize(normalizedJsonTestRun, jsonSerializerOptions)
    |> normalizeWhitespace

let private runAnalyzer testSolution =
    let run () =
        Program.main [| testSolution.Slug
                        testSolution.Directory
                        testSolution.Directory |]

    let readAnalysisResults () =
        let readAnalysisResultFile fileName =
            Path.Combine(testSolution.Directory, fileName)
            |> File.ReadAllText
            |> normalizeAnalysisJson

        { Expected = readAnalysisResultFile "analysis.json"
          Actual = readAnalysisResultFile "expected_analysis.json" }

    run () |> ignore
    readAnalysisResults ()

let private assertSolutionHasExpectedResults (slug: string) (dir: string) =
    let testSolutionDirectory =
        Path.GetFullPath(Path.Combine([| "Solutions"; dir |]))

    let testSolution =
        { Slug = slug
          Directory = testSolutionDirectory
          DirectoryName = Path.GetFileName(testSolutionDirectory) }

    let testRun = runAnalyzer testSolution
    testRun.Actual |> should equal testRun.Expected

[<Fact>]
let ``No analyzer implemented for exercise`` () =
    assertSolutionHasExpectedResults "Fake" ("General" </> "NoAnalyzerImplemented")
