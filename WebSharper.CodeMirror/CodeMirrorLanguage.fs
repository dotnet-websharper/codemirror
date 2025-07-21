namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator
open State

module Language = 

    let ImportFromLanguage (c: CodeModel.Class) = 
        Import c.Name "@codemirror/language" c

    let Parser = T<obj>
    let LRParser = T<obj>

    let Regions =
        Pattern.Config "Regions" {
            Required = [
                "from", T<int>
                "to", T<int>
            ]
            Optional = []
        }

    let Language =
        Class "Language"
        |> ImportFromLanguage
        |+> Instance [
            "data" =? Facet.[T<obj>, T<obj>]
            "name" =? T<string>
            "extension" =? Extension
            "parser" =@ Parser

            "isActiveAt" => EditorState * T<int> * !? T<int> ^-> T<bool>
            "findRegions" => EditorState ^-> !| Regions
            "allowsNesting" =? T<bool>
        ]
        |+> Static [
            Constructor (
                Facet.[T<obj>, T<obj>] * Parser * !| Extension * !? T<string>
            )
        ]

    let LRLanguageSpec =
        Pattern.Config "LRLanguageSpec" {
            Required = [
                "parser", LRParser
            ]
            Optional = [
                "name", T<string>
                "languageData", T<obj>
            ]
            }

    let LRLanguage =
        Class "LRLanguage"
        |=> Inherits Language
        |> ImportFromLanguage
        |+> Instance [
            "parser" =? LRParser
            "configure" => T<obj> * !? T<string> ^-> TSelf
            "allowsNesting" =? T<bool>
        ]
        |+> Static [
            "define" => LRLanguageSpec ^-> TSelf
        ]

    let LanguageSupport =
        Class "LanguageSupport"
        |> ImportFromLanguage
        |+> Instance [
            "language" =? Language
            "support" =? Extension
            "extension" =@ Extension
        ]
        |+> Static [
            Constructor (Language * !? Extension)
        ]

    let HighlightStyleOptions = 
        Pattern.Config "HighlightStyleOptions" {
            Required = []
            Optional = [
                "scope", Language + T<obj>
                "all", T<string> + T<obj>
                "themeType", T<string> // "dark" | "light"
            ]
        }

    let TagStyle =
        Pattern.Config "TagStyle" {
            Required = [
                "tag", T<obj> + !| T<obj>
            ]
            Optional = [
                "class", T<string>
            ]
        }

    let HighlightStyle =
        Class "HighlightStyle"
        |> ImportFromLanguage
        |+> Instance [
            "specs" =? !| TagStyle
            "module" =? T<obj>
            "style" => !| T<obj> ^-> T<string>
            "scope" =? T<obj> ^-> T<bool>
        ]
        |+> Static [
            "define" => !| TagStyle * !? HighlightStyleOptions ^-> TSelf
        ]

    let RangeConfig =
        Pattern.Config "RangeConfig" {
            Required = [
                "from", T<int>
                "to", T<int>
            ]
            Optional = []
        }
    
    let FoldConfig =
        Pattern.Config "FoldConfig" {
            Required = []
            Optional = [
                "placeholderDOM", (View.EditorView * (T<obj> ^-> T<unit>) * T<obj> ^-> T<HTMLElement>) + T<unit>
                "placeholderText", T<string>
                "preparePlaceholder", EditorState * RangeConfig ^-> T<obj>
            ]
        }

    let FoldGutterConfig =
        Pattern.Config "FoldGutterConfig" {
            Required = []
            Optional = [
                "markerDOM", (T<bool> ^-> T<HTMLElement>) + T<unit>
                "openText", T<string>
                "closedText", T<string>
                "domEventHandlers", T<obj>
                "foldingChanged", View.ViewUpdate ^-> T<bool>
            ]
        }

    let Command = View.EditorView ^-> T<bool>

    let KeyBinding =
        Pattern.Config "KeyBinding" {
            Required = []
            Optional = [
                "key", T<string>
                "mac", T<string>
                "win", T<string>
                "linux", T<string>
                "run", Command
                "shift", Command
                "any", View.EditorView * T<Dom.KeyboardEvent> ^-> T<bool>
                "scope", T<string>
                "preventDefault", T<bool>
                "stopPropagation", T<bool>
            ]
        }

    let IndentContextOptions =
        Pattern.Config "IndentContextOptions" {
            Required = []
            Optional = [
                "overrideIndentation", T<int> ^-> T<int>
                "simulateBreak", T<int>
                "simulateDoubleBreak", T<bool>
            ]
        }

    let LineAtResult =
        Pattern.Config "LineAtResult" {
            Required = []
            Optional = [
                "text", T<string>
                "from", T<int>
            ]
        }

    let IndentContext =
        Class "IndentContext"
        |> Import "IndentContext" "@codemirror/language"
        |+> Instance [
            "state" =? EditorState
            "unit" =@ T<int>

            "lineAt" => T<int> * !? T<int> ^-> LineAtResult

            "textAfterPos" => T<int> * !? T<int> ^-> T<string>

            "column" => T<int> * !? T<int> ^-> T<int>

            "countColumn" => T<string> * !? T<int> ^-> T<int>

            "lineIndent" => T<int> * !? T<int> ^-> T<int>

            "simulatedBreak" =? T<int>
        ]
        |+> Static [
            Constructor (EditorState * !? IndentContextOptions)
        ]

    let SyntaxHighlightingOptions =
        Pattern.Config "SyntaxHighlightingOptions" {
            Required = []
            Optional = [
                "fallback", T<bool>
            ]
        }

    let TreeIndentContext =
        Class "TreeIndentContext"
        |> ImportFromLanguage
        |=> Inherits IndentContext
        |+> Instance [
            "pos" =? T<int>

            "node" =? T<obj>

            "textAfter" =? T<string>

            "baseIndent" =? T<int>

            "baseIndentFor" => T<obj> ^-> T<int>

            "continue" => T<unit> ^-> T<int> + T<unit>
        ]

    let RangeSpan =
        Pattern.Config "RangeSpan" {
            Required = [
                "from", T<int>
                "to", T<int>
            ]
            Optional = []
        }

    let MatchResult =
        Pattern.Config "MatchResult" {
            Required = [
                "start", RangeSpan.Type
                "matched", T<bool>
            ]
            Optional = [
                "end", RangeSpan.Type
            ]
        }

    let Config =
        Pattern.Config "Config" {
            Required = []
            Optional = [
                "afterCursor", T<bool>
                "brackets", T<string>
                "maxScanDistance", T<int>
                "renderMatch", MatchResult * EditorState ^-> !| Range.[View.Decoration]
            ]
        }

    let LanguageDescriptionSpec =
        Pattern.Config "LanguageDescriptionSpec" {
            Required = []
            Optional = [
                "name", T<string>
                "alias", !| T<string>
                "extensions", !| T<string>
                "filename", T<RegExp>
                "load", T<unit> ^-> T<Promise<_>>[LanguageSupport]
                "support", LanguageSupport.Type
            ]
        }

    let LanguageDescription =
        Class "LanguageDescription"
        |> ImportFromLanguage
        |+> Instance [
            "name" =? T<string>
            "alias" =? !| T<string>
            "extensions" =? !| T<string>
            "filename" =? T<RegExp> + T<obj>
            "support" =? LanguageSupport.Type
            "load" => T<unit> ^-> T<Promise<_>>[LanguageSupport]
        ]
        |+> Static [
            "of" => LanguageDescriptionSpec ^-> TSelf
            "matchFilename" => !| TSelf * T<string> ^-> TSelf
            "matchLanguageName" => !| TSelf * T<string> * !? T<bool> ^-> TSelf
        ]