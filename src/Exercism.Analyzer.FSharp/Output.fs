module Exercism.Analyzer.FSharp.Output

open System.Collections.Generic
open System.Text.Json
open System.IO
open System.Text.Json.Serialization
open Exercism.Analyzer.FSharp.Core

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

let private jsonSerializerOptions = JsonSerializerOptions()
jsonSerializerOptions.IgnoreNullValues <- true
jsonSerializerOptions.WriteIndented <- true

let private toJsonAnalysisCommentType (analysisCommentType: AnalysisCommentType) =
    match analysisCommentType with
    | Essential -> "essential"
    | Actionable -> "actionable"
    | Informative -> "informative"
    | Celebratory -> "celebratory"

let private toJsonAnalysisComment (analysisComment: AnalysisComment) =
    { Name = analysisComment.Name
      Params = analysisComment.Params
      Type = toJsonAnalysisCommentType analysisComment.Type }

let private toJsonAnalysis (analysis: Analysis) =
    { Summary = analysis.Summary |> Option.toObj
      Comments = analysis.Comments |> Array.map toJsonAnalysisComment }

let private serializeTestResults (analysis: Analysis) =
    JsonSerializer.Serialize(toJsonAnalysis analysis, jsonSerializerOptions)

let writeAnalysisJsonFile context testRun =
    File.WriteAllText(context.AnalysisJsonFile, serializeTestResults testRun)
