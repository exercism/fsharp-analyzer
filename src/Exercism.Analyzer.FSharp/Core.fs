module Exercism.Analyzer.FSharp.Core

open System.Collections.Generic

type AnalysisContext =
    { SolutionFiles: string[]
      AnalysisJsonFile: string }

type AnalysisCommentType =
    | Essential
    | Actionable
    | Informative
    | Celebratory

type AnalysisComment =
    { Name: string
      Params: Dictionary<string, string>
      Type: AnalysisCommentType }

type Analysis =
    { Summary: string option
      Comments: AnalysisComment[] }
