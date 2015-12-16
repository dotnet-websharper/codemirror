#load "tools/includes.fsx"
#r "System.IO.Compression.FileSystem"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Zafir.CodeMirror")
        .VersionFrom("Zafir")
        .WithFSharpVersion(FSharpVersion.FSharp30)
        .WithFramework(fun fw -> fw.Net40)

open System.IO
let ( +/ ) a b = Path.Combine(a, b)

open System.Text.RegularExpressions

let getResources() =
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

    res

let main =
    bt.Zafir.Extension("WebSharper.CodeMirror")
        .SourcesFromProject()
        .Embed(getResources())

let website =
    bt.WithFSharpVersion(FSharpVersion.FSharp31).WithFramework(fun fw -> fw.Net45).Zafir.Library("Website")
        .SourcesFromProject()
        .References(fun r ->
            [
                r.Assembly("System.Web")
                r.NuGet("Zafir.Html").Latest(allowPreRelease=true).ForceFoundVersion().Reference()
                r.Project main
            ])

let web =
    bt.Zafir.HostWebsite("Web")
        .References(fun r ->
            [
                r.Project main
                r.Project website
                // r.NuGet("Zafir").At(["/tools/net45/IntelliFactory.Xml.dll"]).Reference()
                r.NuGet("Zafir.Html").Latest(allowPreRelease=true).ForceFoundVersion().Reference()
            ])

bt.Solution [

    main
    website
    web

    bt.NuGet.CreatePackage()
        .Add(main)
        .Description("CodeMirror bindings for Zafir")

]
|> bt.Dispatch
