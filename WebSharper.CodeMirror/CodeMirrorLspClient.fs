namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module LSPClient = 
    let ImportFromLSPClient (c: CodeModel.Class) = 
        Import c.Name "@codemirror/lsp-client" c

    let LSPClientClass =
        Class "LSPClient"

    let WorkspaceMapping =
        Class "WorkspaceMapping"
        |> ImportFromLSPClient
        |+> Instance [
            "getMapping" => T<string> ^-> State.ChangeDesc
        
            "mapPos" => T<string> * T<int> * !? T<int> ^-> T<int>
            "mapPos" => T<string> * T<int> * T<int> * State.MapMode ^-> T<int>

            "mapPosition" => T<string> * T<obj> * !? T<int> ^-> T<int>
            "mapPosition" => T<string> * T<obj> * T<int> * State.MapMode ^-> T<int>

            "destroy" => T<unit> ^-> T<unit>
        ]

    let WorkspaceFile =
        Pattern.Config "WorkspaceFile" {
            Required = [
                "uri", T<string>
                "languageId", T<string>
                "version", T<int>
                "doc", State.Text.Type
                "getView", !?View.EditorView ^-> View.EditorView
            ]
            Optional = []
        }
        |> ImportFromLSPClient

    let WorkspaceFileUpdate =
        Pattern.Config "WorkspaceFileUpdate" {
            Required = [
                "file", WorkspaceFile.Type
                "prevDoc", State.Text.Type
                "changes", State.ChangeSet.Type
            ]
            Optional = []
        }

    let Workspace =
        Class "Workspace"
        |> ImportFromLSPClient
        |+> Instance [
            "client" =? LSPClientClass
            "files" =? !| WorkspaceFile            

            "getFile" => T<string> ^-> WorkspaceFile
            "syncFiles" => T<unit> ^-> !| WorkspaceFileUpdate
            "requestFile" => T<string> ^-> T<Promise<_>>[WorkspaceFile]
            "openFile" => T<string> * T<string> * View.EditorView ^-> T<unit>
            "closeFile" => T<string> * View.EditorView ^-> T<unit>
            "connected" => T<unit> ^-> T<unit>
            "disconnected" => T<unit> ^-> T<unit>
            "updateFile" => T<string> * State.TransactionSpec ^-> T<unit>
            "displayFile" => T<string> ^-> T<Promise<_>>[View.EditorView]
        ]
        |+> Static [
            Constructor (LSPClientClass)
        ]

    let LSPClientConfig =
        Pattern.Config "LSPClientConfig" {
            Required = []
            Optional = [
                "rootUri", T<string>
                "workspace", LSPClientClass ^-> Workspace
                "timeout", T<int>
                "sanitizeHTML", T<string> ^-> T<string>
                "highlightLanguage", T<string> ^-> Language.Language
                "notificationHandlers", T<obj>
                "unhandledNotification", LSPClientClass * T<string> * T<obj> ^-> T<unit>
            ]
        }
        |> ImportFromLSPClient

    let Transport =
        Pattern.Config "Transport" {
            Required = [
                "send", T<string> ^-> T<unit>
                "subscribe", (T<string> ^-> T<unit>) ^-> T<unit>
                "unsubscribe", (T<string> ^-> T<unit>) ^-> T<unit>
            ]
            Optional = []
        }
        |> ImportFromLSPClient

    let LSPPlugin =
        Class "LSPPlugin"
        |> ImportFromLSPClient
        |+> Instance [
            "view" =? View.EditorView
            "client" =? LSPClientClass
            "uri" =? T<string>

            "docToHTML" => (T<string> + T<obj>) * !? T<obj> ^-> T<string>
            "toPosition" => T<int> * !? State.Text ^-> T<obj>
            "fromPosition" => T<obj> * !? State.Text ^-> T<int>
            "reportError" => T<string> * T<obj> ^-> T<unit>
            "unsyncedChanges" =? State.ChangeSet
            "clear" => T<unit> ^-> T<unit>
        ]
        |+> Static [
            "get" => View.EditorView ^-> TSelf
            "create" => LSPClientClass * T<string> * !? T<string> ^-> State.Extension
        ]

    let ServerCompletionConfig =
        Pattern.Config "ServerCompletionConfig" {
            Required = []
            Optional = [
                "override", T<bool>
            ]
        }

    let SignatureHelpConfig =
        Pattern.Config "SignatureHelpConfig" {
            Required = []
            Optional = [
                "keymap", T<bool>
            ]
        }

    LSPClientClass
        |> ImportFromLSPClient
        |+> Instance [
            "workspace" =? Workspace
            "serverCapabilities" =? T<obj>
            "initializing" =? T<Promise<obj>>
            "connected" =? T<bool>

            "connect" => Transport ^-> TSelf
            "disconnect" => T<unit> ^-> T<unit>
            "didOpen" => WorkspaceFile ^-> T<unit>
            "didClose" => T<string> ^-> T<unit>

            Generic -- fun param result ->
                "request" => T<string> * param ^-> T<Promise<_>>[result]

            Generic - fun param ->
                "notification" => T<string> * param ^-> T<unit>

            "cancelRequest" => T<obj> ^-> T<unit>
            "workspaceMapping" => WorkspaceMapping

            Generic - fun t ->
                "withMapping" => (WorkspaceMapping ^-> T<Promise<_>>[t]) ^-> T<Promise<_>>[t]

            "sync" => T<unit> ^-> T<unit>
        ]
        |+> Static [
            Constructor (!? LSPClientConfig)
        ]
        |> ignore