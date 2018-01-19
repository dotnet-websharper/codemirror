#load "paket-files/build/intellifactory/websharper/tools/WebSharper.Fake.fsx"
#r "System.IO.Compression.FileSystem"
open Fake
open WebSharper.Fake
open System.IO
open System.Text.RegularExpressions

let ( +/ ) a b = Path.Combine(a, b)

let targets =
    GetSemVerOf "WebSharper"
    |> ComputeVersion
    |> WSTargets.Default
    |> MakeTargets

Target "Get-Resources" <| fun () ->
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

Target "Build" DoNothing
targets.BuildDebug ==> "Build"

Target "CI-Release" DoNothing
targets.CommitPublish ==> "CI-Release"

RunTargetOrDefault "Build"
