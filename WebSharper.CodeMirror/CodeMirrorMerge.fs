namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Merge =
    let ImportFromMerge (c: CodeModel.Class) = 
        Import c.Name "@codemirror/merge" c

    let Chunk =
        Class "Chunk"

    let Change =
        Class "Change"
        |> ImportFromMerge
        |+> Instance [
            "fromA" =? T<int>
            "toA" =? T<int>
            "fromB" =? T<int>
            "toB" =? T<int>
        ]
        |+> Static [
            Constructor ( T<int> * T<int> * T<int> * T<int> )
        ]

    let DiffConfig =
        Pattern.Config "DiffConfig" {
            Required = []
            Optional = [
                "scanLimit", T<int>
                "timeout", T<int>
            ]
        }
        |> ImportFromMerge

    let CollapseUnchangedConfig =
        Pattern.Config "CollapseUnchangedConfig" {
            Required = []
            Optional = [
                "margin", T<int>
                "minSize", T<int>
            ]
        }

    let MergeConfig =
        Pattern.Config "MergeConfig" {
            Required = []
            Optional = [
                "orientation", T<string> 
                "revertControls", T<string>
                "renderRevertControl", T<unit -> HTMLElement>
                "highlightChanges", T<bool>
                "gutter", T<bool>
                "collapseUnchanged", CollapseUnchangedConfig.Type
                "diffConfig", DiffConfig.Type
            ]
        }
        |> ImportFromMerge

    let DirectMergeConfig =
        Pattern.Config "DirectMergeConfig" {
            Required = [
                "a", State.EditorStateConfig.Type
                "b", State.EditorStateConfig.Type
            ]
            Optional = [
                "parent", T<Dom.Element> + T<Dom.DocumentFragment>
                "root", T<Dom.Document> + T<Dom.ShadowRoot>

                // Inherit from MergeConfig
                "orientation", T<string> 
                "revertControls", T<string>
                "renderRevertControl", T<unit -> HTMLElement>
                "highlightChanges", T<bool>
                "gutter", T<bool>
                "collapseUnchanged", CollapseUnchangedConfig.Type
                "diffConfig", DiffConfig.Type
            ]
        }
        |> ImportFromMerge

    let UnifiedMergeConfig =
        Pattern.Config "UnifiedMergeConfig" {
            Required = [
                "original", T<string> + State.Text
            ]
            Optional = [
                "highlightChanges", T<bool>
                "gutter", T<bool>
                "syntaxHighlightDeletions", T<bool>
                "allowInlineDiffs", T<bool>
                "syntaxHighlightDeletionsMaxLength", T<int>
                "mergeControls", T<bool>
                "diffConfig", DiffConfig.Type
                "collapseUnchanged", CollapseUnchangedConfig.Type
            ]
        }

    let MergeView =
        Class "MergeView"
        |> ImportFromMerge
        |+> Instance [
            "a" =? View.EditorView
            "b" =? View.EditorView
            "dom" =? T<HTMLElement>
            "chunks" =? !| Chunk
            "destroy" => T<unit> ^-> T<unit>
            "reconfigure" => MergeConfig ^-> T<unit>
        ]
        |+> Static [
            Constructor (DirectMergeConfig)
        ]        

    let UpdateOriginalDoc =
        Pattern.Config "UpdateOriginalDoc" {
            Required = [
                "doc", State.Text.Type
                "changes", State.ChangeSet.Type
            ]
            Optional = []
        }

    let ChunksResult =
        Pattern.Config "ChunksResult" {
            Required = [
                "chunks", !| Chunk
                "side", T<string>
            ]
            Optional = []
        }

    Chunk
        |> ImportFromMerge
        |+> Instance [
            "changes" =? !| Change
            "fromA" =? T<int>
            "toA" =? T<int>
            "fromB" =? T<int>
            "toB" =? T<int>
            "precise" =? T<bool> ^-> T<bool>
            "endA" =? T<int> ^-> T<int>
            "endB" =? T<int> ^-> T<int>
        ]
        |+> Static [
            Constructor ( !| Change * T<int> * T<int> * T<int> * T<int> * !? T<bool> )
            "build" => State.Text * State.Text * !? DiffConfig ^-> !| TSelf
            "updateA" => !| TSelf * State.Text * State.Text * State.ChangeDesc * !? DiffConfig ^-> !| TSelf
            "updateB" => !| TSelf * State.Text * State.Text * State.ChangeDesc * !? DiffConfig ^-> !| TSelf
        ]
        |> ignore

