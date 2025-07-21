namespace WebSharper.CodeMirror.Definition

open WebSharper.InterfaceGenerator
open State

module Collab =

    let Update =
        Pattern.Config "Update" {
            Required = [
                "changes", ChangeSet.Type
                "clientID", T<string>
            ]
            Optional = [
                "effects", !| StateEffect.[T<obj>]
            ]
        }

    let CollabConfig =
        Pattern.Config "CollabConfig" {
            Required = []
            Optional = [
                "startVersion", T<int>
                "clientID", T<string>
                "sharedEffects", (Transaction ^-> !| StateEffect.[T<obj>])
            ]
        }
