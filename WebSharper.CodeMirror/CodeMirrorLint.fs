namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator
open State
open View

module Lint =

    let ImportFromLint (c: CodeModel.Class) = 
        Import c.Name "@codemirror/lint" c

    let Severity =
        Pattern.EnumStrings "Severity" [
            "hint"
            "info"
            "warning"
            "error"
        ]
        |> ImportFromLint

    let Action =
        Pattern.Config "Action" {
            Required = [
                "name", T<string>
                "apply", EditorView * T<int> * T<int> ^-> T<unit>
            ]
            Optional = []
        }
        |> ImportFromLint

    let Diagnostic =
        Pattern.Config "Diagnostic" {
            Required = [
                "from", T<int>
                "to", T<int>
                "severity", Severity.Type
                "message", T<string>
            ]
            Optional = [
                "markClass", T<string>
                "source", T<string>
                "renderMessage", EditorView ^-> T<Dom.Node>
                "actions", !| Action
            ]
        }
        |> ImportFromLint

    let LintSource = EditorView ^-> (!| Diagnostic + T<Promise<_>>[!| Diagnostic])

    let DiagnosticFilter = !| Diagnostic * EditorState ^-> !| Diagnostic

    let LintConfig =
        Pattern.Config "LintConfig" {
            Required = []
            Optional = [
                "delay", T<int>
                "needsRefresh", T<unit> + (ViewUpdate ^-> T<bool>)
                "markerFilter", T<unit> + DiagnosticFilter
                "tooltipFilter", T<unit> + DiagnosticFilter
                "hideOn", Transaction * T<int> * T<int> ^-> (T<bool> + T<unit>)
                "autoPanel", T<bool>
            ]
        }

    let LintGutterConfig =
        Pattern.Config "LintGutterConfig" {
            Required = []
            Optional = [
                "hoverTime", T<int>
                "markerFilter", T<unit> + DiagnosticFilter
                "tooltipFilter", T<unit> + DiagnosticFilter
            ]
        }

