#load "tools/includes.fsx"
#r "System.IO.Compression.FileSystem"
open IntelliFactory.Build

let bt =
    let bt = BuildTool().PackageId("WebSharper.CodeMirror", "2.5")
    bt.WithFramework(bt.Framework.Net40)

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
                yield! Directory.GetFiles(d, "*.js", SearchOption.AllDirectories) |> Seq.filter (fun p -> Path.GetFileName p <> "test.js") 
                yield! Directory.GetFiles(d, "*.css", SearchOption.AllDirectories)
        |]

    File.WriteAllLines(tempdir +/ "res.txt", res |> Seq.map (fun r -> r.[inner.Length + 1 ..]))

    res

let main =
    bt.WebSharper.Extension("IntelliFactory.WebSharper.CodeMirror")
        .SourcesFromProject()
        .Embed(getResources())

let website =
    bt.WebSharper.Library("Website")
        .SourcesFromProject()
        .References(fun r ->
            [
                r.Assembly("System.Web")
                r.Project main
            ])

let web =
    bt.WebSharper.HostWebsite("Web")
        .References(fun r ->
            [
                r.Project main
                r.Project website
                r.NuGet("WebSharper").At(["/tools/net45/IntelliFactory.Xml.dll"]).Reference()
            ])

bt.Solution [

    main
    website
    web

    bt.NuGet.CreatePackage()
        .Add(main)
        .Description("CodeMirror bindings for WebSharper")

]
|> bt.Dispatch
