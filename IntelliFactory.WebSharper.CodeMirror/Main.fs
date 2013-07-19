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

        let ModeMeta =
            Resource "Meta" "CodeMirror.mode.meta.js"
            |> Requires [Js]

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
                "APL", "apl.apl"
                "Asterisk", "asterisk.asterisk"
                "CLike", "clike.clike"
                "Clojure", "clojure.clojure"
                "Cobol", "cobol.cobol"
                "CoffeeScript", "coffeescript.coffeescript"
                "CommonLisp", "commonlisp.commonlisp"
                "CSS", "css.css"
                "D", "d.d"
                "Diff", "diff.diff"
                "ECL", "ecl.ecl"
                "Erlang", "erlang.erlang"
                "Gas", "gas.gas"
                "GFM", "gfm.gfm"
                "Go", "go.go"
                "Groovy", "groovy.groovy"
                "HAML", "haml.haml"
                "Haskell", "haskell.haskell"
                "Haxe", "haxe.haxe"
                "HtmlEmbedded", "htmlembedded.htmlembedded"
                "HtmlMixed", "htmlmixed.htmlmixed"
                "HTTP", "http.http"
                "JavaScript", "javascript.javascript"
                "Jinja2", "jinja2.jinja2"
                "LESS", "less.less"
                "LiveScript", "livescript.livescript"
                "Lua", "lua.lua"
                "Markdown", "markdown.markdown"
                "MIRC", "mirc.mirc"
                "NTriples", "ntriples.ntriples"
                "OCaml", "ocaml.ocaml"
                "Pascal", "pascal.pascal"
                "Perl", "perl.perl"
                "PHP", "php.php"
                "Pig", "pig.pig"
                "Properties", "properties.properties"
                "Python", "python.python"
                "Q", "q.q"
                "R", "r.r"
                "RpmChanges", "rpm.changes.changes"
                "RpmSpec", "rpm.spec.spec"
                "Rst", "rst.rst"
                "Ruby", "ruby.ruby"
                "Rust", "rust.rust"
                "SASS", "sass.sass"
                "Scheme", "scheme.scheme"
                "Shell", "shell.shell"
                "Sieve", "sieve.sieve"
                "Smalltalk", "smalltalk.smalltalk"
                "Smarty", "smarty.smarty"
                "SPARQL", "sparql.sparql"
                "SQL", "sql.sql"
                "Stex", "stex.stex"
                "Tcl", "tcl.tcl"
                "TiddlyWiki", "tiddlywiki.tiddlywiki"
                "Tiki", "tiki.tiki"
                "Turtle", "turtle.turtle"
                "VB", "vb.vb"
                "VBScript", "vbscript.vbscript"
                "Velocity", "velocity.velocity"
                "Verilog", "verilog.verilog"
                "XML", "xml.xml"
                "XQuery", "xquery.xquery"
                "YAML", "yaml.yaml"
                "Z80", "z80.z80"
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

    let FindPosCoords =
        Class "FindPosCoords"
        |=> Inherits CharCoords
        |+> Protocol [
            "hitSide" =? T<bool>
        ]

    let FindPosHUnit =
        Pattern.EnumStrings "FindPosHUnit" ["char"; "column"; "word"]

    let FindPosVUnit =
        Pattern.EnumStrings "FindPosVUnit" ["line"; "page"]

    let Coords =
        Pattern.Config "Coords" {
            Required =
                [
                    "left", T<int>
                    "top", T<int>
                ]
            Optional =
                [
                    "bottom", T<int>
                ]
        }

    let Change =
        Pattern.Config "Change" {
            Optional = []
            Required =
                [
                    "from", CharCoords.Type
                    "to", CharCoords.Type
                    "text", T<string[]>
                ]
        }

    let ChangeArgs =
        let ChangeArgs_t = Type.New()
        Class "ChangeArgs"
        |=> ChangeArgs_t
        |=> Inherits Change
        |+> Protocol [
                "removed" =? T<string>
                "next" =? ChangeArgs_t
            ]

    let BeforeChangeArgs =
        Class "BeforeChangeArgs"
        |+> Protocol [
            "from" =? CharCoords
            "to" =? CharCoords
            "removed" =? T<string>
            "text" =? T<string[]>
            "cancel" => T<unit> ^-> T<unit>
            "update" => !?CharCoords * !?CharCoords * !?T<string[]> ^-> T<unit>
        ]

    let SelectionArgs =
        Class "SelectionArgs"
        |+> Protocol [
            "head" =? CharCoords
            "anchor" =? CharCoords
        ]

    let ScrollInfo =
        Class "ScrollInfo"
        |+> Protocol [
            "left" =? T<int>
            "top" =? T<int>
            "width" =? T<int>
            "height" =? T<int>
            "clientWidth" =? T<int>
            "clientHeight" =? T<int>
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
            "rtlMoveVisually", T<bool>
            "keyMap", T<string>
            "extraKeys", T<obj>
            "lineWrapping", T<bool>
            "lineNumbers", T<bool>
            "firstLineNumber", T<int>
            "lineNumberFormatter", T<int -> string>
            "gutters", T<string[]>
            "fixedGutter", T<bool>
            "coverGutterNextToScrollbar", T<bool>
            "readOnly", T<bool>
            "showCursorWhenSelecting", T<bool>
            "undoDepth", T<int>
            "historyEventDelay", T<int>
            "tabindex", T<int>
            "autofocus", T<bool>
            "dragDrop", T<bool>
            "onDragEvent", CodeMirror_t * T<Event> ^-> T<bool>
            "onKeyEvent", CodeMirror_t * T<Event> ^-> T<bool>
            "cursorBlinkRate", T<int>
            "cursorScrollMargin", T<int>
            "cursorHeight", T<float>
            "workTime", T<int>
            "workDelay", T<int>
            "pollInterval", T<int>
            "flattenSpans", T<bool>
            "maxHighlightLength", T<float>
            "viewportMargin", T<int>

            //// Add-ons

            // matchbrackets.js
            "matchBrackets", T<bool>

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

    let Indent =
        Pattern.EnumStrings "Indent" ["prev"; "smart"; "add"; "substract"]

    let History = Class "History"

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
                "type" => T<string>
                "state" => T<obj>
            ]

    let Range =
        Generic / fun t ->
        Class "Range"
        |+> Protocol [
                "from" =? t
                "to" =? t
            ]

    let TextMarker =
        Generic / fun t ->
        Class "Mark"
        |+> Protocol [
            "clear" => T<unit> ^-> T<unit>
            "find" => T<unit> ^-> t
            "changed" => T<unit> ^-> T<unit>
            "onBeforeCursorEnter" => (T<unit> ^-> T<unit>) ^-> T<unit>
            |> WithInline "$0.on('beforeCursorEnter', $1)"
            "onClear" => (T<unit> ^-> T<unit>) ^-> T<unit>
            |> WithInline "$0.on('clear', $1)"
            "onHide" => (T<unit> ^-> T<unit>) ^-> T<unit>
            |> WithInline "$0.on('hide', $1)"
            "onUnhide" => (T<unit> ^-> T<unit>) ^-> T<unit>
            |> WithInline "$0.on('unhide', $1)"
        ]

    let TextMarkerOptions =
        Pattern.Config "MarkOptions" {
            Required = []
            Optional =
                [
                    "className", T<string>
                    "inclusiveLeft", T<bool>
                    "inclusiveRight", T<bool>
                    "atomic", T<bool>
                    "collapsed", T<bool>
                    "replacedWith", T<Element>
                    "handleMouseEvents", T<bool>
                    "readOnly", T<bool>
                    "addToHistory", T<bool>
                    "startStyle", T<string>
                    "endStyle", T<string>
                    "title", T<string>
                    "shared", T<bool>
                ]
        }

    let BookmarkOptions =
        Pattern.Config "BookmarkOptions" {
            Required = []
            Optional =
                [
                    "widget", T<Element>
                    "insertLeft", T<bool>
                ]
        }

    let LineClassWhere =
        Pattern.EnumStrings "LineClassWhere" ["text"; "background"; "wrap"]

    let LineHandle =
        let LineHandle_t = Type.New()
        Class "LineHandle"
        |=> LineHandle_t
        |+> Protocol [
            "onDelete" => (T<unit> ^-> T<unit>) ^-> T<unit>
            |> WithInline "$0.on('delete', $1)"
            "onChange" => (LineHandle_t * T<obj> ^-> T<unit>) ^-> T<unit>
            |> WithInline "$0.on('change', $1)"
        ]

    let line = T<int> + LineHandle

    let LineWidget =
        Class "LineWidget"
        |+> Protocol [
            "clear" => T<unit> ^-> T<unit>
            "changed" => T<unit> ^-> T<unit>
        ]

    let LineInfo =
        Class "LineInfo"
        |+> Protocol [
                "line" =? T<int>
                "handle" =? LineHandle
                "text" =? T<string>
                "gutterMarkers" =? T<obj>
                "textClass" =? T<string>
                "bgClass" =? T<string>
                "wrapClass" =? T<string>
                "widgets" =? Type.ArrayOf LineWidget
            ]

    let LineWidgetOptions =
        Pattern.Config "LineWidgetOptions" {
            Required = []
            Optional =
                [
                    "coverGutter", T<bool>
                    "noHScroll", T<bool>
                    "above", T<bool>
                    "showIfHidden", T<bool>
                    "handleMouseEvents", T<bool>
                    "insertAt", T<int>
                ]
        }

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

    let Rectangle =
        Pattern.Config "Rectangle" {
            Optional = []
            Required =
                [
                    "left", T<int>
                    "top", T<int>
                    "right", T<int>
                    "bottom", T<int>
                ]
        }

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

    let Collapse =
        Pattern.EnumStrings "CodeMirror.Collapse" ["start"; "end"]

    let OverlayOptions =
        Pattern.Config "CodeMirror.OverlayOptions" {
            Required = []
            Optional =
                [
                    "opaque", T<bool>
                ]
        }

    let CodeMirror =
        Class "CodeMirror"
        |=> CodeMirror_t
        |+> [
                Constructor ((T<Node> + T<Element -> unit>) * !?CodeMirror_Options)
                "version" =? T<string>
                "fromTextArea" => T<Node> * !?CodeMirror_Options ^-> CodeMirrorTextArea
                "defaults" =? CodeMirror_Options
                "defineExtension" => T<string> * T<obj> ^-> T<unit>
                Generic - fun t -> "defineOption" => T<string> * t?``default`` * (CodeMirror_t * t ^-> T<unit>) ^-> T<unit>
                "defineInitHook" => (CodeMirror_t ^-> T<unit>) ^-> T<unit>
                "registerHelper" => T<string>?``type`` * T<string>?name * T<obj>?helper ^-> T<unit>
                "Pos" => T<int>?line * !?T<int>?ch ^-> CharCoords
                "changeEnd" => Change ^-> CharCoords

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
                // Content manipulation methods
                "getValue" => T<unit> ^-> T<string>
                "setValue" => T<string> ^-> T<unit>
                "getRange" => CharCoords?from * CharCoords?``to`` * !?T<string>?separator ^-> T<string>
                "replaceRange" => T<string> * CharCoords * !?CharCoords ^-> T<unit>
                "getLine" => T<int> ^-> T<string>
                "setLine" => T<int> * T<string> ^-> T<unit>
                "removeLine" => T<unit> ^-> T<unit>
                "getLineHandle" => T<int> ^-> LineHandle
                "getLineNumber" => LineHandle ^-> T<int>
                "eachLine" => (LineHandle ^-> T<unit>) ^-> T<unit>
                "eachLine" => T<int> * T<int> * (LineHandle ^-> T<unit>) ^-> T<unit>
                "markClean" => T<unit> ^-> T<unit>
                "changeGeneration" => T<unit> ^-> T<int>
                "isClean" => !?T<int> ^-> T<bool>

                // Cursor and selection methods
                "getSelection" => T<unit> ^-> T<string>
                "replaceSelection" => T<string> * !?Collapse ^-> T<unit>
                "getCursor" => !?T<bool> ^-> CharCoords
                "somethingSelected" => T<unit> ^-> T<bool>
                "setCursor" => CharCoords ^-> T<unit>
                "setCursor" => T<int> * T<int> ^-> T<unit>
                "setSelection" => CharCoords * !?CharCoords ^-> T<unit>
                "extendSelection" => CharCoords * !?CharCoords ^-> T<unit>
                "setExtending" => T<bool> ^-> T<unit>
                "hasFocus" => T<unit> ^-> T<bool>
                "findPosH" => CharCoords?start * T<int>?amount * FindPosHUnit * T<bool>?visually ^-> FindPosCoords
                "findPosV" => CharCoords?start * T<int>?amount * FindPosVUnit * T<bool>?visually ^-> FindPosCoords

                // Configuration options
                "setOption" => T<string> * T<obj> ^-> T<unit>
                "getOption" => T<string> ^-> T<obj>
                "addKeyMap" => T<obj> * T<bool>?bottom ^-> T<unit>
                "removeKeyMap" => T<obj> ^-> T<unit>
                "addOverlay" => (T<string> + T<obj>) * !?OverlayOptions ^-> T<unit>
                "removeOverlay" => (T<string> + T<obj>) ^-> T<unit>
                "on" => T<string> * T<obj> ^-> T<unit>
                "off" => T<string> * T<obj> ^-> T<unit>

                // History-related methods
                "undo" => T<unit> ^-> T<unit>
                "redo" => T<unit> ^-> T<unit>
                "historySize" => T<unit> ^-> HistorySize
                "clearHistory" => T<unit> ^-> T<unit>
                "getHistory" => T<unit> ^-> History
                "setHistory" => History ^-> T<unit>

                // Text-marking methods
                "markText" => CharCoords * CharCoords * !?TextMarkerOptions ^-> TextMarker (Range CharCoords)
                "setBookmark" => CharCoords * !?BookmarkOptions ^-> TextMarker CharCoords
                "findMarksAt" => CharCoords ^-> Type.ArrayOf (TextMarker T<obj>)
                "getAllMarks" => T<unit> ^-> Type.ArrayOf (TextMarker T<obj>)

                // Widget, gutter, and decoration methods
                "setGutterMarker" => line * T<string>?gutterID * T<Element> ^-> LineHandle
                "clearGutter" => T<string> ^-> T<unit>
                "addLineClass" => line * LineClassWhere * T<string> ^-> LineHandle
                "removeLineClass" => line * LineClassWhere * T<string> ^-> LineHandle
                "lineInfo" => line ^-> LineInfo
                "addWidget" => CharCoords * T<Node> * T<bool> ^-> T<unit>
                "addLineWidget" => line * T<Node> * !?LineWidgetOptions ^-> LineWidget
                "setSize" => (T<int> + T<string>)?width * (T<int> + T<string>)?height ^-> T<unit>
                "scrollTo" => T<int> * T<int> ^-> T<unit>
                "getScrollInfo" => T<unit> ^-> ScrollInfo
                "scrollIntoView" => (CharCoords + Rectangle) * !?T<int>?margin ^-> T<unit>
                "cursorCoords" => T<bool> * CoordsMode ^-> Coords
                "charCoords" => CharCoords * CoordsMode ^-> Coords
                "coordsChar" => Coords ^-> CharCoords
                "lineAtHeight" => T<int>?height * !?T<string>?mode ^-> T<int>
                "heightAtLine" => T<int>?number * !?T<string>?mode ^-> T<int>
                "defaultTextHeight" => T<unit> ^-> T<int>
                "defaultCharWidth" => T<unit> ^-> T<int>
                "getViewport" => T<unit> ^-> Range T<int>
                "refresh" => T<unit> ^-> T<unit>

                // Mode, state, and token-related methods
                Generic - fun t -> "getMode" => T<unit> ^-> Mode t
                Generic - fun t -> "getModeAt" => CharCoords ^-> Mode t
                "getTokenAt" => CharCoords * !?T<bool>?precise ^-> Token
                "getTokenTypeAt" => CharCoords ^-> T<string>
                "getHelper" => CharCoords * T<string>?``type`` ^-> T<obj>
                "getStateAfter" => !?T<int> * !?T<bool>?precise ^-> T<obj>

                // Miscellaneous methods
                Generic - fun t -> "operation" => (T<unit> ^-> t) ^-> t
                "indentLine" => line * !?(Indent + T<int>) ^-> T<unit>
                "toggleOverwrite" => !?T<bool>?value ^-> T<unit>
                "posFromIndex" => T<int> ^-> CharCoords
                "indexFromPos" => CharCoords ^-> T<int>
                "focus" => T<unit> ^-> T<unit>
                "getInputField" => T<unit> ^-> T<Element>
                "getWrapperElement" => T<unit> ^-> T<Element>
                "getScrollerElement" => T<unit> ^-> T<Element>
                "getGutterElement" => T<unit> ^-> T<Element>

                "lineCount" => T<unit> ^-> T<int>
                Generic - fun t -> "compoundChange" => (T<unit> ^-> t) ^-> t

                // Events
                "onChange" => (CodeMirror_t * ChangeArgs ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('change', $1)"
                "onBeforeChange" => (CodeMirror_t * BeforeChangeArgs ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('beforeChange', $1)"
                "onCursorActivity" => (CodeMirror_t ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('cursorActivity', $1)"
                "onKeyHandled" => (CodeMirror_t * T<string> * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('keyHandled', $1)"
                "onInputRead" => (CodeMirror_t * T<obj> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('inputRead', $1)"
                "onBeforeSelectionChange" => (CodeMirror_t * SelectionArgs ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('beforeSelectionChange', $1)"
                "onViewportChange" => (CodeMirror_t * T<int> * T<int> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('viewportChange', $1)"
                "onGutterClick" => (CodeMirror_t * T<int> * T<string> * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('gutterClick', $1)"
                "onFocus" => (CodeMirror_t ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('focus', $1)"
                "onBlur" => (CodeMirror_t ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('blur', $1)"
                "onScroll" => (CodeMirror_t ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('scroll', $1)"
                "onUpdate" => (CodeMirror_t ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('update', $1)"
                "onRenderLine" => (CodeMirror_t * LineHandle ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('renderLine', $1)"
                "onMousedown" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('mousedown', $1)"
                "onDblclick" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('dblclick', $1)"
                "onContextmenu" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('contextmenu', $1)"
                "onKeydown" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('keydown', $1)"
                "onKeypress" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('keypress', $1)"
                "onKeyup" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('keyup', $1)"
                "onDragstart" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('dragstart', $1)"
                "onDragenter" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('dragenter', $1)"
                "onDragover" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('dragover', $1)"
                "onDrop" => (CodeMirror_t * T<Event> ^-> T<unit>) ^-> T<unit>
                |> WithInline "$0.on('drop', $1)"

                //// Add-ons

                // dialog.js
                "openDialog" => Dialog * T<string -> unit>?callback ^-> (T<unit -> unit>)
                "openConfirm" => T<string>?template * (Type.ArrayOf (CodeMirror_t ^-> T<unit>))?callbacks ^-> Dialog
                "openConfirm" => T<Element>?template * (Type.ArrayOf (CodeMirror_t ^-> T<unit>))?callbacks ^-> Dialog
                |> WithInline "$this.openConfirm($template.outerHTML, $callbacks)"

                // edit/matchbrackets.js
                "matchBrackets" => T<unit> ^-> T<unit>

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
                BookmarkOptions
                Change
                ChangeArgs
                CharCoords
                CodeMirror
                CodeMirror_Options
                CodeMirrorTextArea
                Collapse
                Coords
                CoordsMode
                Dialog
                FindPosCoords
                FindPosHUnit
                FindPosVUnit
                Hint
                HintOptions
                History
                HistorySize
                Indent
                JavaScriptHint
                LineClassWhere
                LineHandle
                LineInfo
                LineWidget
                LineWidgetOptions
                MatchHighlighter
                MultiplexMode
                MIME
                Generic - Mode
                Generic - Range
                RangeFinder
                Rectangle
                RunModeOutput
                ScrollInfo
                SearchCursor
                SelectionArgs
                Stream
                TagClosing
                Generic - TextMarker
                TextMarkerOptions
                Token
            ]
            Namespace "IntelliFactory.WebSharper.CodeMirror.Resources" [
                Res.Css
                Res.ModeMeta
                Res.Js
            ]
            Namespace "IntelliFactory.WebSharper.CodeMirror.Resources.Addons" [
                Res.Addons.DialogCss
                Res.Addons.Dialog
                Res.Addons.SearchCursor
                Res.Addons.Search
                Res.Addons.FoldCode
                Res.Addons.Multiplex
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
