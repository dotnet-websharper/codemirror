namespace WebSharper.CodeMirror.Definition

open WebSharper.InterfaceGenerator
open State
open View

module Search =

    let ImportFromSearch (c: CodeModel.Class) = 
        Import c.Name "@codemirror/search" c

    let SearchQueryConfig =
        Pattern.Config "SearchQueryConfig" {
            Required = [
                "search", T<string>
            ]
            Optional = [
                "caseSensitive", T<bool>
                "literal", T<bool>
                "regexp", T<bool>
                "replace", T<string>
                "wholeWord", T<bool>
            ]
        }

    let SearchQuery =
        Class "SearchQuery"
        |> ImportFromSearch
        |+> Instance [
            "search" =? T<string>
            "caseSensitive" =? T<bool>
            "literal" =? T<bool>
            "regexp" =? T<bool>
            "replace" =? T<string>
            "valid" =? T<bool>
            "wholeWord" =? T<bool>

            "eq" => TSelf ^-> T<bool>

            "getCursor" => (EditorState + Text) * !? T<int> * !? T<int> ^-> Language.RangeSpan
        ]
        |+> Static [
            Constructor SearchQueryConfig
        ]

    let HighlightOptions =
        Pattern.Config "HighlightOptions" {
            Required = []
            Optional = [
                "highlightWordAroundCursor", T<bool>
                "minSelectionLength", T<int>
                "maxMatches", T<int>
                "wholeWords", T<bool>
            ]
        }

    let SearchConfig =
        Pattern.Config "SearchConfig" {
            Required = []
            Optional = [
                "top", T<bool>
                "caseSensitive", T<bool>
                "literal", T<bool>
                "wholeWord", T<bool>
                "regexp", T<bool>
                "createPanel", EditorView ^-> Panel
                "scrollToMatch", SelectionRange * EditorView ^-> StateEffect.[T<obj>]
            ]
        }

    let RegExpMatchValue =
        Pattern.Config "RegExpMatchValue" {
            Required = [
                "from", T<int>
                "to", T<int>
                "match", T<obj>
            ]
            Optional = []
        }

    let RegExpCursorOptions =
        Pattern.Config "RegExpCursorOptions" {
            Required = []
            Optional = [
                "ignoreCase", T<bool>
                "test", T<int> * T<int> * T<obj> ^-> T<bool>
            ]
        }

    let RegExpCursor =
        Class "RegExpCursor"
        |> ImportFromSearch
        |+> Instance [
            "done" =? T<bool>
            "value" =? RegExpMatchValue

            "next" => T<unit> ^-> TSelf
        ]
        |+> Static [
            Constructor (Text * T<string> * !? RegExpCursorOptions * !? T<int> * !? T<int>)
        ]

    let SearchCursor =
        Class "SearchCursor"
        |> ImportFromSearch
        |+> Instance [
            "value" =? Language.RangeSpan
            "done" =? T<bool>

            "next" => T<unit> ^-> TSelf
            "nextOverlapping" => T<unit> ^-> TSelf
        ]
        |+> Static [
            Constructor (
                Text * T<string> * !? T<int> * !? T<int> * !? (T<string> ^-> T<string>) * !? (T<int> * T<int> * T<string> * T<int> ^-> T<bool>)
            )
        ]
