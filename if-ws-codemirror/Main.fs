﻿namespace IntelliFactory.WebSharper.CodeMirror.Definition

module Definition =
    open IntelliFactory.WebSharper.InterfaceGenerator
    open IntelliFactory.WebSharper.Dom

    let CharCoords =
        Pattern.Config "CharCoords" {
            Optional = []
            Required =
                [
                    "line", T<int>
                    "ch", T<int>
                ]
        }

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

    let CodeMirror =
        Class "CodeMirror"
        |=> CodeMirror_t
        |+> [
                Constructor ((T<Node> + T<Element -> unit>) * !?CodeMirror_Options)
                "fromTextArea" => T<Node> * !?CodeMirror_Options ^-> CodeMirrorTextArea
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
                "indentLine" => line * T<bool> ^-> T<unit>
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
                "getline" => T<int> ^-> T<string>
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
            ] @ List.map (fun (name, ty) ->
                    ("option_" + name) =% ty
                    |> WithGetterInline ("$this.getOption('" + name + "')")
                    |> WithSetterInline ("$this.setOption('" + name + "', $value)")
                    :> CodeModel.Member)
                options)

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
                HistorySize
                LineHandle
                LineInfo
                Generic - Mark
                Range
                Token
            ]
        ]

module Main =
    open IntelliFactory.WebSharper.InterfaceGenerator

    do Compiler.Compile stdout Definition.Assembly
