namespace WebSharper.CodeMirror.Definition

open WebSharper.InterfaceGenerator
open State
open View

module CodeMirrorCommand =
    let BlockComment =
        Pattern.Config "BlockComment" {
            Required = [
                "open", T<string>
                "close", T<string>
            ]
            Optional = []
        }

    let CommentTokens =
        Pattern.Config "CommentTokens" {
            Required = []
            Optional = [
                "block", BlockComment.Type
                "line", T<string>
            ]
        }
        |> Import "CommentTokens" "@codemirror/commands"

    let HistoryConfig =
        Pattern.Config "HistoryConfig" {
            Required = []
            Optional = [
                "minDepth", T<int>
                "newGroupDelay", T<int>
                "joinToEvent", Transaction * T<bool> ^-> T<bool>
            ]
        }