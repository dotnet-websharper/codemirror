namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator
open State
open View

module AutoComplete =

    let ImportFromAutoComplete (c: CodeModel.Class) = 
        Import c.Name "@codemirror/autocomplete" c

    let AbortListenerOptions =
        Pattern.Config "AbortListenerOptions" {
            Required = []
            Optional = [
                "onDocChange", T<bool>
            ]
        }

    let TokenBeforeResult =
        Pattern.Config "TokenBeforeResult" {
            Required = [
                "from", T<int>
                "to", T<int>
                "text", T<string>
                "type", T<obj>
            ]
            Optional = []
        }

    let MatchBeforeResult =
        Pattern.Config "MatchBeforeResult" {
            Required = [
                "from", T<int>
                "to", T<int>
                "text", T<string>
            ]
            Optional = []
        }

    let CompletionInfoObject =
        Pattern.Config "CompletionInfoObject" {
            Required = [
                "dom", T<Dom.Node>
            ]
            Optional = [
                "destroy", T<unit> ^-> T<unit>
            ]
        }

    let CompletionInfo = T<Dom.Node> + CompletionInfoObject

    let CompletionSection =
        Pattern.Config "CompletionSection" {
            Required = [
                "name", T<string>
            ]
            Optional = [
                "header", TSelf ^-> T<HTMLElement>
                "rank", T<int>
            ]
        }
        |> ImportFromAutoComplete

    let Completion =
        Pattern.Config "Completion" {
            Required = []
            Optional = [
                "label", T<string>
                "displayLabel", T<string>
                "detail", T<string>
                "info", T<string> + (TSelf ^-> CompletionInfo + T<Promise<_>>[CompletionInfo])
                "apply", T<string> + (EditorView * TSelf * T<int> * T<int> ^-> T<unit>)
                "type", T<string>
                "commitCharacters", !| T<string>
                "boost", T<int>
                "section", T<string> + CompletionSection
            ]
        }
        |> ImportFromAutoComplete

    let CompletionContext = Class "CompletionContext"

    let CompletionResult =
        Pattern.Config "CompletionResult" {
            Required = [
                "from", T<int>
                "options", !| Completion
            ]
            Optional = [
                "to", T<int>
                "validFor", T<RegExp> + (T<string> * T<int> * T<int> * EditorState ^-> T<bool>)
                "filter", T<bool>
                "getMatch", Completion * !? (!| T<int>) ^-> !| T<int>
                "update", TSelf * T<int> * T<int> * CompletionContext ^-> TSelf
                "map", TSelf * ChangeDesc ^-> TSelf
                "commitCharacters", !| T<string>
            ]
        }
        |> ImportFromAutoComplete

    let CompletionSource = CompletionContext ^-> CompletionResult + T<Promise<_>>[CompletionResult]

    let AddToOptions = 
        Pattern.Config "AddToOptions" {
            Required = [
                "render", Completion * EditorState * EditorView ^-> T<Dom.Node>
                "position", T<int>
            ]
            Optional = []
        }

    let PositionInfoResult =
        Pattern.Config "PositionInfoResult" {
            Required = []
            Optional = [
                "style", T<string>
                "class", T<string>
            ]
        }

    let CompletionConfig =
        Pattern.Config "CompletionConfig" {
            Required = []
            Optional = [
                "activateOnTyping", T<bool>
                "activateOnCompletion", Completion ^-> T<bool>
                "activateOnTypingDelay", T<int>
                "selectOnOpen", T<bool>
                "override", !| CompletionSource
                "closeOnBlur", T<bool>
                "maxRenderedOptions", T<int>
                "defaultKeymap", T<bool>
                "aboveCursor", T<bool>
                "tooltipClass", EditorState ^-> T<string>
                "optionClass", Completion ^-> T<string>
                "icons", T<bool>
                "addToOptions", !| AddToOptions
                "positionInfo", EditorView * Rect * Rect * Rect * Rect ^-> PositionInfoResult
                "compareCompletions", Completion * Completion ^-> T<int>
                "filterStrict", T<bool>
                "interactionDelay", T<int>
                "updateSyncTime", T<int>
            ]
        }

    CompletionContext
        |> ImportFromAutoComplete
        |+> Instance [
            "state" =? EditorState
            "pos" =? T<int>
            "explicit" =? T<bool>
            "view" =? EditorView

            "tokenBefore" => !| T<string> ^-> TokenBeforeResult

            "matchBefore" => T<RegExp> ^-> MatchBeforeResult

            "aborted" =? T<bool>

            "addEventListener" => T<string> * (T<unit> ^-> T<unit>) * !? AbortListenerOptions ^-> T<unit>
        ]
        |+> Static [
            Constructor (EditorState * T<int> * T<bool> * !? EditorView)
        ]
        |> ignore


