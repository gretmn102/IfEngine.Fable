// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------
#r "paket: groupref build //"
#load "./.fake/build.fsx/intellisense.fsx"
#r "netstandard"

open Fake.Core
open Fake.IO
open Fake.IO.Globbing.Operators
// --------------------------------------------------------------------------------------
// Build variables
// --------------------------------------------------------------------------------------
let f projName =
    let pattern = sprintf @"**/%s.fsproj" projName
    let xs = !! pattern
    xs
    |> Seq.tryExactlyOne
    |> Option.defaultWith (fun () ->
        xs
        |> List.ofSeq
        |> failwithf "'%s' expected exactly one but:\n%A" pattern
    )

let mainProjName = "IfEngine.Fable"
let mainProjPath = f mainProjName
let mainProjDir = Path.getDirectory mainProjPath

let deployDir = Path.getFullName "./deploy"
let deployJsDir = Path.getFullName "./deployJs"

let release = ReleaseNotes.load "RELEASE_NOTES.md"

let testGamePath = "tests/src/TestGame.fsproj"
let testGameName = Path.getFullName testGamePath
let testGameDir = Path.getDirectory testGamePath
// --------------------------------------------------------------------------------------
// Helpers
// --------------------------------------------------------------------------------------
open Fake.DotNet

let dotnet cmd workingDir =
    let result = DotNet.exec (DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

module XmlText =
    let escape rawText =
        let doc = new System.Xml.XmlDocument()
        let node = doc.CreateElement("root")
        node.InnerText <- rawText
        node.InnerXml
// --------------------------------------------------------------------------------------
// Targets
// --------------------------------------------------------------------------------------
Target.create "DotnetClean" (fun _ ->
    dotnet "clean" "."
)

Target.create "Clean" (fun _ -> Shell.cleanDir deployDir)

Target.create "CleanJs" (fun _ -> Shell.cleanDir deployJsDir)

Target.create "Meta" (fun _ ->
    [
        "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
        "<ItemGroup>"
        "    <PackageReference Include=\"Microsoft.SourceLink.GitHub\" Version=\"1.0.0\" PrivateAssets=\"All\"/>"
        "</ItemGroup>"
        "<PropertyGroup>"
        "    <EmbedUntrackedSources>true</EmbedUntrackedSources>"
        "    <PackageProjectUrl>https://github.com/gretmn102/IfEngine.Fable</PackageProjectUrl>"
        "    <PackageLicenseExpression>MIT</PackageLicenseExpression>"
        "    <RepositoryUrl>https://github.com/gretmn102/IfEngine.Fable.git</RepositoryUrl>"
        sprintf "    <PackageReleaseNotes>%s</PackageReleaseNotes>"
            (String.concat "\n" release.Notes |> XmlText.escape)
        "    <PackageTags>interactive-fiction;fsharp</PackageTags>"
        "    <Authors>Fering</Authors>"
        sprintf "    <Version>%s</Version>" (string release.SemVer)
        "</PropertyGroup>"
        "</Project>"
    ]
    |> File.write false "Directory.Build.props"
)

let commonBuildArgs = "-c Release"

Target.create "Build" (fun _ ->
    mainProjDir
    |> dotnet (sprintf "build %s" commonBuildArgs)
)

Target.create "Deploy" (fun _ ->
    mainProjDir
    |> dotnet (sprintf "build %s -o \"%s\"" commonBuildArgs deployDir)
)

Target.create "DeployJs" (fun _ ->
    let args =
        [
            "fable"
            @"src\IfEngine.Fable.fsproj"
            sprintf "--outDir %s" deployJsDir
        ]
        |> String.concat " "

    dotnet args "."
)

Target.create "Pack" (fun _ ->
    mainProjDir
    |> dotnet (sprintf "pack %s -o \"%s\"" commonBuildArgs deployDir)
)

Target.create "PushToGitlab" (fun _ ->
    let packPathPattern = sprintf "%s/*.nupkg" deployDir
    let packPath =
        !! packPathPattern |> Seq.tryExactlyOne
        |> Option.defaultWith (fun () -> failwithf "'%s' not found" packPathPattern)

    deployDir
    |> dotnet (sprintf "nuget push -s %s %s" "gitlab" packPath)
)

Target.create "BuildTestGame" (fun _ ->
    testGameDir
    |> dotnet (sprintf "build %s" commonBuildArgs)
)

let runTestGame () =
    let npmPath =
        ProcessUtils.findFilesOnPath "npm"
        |> Seq.tryHead
        |> Option.defaultWith (fun () ->
            failwithf "not found npm!"
        )

    Command.RawCommand(npmPath, Arguments.OfArgs ["run"; "dev"])
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory testGameDir
    |> Proc.run
    |> ignore

Target.create "RunTestGame" (fun _ ->
    runTestGame ()
)

Target.create "RunTestGameClean" (fun _ ->
    runTestGame ()
)

// --------------------------------------------------------------------------------------
// Build order
// --------------------------------------------------------------------------------------
open Fake.Core.TargetOperators

"Build"

"DotnetClean"
  ==> "Clean"
  ==> "Deploy"

"DotnetClean"
  ==> "CleanJs"
  ==> "DeployJs"

"DotnetClean"
  ==> "RunTestGameClean"

"RunTestGame"

"DotnetClean"
  ==> "BuildTestGame"

"DotnetClean"
  ==> "Clean"
  ==> "Meta"
  ==> "Pack"
  ==> "PushToGitlab"

Target.runOrDefault "Deploy"
