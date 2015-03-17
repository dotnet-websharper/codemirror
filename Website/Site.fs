namespace Website

open WebSharper

type Action = Index

module Client =

    open WebSharper.JavaScript
    open WebSharper.Html.Client
    open WebSharper.CodeMirror

    [<Require(typeof<CodeMirror.Resources.Modes.Javascript>)>]
    [<Require(typeof<CodeMirror.Resources.Modes.Xml>)>]
    [<Require(typeof<CodeMirror.Resources.Modes.Htmlmixed>)>]
    [<Sealed>]
    type Control() =
        inherit Web.Control()

        [<JavaScript>]
        override this.Body =
            let options =
                CodeMirror.Options(
                    Mode = "javascript",
                    LineNumbers = true,
                    ExtraKeys = New [
                        "Ctrl-Space", box(fun cm ->
                            CodeMirror.ShowHint(cm, Hint.JavaScript()))
                    ]
                )
            let cm =
                CodeMirror.FromTextArea(
                    JS.Document.GetElementById "editor",
                    options)
            // cm.OnCursorActivity(fun cm ->
            //             cm.MatchHighlight(MatchHighlighter("match-highlight")))
            //cm.OnGutterClick(CodeMirror.NewFoldFunction(RangeFinder(CodeMirror.IndentRangeFinder)))
            Console.Log (cm.CharCoords(CharCoords(1, 1), CoordsMode.Page))
            let dial = cm.OpenDialog(Dialog("bar:<input/>"), fun x -> Console.Log(x))
            let resultContainer = Pre [Attr.Class "cm-s-default"]

//            let cmXml =
//                let options =
//                    CodeMirror.Options(
//                        Mode = "text/html",
//                        ExtraKeys = New [
//                            "'>'", box(fun (cm: CodeMirror) -> cm.CloseTag.CloseTag(cm, ">"))
//                            "'/'", box(fun (cm: CodeMirror) -> cm.CloseTag.CloseTag(cm, "/"))
//                        ]
//                    )
//                CodeMirror.FromTextArea(
//                    JS.Document.GetElementById "editor-xml", options)

            Div [
                Button [Text "Run mode"]
                |>! OnClick (fun _ _ ->
                    CodeMirror.RunMode(cm.GetValue(), "javascript",
                        RunModeOutput(resultContainer.Dom)))
                Button [Text "Clear"]
                |>! OnClick (fun _ _ -> resultContainer.Clear())
                resultContainer
            ] :> _

open WebSharper.Sitelets

module Site =

    open WebSharper.Html.Server

    type Page =
        {
            Title : string
            Body : list<Content.HtmlElement>
        }

    let Template =
        Content.Template<Page>(System.Web.HttpContext.Current.Server.MapPath "/Main.html")
            .With("title", fun p -> p.Title)
            .With("body", fun p -> p.Body)

    let Index =
        Content.WithTemplate Template (fun ctx ->
            {
                Title = "CodeMirror sample"
                Body = [Div [new Client.Control()]]
            })

/// The class that contains the website
type Website() =
    interface IWebsite<Action> with
        member this.Sitelet = Sitelet.Content "/" Index Site.Index
        member this.Actions = [Index]

[<assembly: WebsiteAttribute(typeof<Website>)>]
do ()
