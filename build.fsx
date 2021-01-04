#if INTERACTIVE
#r "nuget: FAKE.Core"
#r "nuget: Fake.Core.Target"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.Tools.Git"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.DotNet.AssemblyInfoFile"
#r "nuget: Fake.DotNet.Paket"
#r "nuget: Paket.Core"
#else
#r "paket:
nuget FAKE.Core
nuget Fake.Core.Target
nuget Fake.IO.FileSystem
nuget Fake.Tools.Git
nuget Fake.DotNet.Cli
nuget Fake.DotNet.AssemblyInfoFile
nuget Fake.DotNet.Paket
nuget Paket.Core //"
#endif

#load "paket-files/wsbuild/github.com/dotnet-websharper/build-script/WebSharper.Fake.fsx"
#r "System.IO.Compression.FileSystem"
open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open WebSharper.Fake
open System.IO
open System.Text.RegularExpressions

let ( +/ ) a b = Path.Combine(a, b)

let targets =
    WSTargets.Default (fun () -> GetSemVerOf "WebSharper" |> ComputeVersion)
    |> MakeTargets

Target.create "Get-Resources" <| fun _ ->
    let ( @- ) a b = Seq.append a b

    let tempdir = __SOURCE_DIRECTORY__ +/ ".temp"
    Directory.CreateDirectory tempdir |> ignore

    use cl = new System.Net.WebClient()

    let version = 
        Regex(">version ((?:[0-9]|\.)*)<")
            .Match(cl.DownloadString "http://codemirror.net/doc/manual.html")
            .Groups.[1].Value

    printfn "Codemirror version: %s" version

    let zipfile =  tempdir +/ "codemirror.zip"
    let unzipped = tempdir +/ ("codemirror-" + version)

    if not <| Directory.Exists unzipped then
        printfn "donwloading http://codemirror.net/codemirror.zip"
        cl.DownloadFile("http://codemirror.net/codemirror.zip", zipfile)
        Compression.ZipFile.ExtractToDirectory(zipfile, unzipped)

    let inner = Directory.GetDirectories(unzipped).[0]

    Directory.CreateDirectory("msbuild")
    File.WriteAllText("msbuild/CodeMirrorDir.targets",
        sprintf """<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <CodeMirrorDir>%s</CodeMirrorDir>
  </PropertyGroup>
</Project>""" inner)

    let res =
        [|
            for d in [| "addon"; "keymap"; "lib"; "mode"; "theme" |] do
                let d = inner +/ d
                for p in Directory.GetFiles(d, "*.js", SearchOption.AllDirectories) do
                    if Path.GetFileName p <> "test.js" then
                        File.WriteAllText(p,
                            "if(typeof(define) == 'function'){__define__old=define;delete define}\n"
                            + File.ReadAllText(p)
                            + "\nif(typeof(__define__old) == 'function'){define=__define__old;delete __define__old}")
                        yield p
                yield! Directory.GetFiles(d, "*.css", SearchOption.AllDirectories)
        |]

    File.WriteAllLines(tempdir +/ "res.txt", res |> Seq.map (fun r -> r.[inner.Length + 1 ..]))

targets.AddPrebuild "Get-Resources"

Target.runOrDefault "Build"
