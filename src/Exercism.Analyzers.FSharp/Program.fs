module Exercism.Analyzers.FSharp.Program

open Dotnet.ProjInfo.Workspace
open System
open System.IO
open System.Reflection
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
open Fantomas

module Process =
    let exec fileName arguments workingDirectory =
        let psi = System.Diagnostics.ProcessStartInfo()
        psi.FileName <- fileName
        psi.Arguments <- arguments
        psi.WorkingDirectory <- workingDirectory
        psi.CreateNoWindow <- true
        psi.UseShellExecute <- false

        use p = new System.Diagnostics.Process()
        p.StartInfo <- psi

        p.Start() |> ignore
        p.WaitForExit()

        if p.ExitCode = 0 then Result.Ok() else Result.Error()

type TestRunContext =
    { InputFile: string
      TestsFile: string
      ProjectFile: string
      ResultsFile: string }

type CompilerError =
    | ProjectNotFound
    | TestsFileNotFound
    | CompilationFailed
    | CompilationError of FSharpErrorInfo []

let private checker = FSharpChecker.Create()

let private msBuildLocator = MSBuildLocator()

let private loaderConfig = LoaderConfig.Default msBuildLocator
let private loader = Loader.Create(loaderConfig)
let private infoConfig = NetFWInfoConfig.Default msBuildLocator
let private netFwInfo = NetFWInfo.Create(infoConfig)
let private binder = FCS.FCSBinder(netFwInfo, loader, checker)

let private dotnetRestore (projectFile: string) =
    if not (File.Exists projectFile) then
        Result.Error ProjectNotFound
    else
        let homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        let nugetDir = Path.Combine(homeDir, ".nuget")
        let arguments = sprintf "restore -s %s" nugetDir
        let workingDir = Path.GetDirectoryName projectFile
        Process.exec "dotnet" arguments workingDir |> Result.mapError (fun _ -> CompilationFailed)

let getProjectOptions (context: TestRunContext) =
    dotnetRestore context.ProjectFile
    |> Result.bind (fun _ ->
        loader.LoadProjects [ context.ProjectFile ] |> ignore
        binder.GetProjectOptions(context.ProjectFile) |> Result.mapError (fun _ -> CompilationFailed))

let private parseFile (filePath: string) (parseOptions: FSharpParsingOptions) =
    let parsedResult =
        checker.ParseFile(filePath, File.ReadAllText(filePath) |> SourceText.ofString, parseOptions)
        |> Async.RunSynchronously

    match parsedResult.ParseTree with
    | Some tree -> Result.Ok tree
    | None -> Result.Error CompilationFailed

//let private treeToCode tree =
//    CodeFormatter.FormatASTAsync(tree, "", [], None, FormatConfig.FormatConfig.Default) |> Async.RunSynchronously

let private compile (projectOptions: FCS.FCS_ProjectOptions) =
    let wholeProjectResults = checker.ParseAndCheckProject(projectOptions, "") |> Async.RunSynchronously
    let criticalErrors =
        wholeProjectResults.Errors |> Array.filter (fun error -> error.Severity = FSharpErrorSeverity.Error)

    if wholeProjectResults.HasCriticalErrors
    then Result.Ok(wholeProjectResults)
    else Result.Error(CompilationError criticalErrors)

let private compileProject (context: TestRunContext) =
    getProjectOptions context
    |> Result.bind compile

[<EntryPoint>]
let main argv =
    let context =
        { InputFile: string
          TestsFile: string
          ProjectFile: string
          ResultsFile: string }
    
    printfn "Hello World from F#!"
    0 // return an integer exit code
