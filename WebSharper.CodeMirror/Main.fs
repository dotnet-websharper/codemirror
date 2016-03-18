namespace WebSharper.CodeMirror.Definition

module Definition =
    open System.IO
    open System.Collections.Generic
    open WebSharper.InterfaceGenerator
    open WebSharper.JavaScript.Dom
    type RegExp = WebSharper.JavaScript.RegExp

    let ( +/ ) a b = Path.Combine(a, b)
    
    let ( .- ) (m, n) k = 
        match m |> Map.tryFind k with
        | None ->
            printfn "lookup failed in '%s' : %A" n k
            failwith "lookup failed"
        | Some v -> v

    module Res =
        let customRes =
            Set [
                "codemirror.css"
                "codemirror.js"
                "meta.js"
                "show-hint.css"
                "show-hint.js"
            ]

        let Css =
            Resource "CodeMirrorCss" "codemirror.css"
            |> fun js -> js.AssemblyWide()

        let Js =
            Resource "CodeMirror" "codemirror.js"
            |> fun js -> js.AssemblyWide()

        let ModeMeta =
            Resource "Meta" "meta.js" 

        let ShowHintCss = 
            Resource "hint_show_hint_css" "show-hint.css"

        let ShowHint = 
            Resource "hint_show_hint" "show-hint.js" |> Requires [Js; ShowHintCss]

        type GenRes =
            {
                Folder : string
                ResName : string
                FileName : string
            }

        let GroupedGen, Gen =
            let input =
                File.ReadAllLines(__SOURCE_DIRECTORY__ +/ "../.temp/res.txt")
                |> Seq.choose (fun r ->
                    let p = r.Split '\\'
                    let d = p.[0]
                    let f = p.[p.Length - 1]
                    if customRes.Contains f then None else
                        Some {
                            Folder = d
                            ResName = 
                                if d = "mode" then
                                    Path.GetFileNameWithoutExtension f
                                else
                                    Seq.append (p.[1.. p.Length - 2]) (Seq.singleton (Path.GetFileNameWithoutExtension f))
                                    |> Seq.map (fun n -> n.Replace('-', '_').Replace('.', '_') ) |> String.concat "_"
                            FileName = f
                        }
                )
                |> Array.ofSeq

            let gen = Dictionary()
            
            for r in input do
                if Path.GetExtension r.FileName = ".css" then
                    let d = if r.Folder = "theme" then "theme" else "css"
                    gen.Add (r.FileName, (d, Resource r.ResName r.FileName))

            for r in input do
                if Path.GetExtension r.FileName = ".js" then
                    let req =
                        [
                            yield Js
                            match gen.TryGetValue(r.FileName.[.. r.FileName.Length - 3] + "css") with
                            | true, (_, cssRes) -> yield cssRes
                            | _ -> ()
                            if r.Folder = "addon" && r.ResName.StartsWith "hint_" then
                                yield ShowHint
                        ]
                    let res = Resource r.ResName r.FileName |> Requires req
                    gen.Add (r.FileName, (r.Folder, res))

            (
                gen.Values |> Seq.groupBy fst 
                |> Seq.map (fun (f, s) -> f, s |> Seq.map (fun (_, r) -> r :> CodeModel.NamespaceEntity) |> List.ofSeq) |> Map.ofSeq
                , "Resources by folder"
            ), (
                gen |> Seq.map (fun (KeyValue(f, (_, r))) -> f, r) |> Map.ofSeq
                , "Resources by file name"
            )

    let event name (args: Type.IParameters) =
        let capName = name |> String.mapi (fun i c -> if i = 0 then System.Char.ToUpper c else c)
        ("on" + capName) => (args ^-> T<unit>)?callback ^-> T<unit>
        |> WithInteropInline (fun t -> sprintf "CodeMirror.on($this, '%s', %s)" name (t "callback"))

    let CharCoords =
        Pattern.Config "CharCoords" {
            Optional = []
            Required =
                [
                    "line", T<int>
                    "ch", T<int>
                ]
        }

    let FindPosCoords =
        Class "FindPosCoords"
        |=> Inherits CharCoords
        |+> Instance [
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
        |+> Instance [
                "removed" =? T<string>
                "next" =? ChangeArgs_t
            ]

    let BeforeChangeArgs =
        Class "BeforeChangeArgs"
        |+> Instance [
            "from" =? CharCoords
            "to" =? CharCoords
            "removed" =? T<string>
            "text" =? T<string[]>
            "cancel" => T<unit> ^-> T<unit>
            "update" => !?CharCoords * !?CharCoords * !?T<string[]> ^-> T<unit>
        ]

    let SelectionArgs =
        Class "SelectionArgs"
        |+> Instance [
            "head" =? CharCoords
            "anchor" =? CharCoords
        ]

    let ScrollInfo =
        Class "ScrollInfo"
        |+> Instance [
            "left" =? T<int>
            "top" =? T<int>
            "width" =? T<int>
            "height" =? T<int>
            "clientWidth" =? T<int>
            "clientHeight" =? T<int>
        ]

    let Range =
        Generic - fun t ->
        Pattern.Config "CodeMirror.Range" {
            Required =
                [
                    "from", t.Type
                    "to", t.Type
                ]
            Optional = []
        }

    let CodeMirror_t = Type.New()

    module Lint =

        let Severity =
            Pattern.EnumStrings "CodeMirror.Lint.Severity" ["warning"; "error"]

        let Annotation =
            Pattern.Config "CodeMirror.Lint.Annotation" {
                Required =
                    [
                        "from", CharCoords.Type
                        "to", CharCoords.Type
                        "severity", Severity.Type
                    ]
                Optional =
                    [
                        "message", T<string>
                    ]
            }

        let Updater =
            Class "CodeMirror.Lint.Updater"
            |+> Instance [
                    "update" => CodeMirror_t?cm * (Type.ArrayOf Annotation)?ann ^-> T<unit>
                    |> WithInline "$this($cm, $ann)"
                ]

        let Options =
            let Options = Type.New()
            Pattern.Config "CodeMirror.Lint.Options" {
                Required = []
                Optional =
                    [
                        "async", T<bool>
                        "formatAnnotation", Annotation ^-> Annotation
                        "tooltips", T<bool>
                        "onUpdateLinting", Type.ArrayOf Annotation * Type.ArrayOf Annotation * CodeMirror_t ^-> T<unit>
                        "delay", T<int>
                    ]
            }
            |+> Instance [
                    "getAnnotations" =@ (T<string> * Options * CodeMirror_t).Parameter ^-> Type.ArrayOf Annotation
                    |> WithSetterInline "$this.async=false, $this.getAnnotations=$wsruntime.CreateFuncWithArgs($value)"
                    |> WithGetterInline "function(args) { return $this.getAnnotations.apply(this, args) }"
                    "getAnnotationsAsync" =@ (T<string> * Updater * Options * CodeMirror_t).Parameter ^-> T<unit>
                    |> WithSetterInline "$this.async=true, $this.getAnnotations=$wsruntime.CreateFuncWithArgs($value)"
                    |> WithGetterInline "function(args) { return $this.getAnnotations.apply(this, args) }"
                ]
            |=> Options
            |> Requires [Res.Gen .- "lint.js"]

    module Fold =

        let Options =
            Pattern.Config "CodeMirror.Fold.Options" {
                Required =
                    [
                        "rangeFinder", CodeMirror_t * CharCoords ^-> Range.[CharCoords]
                    ]
                Optional =
                    [
                        "widget", T<Element>
                        "scanUp", T<bool>
                        "minFoldSize", T<int>
                    ]
            }
            |> Requires [Res.Gen .- "foldcode.js"]

        let BraceOptions =
            Class "CodeMirror.Fold.BraceOptions"
            |=> Inherits Options
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "{rangeFinder:CodeMirror.braceRangeFinder}"
                ]
            |> Requires [Res.Gen .- "brace-fold.js"]

        let IndentOptions =
            Class "CodeMirror.Fold.IndentOptions"
            |=> Inherits Options
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "{rangeFinder:CodeMirror.indentRangeFinder}"
                ]
            |> Requires [Res.Gen .- "indent-fold.js"]

        let XmlOptions =
            Class "CodeMirror.Fold.XmlOptions"
            |=> Inherits Options
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "{rangeFinder:CodeMirror.xmlRangeFinder}"
                ]
            |> Requires [Res.Gen .- "xml-fold.js"]

        let GutterOptions =
            Pattern.Config "CodeMirror.Fold.GutterOptions" {
                Required = []
                Optional =
                    [
                        "gutter", T<string>
                        "indicatorOpen", T<Element>
                        "indicatorFolded", T<Element>
                        "rangeFinder", CodeMirror_t * CharCoords ^-> Range.[CharCoords]
                    ]
            }
            |+> Static [
                    Constructor Options?Options
                    |> WithInline "{rangeFinder:$Options.rangeFinder}"
                ]
            |> Requires [Res.Gen .- "foldgutter.js"]

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
            "specialChars",T<RegExp>
            "specialCharPlaceholder", T<string -> Element>
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
            
            // specialized
            "dragDrop", T<bool>
            "cursorBlinkRate", T<int>
            "cursorScrollMargin", T<int>
            "cursorHeight", T<float>
            "resetSelectionOnContextMenu", T<bool>
            "workTime", T<int>
            "workDelay", T<int>
            "pollInterval", T<int>
            "flattenSpans", T<bool>
            "addModeClass", T<bool>
            "maxHighlightLength", T<float>
            "crudeMeasuringFrom", T<int>
            "viewportMargin", T<int>

            //// Add-ons

            // matchbrackets.js
            "matchBrackets", T<bool>

            // closetag.js
            "closeTagEnabled", T<bool>
            "closeTagIndent", T<string[]>

            // lint.js
            "lint", Lint.Options.Type

            // foldgutter.js
            "foldGutter", Fold.GutterOptions.Type
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
        |+> Instance [
                "undo" =? T<int>
                "redo" =? T<int>
            ]

    let Token =
        Class "Token"
        |+> Instance [
                "start" =? T<int>
                "end" =? T<int>
                "string" =? T<string>
                "type" =? T<string>
                "state" =? T<obj>
            ]

    let TextMarker =
        Generic - fun t ->
        Class "Mark"
        |+> Instance [
            "clear" => T<unit> ^-> T<unit>
            "find" => T<unit> ^-> t
            "changed" => T<unit> ^-> T<unit>
            event "beforeCursorEnter" T<unit>
            event "clear" T<unit>
            event "hide" T<unit>
            event "unhide" T<unit>
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
        |+> Instance [
            event "delete" T<unit>
            event "change" (LineHandle_t * T<obj>)
        ]

    let line = T<int> + LineHandle

    let LineWidget =
        Class "LineWidget"
        |+> Instance [
            "clear" => T<unit> ^-> T<unit>
            "changed" => T<unit> ^-> T<unit>
            event "redraw" T<unit>
        ]

    let LineInfo =
        Class "LineInfo"
        |+> Instance [
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
        |+> Instance [
                "save" => T<unit> ^-> T<unit>
                "toTextArea" => T<unit> ^-> T<unit>
                "getTextArea" => T<unit> ^-> T<Node>
            ]

    let Dialog =
        Class "Dialog"
        |+> Static [
                Constructor (T<string>)?template
                |> WithInline "$template"
                Constructor (T<Element>)?template
                |> WithInline "$template"
            ]
        |> Requires [Res.Gen .- "dialog.js"]

    let DialogOptions =
        Pattern.Config "DialogOptions" {
            Required = []
            Optional =
                [
                    "closeOnEnter", T<bool>
                    "onKeyDown", T<Event> * T<string> * (T<unit> ^-> T<unit>) ^-> T<unit>
                    "onKeyUp", T<Event> * T<string> * (T<unit> ^-> T<unit>) ^-> T<unit>
                    "onInput", T<Event> * T<string> * (T<unit> ^-> T<unit>) ^-> T<unit>
                    "onClose", T<obj> ^-> T<unit>
                ]
        }

    let NotificationOptions =
        Pattern.Config "NotificationOptions" {
            Required = []
            Optional =
                [
                    "duration", T<int>
                ]
        }

    let SearchCursor =
        Class "SearchCursor"
        |+> Instance [
                "findNext" => T<unit> ^-> T<bool>
                "findPrevious" => T<unit> ^-> T<bool>
                "from" => T<unit> ^-> CharCoords
                "to" => T<unit> ^-> CharCoords
                "replace" => T<string> ^-> T<unit>
            ]
        |> Requires [Res.Gen .- "searchcursor.js"]

    let MIME =
        Class "MIME"
        |+> Instance [
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
        |+> Instance [
                Constructor ((T<string> * T<string>).Parameter ^-> T<unit>)?displayFunc
                |> WithInline "$wsruntime.CreateFuncWithArgs($displayFunc)"
                Constructor T<Element>?container
                |> WithInline "$container"
            ]
        |> Requires [Res.Gen .- "runmode.js"]

    let MatchHighlighter =
        Class "MatchHighlighter"
        |+> Static [
                Constructor T<string>?``class``
                |> WithInline "$class"
            ]
        |> Requires [Res.Gen .- "match-highlighter.js"]

    let TagClosing =
        Class "TagClosing"
        |+> Instance [
                "closeTag" => CodeMirror_t?editor * T<string>?char ^-> T<unit>
                |> WithInline "$this.call($editor, $editor, $char)"
                "closeTag" => CodeMirror_t?editor * T<string>?char * (T<string[]> + T<bool>)?indent ^-> T<unit>
                |> WithInline "$this.call($editor, $editor, $char, $indent)"
            ]
        |> Requires [Res.Gen .- "closetag.js"]

    let Stream =
        Class "Stream"
        |+> Instance [
                "eol" => T<unit -> bool>
                "sol" => T<unit -> bool>
                "peek" => T<unit -> char>
                "next" => T<unit -> char>
                "eat" => (T<char> + T<RegExp> + T<char -> bool>) ^-> T<char>
                "eatWhile" => (T<char> + T<RegExp> + T<char -> bool>) ^-> T<bool>
                "eatSpace" => T<unit -> bool>
                "skipToEnd" => T<unit -> unit>
                "skipTo" => T<char -> bool>
                "match" => T<string>?pattern * !?T<bool>?consume * !?T<bool>?caseFold ^-> T<bool>
                "match" => T<RegExp>?pattern * !?T<bool>?consume * !?T<bool>?caseFold ^-> T<string[]>
                "backUp" => T<int -> unit>
                "column" => T<unit -> int>
                "indentation" => T<unit -> int>
                "current" => T<unit -> string>
            ]

    let Mode =
        Generic - fun state ->
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
                    "lineComment", T<string>
                    "blockCommentStart", T<string>
                    "blockCommentEnd", T<string>
                    "blockCommentLead", T<string>
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
        |> Requires [Res.Gen .- "multiplex.js"]

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

    module Hint =

        let Hint_t = Type.New()

        let Item =
            let Item_t = Type.New()
            Pattern.Config "CodeMirror.Hint.Item" {
                Required = [ "text", T<string> ]
                Optional =
                    [
                        "displayText", T<string>
                        "className", T<string>
                        "render", T<Element> * Hint_t * Item_t ^-> T<unit>
                        "hint", CodeMirror_t * Hint_t * Item_t ^-> T<unit>
                    ]
            }
            |=> Item_t

        let Hint =
            Pattern.Config "CodeMirror.Hint.Hint" {
                Required =
                    [
                        "list", Type.ArrayOf Item
                        "from", CharCoords.Type
                        "to", CharCoords.Type
                    ]
                Optional = []
            }
            |=> Hint_t
            |+> Instance [
                event "shown" T<unit>
                event "select" (Item * T<Element>)
                event "pick" Item
                event "close" T<unit>
            ]
            |> Requires [Res.ShowHint]

        let HintHandle =
            Class "CodeMirror.Hint.HintHandle"
            |+> Instance [
                "moveFocus" => T<int>?n * !?T<bool>?avoidWrap ^-> T<unit>
                "setFocus" => T<int>?n ^-> T<unit>
                "menuSize" => T<unit -> int>
                "length" =? T<int>
                "close" => T<unit -> unit>
                "pick" => T<unit -> unit>
            ]

        let Obj =
            let Obj_t = Type.New()
            Generic - fun t ->
            Class "Obj"
            |=> Obj_t
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "{}"
                ]
            |+> Instance [
                    "with" => T<string>?field * t?value ^-> Obj_t.[t]
                    |> WithInline "($this[$field]=$value),$this"
                    "get" => T<string>?field ^-> t
                    |> WithInline "$this[$field]"
                ]

        let SchemaNode =
            Pattern.Config "CodeMirror.Hint.SchemaNode" {
                Optional = []
                Required =
                    [
                        "attrs", Obj.[T<string[]>]
                        "children", T<string[]>
                    ]
            }

        let SchemaInfo =
            Pattern.Config "CodeMirror.Hint.SchemaInfo" {
                Required = []
                Optional =
                    [
                        "!top", T<string[]>
                    ]
            }
            |=> Inherits Obj.[SchemaNode]
            |> Requires [Res.Gen .- "xml-hint.js"]

        let Options =
            Class "CodeMirror.Hint.Options"
            |+> Instance [
                    "completeSingle" =@ T<bool>
                    "alignWithWord" =@ T<bool>
                    "closeOnUnfocus" =@ T<bool>
                    "customKeys" =@ T<obj>
                    "extraKeys" =@ T<obj>
                    "schemaInfo" =@ SchemaInfo
                ]

        let AsyncOptions =
            Class "CodeMirror.Hint.AsyncOptions"
            |=> Inherits Options
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "{async:true}"
                ]

        let SyncOptions =
            Class "CodeMirror.Hint.SyncOptions"
            |=> Inherits Options
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "{async:false}"
                ]

        let BuiltinFun =
            Class "CodeMirror.Hint.BuiltinFun"

        let JavaScriptHint =
            Class "CodeMirror.Hint.JavaScript"
            |=> Inherits BuiltinFun
            |> Requires [Res.Gen .- "javascript-hint.js"]
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "CodeMirror.javascriptHint"
                ]

        let CoffeeScriptHint =
            Class "CodeMirror.Hint.CoffeeScript"
            |=> Inherits BuiltinFun
            |> Requires [Res.Gen .- "javascript-hint.js"]
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "CodeMirror.coffeescriptHint"
                ]

        let XmlHint =
            Class "CodeMirror.Hint.Xml"
            |=> Inherits BuiltinFun
            |> Requires [Res.Gen .- "xml-hint.js"]
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "CodeMirror.xmlHint"
                ]

        let HtmlHint =
            Class "CodeMirror.Hint.Html"
            |=> Inherits BuiltinFun
            |> Requires [Res.Gen .- "html-hint.js"]
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "CodeMirror.htmlHint"
                ]

        let AnywordHint =
            Class "CodeMirror.Hint.AnyWord"
            |=> Inherits BuiltinFun
            |> Requires [Res.Gen .- "anyword-hint.js"]
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "CodeMirror.anywordHint"
                ]

        let SqlHint =
            Class "CodeMirror.Hint.Sql"
            |=> Inherits BuiltinFun
            |> Requires [Res.Gen .- "sql-hint.js"]
            |+> Static [
                    Constructor T<unit>
                    |> WithInline "CodeMirror.sqlHint"
                ]

        let SyncFun = CodeMirror_t * SyncOptions ^-> Hint
        let AsyncFun = CodeMirror_t * (Hint ^-> T<unit>) * AsyncOptions ^-> T<unit>

    let CodeMirror =
        Class "CodeMirror"
        |=> CodeMirror_t
        |+> Static [
                Constructor ((T<Node> + T<Element -> unit>) * !?CodeMirror_Options)
                "version" =? T<string>
                "fromTextArea" => T<Node>?textArea * !?CodeMirror_Options ^-> CodeMirrorTextArea
                "defaults" =? CodeMirror_Options
                "defineExtension" => T<string> * T<obj> ^-> T<unit>
                Generic - fun t -> "defineOption" => T<string> * t?``default`` * (CodeMirror_t * t ^-> T<unit>) ^-> T<unit>
                "defineInitHook" => (CodeMirror_t ^-> T<unit>) ^-> T<unit>
                "registerHelper" => T<string>?``type`` * T<string>?name * T<obj>?helper ^-> T<unit>
                "Pos" => T<int>?line * !?T<int>?ch ^-> CharCoords
                "changeEnd" => Change ^-> CharCoords

                Generic - fun t -> "defineMode" => T<string> * (CodeMirror_Options * T<obj> ^-> Mode.[t]) ^-> T<unit>
                "defineMIME" => T<string> * T<obj> ^-> T<unit>
                Generic - fun t -> "getMode" => CodeMirror_Options * T<obj> ^-> Mode.[t]
                "modes" =? T<WebSharper.JavaScript.Object<_>>.[CodeMirror_Options * T<obj> ^-> Mode.[T<obj>]]
                "mimeModes" =? T<WebSharper.JavaScript.Object<_>>.[T<obj>]
                Generic - fun t -> "copyState" => Mode.[t] * t ^-> t
                "Pass" => T<unit -> unit>
                |> WithInline "CodeMirror.Pass"

                //// Add-ons

                // hint/show-hint.js
                "showHint" => CodeMirror_t?cm * Hint.BuiltinFun?f * !?Hint.SyncOptions ^-> T<unit>
                "showHint" => CodeMirror_t?cm * Hint.SyncFun?f * !?Hint.SyncOptions ^-> T<unit>
                "showHint" => CodeMirror_t?cm * Hint.AsyncFun?f * Hint.AsyncOptions ^-> T<unit>

                // hint/html-hint.js
                "htmlSchema" =? Hint.SchemaInfo

                // runmode.js
                "runMode" => T<string> * T<obj> * RunModeOutput ^-> T<unit>

                // multiplex.js
                "multiplexingMode" => T<obj>?mode * MultiplexMode ^-> T<obj>
            ]
        |+> Instance (
            [
                // Content manipulation methods
                "getValue" => T<unit> ^-> T<string>
                "setValue" => T<string> ^-> T<unit>
                "getRange" => CharCoords?from * CharCoords?``to`` * !?T<string>?separator ^-> T<string>
                "replaceRange" => T<string>?replacement * CharCoords?from * !?CharCoords?``to`` ^-> T<unit>
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
                "replaceSelection" => T<string>?replacement * !?Collapse ^-> T<unit>
                "getCursor" => !?T<bool> ^-> CharCoords
                "somethingSelected" => T<unit> ^-> T<bool>
                "setCursor" => CharCoords ^-> T<unit>
                "setCursor" => T<int> * T<int> ^-> T<unit>
                "setSelection" => CharCoords?anchor * !?CharCoords?head ^-> T<unit>
                "extendSelection" => CharCoords?from * !?CharCoords?``to`` ^-> T<unit>
                "setExtending" => T<bool> ^-> T<unit>
                "hasFocus" => T<unit> ^-> T<bool>
                "findPosH" => CharCoords?start * T<int>?amount * FindPosHUnit * T<bool>?visually ^-> FindPosCoords
                "findPosV" => CharCoords?start * T<int>?amount * FindPosVUnit * T<bool>?visually ^-> FindPosCoords

                // Configuration options
                "setOption" => T<string> * T<obj> ^-> T<unit>
                "getOption" => T<string> ^-> T<obj>
                "addKeyMap" => T<obj> * T<bool>?bottom ^-> T<unit>
                "removeKeyMap" => T<obj> ^-> T<unit>
                "addOverlay" => (T<string> + T<obj>)?mode * !?OverlayOptions ^-> T<unit>
                "removeOverlay" => (T<string> + T<obj>)?mode ^-> T<unit>
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
                "markText" => CharCoords?from * CharCoords?``to`` * !?TextMarkerOptions ^-> TextMarker.[Range.[CharCoords]]
                "setBookmark" => CharCoords?pos * !?BookmarkOptions ^-> TextMarker.[CharCoords]
                "findMarksAt" => CharCoords ^-> Type.ArrayOf (TextMarker.[T<obj>])
                "getAllMarks" => T<unit> ^-> Type.ArrayOf (TextMarker.[T<obj>])

                // Widget, gutter, and decoration methods
                "setGutterMarker" => line * T<string>?gutterID * T<Element> ^-> LineHandle
                "clearGutter" => T<string> ^-> T<unit>
                "addLineClass" => line * LineClassWhere * T<string> ^-> LineHandle
                "removeLineClass" => line * LineClassWhere * T<string> ^-> LineHandle
                "lineInfo" => line ^-> LineInfo
                "addWidget" => CharCoords * T<Node> * T<bool> ^-> T<unit>
                "addLineWidget" => line?line * T<Node>?node * !?LineWidgetOptions ^-> LineWidget
                "setSize" => (T<int> + T<string>)?width * (T<int> + T<string>)?height ^-> T<unit>
                "scrollTo" => T<int> * T<int> ^-> T<unit>
                "getScrollInfo" => T<unit> ^-> ScrollInfo
                "scrollIntoView" => (CharCoords + Rectangle)?pos * !?T<int>?margin ^-> T<unit>
                "cursorCoords" => T<bool> * CoordsMode ^-> Coords
                "charCoords" => CharCoords * CoordsMode ^-> Coords
                "coordsChar" => Coords ^-> CharCoords
                "lineAtHeight" => T<int>?height * !?T<string>?mode ^-> T<int>
                "heightAtLine" => T<int>?number * !?T<string>?mode ^-> T<int>
                "defaultTextHeight" => T<unit> ^-> T<int>
                "defaultCharWidth" => T<unit> ^-> T<int>
                "getViewport" => T<unit> ^-> Range.[T<int>]
                "refresh" => T<unit> ^-> T<unit>

                // Mode, state, and token-related methods
                Generic - fun t -> "getMode" => T<unit> ^-> Mode.[t]
                Generic - fun t -> "getModeAt" => CharCoords ^-> Mode.[t]
                "getTokenAt" => CharCoords?pos * !?T<bool>?precise ^-> Token
                "getTokenTypeAt" => CharCoords ^-> T<string>
                "getHelper" => CharCoords * T<string>?``type`` ^-> T<obj>
                "getStateAfter" => !?T<int> * !?T<bool>?precise ^-> T<obj>

                // Miscellaneous methods
                Generic - fun t -> "operation" => (T<unit> ^-> t) ^-> t
                "indentLine" => line?line * !?(Indent + T<int>) ^-> T<unit>
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
                event "change" (CodeMirror_t * ChangeArgs)
                event "changes" (CodeMirror_t * Type.ArrayOf ChangeArgs)
                event "beforeChange" (CodeMirror_t * BeforeChangeArgs)
                event "cursorActivity" CodeMirror_t
                event "keyHandled" (CodeMirror_t * T<string> * T<Event>)
                event "inputRead" (CodeMirror_t * T<obj>)
                event "beforeSelectionChange" (CodeMirror_t * SelectionArgs)
                event "viewportChange" (CodeMirror_t * T<int> * T<int>)
                event "gutterClick" (CodeMirror_t * T<int> * T<string> * T<Event>)
                event "gutterContextMenu" (CodeMirror_t * T<int> * T<string> * T<Event>)
                event "focus" CodeMirror_t
                event "blur" CodeMirror_t
                event "scroll" CodeMirror_t
                event "scrollCursorIntoView" (CodeMirror_t * T<Event>)
                event "update" CodeMirror_t
                event "renderLine" (CodeMirror_t * LineHandle)
                event "mousedown" (CodeMirror_t * T<Event>)
                event "dblclick" (CodeMirror_t * T<Event>)
                event "contextmenu" (CodeMirror_t * T<Event>)
                event "keydown" (CodeMirror_t * T<Event>)
                event "keypress" (CodeMirror_t * T<Event>)
                event "keyup" (CodeMirror_t * T<Event>)
                event "dragstart" (CodeMirror_t * T<Event>)
                event "dragenter" (CodeMirror_t * T<Event>)
                event "dragover" (CodeMirror_t * T<Event>)
                event "drop" (CodeMirror_t * T<Event>)

                //// Add-ons

                // fold/foldcode.js
                "foldCode" => (T<int> + CharCoords) * Fold.Options ^-> T<unit>

                // dialog.js
                "openDialog" => Dialog * T<string -> unit>?callback * !?DialogOptions ^-> (T<unit -> unit>)
                "openNotification" => Dialog * !?NotificationOptions ^-> (T<unit> ^-> T<unit>)

                // edit/matchbrackets.js
                "matchBrackets" => T<unit> ^-> T<unit>

                // searchcursor.js
                "getSearchCursor" => (T<string> + T<RegExp>)?query * !?CharCoords * !?T<bool> ^-> SearchCursor

                // match-highlighter.js
                "matchHighlight" => MatchHighlighter ^-> T<unit>

                // closetag.js
                "closeTag" =? TagClosing
            ]
            @ List.map (fun (name, ty) ->
                    ("option_" + name) =@ ty
                    |> WithGetterInline ("$this.getOption('" + name + "')")
                    |> WithSetterInline ("$this.setOption('" + name + "', $value)")
                    :> _)
                options)
        |> Requires [Res.Js]

    let Assembly =
        Assembly [
            Namespace "WebSharper.CodeMirror" [
                BeforeChangeArgs
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
                DialogOptions
                FindPosCoords
                FindPosHUnit
                FindPosVUnit
                History
                HistorySize
                Indent
                LineClassWhere
                LineHandle
                LineInfo
                LineWidget
                LineWidgetOptions
                MatchHighlighter
                MultiplexMode
                MIME
                Mode
                NotificationOptions
                OverlayOptions
                Range
                Rectangle
                RunModeOutput
                ScrollInfo
                SearchCursor
                SelectionArgs
                Stream
                TagClosing
                TextMarker
                TextMarkerOptions
                Token
            ]
            Namespace "WebSharper.CodeMirror.Lint" [
                Lint.Annotation
                Lint.Options
                Lint.Severity
                Lint.Updater
            ]
            Namespace "WebSharper.CodeMirror.Hint" [
                Hint.Hint
                Hint.HintHandle
                Hint.Item
                Hint.BuiltinFun
                Hint.JavaScriptHint
                Hint.CoffeeScriptHint
                Hint.XmlHint
                Hint.HtmlHint
                Hint.Options
                Hint.AsyncOptions
                Hint.SyncOptions
                Hint.Obj
                Hint.SchemaNode
                Hint.SchemaInfo
                Hint.AnywordHint
                Hint.SqlHint
            ]
            Namespace "WebSharper.CodeMirror.Fold" [
                Fold.Options
                Fold.BraceOptions
                Fold.IndentOptions
                Fold.XmlOptions
                Fold.GutterOptions
            ]
            Namespace "WebSharper.CodeMirror.Resources" [
                Res.Css
                Res.ModeMeta
                Res.Js
            ]
            Namespace "WebSharper.CodeMirror.Resources.Addons"  (upcast Res.ShowHint :: upcast Res.ShowHintCss :: Res.GroupedGen .- "addon")
            Namespace "WebSharper.CodeMirror.Resources.Modes"   (Res.GroupedGen .- "mode") 
            Namespace "WebSharper.CodeMirror.Resources.Keymaps" (Res.GroupedGen .- "keymap") 
            Namespace "WebSharper.CodeMirror.Resources.Themes"  (Res.GroupedGen .- "theme") 
            Namespace "WebSharper.CodeMirror.Resources.Css"     (Res.GroupedGen .- "css") 
        ]

open WebSharper.InterfaceGenerator

[<Sealed>]
type CodeMirrorExtension() =
    interface IExtension with
        member ext.Assembly = Definition.Assembly

[<assembly: Extension(typeof<CodeMirrorExtension>)>]
do ()
