namespace WebSharper.CodeMirror.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.CodeMirror

[<JavaScript>]
module Client =

    // The template is the whole document, loaded from the DOM, so editing
    // index.html and refreshing the browser is enough.
    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    // ---------------------------------------------------------------
    // State bound into the template
    // ---------------------------------------------------------------

    let notice = Var.Create ""
    let status = Var.Create "Ready."

    // Theme: light is the default; the choice is persisted in localStorage.
    // The storage key is shared with the bootstrap script in index.html.
    let themeStorageKey = "websharper-codemirror-theme"
    let theme = Var.Create "light"
    let themeLabel = Var.Create "🌙 Dark mode"

    let language = Var.Create "JavaScript"

    let docInfo = Var.Create "-"
    let cursorInfo = Var.Create "-"
    let diagInfo = Var.Create "-"
    let searchInfo = Var.Create "-"
    let completionInfo = Var.Create "-"
    let mergeInfo = Var.Create "-"
    let helperInfo = Var.Create "-"

    // Holds the CodeMirror theme extension so it can be swapped in place.
    let themeCompartment = Compartment()

    let mutable view : option<EditorView> = None
    let mutable mergeEditor : option<EditorView> = None
    let mutable bound = false

    // ---------------------------------------------------------------
    // Languages
    // ---------------------------------------------------------------

    let sampleCode (name: string) =
        match name with
        | "Angular" -> "<div *ngIf=\"visible\">Hello {{ name }}</div>"
        | "C++" -> "#include <iostream>\n\nint main() {\n  std::cout << \"Hello\";\n  return 0;\n}"
        | "CSS" -> "body {\n  color: red;\n}\n\n/* TODO: fix the layout */"
        | "Go" -> "package main\n\nimport \"fmt\"\n\nfunc main() {\n\tfmt.Println(\"Hello\")\n}"
        | "HTML" -> "<!DOCTYPE html>\n<html>\n  <body>\n    <h1>Hello</h1>\n  </body>\n</html>"
        | "Java" -> "public class Main {\n  public static void main(String[] args) {\n    System.out.println(\"Hi\");\n  }\n}"
        | "JavaScript" ->
            "// TypeScript-aware when configured\nconst x: number = 5;\n\nfunction double(n: number) {\n  return n * 2; // TODO: handle bigint\n}\n\nconsole.log(double(x));"
        | "Jinja" -> "{% if user %}Hello {{ user }}{% endif %}"
        | "JSON" -> "{ \"name\": \"WebSharper\", \"tags\": [\"F#\", \"C#\"] }"
        | "Less" -> "@color: red;\n.box {\n  color: @color;\n}"
        | "Lezer" -> "expr -> expr '+' expr"
        | "Liquid" -> "{{ user.name }} is logged in"
        | "Markdown" -> "# Hello\n\nThis is **bold** text.\n\n- one\n- two"
        | "PHP" -> "<?php echo 'Hello, world!'; ?>"
        | "Python" -> "def hello():\n    print('Hello')  # TODO\n\nhello()"
        | "Rust" -> "fn main() {\n    println!(\"Hello\");\n}"
        | "Sass" -> "$color: red\n.box\n  color: $color"
        | "SQL" -> "select id, name from users where active = true;"
        | "Vue" -> "<template><div>{{ msg }}</div></template>"
        | "WAST" -> "(module (func (result i32) i32.const 42))"
        | "XML" -> "<note>\n  <to>Tove</to>\n</note>"
        | "YAML" -> "name: John\nage: 30"
        | _ -> ""

    let languageExtension (name: string) : obj =
        match name with
        | "Angular" -> CodeMirror.Angular()
        | "C++" -> CodeMirror.Cpp()
        | "CSS" -> CodeMirror.Css()
        | "Go" -> CodeMirror.Go()
        | "HTML" -> CodeMirror.Html(HtmlConfig(AutoCloseTags = true, MatchClosingTags = true))
        | "Java" -> CodeMirror.Java()
        | "JavaScript" -> CodeMirror.Javascript(JavaScriptConfig(Typescript = true))
        | "Jinja" -> CodeMirror.Jinja()
        | "JSON" -> CodeMirror.Json()
        | "Less" -> CodeMirror.Less()
        | "Lezer" -> CodeMirror.Lezer()
        | "Liquid" -> CodeMirror.Liquid()
        | "Markdown" -> CodeMirror.Markdown(MarkdownConfig(AddKeymap = true, CompleteHTMLTags = true))
        | "PHP" -> CodeMirror.Php()
        | "Python" -> CodeMirror.Python()
        | "Rust" -> CodeMirror.Rust()
        | "Sass" -> CodeMirror.Sass(SassConfig(Indented = true))
        | "SQL" -> CodeMirror.Sql(SQLConfig(Dialect = CodeMirror.PostgreSQL, UpperCaseKeywords = true))
        | "Vue" -> CodeMirror.Vue()
        | "WAST" -> CodeMirror.Wast()
        | "XML" -> CodeMirror.Xml(XMLConfig(AutoCloseTags = true))
        | "YAML" -> CodeMirror.Yaml()
        | _ -> CodeMirror.Javascript()

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    let byId (id: string) =
        let el = JS.Document.GetElementById id
        if isNull el then None else Some el

    let withView (f: EditorView -> unit) =
        match view with
        | Some v -> f v
        | None -> notice.Value <- "The editor is not ready yet."

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <- if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let themeExtension () : obj =
        if theme.Value = "dark" then box CodeMirror.OneDark else box [| |]

    let stateCommandTarget (v: EditorView) =
        StateCommandTarget(v.State, (fun (t: Transaction) -> v.Dispatch(t)))

    let runState (cmd: System.Func<StateCommandTarget, bool>) (v: EditorView) =
        cmd.Invoke(stateCommandTarget v)

    let runCommand (cmd: System.Func<EditorView, bool>) =
        withView (fun v ->
            let ok = cmd.Invoke(v)
            status.Value <- if ok then "Command applied." else "Command had no effect here.")

    let runStateCommand (cmd: System.Func<StateCommandTarget, bool>) =
        withView (fun v ->
            let ok = runState cmd v
            status.Value <- if ok then "Command applied." else "Command had no effect here.")

    // ---------------------------------------------------------------
    // Readouts
    // ---------------------------------------------------------------

    let refreshReadout () =
        match view with
        | None -> ()
        | Some v ->
            let state = v.State
            let doc = state.Doc
            let selection = state.Selection.Main
            let line = doc.LineAt(selection.Head)
            let selected = state.SliceDoc(selection.From, selection.To)
            docInfo.Value <- sprintf "%d lines · %d chars" doc.Lines doc.Length
            cursorInfo.Value <-
                sprintf "line %d, column %d · selection %d–%d (%d selected)"
                    line.Number (selection.Head - line.From + 1) selection.From selection.To selected.Length
            diagInfo.Value <- sprintf "%d diagnostic(s)" (CodeMirror.DiagnosticCount(state))

            let completionState = CodeMirror.CompletionStatus(state)
            let completions = CodeMirror.CurrentCompletions(state)
            completionInfo.Value <-
                if isNull completionState then "no active completion"
                else sprintf "%s · %d option(s)" completionState completions.Length

            let query = CodeMirror.GetSearchQuery(state)
            searchInfo.Value <-
                if isNull query then "no search query"
                else
                    sprintf "\"%s\" · panel %s"
                        query.Search
                        (if CodeMirror.SearchPanelOpen(state) then "open" else "closed")

    let refreshMerge () =
        match mergeEditor with
        | None -> mergeInfo.Value <- "unavailable"
        | Some v ->
            let result = CodeMirror.GetChunks(v.State)
            mergeInfo.Value <- sprintf "%d differing chunk(s) · side %s" result.Chunks.Length result.Side

    let originalDoc =
        "function add(a, b) {\n  return a - b;\n}\n\nfunction mul(a, b) {\n  return a * b;\n}\n"

    let modifiedDoc =
        "function add(a, b) {\n  return a + b;\n}\n\nfunction mul(a, b) {\n  return a * b;\n}\n\nfunction div(a, b) {\n  return a / b;\n}\n"

    let refreshHelpers () =
        let parts = ResizeArray<string>()
        parts.Add(sprintf "findClusterBreak(\"a👍b\", 1) = %d" (CodeMirror.FindClusterBreak("a👍b", 1)))
        parts.Add(sprintf "codePointAt(\"👍\", 0) = %d" (CodeMirror.CodePointAt("👍", 0)))
        parts.Add(sprintf "fromCodePoint(128077) = \"%s\"" (CodeMirror.FromCodePoint(128077)))
        parts.Add(sprintf "codePointSize(128077) = %d" (CodeMirror.CodePointSize(128077)))
        parts.Add(sprintf "countColumn(\"    x\", 4, 3) = %d" (CodeMirror.CountColumn("    x", 4, 3)))
        parts.Add(sprintf "findColumn(\"    x\", 3, 4) = %d" (CodeMirror.FindColumn("    x", 3, 4)))
        match view with
        | None -> ()
        | Some v ->
            let state = v.State
            parts.Add(sprintf "getIndentUnit = %d" (CodeMirror.GetIndentUnit(state)))
            parts.Add(sprintf "indentString(state, 1) = \"%s\"" (CodeMirror.IndentString(state, 1)))
            parts.Add(sprintf "syntaxTreeAvailable(state, 0) = %b" (CodeMirror.SyntaxTreeAvailable(state, 0)))
            parts.Add(sprintf "foldedRanges = %d" (CodeMirror.FoldedRanges(state).Size))
            parts.Add(sprintf "highlightingFor(state) = \"%s\"" (CodeMirror.HighlightingFor(state, [| |])))
            parts.Add(sprintf "findFromDOM(editor.dom) = %b" (not (isNull (box (EditorView.FindFromDOM(v.Dom))))))
        parts.Add(sprintf "diff = %d change(s)" (CodeMirror.Diff(originalDoc, modifiedDoc, DiffConfig(ScanLimit = 500)).Length))
        parts.Add(sprintf "presentableDiff = %d change(s)" (CodeMirror.PresentableDiff(originalDoc, modifiedDoc).Length))
        let query = SearchQuery(SearchQueryConfig("function", CaseSensitive = false))
        parts.Add(sprintf "SearchQuery(\"function\").valid = %b, caseSensitive = %b" query.Valid query.CaseSensitive)
        helperInfo.Value <- String.concat "  ·  " (parts.ToArray())

    // ---------------------------------------------------------------
    // Diagnostics
    // ---------------------------------------------------------------

    let sampleLintSource (v: EditorView) : Diagnostic[] =
        let doc = v.State.Doc
        let mutable diags : Diagnostic[] = [| |]
        for i in 1 .. doc.Lines do
            let line = doc.Line(i)
            let text = line.Text
            let todo = text.IndexOf("TODO")
            if todo >= 0 then
                diags <-
                    Array.append diags
                        [| Diagnostic(line.From + todo, line.From + todo + 4, Severity.Warning, "TODO marker", Source = "sample") |]
            if text.Length > 60 then
                diags <-
                    Array.append diags
                        [| Diagnostic(line.From, line.To, Severity.Info, "Line is longer than 60 characters", Source = "sample") |]
        diags

    // ---------------------------------------------------------------
    // Editor extensions
    // ---------------------------------------------------------------

    let editorExtensions (name: string) : obj[] =
        let exts = ResizeArray<obj>()
        exts.Add(CodeMirror.HighlightSpecialChars())
        exts.Add(CodeMirror.History())
        exts.Add(CodeMirror.DrawSelection())
        exts.Add(CodeMirror.DropCursor())
        exts.Add(CodeMirror.IndentOnInput())
        exts.Add(CodeMirror.SyntaxHighlighting(CodeMirror.DefaultHighlightStyle))
        exts.Add(CodeMirror.BracketMatching())
        exts.Add(CodeMirror.CloseBrackets())
        let wordCompletion =
            As<System.Func<CompletionContext, obj>>(
                CodeMirror.CompleteFromList([| "WebSharper"; "CodeMirror"; "FSharp" |]))
        exts.Add(CodeMirror.Autocompletion(
                    CompletionConfig(
                        ActivateOnTyping = true,
                        Override = [| wordCompletion |])))
        exts.Add(CodeMirror.RectangularSelection())
        exts.Add(CodeMirror.CrosshairCursor())
        exts.Add(CodeMirror.HighlightActiveLine())
        exts.Add(CodeMirror.HighlightActiveLineGutter())
        exts.Add(CodeMirror.HighlightWhitespace())
        exts.Add(CodeMirror.HighlightSelectionMatches())
        exts.Add(CodeMirror.LineNumbers())
        exts.Add(CodeMirror.CodeFolding())
        exts.Add(CodeMirror.FoldGutter())
        exts.Add(CodeMirror.LintGutter(LintGutterConfig(HoverTime = 300)))
        exts.Add(CodeMirror.Linter(sampleLintSource))
        exts.Add(CodeMirror.ScrollPastEnd())
        exts.Add(CodeMirror.Placeholder("Type here…"))
        exts.Add(EditorState.AllowMultipleSelections.Of(true))
        let customKeys : KeyBinding[] =
            [|
                CodeMirror.IndentWithTab
                KeyBinding(
                    Key = "Mod-Enter",
                    Run = (fun (v: EditorView) -> runState CodeMirror.InsertNewlineAndIndent v))
            |]
        exts.Add(CodeMirror.Keymap.Of(customKeys))
        exts.Add(EditorView.UpdateListener.Of(fun (_: ViewUpdate) -> refreshReadout ()))
        exts.Add(EditorView.LineWrappingStatic)
        exts.Add(themeCompartment.Of(themeExtension ()))
        exts.Add(languageExtension name)
        exts.ToArray()

    // ---------------------------------------------------------------
    // Rendering
    // ---------------------------------------------------------------

    let renderEditor () =
        match byId "editor-host" with
        | None -> ()
        | Some host ->
            match view with
            | Some v -> v.Destroy()
            | None -> ()
            view <- None
            host.InnerHTML <- ""
            try
                let name = language.Value
                let v =
                    EditorView(
                        EditorViewConfig(
                            Doc = sampleCode name,
                            Parent = host,
                            Extensions = editorExtensions name))
                view <- Some v
                refreshReadout ()
                status.Value <- sprintf "%s editor · %d lines" name v.State.Doc.Lines
            with ex ->
                notice.Value <- "Could not create the CodeMirror editor: " + ex.Message

    let renderMerge () =
        match byId "merge-host" with
        | None -> ()
        | Some host ->
            match mergeEditor with
            | Some v -> v.Destroy()
            | None -> ()
            mergeEditor <- None
            host.InnerHTML <- ""
            try
                let v =
                    EditorView(
                        EditorViewConfig(
                            Doc = modifiedDoc,
                            Parent = host,
                            Extensions =
                                [|
                                    CodeMirror.MinimalSetup
                                    CodeMirror.UnifiedMergeView(
                                        UnifiedMergeConfig(originalDoc, HighlightChanges = true, Gutter = true))
                                |]))
                mergeEditor <- Some v
                refreshMerge ()
            with ex ->
                notice.Value <- "Could not create the merge view: " + ex.Message

    // ---------------------------------------------------------------
    // Handlers
    // ---------------------------------------------------------------

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try JS.Window.LocalStorage.SetItem(themeStorageKey, theme.Value) with _ -> ()
        applyTheme ()
        withView (fun v ->
            v.Dispatch(TransactionSpec(Effects = themeCompartment.Reconfigure(themeExtension ()))))

    let resetSample () =
        renderEditor ()

    let insertTab () = runStateCommand CodeMirror.InsertTab

    let acceptCompletion () = runCommand CodeMirror.AcceptCompletion

    let forceLint () =
        withView (fun v ->
            CodeMirror.ForceLinting(v)
            status.Value <- "Linting refreshed.")

    let mergeAccept () =
        match mergeEditor with
        | None -> notice.Value <- "The merge view is not available."
        | Some v ->
            let chunks = (CodeMirror.GetChunks(v.State)).Chunks
            let ok =
                if chunks.Length = 0 then false
                else CodeMirror.AcceptChunk(v, chunks.[0].FromB)
            status.Value <- if ok then "Accepted a chunk into the modified document." else "No chunk to accept here."
            refreshMerge ()

    let mergeReject () =
        match mergeEditor with
        | None -> notice.Value <- "The merge view is not available."
        | Some v ->
            let chunks = (CodeMirror.GetChunks(v.State)).Chunks
            let ok =
                if chunks.Length = 0 then false
                else CodeMirror.RejectChunk(v, chunks.[0].FromB)
            status.Value <- if ok then "Rejected a chunk." else "No chunk to reject here."
            refreshMerge ()

    // ---------------------------------------------------------------
    // Entry point
    // ---------------------------------------------------------------

    [<SPAEntryPoint>]
    let Main () =
        // Restore a previously chosen theme (light is the default).
        let savedTheme =
            try JS.Window.LocalStorage.GetItem(themeStorageKey) with _ -> null
        if savedTheme = "dark" || savedTheme = "light" then
            theme.Value <- savedTheme
        applyTheme ()

        // `.Bind()` renders the whole document body, so the elements the
        // editors live in are only in the DOM after this call.
        IndexTemplate()
            .notice(notice.V)
            .status(status.V)
            .themeLabel(themeLabel.V)
            .docInfo(docInfo.V)
            .cursorInfo(cursorInfo.V)
            .diagInfo(diagInfo.V)
            .searchInfo(searchInfo.V)
            .completionInfo(completionInfo.V)
            .mergeInfo(mergeInfo.V)
            .helperInfo(helperInfo.V)
            .Language(language)
            .ToggleTheme(fun _ -> toggleTheme ())
            .Reset(fun _ -> resetSample ())
            .Undo(fun _ -> runStateCommand CodeMirror.Undo)
            .Redo(fun _ -> runStateCommand CodeMirror.Redo)
            .ToggleComment(fun _ -> runStateCommand CodeMirror.ToggleComment)
            .MoveLineUp(fun _ -> runStateCommand CodeMirror.MoveLineUp)
            .MoveLineDown(fun _ -> runStateCommand CodeMirror.MoveLineDown)
            .IndentMore(fun _ -> runStateCommand CodeMirror.IndentMore)
            .IndentLess(fun _ -> runStateCommand CodeMirror.IndentLess)
            .CopyLineDown(fun _ -> runStateCommand CodeMirror.CopyLineDown)
            .DeleteLine(fun _ -> runCommand CodeMirror.DeleteLine)
            .InsertTab(fun _ -> insertTab ())
            .SelectAll(fun _ -> runStateCommand CodeMirror.SelectAll)
            .FoldAll(fun _ -> runCommand CodeMirror.FoldAll)
            .UnfoldAll(fun _ -> runCommand CodeMirror.UnfoldAll)
            .GotoLine(fun _ -> runCommand CodeMirror.GotoLine)
            .OpenSearch(fun _ -> runCommand CodeMirror.OpenSearchPanel)
            .FindNext(fun _ -> runCommand CodeMirror.FindNext)
            .FindPrevious(fun _ -> runCommand CodeMirror.FindPrevious)
            .ReplaceAll(fun _ -> runCommand CodeMirror.ReplaceAll)
            .CloseSearch(fun _ -> runCommand CodeMirror.CloseSearchPanel)
            .OpenLint(fun _ -> runCommand CodeMirror.OpenLintPanel)
            .NextDiagnostic(fun _ -> runCommand CodeMirror.NextDiagnostic)
            .PreviousDiagnostic(fun _ -> runCommand CodeMirror.PreviousDiagnostic)
            .ForceLint(fun _ -> forceLint ())
            .CloseLint(fun _ -> runCommand CodeMirror.CloseLintPanel)
            .StartCompletion(fun _ -> runCommand CodeMirror.StartCompletion)
            .AcceptCompletion(fun _ -> acceptCompletion ())
            .CloseCompletion(fun _ -> runCommand CodeMirror.CloseCompletion)
            .MergeAccept(fun _ -> mergeAccept ())
            .MergeReject(fun _ -> mergeReject ())
            .MergeRefresh(fun _ -> refreshMerge ())
            .RunHelpers(fun _ -> refreshHelpers ())
            .Bind()

        renderEditor ()
        renderMerge ()
        refreshHelpers ()

        // Re-run the language switch whenever the select changes. `Sink` fires
        // once immediately, so subscribe before flipping `bound`.
        language.View |> View.Sink (fun _ -> if bound then renderEditor ())
        bound <- true
