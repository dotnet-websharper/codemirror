namespace IntelliFactory.WebSharper.CodeMirror.Definition

module Definition =
    open IntelliFactory.WebSharper.InterfaceGenerator
    open IntelliFactory.WebSharper.Dom
    open IntelliFactory.WebSharper.EcmaScript

    module Res =

        let Css =
            Resource "Css" "CodeMirror.lib.codemirror.css"

        let Js =
            Resource "Js" "CodeMirror.lib.codemirror.js"
            |> Requires [Css]

        module Addons =

            let DialogCss =
                Resource "DialogCss" "CodeMirror.lib.util.dialog.css"

            let Dialog =
                Resource "Dialog" "CodeMirror.lib.util.dialog.js"
                |> Requires [Js; DialogCss]

            let SearchCursor =
                Resource "SearchCursor" "CodeMirror.lib.util.searchcursor.js"
                |> Requires [Js]

            let Search =
                Resource "Search" "CodeMirror.lib.util.search.js"
                |> Requires [Js; SearchCursor]

            let FoldCode =
                Resource "FoldCode" "CodeMirror.lib.util.foldcode.js"
                |> Requires [Js]

            let Multiplex =
                Resource "Multiplex" "CodeMirror.lib.util.multiplex.js"
                |> Requires [Js]

            let RunMode =
                Resource "RunMode" "CodeMirror.lib.util.runmode.js"
                |> Requires [Js]

            let SimpleHintCss =
                Resource "SimpleHintCss" "CodeMirror.lib.util.simple-hint.css"
                |> Requires [Js]

            let SimpleHint =
                Resource "SimpleHint" "CodeMirror.lib.util.simple-hint.js"
                |> Requires [Js; SimpleHintCss]

            let JavaScriptHint =
                Resource "JavaScriptHint" "CodeMirror.lib.util.javascript-hint.js"
                |> Requires [Js; SimpleHint]

            let MatchHighlighter =
                Resource "MatchHighlighter" "CodeMirror.lib.util.match-highlighter.js"
                |> Requires [Js; SearchCursor]

            let CloseTag =
                Resource "CloseTag" "CodeMirror.lib.util.closetag.js"
                |> Requires [Js]

        let Modes =
            [
                "CLike", "clike.clike"
                "Clojure", "clojure.clojure"
                "CoffeeScript", "coffeescript.coffeescript"
                "CSS", "css.css"
                "Diff", "diff.diff"
                "ECL", "ecl.ecl"
                "Erlang", "erlang.erlang"
                "GFM", "gfm.gfm"
                "Go", "go.go"
                "Groovy", "groovy.groovy"
                "Haskell", "haskell.haskell"
                "HtmlEmbedded", "htmlembedded.htmlembedded"
                "HtmlMixed", "htmlmixed.htmlmixed"
                "JavaScript", "javascript.javascript"
                "Jinja2", "jinja2.jinja2"
                "LESS", "less.less"
                "Lua", "lua.lua"
                "Markdown", "markdown.markdown"
                "MySQL", "mysql.mysql"
                "NTriples", "ntriples.ntriples"
                "Pascal", "pascal.pascal"
                "Perl", "perl.perl"
                "PHP", "php.php"
                "Pig", "pig.pig"
                "PLSQL", "plsql.plsql"
                "Properties", "properties.properties"
                "Python", "python.python"
                "R", "r.r"
                "RpmChanges", "rpm.changes.changes"
                "RpmSpec", "rpm.spec.spec"
                "Rst", "rst.rst"
                "Ruby", "ruby.ruby"
                "Rust", "rust.rust"
                "Scheme", "scheme.scheme"
                "Shell", "shell.shell"
                "Smalltalk", "smalltalk.smalltalk"
                "Smarty", "smarty.smarty"
                "SPARQL", "sparql.sparql"
                "Stex", "stex.stex"
                "TiddlyWiki", "tiddlywiki.tiddlywiki"
                "Tiki", "tiki.tiki"
                "VBScript", "vbscript.vbscript"
                "Velocity", "velocity.velocity"
                "Verilog", "verilog.verilog"
                "XML", "xml.xml"
                "XQuery", "xquery.xquery"
                "YAML", "yaml.yaml"
            ]
            |> List.map (fun (name, path) ->
                Resource name ("CodeMirror.mode." + path + ".js")
                |> Requires [Js]
                :> CodeModel.NamespaceEntity)

    let CharCoords_t = Type.New()
    let CharCoords =
        Pattern.Config "CharCoords" {
            Optional = []
            Required =
                [
                    "line", T<int>
                    "ch", T<int>
                ]
        }
        |=> CharCoords_t

    let Coords =
        Pattern.Config "Coords" {
            Required =
                [
                    "x", T<int>
                    "y", T<int>
                ]
            Optional =
                [
                    "yBot", T<int>
                ]
        }

    let ChangeArgs =
        let ChangeArgs_t = Type.New()
        Class "ChangeArgs"
        |=> ChangeArgs_t
        |+> Protocol [
                "from" =? CharCoords
                "to" =? CharCoords
                "text" =? T<string[]>
                "next" =? ChangeArgs_t
            ]

    let CodeMirror_t = Type.New()

    let options =
        [
            "value", T<string>
            "mode", T<obj>
            "theme", T<string>
            "indentUnit", T<int>
            "smartIndent", T<bool>
            "tabSize", T<int>
            "indentWithTabs", T<bool>
            "electricChars", T<bool>
            "autoClearEmptyLines", T<bool>
            "keyMap", T<string>
            "extraKeys", T<obj>
            "lineWrapping", T<bool>
            "lineNumbers", T<bool>
            "firstLineNumber", T<int>
            "gutter", T<bool>
            "fixedGutter", T<bool>
            "readOnly", T<bool>
            "onChange", CodeMirror_t * ChangeArgs ^-> T<unit>
            "onCursorActivity", CodeMirror_t ^-> T<unit>
            "onGutterClick", CodeMirror_t * T<int> * T<Event> ^-> T<unit>
            "onFocus", CodeMirror_t ^-> T<unit>
            "onBlur", CodeMirror_t ^-> T<unit>
            "onScroll", CodeMirror_t ^-> T<unit>
            "onHighlightComplete", CodeMirror_t ^-> T<unit>
            "onUpdate", CodeMirror_t ^-> T<unit>
            "matchBrackets", T<bool>
            "workTime", T<int>
            "workDelay", T<int>
            "pollInterval", T<int>
            "undoDepth", T<int>
            "tabindex", T<int>
            "autofocus", T<bool>
            "dragDrop", T<bool>
            "onDragEvent", CodeMirror_t * T<Event> ^-> T<bool>
            "onKeyEvent", CodeMirror_t * T<Event> ^-> T<bool>

            //// Add-ons

            // closetag.js
            "closeTagEnabled", T<bool>
            "closeTagIndent", T<string[]>
        ]

    let CodeMirror_Options =
        Pattern.Config "Options" {
            Required = []
            Optional = options
        }

    let CoordsMode =
        Pattern.EnumStrings "CoordsMode" ["page"; "local"]

    let HistorySize =
        Class "HistorySize"
        |+> Protocol [
                "undo" =? T<int>
                "redo" =? T<int>
            ]

    let Token =
        Class "Token"
        |+> Protocol [
                "start" =? T<int>
                "end" =? T<int>
                "string" => T<string>
                "className" => T<string>
                "state" => T<obj>
            ]

    let Range =
        Class "Range"
        |+> Protocol [
                "from" =? CharCoords
                "to" =? CharCoords
            ]

    let Mark =
        Generic / fun t ->
        Class "Mark"
        |+> Protocol [
                "clear" => T<unit> ^-> T<unit>
                "find" => T<unit> ^-> t
            ]

    let LineHandle =
        Class "LineHandle"

    let line = T<int> + LineHandle

    let LineInfo =
        Class "LineInfo"
        |+> Protocol [
                "line" =? T<int>
                "handle" =? LineHandle
                "text" => T<string>
                "markerText" => T<string>
                "markerClass" => T<string>
                "lineClass" => T<string>
                "bgClass" => T<string>
            ]

    let CodeMirrorTextArea =
        Class "CodeMirrorTextArea"
        |=> Inherits CodeMirror_t
        |+> Protocol [
                "save" => T<unit> ^-> T<unit>
                "toTextArea" => T<unit> ^-> T<unit>
                "getTextArea" => T<unit> ^-> T<Node>
            ]

    let Dialog =
        Class "Dialog"
        |+> [
                Constructor (T<string>)?template
                |> WithInline "$template"
                Constructor (T<Element>)?template
                |> WithInline "$template.outerHTML"
            ]
        |> Requires [Res.Addons.Dialog]

    let SearchCursor =
        Class "SearchCursor"
        |+> Protocol [
                "findNext" => T<unit> ^-> T<bool>
                "findPrevious" => T<unit> ^-> T<bool>
                "from" => T<unit> ^-> CharCoords
                "to" => T<unit> ^-> CharCoords
                "replace" => T<string> ^-> T<unit>
            ]
        |> Requires [Res.Addons.SearchCursor]

    let RangeFinder =
        Class "RangeFinder"
        |+> [
                Constructor ((CodeMirror_t * T<int> * T<bool> ^-> T<int>)?func)
                |> WithInline "$func"
            ]
        |> Requires [Res.Addons.FoldCode]

    let Hint =
        Pattern.Config "Hint" {
            Required =
                [
                    "list", T<string[]>
                    "from", CharCoords_t
                    "to", CharCoords_t
                ]
            Optional = []
        }
        |> Requires [Res.Addons.SimpleHint]

    let HintOptions =
        Pattern.Config "HintOptions" {
            Required = []
            Optional =
                [
                    "closeOnBackspace", T<bool>
                    "closeOnTokenChange", T<bool>
                ]
        }

    let MIME =
        Class "MIME"
        |+> Protocol [
                "mime" =? T<string>
                "mode" =? T<string>
            ]

    let RunModeOutput =
        Class "RunModeOutput"
        |+> Protocol [
                Constructor (T<string> * T<string> ^-> T<unit>)?displayFunc
                |> WithInline "$displayFunc"
                Constructor T<Element>?container
                |> WithInline "$container"
            ]
        |> Requires [Res.Addons.RunMode]

    let JavaScriptHint =
        Class "JavaScriptHint"
        |+> Protocol [
                "hint" =? CodeMirror_t ^-> Hint
                |> WithGetterInline "$this"
            ]
        |> Requires [Res.Addons.JavaScriptHint]

    let MatchHighlighter =
        Class "MatchHighlighter"
        |+> [
                Constructor T<string>?``class``
                |> WithInline "$class"
            ]
        |> Requires [Res.Addons.MatchHighlighter]

    let TagClosing =
        Class "TagClosing"
        |+> Protocol [
                "closeTag" => CodeMirror_t?editor * T<string>?char ^-> T<unit>
                |> WithInline "$this.call($editor, $editor, $char)"
                "closeTag" => CodeMirror_t?editor * T<string>?char * (T<string[]> + T<bool>)?indent ^-> T<unit>
                |> WithInline "$this.call($editor, $editor, $char, $indent)"
            ]
        |> Requires [Res.Addons.CloseTag]

    let Stream =
        Class "Stream"
        |+> Protocol [
                "eol" => T<unit -> bool>
                "sol" => T<unit -> bool>
                "peek" => T<unit -> char>
                "next" => T<unit -> char>
                "eat" => (T<char> + T<RegExp> + T<char -> bool>) ^-> T<char>
                "eatWhile" => (T<char> + T<RegExp> + T<char -> bool>) ^-> T<bool>
                "eatSpace" => T<unit -> bool>
                "skipToEnd" => T<unit -> unit>
                "skipTo" => T<char -> bool>
                "match" => T<string>* !?T<bool>?consume * !?T<bool>?caseFold ^-> T<bool>
                "match" => T<RegExp>* !?T<bool>?consume * !?T<bool>?caseFold ^-> T<string[]>
                "backUp" => T<int -> unit>
                "column" => T<unit -> int>
                "indentation" => T<unit -> int>
                "current" => T<unit -> string>
            ]

    let Mode =
        Generic / fun state ->
        Pattern.Config "Mode" {
            Required =
                [
                    "token", Stream * state ^-> T<string>
                ]
            Optional =
                [
                    "startState", T<int> ^-> state
                    "blankLine", state ^-> T<unit>
                    "copyState", state ^-> state
                    "compareStates", state * state ^-> T<bool>
                    "indent", state * T<string> ^-> T<int>
                    "electricChars", T<string>
                ]
        }

    let MultiplexMode =
        Pattern.Config "CodeMirror.MultiplexMode" {
            Required =
                [
                    "open", T<string>
                    "close", T<string>
                    "mode", T<obj>
                ]
            Optional =
                [
                    "delimStyle", T<string>
                ]
        }
        |> Requires [Res.Addons.Multiplex]

    let CodeMirror =
        Class "CodeMirror"
        |=> CodeMirror_t
        |+> [
                Constructor ((T<Node> + T<Element -> unit>) * !?CodeMirror_Options)
                "fromTextArea" => T<Node> * !?CodeMirror_Options ^-> CodeMirrorTextArea
                "listModes" => T<unit> ^-> T<string[]>
                "listMIMEs" => T<unit> ^-> Type.ArrayOf MIME
                Generic - fun t -> "defineMode" => T<string> * (CodeMirror_Options * T<obj> ^-> Mode t) ^-> T<unit>
                "defineMIME" => T<string> * T<obj> ^-> T<unit>
                Generic - fun t -> "getMode" => CodeMirror_Options * T<obj> ^-> Mode t
                Generic - fun t -> "copyState" => Mode t * t ^-> t

                //// Add-ons

                // foldcode.js
                "newFoldFunction" => RangeFinder * !?T<string>?markText * !?T<bool>?hideEnd ^-> (CodeMirror_t * T<int> * T<Event> ^-> T<unit>)
                "braceRangeFinder" =? (CodeMirror_t * T<int> * T<bool> ^-> T<int>)
                "indentRangeFinder" =? (CodeMirror_t * T<int> * T<bool> ^-> T<int>)
                "tagRangeFinder" =? (CodeMirror_t * T<int> * T<bool> ^-> T<int>)

                // runmode.js
                "runMode" => T<string> * T<obj> * RunModeOutput ^-> T<unit>

                // simple-hint.js
                "simpleHint" => CodeMirror_t?cm * (CodeMirror_t ^-> Hint)?getHint * !?HintOptions?options ^-> T<unit>

                // javascript-hint.js
                "javascriptHint" =? JavaScriptHint
                "coffeescriptHint" =? JavaScriptHint

                // multiplex.js
                "multiplexingMode" => T<obj>?mode * MultiplexMode ^-> T<obj>
            ]
        |+> Protocol (
            [
                "getValue" => T<unit> ^-> T<string>
                "setValue" => T<string> ^-> T<unit>
                "getSelection" => T<unit> ^-> T<string>
                "replaceSelection" => T<string> ^-> T<unit>
                "focus" => T<unit> ^-> T<unit>
                "scrollTo" => T<int> * T<int> ^-> T<unit>
                "setOption" => T<string> * T<obj> ^-> T<unit>
                "getOption" => T<string> ^-> T<obj>
                "cursorCoords" => T<bool> * CoordsMode ^-> Coords
                "charCoords" => CharCoords * CoordsMode ^-> Coords
                "coordsChar" => Coords ^-> CharCoords
                "undo" => T<unit> ^-> T<unit>
                "redo" => T<unit> ^-> T<unit>
                "historySize" => T<unit> ^-> HistorySize
                "clearHistory" => T<unit> ^-> T<unit>
                "indentLine" => line * !?T<bool> ^-> T<unit>
                "getTokenAt" => CharCoords ^-> Token
                "markText" => CharCoords * CharCoords * T<string> ^-> Mark Range
                "setBookmark" => CharCoords ^-> Mark CharCoords
                "findMarksAt" => CharCoords ^-> Type.ArrayOf (Mark T<obj>)
                "setMarker" => T<int> * !?T<string>?text * !?T<string>?className ^-> LineHandle
                "clearMarker" => line ^-> T<unit>
                "setLineClass" => line * T<string>?className * T<string>?backgroundClassName ^-> LineHandle
                "hideLine" => line ^-> LineHandle
                "showLine" => line ^-> LineHandle
                "onDeleteLine" => line * T<unit -> unit> ^-> T<unit>
                "lineInfo" => line ^-> LineInfo
                "getLineHandle" => T<int> ^-> LineHandle
                "addWidget" => CharCoords * T<Node> * T<bool> ^-> T<unit>
                "matchBrackets" => T<unit> ^-> T<unit>
                "lineCount" => T<unit> ^-> T<int>
                "getCursor" => !?T<bool> ^-> CharCoords
                "somethingSelected" => T<unit> ^-> T<bool>
                "setCursor" => CharCoords ^-> T<unit>
                "setCursor" => T<int> * T<int> ^-> T<unit>
                "setSelection" => CharCoords * CharCoords ^-> T<unit>
                "getLine" => T<int> ^-> T<string>
                "setLine" => T<int> * T<string> ^-> T<unit>
                "removeLine" => T<unit> ^-> T<unit>
                "getRange" => CharCoords * CharCoords ^-> T<string>
                "replaceRange" => T<string> * CharCoords * !?CharCoords ^-> T<unit>
                "posFromIndex" => T<int> ^-> CharCoords
                "indexFromPos" => CharCoords ^-> T<int>
                Generic - fun t -> "operation" => (T<unit> ^-> t) ^-> t
                Generic - fun t -> "compoundChange" => (T<unit> ^-> t) ^-> t
                "refresh" => T<unit> ^-> T<unit>
                "getInputField" => T<unit> ^-> T<Element>
                "getWrapperElement" => T<unit> ^-> T<Element>
                "getScrollerElement" => T<unit> ^-> T<Element>
                "getStateAfter" => T<int> ^-> T<obj>

                //// Add-ons

                // dialog.js
                "openDialog" => Dialog * T<string -> unit>?callback ^-> (T<unit -> unit>)
                "openConfirm" => T<string>?template * (Type.ArrayOf (CodeMirror_t ^-> T<unit>))?callbacks ^-> Dialog
                "openConfirm" => T<Element>?template * (Type.ArrayOf (CodeMirror_t ^-> T<unit>))?callbacks ^-> Dialog
                |> WithInline "$this.openConfirm($template.outerHTML, $callbacks)"

                // searchcursor.js
                "getSearchCursor" => (T<string> + T<RegExp>) * !?CharCoords * !?T<bool> ^-> SearchCursor

                // match-highlighter.js
                "matchHighlight" => MatchHighlighter ^-> T<unit>

                // closetag.js
                "closeTag" =? TagClosing
            ]
            @ List.map (fun (name, ty) ->
                    ("option_" + name) =% ty
                    |> WithGetterInline ("$this.getOption('" + name + "')")
                    |> WithSetterInline ("$this.setOption('" + name + "', $value)")
                    :> CodeModel.Member)
                options)
        |> Requires [Res.Js]

    let Assembly =
        Assembly [
            Namespace "IntelliFactory.WebSharper.CodeMirror" [
                ChangeArgs
                CharCoords
                CodeMirror
                CodeMirror_Options
                CodeMirrorTextArea
                Coords
                CoordsMode
                Dialog
                Hint
                HintOptions
                HistorySize
                JavaScriptHint
                LineHandle
                LineInfo
                Generic - Mark
                MatchHighlighter
                MultiplexMode
                MIME
                Generic - Mode
                Range
                RangeFinder
                RunModeOutput
                SearchCursor
                Stream
                TagClosing
                Token
            ]
            Namespace "IntelliFactory.WebSharper.CodeMirror.Resources" [
                Res.Css
                Res.Js
            ]
            Namespace "IntelliFactory.WebSharper.CodeMirror.Resources.Addons" [
                Res.Addons.DialogCss
                Res.Addons.Dialog
                Res.Addons.SearchCursor
                Res.Addons.Search
                Res.Addons.FoldCode
                Res.Addons.RunMode
                Res.Addons.SimpleHintCss
                Res.Addons.SimpleHint
                Res.Addons.JavaScriptHint
                Res.Addons.MatchHighlighter
                Res.Addons.CloseTag
            ]
            Namespace "IntelliFactory.WebSharper.CodeMirror.Resources.Modes"
                Res.Modes
        ]

module Main =
    open IntelliFactory.WebSharper.InterfaceGenerator

    do Compiler.Compile stdout Definition.Assembly
