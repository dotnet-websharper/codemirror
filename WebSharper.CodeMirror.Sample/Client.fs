namespace WebSharper.CodeMirror.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Html
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.CodeMirror

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>    

    [<SPAEntryPoint>]
    let Main () =
        let editorContainer = JS.Document.GetElementById("editor-container")

        // Each language with its name, extension, and sample code
        let samples =
            [|
                "Angular", CodeMirror.Angular(), "<div *ngIf=\"visible\">Hello</div>"
                "C++", CodeMirror.Cpp(), "#include <iostream>\nint main() { std::cout << \"Hi\"; }"
                "CSS", CodeMirror.Css(), "body { color: red; }"
                "Go", CodeMirror.Go(), "package main\nfunc main() { println(\"Hello\") }"
                "HTML", CodeMirror.Html(), "<!DOCTYPE html>\n<html><body>Hello</body></html>"
                "Java", CodeMirror.Java(), "public class Main { public static void main(String[] args) {} }"
                "JavaScript", CodeMirror.Javascript(JavaScriptConfig(Typescript = true)), "const x: number = 5;"
                "Jinja", CodeMirror.Jinja(), "{% if user %}Hello {{ user }}{% endif %}"
                "JSON", CodeMirror.Json(), "{ \"key\": \"value\" }"
                "Less", CodeMirror.Less(), "@color: red; .box { color: @color; }"
                "Lezer", CodeMirror.Lezer(), "expr -> expr '+' expr"
                "Liquid", CodeMirror.Liquid(), "{{ user.name }} is logged in"
                "Markdown", CodeMirror.Markdown(), "# Hello\nThis is **bold** text."
                "PHP", CodeMirror.Php(), "<?php echo 'Hello, world!'; ?>"
                "Python", CodeMirror.Python(), "def hello():\n    print('Hello')"
                "Rust", CodeMirror.Rust(), "fn main() { println!(\"Hello\"); }"
                "Sass", CodeMirror.Sass(), "$color: red\n.box\n  color: $color"
                "SQL", CodeMirror.Sql(), "SELECT * FROM users;"
                "Vue", CodeMirror.Vue(), "<template><div>{{ msg }}</div></template>"
                "WAST", CodeMirror.Wast(), "(module (func (result i32) i32.const 42))"
                "XML", CodeMirror.Xml(), "<note><to>Tove</to></note>"
                "YAML", CodeMirror.Yaml(), "name: John\nage: 30"
            |]

        // Create each editor dynamically
        for (name, ext, code) in samples do
            let title = Elt.h3 [] [text name]
            let editorHost = Elt.div [attr.``class`` "editor"] []

            Doc.RunAppend editorContainer title
            Doc.RunAppend editorContainer editorHost

            EditorView(
                EditorViewConfig(
                    Doc = code,
                    Parent = editorHost.Dom,
                    Extensions = [| CodeMirror.BasicSetup; ext; CodeMirror.OneDark |]
                )
            ) |> ignore
