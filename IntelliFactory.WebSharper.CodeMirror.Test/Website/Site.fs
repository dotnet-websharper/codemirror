namespace Website

open IntelliFactory.WebSharper
open IntelliFactory.WebSharper.Sitelets

type Action = Index

module Client =

    open IntelliFactory.WebSharper
    open IntelliFactory.WebSharper.Html
    open IntelliFactory.WebSharper.CodeMirror

    [<Require(typeof<CodeMirror.Resources.Modes.Haskell>)>]
    [<Sealed>]
    type Control() =
        inherit Web.Control()

        [<JavaScript>]
        override this.Body =
            let options = CodeMirror.Options(Mode = "haskell")
            let cm =
                CodeMirror.FromTextArea(
                    Dom.Document.Current.GetElementById "editor",
                    options)
            JavaScript.Log <| cm.CharCoords(CharCoords(1, 1), CoordsMode.Page)
            Div [] :> _

module Site =

    open IntelliFactory.Html

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
