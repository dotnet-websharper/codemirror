namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator   
open State
open View
open Language

module Definition =    
    open WebSharper.JavaScript.Dom
    type RegExp = WebSharper.JavaScript.RegExp

    let HoverTooltipOptions =
        Pattern.Config "HoverTooltipOptions" {
            Required = []
            Optional = [
                "hideOn", Transaction * Tooltip ^-> T<bool>
                "hideOnChange", T<bool> + T<string>
                "hoverTime", T<int>
            ]
        }

    let JavaScriptConfig =
        Pattern.Config "JavaScriptConfig" {
            Required = []
            Optional = [
                "jsx", T<bool>
                "typescript", T<bool>
            ]
        }

    let CompletionPathResult =
        Pattern.Config "CompletionPathResult" {
            Required = [
                "path", !| T<string>
                "name", T<string>
            ]
            Optional = []
        }

    let MarkdownConfig =
        Pattern.Config "MarkdownConfig" {
            Required = []
            Optional = [
                "defaultCodeLanguage", Language + LanguageSupport
                "codeLanguages", (!| LanguageDescription) + ((T<string> ^-> Language) + LanguageDescription)
                "addKeymap", T<bool>
                "extensions", T<obj>
                "base", Language.Type
                "completeHTMLTags", T<bool>
                "htmlTagLanguage", LanguageSupport.Type
            ]
        }

    let YamlFrontmatterConfig =
        Pattern.Config "YamlFrontmatterConfig" {
            Required = [
                "content", Language + LanguageSupport
            ]
            Optional = []
        }

    let PhpConfig =
        Pattern.Config "PhpConfig" {
            Required = []
            Optional = [
                "baseLanguage", Language.Type
                "plain", T<bool>
            ]
        }

    let SassConfig =
        Pattern.Config "SassConfig" {
            Required = []
            Optional = [
                "indented", T<bool>
            ]
        }

    let LiquidCompletionConfig =
        Pattern.Config "LiquidCompletionConfig" {
            Required = []
            Optional = [
                "tags", !| AutoComplete.Completion
                "filters", !| AutoComplete.Completion
                "variables", !| AutoComplete.Completion
                "properties", T<string[]> * EditorState * AutoComplete.CompletionContext ^-> !| AutoComplete.Completion
                "base", LanguageSupport.Type
            ]
        }
        |> Import "JinjaCompletionConfig" "@codemirror/lang-liquid"

    let JinjaCompletionConfig =
        Pattern.Config "JinjaCompletionConfig" {
            Required = []
            Optional = [
                "tags", !| AutoComplete.Completion
                "variables", !| AutoComplete.Completion
                "properties", T<string[]> * EditorState * AutoComplete.CompletionContext ^-> !| AutoComplete.Completion
            ]
        }
        |> Import "JinjaCompletionConfig" "@codemirror/lang-jinja"

    let AngularConfig =
        Pattern.Config "AngularConfig" {
            Required = []
            Optional = [
                "base", LanguageSupport.Type
            ]
        }

    let VueConfig =
        Pattern.Config "VueConfig" {
            Required = []
            Optional = [
                "base", LanguageSupport.Type
            ]
        }   

    let ThemeColor =
        Pattern.Config "ThemeColor" {
            Required = []
            Optional = [
                "chalky", T<string>
                "coral", T<string>
                "cyan", T<string>
                "invalid", T<string>
                "ivory", T<string>
                "stone", T<string>
                "malibu", T<string>
                "sage", T<string>
                "whiskey", T<string>
                "violet", T<string>
                "darkBackground", T<string>
                "highlightBackground", T<string>
                "background", T<string>
                "tooltipBackground", T<string>
                "selection", T<string>
                "cursor", T<string>
            ]
        }

    let CodeMirror =
        Class "CodeMirror"
        |+> Static [
            "basicSetup" =? Extension
            |> Import "basicSetup" "codemirror"

            "minimalSetup" =? Extension
            |> Import "minimalSetup" "codemirror"

            //
            // @codemirror/state
            //
            "findClusterBreak" => T<string> * T<int> * !? T<bool> * !? T<bool> ^-> T<int>
            |> Import "findClusterBreak" "@codemirror/state"

            "codePointAt" => T<string> * T<int> ^-> T<int>
            |> Import "codePointAt" "@codemirror/state"

            "fromCodePoint" => T<int> ^-> T<string>
            |> Import "fromCodePoint" "@codemirror/state"

            "codePointSize" => T<int> ^-> T<int>
            |> Import "codePointSize" "@codemirror/state"

            "combineConfig" => !|T<obj> * T<obj> * !? T<obj> ^-> T<obj>
            |> Import "combineConfig" "@codemirror/state"

            "countColumn" => T<string> * T<int> * !? T<int> ^-> T<int>
            |> Import "countColumn" "@codemirror/state"

            "findColumn" => T<string> * T<int> * T<int> * !? T<bool> ^-> T<int>
            |> Import "findColumn" "@codemirror/state"

            //
            // @codemirror/view
            //
            "keymap" =? Facet.[!| KeyBinding, !| !| KeyBinding]
            |> Import "keymap" "@codemirror/view"

            "showTooltip" =? Facet.[Tooltip, !| (Tooltip)]
            |> Import "showTooltip" "@codemirror/view"

            "closeHoverTooltips" =? StateEffect.[T<unit>]
            |> Import "closeHoverTooltips" "@codemirror/view"

            "showPanel" =? Facet.[PanelConstructor, !| (PanelConstructor)]
            |> Import "showPanel" "@codemirror/view"

            "gutterLineClass" =? Facet.[RangeSet.[GutterMarker], !| RangeSet.[GutterMarker]]
            |> Import "gutterLineClass" "@codemirror/view"

            "gutterWidgetClass" =? Facet.[(EditorView * WidgetType * BlockInfo ^-> GutterMarker), !| (EditorView * WidgetType * BlockInfo ^-> GutterMarker)]
            |> Import "gutterWidgetClass" "@codemirror/view"

            "lineNumberMarkers" =? Facet.[RangeSet.[GutterMarker], !| RangeSet.[GutterMarker]]
            |> Import "lineNumberMarkers" "@codemirror/view"

            "lineNumberWidgetMarker" =? Facet.[(EditorView * WidgetType * BlockInfo ^-> GutterMarker), !| (EditorView * WidgetType * BlockInfo ^-> GutterMarker)]
            |> Import "lineNumberWidgetMarker" "@codemirror/view"

            "rectangularSelection" => RectangularSelectionOptions ^-> Extension
            |> Import "rectangularSelection" "@codemirror/view"

            "crosshairCursor" => CrosshairCursorOptions ^-> Extension
            |> Import "crosshairCursor" "@codemirror/view"

            "drawSelection" => !? SelectionConfig ^-> Extension
            |> Import "drawSelection" "@codemirror/view"

            "dropCursor" => T<unit> ^-> Extension
            |> Import "dropCursor" "@codemirror/view"

            "logException" => EditorState * T<obj> * !? T<string> ^-> T<unit>
            |> Import "logException" "@codemirror/view"

            "runScopeHandlers" => EditorView * T<KeyboardEvent> * T<string> ^-> T<bool>
            |> Import "runScopeHandlers" "@codemirror/view"

            "getDrawSelectionConfig" => EditorState ^-> SelectionConfig
            |> Import "getDrawSelectionConfig" "@codemirror/view"

            "highlightSpecialChars" => !? SpecialCharConfig ^-> Extension
            |> Import "highlightSpecialChars" "@codemirror/view"

            "scrollPastEnd" => T<unit> ^-> Extension
            |> Import "scrollPastEnd" "@codemirror/view"

            "highlightActiveLine" => T<unit> ^-> Extension
            |> Import "highlightActiveLine" "@codemirror/view"

            "placeholder" => T<string> + T<HTMLElement> + (EditorView ^-> T<HTMLElement>) ^-> Extension
            |> Import "placeholder" "@codemirror/view"

            "layer" => LayerConfig ^-> Extension
            |> Import "layer" "@codemirror/view"

            "tooltips" => !? View.TooltipsOptions ^-> Extension
            |> Import "tooltips" "@codemirror/view"

            "hoverTooltip" => HoverTooltipSource * !? HoverTooltipOptions ^-> (Extension + StateField.[!| Tooltip])
            |> Import "hoverTooltip" "@codemirror/view"

            "getTooltip" => EditorView * Tooltip ^-> TooltipView
            |> Import "getTooltip" "@codemirror/view"

            "hasHoverTooltips" => EditorState ^-> T<bool>
            |> Import "hasHoverTooltips" "@codemirror/view"

            "repositionTooltips" => EditorView ^-> T<unit>
            |> Import "repositionTooltips" "@codemirror/view"

            "panels" => !? PanelConfig ^-> Extension
            |> Import "panels" "@codemirror/view"

            "getPanel" => EditorView * PanelConstructor ^-> Panel
            |> Import "getPanel" "@codemirror/view"

            "showDialog" => EditorView * DialogConfig ^-> ShowDialogResult
            |> Import "showDialog" "@codemirror/view"

            "getDialog" => EditorView * T<string> ^-> Panel
            |> Import "getDialog" "@codemirror/view"

            "gutter" => GutterConfig ^-> Extension
            |> Import "gutter" "@codemirror/view"

            "gutters" => !? GuttersConfig ^-> Extension
            |> Import "gutters" "@codemirror/view"

            "lineNumbers" => !? LineNumberConfig ^-> Extension
            |> Import "lineNumbers" "@codemirror/view"

            "highlightActiveLineGutter" => T<unit> ^-> Extension
            |> Import "highlightActiveLineGutter" "@codemirror/view"

            "highlightWhitespace" => T<unit> ^-> Extension
            |> Import "highlightWhitespace" "@codemirror/view"

            "highlightTrailingWhitespace" => T<unit> ^-> Extension
            |> Import "highlightTrailingWhitespace" "@codemirror/view"

            //
            // @codemirror/language-data
            //
            "languages" =? !|LanguageDescription
            |> Import "languages" "@codemirror/language-data"

            //
            // @codemirror/language
            //
            "languageDataProp" =? T<obj>
            |> Import "languageDataProp" "@codemirror/language"

            "sublanguageProp" =? T<obj>
            |> Import "sublanguageProp" "@codemirror/language"

            "language" =? Facet.[Language, Language + T<unit>]
            |> Import "language" "@codemirror/language"

            "indentService" =? Facet.[(IndentContext * T<int> ^-> T<int> + T<unit>), !| (IndentContext * T<int> ^-> T<int> + T<unit>)]
            |> Import "indentService" "@codemirror/language"

            "indentUnit" =? Facet.[T<string>, T<string>]
            |> Import "indentUnit" "@codemirror/language"

            "indentNodeProp" =? T<obj>
            |> Import "indentNodeProp" "@codemirror/language"

            "flatIndent" => TreeIndentContext ^-> T<int>
            |> Import "flatIndent" "@codemirror/language"

            "foldService" =? Facet.[(EditorState * T<int> * T<int> ^-> DocRange + T<unit>), !| (EditorState * T<int> * T<int> ^-> DocRange + T<unit>)]
            |> Import "foldService" "@codemirror/language"

            "foldNodeProp" =? T<obj>
            |> Import "foldNodeProp" "@codemirror/language"

            "foldEffect" =? StateEffectType.[DocRange]
            |> Import "foldEffect" "@codemirror/language"

            "unfoldEffect" =? StateEffectType.[DocRange]
            |> Import "unfoldEffect" "@codemirror/language"

            "foldState" =? StateField.[DecorationSet]
            |> Import "foldState" "@codemirror/language"

            "foldCode" =? Command
            |> Import "foldCode" "@codemirror/language"

            "unfoldCode" =? Command
            |> Import "unfoldCode" "@codemirror/language"

            "foldAll" =? Command
            |> Import "foldAll" "@codemirror/language"

            "unfoldAll" =? Command
            |> Import "unfoldAll" "@codemirror/language"

            "toggleFold" =? Command
            |> Import "toggleFold" "@codemirror/language"

            "foldKeymap" =? !| KeyBinding
            |> Import "foldKeymap" "@codemirror/language"

            "defaultHighlightStyle" =? HighlightStyle
            |> Import "defaultHighlightStyle" "@codemirror/language"

            "bracketMatchingHandle" =? T<obj>
            |> Import "bracketMatchingHandle" "@codemirror/language"

            "defineLanguageFacet" => !? T<obj> ^-> Facet.[T<obj>, !| T<obj>]
            |> Import "defineLanguageFacet" "@codemirror/language"

            "syntaxTree" => EditorState ^-> T<obj>
            |> Import "syntaxTree" "@codemirror/language"

            "ensureSyntaxTree" => EditorState * T<int> * !? T<int> ^-> T<obj>
            |> Import "ensureSyntaxTree" "@codemirror/language"

            "syntaxTreeAvailable" => EditorState * !? T<int> ^-> T<bool>
            |> Import "syntaxTreeAvailable" "@codemirror/language"

            "forceParsing" => EditorView * !? T<int> * !? T<int> ^-> T<bool>
            |> Import "forceParsing" "@codemirror/language"

            "syntaxParserRunning" => EditorView ^-> T<bool>
            |> Import "syntaxParserRunning" "@codemirror/language"

            "getIndentUnit" => EditorState ^-> T<int>
            |> Import "getIndentUnit" "@codemirror/language"

            "indentString" => EditorState * T<int> ^-> T<string>
            |> Import "indentString" "@codemirror/language"

            "getIndentation" => (IndentContext + EditorState) * T<int> ^-> T<int> + T<unit>
            |> Import "getIndentation" "@codemirror/language"

            "indentRange" => EditorState * T<int> * T<int> ^-> ChangeSet
            |> Import "indentRange" "@codemirror/language"

            "delimitedIndent" => DelimitedIndentOptions ^-> (TreeIndentContext ^-> T<int>)
            |> Import "delimitedIndent" "@codemirror/language"

            "continuedIndent" => !? ContinuedIndentOptions ^-> (TreeIndentContext ^-> T<int>)
            |> Import "continuedIndent" "@codemirror/language"

            "indentOnInput" => T<unit> ^-> Extension
            |> Import "indentOnInput" "@codemirror/language"

            "foldInside" => T<obj> ^-> DocRange + T<unit>
            |> Import "foldInside" "@codemirror/language"

            "foldable" => EditorState * T<int> * T<int> ^-> DocRange + T<unit>
            |> Import "foldable" "@codemirror/language"

            "foldedRanges" => EditorState ^-> DecorationSet
            |> Import "foldedRanges" "@codemirror/language"

            "codeFolding" => !? FoldConfig ^-> Extension
            |> Import "codeFolding" "@codemirror/language"

            "foldGutter" => !? FoldGutterConfig ^-> Extension
            |> Import "foldGutter" "@codemirror/language"

            "syntaxHighlighting" => T<obj> * !? SyntaxHighlightingOptions ^-> Extension
            |> Import "syntaxHighlighting" "@codemirror/language"

            "highlightingFor" => EditorState * !| T<obj> * !? T<obj> ^-> T<string>
            |> Import "highlightingFor" "@codemirror/language"

            "bracketMatching" => !? Config ^-> Extension
            |> Import "bracketMatching" "@codemirror/language"

            "matchBrackets" => EditorState * T<int> * T<int> * !? Config ^-> MatchResult + T<unit>
            |> Import "matchBrackets" "@codemirror/language"

            "bidiIsolates" => !? BidiIsolatesOptions ^-> Extension
            |> Import "bidiIsolates" "@codemirror/language"

            //
            // @codemirror/autocomplete
            //
            "autocompletion" => !? AutoComplete.CompletionConfig ^-> Extension
            |> Import "autocompletion" "@codemirror/autocomplete"

            "snippet" => T<string> ^-> (AutoComplete.SnippetContext * AutoComplete.Completion * T<int> * T<int> ^-> T<unit>)
            |> Import "snippet" "@codemirror/autocomplete"

            "clearSnippet" =? StateCommand
            |> Import "clearSnippet" "@codemirror/autocomplete"

            "nextSnippetField" =? StateCommand
            |> Import "nextSnippetField" "@codemirror/autocomplete"

            "prevSnippetField" =? StateCommand
            |> Import "prevSnippetField" "@codemirror/autocomplete"

            "hasNextSnippetField" => EditorState ^-> T<bool>
            |> Import "hasNextSnippetField" "@codemirror/autocomplete"

            "hasPrevSnippetField" => EditorState ^-> T<bool>
            |> Import "hasPrevSnippetField" "@codemirror/autocomplete"

            "snippetKeymap" =? Facet.[!| KeyBinding, !| KeyBinding]
            |> Import "snippetKeymap" "@codemirror/autocomplete"

            "snippetCompletion" => T<string> * AutoComplete.Completion ^-> AutoComplete.Completion
            |> Import "snippetCompletion" "@codemirror/autocomplete"

            "moveCompletionSelection" => T<bool> * !? T<string> ^-> Command
            |> Import "moveCompletionSelection" "@codemirror/autocomplete"

            "acceptCompletion" =? Command
            |> Import "acceptCompletion" "@codemirror/autocomplete"

            "startCompletion" =? Command
            |> Import "startCompletion" "@codemirror/autocomplete"

            "closeCompletion" =? Command
            |> Import "closeCompletion" "@codemirror/autocomplete"

            "completeAnyWord" =? AutoComplete.CompletionSource
            |> Import "completeAnyWord" "@codemirror/autocomplete"

            "closeBrackets" => T<unit> ^-> Extension
            |> Import "closeBrackets" "@codemirror/autocomplete"

            "deleteBracketPair" =? StateCommand
            |> Import "deleteBracketPair" "@codemirror/autocomplete"

            "closeBracketsKeymap" =? !| KeyBinding
            |> Import "closeBracketsKeymap" "@codemirror/autocomplete"

            "insertBracket" => EditorState * T<string> ^-> Transaction
            |> Import "insertBracket" "@codemirror/autocomplete"

            "completionKeymap" =? !| KeyBinding
            |> Import "completionKeymap" "@codemirror/autocomplete"

            "completionStatus" => EditorState ^-> (T<string>)
            |> Import "completionStatus" "@codemirror/autocomplete"

            "currentCompletions" => EditorState ^-> !| AutoComplete.Completion
            |> Import "currentCompletions" "@codemirror/autocomplete"

            "selectedCompletion" => EditorState ^-> AutoComplete.Completion
            |> Import "selectedCompletion" "@codemirror/autocomplete"

            "selectedCompletionIndex" => EditorState ^-> T<int>
            |> Import "selectedCompletionIndex" "@codemirror/autocomplete"

            "setSelectedCompletion" => T<int> ^-> StateEffect.[T<obj>]
            |> Import "setSelectedCompletion" "@codemirror/autocomplete"

            "insertCompletionText" => EditorState * T<string> * T<int> * T<int> ^-> TransactionSpec
            |> Import "insertCompletionText" "@codemirror/autocomplete"

            "completeFromList" => (!| (T<string> + AutoComplete.Completion)) ^-> AutoComplete.CompletionSource
            |> Import "completeFromList" "@codemirror/autocomplete"

            "ifIn" => !| T<string> * AutoComplete.CompletionSource ^-> AutoComplete.CompletionSource
            |> Import "ifIn" "@codemirror/autocomplete"

            "ifNotIn" => !| T<string> * AutoComplete.CompletionSource ^-> AutoComplete.CompletionSource
            |> Import "ifNotIn" "@codemirror/autocomplete"

            "pickedCompletion" =? AnnotationType.[AutoComplete.Completion]
            |> Import "pickedCompletion" "@codemirror/autocomplete"

            //
            // @codemirror/collab
            //
            "collab" => !?Collab.CollabConfig?config ^-> Extension
            |> Import "collab" "@codemirror/collab"

            "receiveUpdates" => EditorState * !| Collab.Update ^-> Transaction
            |> Import "receiveUpdates" "@codemirror/collab"

            "sendableUpdates" => EditorState ^-> !| Collab.Update
            |> Import "sendableUpdates" "@codemirror/collab"

            "getSyncedVersion" => EditorState ^-> T<int>
            |> Import "getSyncedVersion" "@codemirror/collab"

            "getClientID" => EditorState ^-> T<string>
            |> Import "getClientID" "@codemirror/collab"

            "rebaseUpdates" => !| Collab.Update * !| Collab.UpdateOver ^-> !| Collab.Update
            |> Import "rebaseUpdates" "@codemirror/collab"

            //
            // @codemirror/lint
            //
            "setDiagnosticsEffect" =? StateEffectType.[!| Lint.Diagnostic]
            |> Import "setDiagnosticsEffect" "@codemirror/lint"

            "openLintPanel" =? Command
            |> Import "openLintPanel" "@codemirror/lint"

            "closeLintPanel" =? Command
            |> Import "closeLintPanel" "@codemirror/lint"

            "nextDiagnostic" =? Command
            |> Import "nextDiagnostic" "@codemirror/lint"

            "previousDiagnostic" =? Command
            |> Import "previousDiagnostic" "@codemirror/lint"

            "lintKeymap" =? !| KeyBinding
            |> Import "lintKeymap" "@codemirror/lint"

            "setDiagnostics" => EditorState * !| Lint.Diagnostic ^-> TransactionSpec
            |> Import "setDiagnostics" "@codemirror/lint"

            "diagnosticCount" => EditorState ^-> T<int>
            |> Import "diagnosticCount" "@codemirror/lint"

            "linter" => (Lint.LintSource + T<unit>) * !? Lint.LintConfig ^-> Extension
            |> Import "linter" "@codemirror/lint"

            "forceLinting" => EditorView ^-> T<unit>
            |> Import "forceLinting" "@codemirror/lint"

            "lintGutter" => !? Lint.LintGutterConfig ^-> Extension
            |> Import "lintGutter" "@codemirror/lint"

            "forEachDiagnostic" => EditorState * (Lint.Diagnostic * T<int> * T<int> ^-> T<unit>) ^-> T<unit>
            |> Import "forEachDiagnostic" "@codemirror/lint"

            //
            // @codemirror/search
            //
            "gotoLine" =? Command
            |> Import "gotoLine" "@codemirror/search"

            "selectNextOccurrence" =? StateCommand
            |> Import "selectNextOccurrence" "@codemirror/search"

            "setSearchQuery" =? StateEffectType.[Search.SearchQuery]
            |> Import "setSearchQuery" "@codemirror/search"

            "findNext" =? Command
            |> Import "findNext" "@codemirror/search"

            "findPrevious" =? Command
            |> Import "findPrevious" "@codemirror/search"

            "selectMatches" =? Command
            |> Import "selectMatches" "@codemirror/search"

            "selectSelectionMatches" =? StateCommand
            |> Import "selectSelectionMatches" "@codemirror/search"

            "replaceNext" =? Command
            |> Import "replaceNext" "@codemirror/search"

            "replaceAll" =? Command
            |> Import "replaceAll" "@codemirror/search"

            "openSearchPanel" =? Command
            |> Import "openSearchPanel" "@codemirror/search"

            "closeSearchPanel" =? Command
            |> Import "closeSearchPanel" "@codemirror/search"

            "searchKeymap" =? !| KeyBinding
            |> Import "searchKeymap" "@codemirror/search"

            "highlightSelectionMatches" => !? Search.HighlightOptions ^-> Extension
            |> Import "highlightSelectionMatches" "@codemirror/search"

            "search" => !? Search.SearchConfig ^-> Extension
            |> Import "search" "@codemirror/search"

            "getSearchQuery" => EditorState ^-> Search.SearchQuery
            |> Import "getSearchQuery" "@codemirror/search"

            "searchPanelOpen" => EditorState ^-> T<bool>
            |> Import "searchPanelOpen" "@codemirror/search"

            //
            // @codemirror/commands
            //
            "toggleComment" =? StateCommand
            |> Import "toggleComment" "@codemirror/commands"

            "toggleLineComment" =? StateCommand
            |> Import "toggleLineComment" "@codemirror/commands"

            "lineComment" =? StateCommand
            |> Import "lineComment" "@codemirror/commands"

            "lineUncomment" =? StateCommand
            |> Import "lineUncomment" "@codemirror/commands"

            "toggleBlockComment" =? StateCommand
            |> Import "toggleBlockComment" "@codemirror/commands"

            "blockComment" =? StateCommand
            |> Import "blockComment" "@codemirror/commands"

            "blockUncomment" =? StateCommand
            |> Import "blockUncomment" "@codemirror/commands"

            "toggleBlockCommentByLine" =? StateCommand
            |> Import "toggleBlockCommentByLine" "@codemirror/commands"

            "isolateHistory" =? AnnotationType.[T<string>]
            |> Import "isolateHistory" "@codemirror/commands"

            "invertedEffects" =? Facet.[(Transaction ^-> !| StateEffect.[T<obj>]), !| (Transaction ^-> !| StateEffect.[T<obj>])]
            |> Import "invertedEffects" "@codemirror/commands"

            "history" => !? CodeMirrorCommand.HistoryConfig ^-> Extension
            |> Import "history" "@codemirror/commands"

            "historyField" =? StateField.[T<obj>]
            |> Import "historyField" "@codemirror/commands"

            "undo" =? StateCommand
            |> Import "undo" "@codemirror/commands"

            "redo" =? StateCommand
            |> Import "redo" "@codemirror/commands"

            "undoSelection" =? StateCommand
            |> Import "undoSelection" "@codemirror/commands"

            "redoSelection" =? StateCommand
            |> Import "redoSelection" "@codemirror/commands"

            "undoDepth" => EditorState ^-> T<int>
            |> Import "undoDepth" "@codemirror/commands"

            "redoDepth" => EditorState ^-> T<int>
            |> Import "redoDepth" "@codemirror/commands"

            "historyKeymap" =? !| KeyBinding
            |> Import "historyKeymap" "@codemirror/commands"

            "cursorCharLeft" =? Command
            |> Import "cursorCharLeft" "@codemirror/commands"

            "cursorCharRight" =? Command
            |> Import "cursorCharRight" "@codemirror/commands"

            "cursorCharForward" =? Command
            |> Import "cursorCharForward" "@codemirror/commands"

            "cursorCharBackward" =? Command
            |> Import "cursorCharBackward" "@codemirror/commands"

            "cursorCharForwardLogical" =? StateCommand
            |> Import "cursorCharForwardLogical" "@codemirror/commands"

            "cursorCharBackwardLogical" =? StateCommand
            |> Import "cursorCharBackwardLogical" "@codemirror/commands"

            "cursorGroupLeft" =? Command
            |> Import "cursorGroupLeft" "@codemirror/commands"

            "cursorGroupRight" =? Command
            |> Import "cursorGroupRight" "@codemirror/commands"

            "cursorGroupForward" =? Command
            |> Import "cursorGroupForward" "@codemirror/commands"

            "cursorGroupBackward" =? Command
            |> Import "cursorGroupBackward" "@codemirror/commands"

            "cursorGroupForwardWin" =? Command
            |> Import "cursorGroupForwardWin" "@codemirror/commands"

            "cursorSubwordForward" =? Command
            |> Import "cursorSubwordForward" "@codemirror/commands"

            "cursorSubwordBackward" =? Command
            |> Import "cursorSubwordBackward" "@codemirror/commands"

            "cursorSyntaxLeft" =? Command
            |> Import "cursorSyntaxLeft" "@codemirror/commands"

            "cursorSyntaxRight" =? Command
            |> Import "cursorSyntaxRight" "@codemirror/commands"

            "cursorLineUp" =? Command
            |> Import "cursorLineUp" "@codemirror/commands"

            "cursorLineDown" =? Command
            |> Import "cursorLineDown" "@codemirror/commands"

            "cursorPageUp" =? Command
            |> Import "cursorPageUp" "@codemirror/commands"

            "cursorPageDown" =? Command
            |> Import "cursorPageDown" "@codemirror/commands"

            "cursorLineBoundaryForward" =? Command
            |> Import "cursorLineBoundaryForward" "@codemirror/commands"

            "cursorLineBoundaryBackward" =? Command
            |> Import "cursorLineBoundaryBackward" "@codemirror/commands"

            "cursorLineBoundaryLeft" =? Command
            |> Import "cursorLineBoundaryLeft" "@codemirror/commands"

            "cursorLineBoundaryRight" =? Command
            |> Import "cursorLineBoundaryRight" "@codemirror/commands"

            "cursorLineStart" =? Command
            |> Import "cursorLineStart" "@codemirror/commands"

            "cursorLineEnd" =? Command
            |> Import "cursorLineEnd" "@codemirror/commands"

            "cursorMatchingBracket" =? StateCommand
            |> Import "cursorMatchingBracket" "@codemirror/commands"

            "selectMatchingBracket" =? StateCommand
            |> Import "selectMatchingBracket" "@codemirror/commands"

            "selectCharLeft" =? Command
            |> Import "selectCharLeft" "@codemirror/commands"

            "selectCharRight" =? Command
            |> Import "selectCharRight" "@codemirror/commands"

            "selectCharForward" =? Command
            |> Import "selectCharForward" "@codemirror/commands"

            "selectCharBackward" =? Command
            |> Import "selectCharBackward" "@codemirror/commands"

            "selectCharForwardLogical" =? StateCommand
            |> Import "selectCharForwardLogical" "@codemirror/commands"

            "selectCharBackwardLogical" =? StateCommand
            |> Import "selectCharBackwardLogical" "@codemirror/commands"

            "selectGroupLeft" =? Command
            |> Import "selectGroupLeft" "@codemirror/commands"

            "selectGroupRight" =? Command
            |> Import "selectGroupRight" "@codemirror/commands"

            "selectGroupForward" =? Command
            |> Import "selectGroupForward" "@codemirror/commands"

            "selectGroupBackward" =? Command
            |> Import "selectGroupBackward" "@codemirror/commands"

            "selectGroupForwardWin" =? Command
            |> Import "selectGroupForwardWin" "@codemirror/commands"

            "selectSubwordForward" =? Command
            |> Import "selectSubwordForward" "@codemirror/commands"

            "selectSubwordBackward" =? Command
            |> Import "selectSubwordBackward" "@codemirror/commands"

            "selectSyntaxLeft" =? Command
            |> Import "selectSyntaxLeft" "@codemirror/commands"

            "selectSyntaxRight" =? Command
            |> Import "selectSyntaxRight" "@codemirror/commands"

            "selectLineUp" =? Command
            |> Import "selectLineUp" "@codemirror/commands"

            "selectLineDown" =? Command
            |> Import "selectLineDown" "@codemirror/commands"

            "selectPageUp" =? Command
            |> Import "selectPageUp" "@codemirror/commands"

            "selectPageDown" =? Command
            |> Import "selectPageDown" "@codemirror/commands"

            "selectLineBoundaryForward" =? Command
            |> Import "selectLineBoundaryForward" "@codemirror/commands"

            "selectLineBoundaryBackward" =? Command
            |> Import "selectLineBoundaryBackward" "@codemirror/commands"

            "selectLineBoundaryLeft" =? Command
            |> Import "selectLineBoundaryLeft" "@codemirror/commands"

            "selectLineBoundaryRight" =? Command
            |> Import "selectLineBoundaryRight" "@codemirror/commands"

            "selectLineStart" =? Command
            |> Import "selectLineStart" "@codemirror/commands"

            "selectLineEnd" =? Command
            |> Import "selectLineEnd" "@codemirror/commands"

            "cursorDocStart" =? StateCommand
            |> Import "cursorDocStart" "@codemirror/commands"

            "cursorDocEnd" =? StateCommand
            |> Import "cursorDocEnd" "@codemirror/commands"

            "selectDocStart" =? StateCommand
            |> Import "selectDocStart" "@codemirror/commands"

            "selectDocEnd" =? StateCommand
            |> Import "selectDocEnd" "@codemirror/commands"

            "selectAll" =? StateCommand
            |> Import "selectAll" "@codemirror/commands"

            "selectLine" =? StateCommand
            |> Import "selectLine" "@codemirror/commands"

            "selectParentSyntax" =? StateCommand
            |> Import "selectParentSyntax" "@codemirror/commands"

            "simplifySelection" =? StateCommand
            |> Import "simplifySelection" "@codemirror/commands"

            "deleteCharBackward" =? Command
            |> Import "deleteCharBackward" "@codemirror/commands"

            "deleteCharBackwardStrict" =? Command
            |> Import "deleteCharBackwardStrict" "@codemirror/commands"

            "deleteCharForward" =? Command
            |> Import "deleteCharForward" "@codemirror/commands"

            "deleteGroupBackward" =? StateCommand
            |> Import "deleteGroupBackward" "@codemirror/commands"

            "deleteGroupForward" =? StateCommand
            |> Import "deleteGroupForward" "@codemirror/commands"

            "deleteToLineEnd" =? Command
            |> Import "deleteToLineEnd" "@codemirror/commands"

            "deleteToLineStart" =? Command
            |> Import "deleteToLineStart" "@codemirror/commands"

            "deleteLineBoundaryBackward" =? Command
            |> Import "deleteLineBoundaryBackward" "@codemirror/commands"

            "deleteLineBoundaryForward" =? Command
            |> Import "deleteLineBoundaryForward" "@codemirror/commands"

            "deleteTrailingWhitespace" =? StateCommand
            |> Import "deleteTrailingWhitespace" "@codemirror/commands"

            "splitLine" =? StateCommand
            |> Import "splitLine" "@codemirror/commands"

            "transposeChars" =? StateCommand
            |> Import "transposeChars" "@codemirror/commands"

            "moveLineUp" =? StateCommand
            |> Import "moveLineUp" "@codemirror/commands"

            "moveLineDown" =? StateCommand
            |> Import "moveLineDown" "@codemirror/commands"

            "copyLineUp" =? StateCommand
            |> Import "copyLineUp" "@codemirror/commands"

            "copyLineDown" =? StateCommand
            |> Import "copyLineDown" "@codemirror/commands"

            "deleteLine" =? Command
            |> Import "deleteLine" "@codemirror/commands"

            "insertNewline" =? StateCommand
            |> Import "insertNewline" "@codemirror/commands"

            "insertNewlineKeepIndent" =? StateCommand
            |> Import "insertNewlineKeepIndent" "@codemirror/commands"

            "insertNewlineAndIndent" =? StateCommand
            |> Import "insertNewlineAndIndent" "@codemirror/commands"

            "insertBlankLine" =? StateCommand
            |> Import "insertBlankLine" "@codemirror/commands"

            "indentSelection" =? StateCommand
            |> Import "indentSelection" "@codemirror/commands"

            "indentMore" =? StateCommand
            |> Import "indentMore" "@codemirror/commands"

            "indentLess" =? StateCommand
            |> Import "indentLess" "@codemirror/commands"

            "toggleTabFocusMode" =? Command
            |> Import "toggleTabFocusMode" "@codemirror/commands"

            "temporarilySetTabFocusMode" =? Command
            |> Import "temporarilySetTabFocusMode" "@codemirror/commands"

            "insertTab" =? StateCommand
            |> Import "insertTab" "@codemirror/commands"

            "emacsStyleKeymap" =? !| KeyBinding
            |> Import "emacsStyleKeymap" "@codemirror/commands"

            "standardKeymap" =? !| KeyBinding
            |> Import "standardKeymap" "@codemirror/commands"

            "defaultKeymap" =? !| KeyBinding
            |> Import "defaultKeymap" "@codemirror/commands"

            "indentWithTab" =? KeyBinding
            |> Import "indentWithTab" "@codemirror/commands"

            //
            // @codemirror/lang-javascript
            //
            "javascriptLanguage" =? LRLanguage
            |> Import "javascriptLanguage" "@codemirror/lang-javascript"

            "typescriptLanguage" =? LRLanguage
            |> Import "typescriptLanguage" "@codemirror/lang-javascript"

            "jsxLanguage" =? LRLanguage
            |> Import "jsxLanguage" "@codemirror/lang-javascript"

            "tsxLanguage" =? LRLanguage
            |> Import "tsxLanguage" "@codemirror/lang-javascript"

            "javascript" => !?JavaScriptConfig ^-> LanguageSupport
            |> Import "javascript" "@codemirror/lang-javascript"

            "autoCloseTagsJS" =? Extension
            |> Import "autoCloseTags" "@codemirror/lang-javascript"

            "snippets" =? !| AutoComplete.Completion
            |> Import "snippets" "@codemirror/lang-javascript"

            "typescriptSnippets" =? !| AutoComplete.Completion
            |> Import "typescriptSnippets" "@codemirror/lang-javascript"

            "esLint" => T<obj> * !? T<obj> ^-> (EditorView ^-> !| Lint.Diagnostic)
            |> Import "esLint" "@codemirror/lang-javascript"

            "localCompletionSourceJS" => AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult
            |> Import "localCompletionSource" "@codemirror/lang-javascript"

            "completionPath" => AutoComplete.CompletionContext ^-> CompletionPathResult
            |> Import "completionPath" "@codemirror/lang-javascript"

            "scopeCompletionSource" => T<obj> ^-> AutoComplete.CompletionSource
            |> Import "scopeCompletionSource" "@codemirror/lang-javascript"

            //
            // @codemirror/lang-sql
            //
            "keywordCompletionSource" => LangSQL.SQLDialect * !? T<bool> * !? (T<string> * T<string> ^-> AutoComplete.Completion) ^-> AutoComplete.CompletionSource
            |> Import "keywordCompletionSource" "@codemirror/lang-sql"

            "schemaCompletionSource" => LangSQL.SQLConfig ^-> AutoComplete.CompletionSource
            |> Import "schemaCompletionSource" "@codemirror/lang-sql"

            "sql" => !? LangSQL.SQLConfig ^-> LanguageSupport
            |> Import "sql" "@codemirror/lang-sql"

            "StandardSQL" =? LangSQL.SQLDialect
            |> Import "StandardSQL" "@codemirror/lang-sql"

            "PostgreSQL" =? LangSQL.SQLDialect
            |> Import "PostgreSQL" "@codemirror/lang-sql"

            "MySQL" =? LangSQL.SQLDialect
            |> Import "MySQL" "@codemirror/lang-sql"

            "MariaSQL" =? LangSQL.SQLDialect
            |> Import "MariaSQL" "@codemirror/lang-sql"

            "MSSQL" =? LangSQL.SQLDialect
            |> Import "MSSQL" "@codemirror/lang-sql"

            "SQLite" =? LangSQL.SQLDialect
            |> Import "SQLite" "@codemirror/lang-sql"

            "Cassandra" =? LangSQL.SQLDialect
            |> Import "Cassandra" "@codemirror/lang-sql"

            "PLSQL" =? LangSQL.SQLDialect
            |> Import "PLSQL" "@codemirror/lang-sql"

            //
            // @codemirror/lang-python
            //
            "localCompletionSourcePython" => AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult
            |> Import "localCompletionSource" "@codemirror/lang-python"

            "globalCompletion" =? AutoComplete.CompletionSource
            |> Import "globalCompletion" "@codemirror/lang-python"

            "pythonLanguage" =? LRLanguage
            |> Import "pythonLanguage" "@codemirror/lang-python"

            "python" => T<unit> ^-> LanguageSupport
            |> Import "python" "@codemirror/lang-python"

            //
            // @codemirror/lang-java
            //
            "javaLanguage" =? LRLanguage
            |> Import "javaLanguage" "@codemirror/lang-java"

            "java" => T<unit> ^-> LanguageSupport
            |> Import "java" "@codemirror/lang-java"

            //
            // @codemirror/lang-html
            //
            "autoCloseTagsHtml" =? Extension
            |> Import "autoCloseTags" "@codemirror/lang-html"

            "html" => !?LangHtml.HtmlConfig?config ^-> LanguageSupport
            |> Import "html" "@codemirror/lang-html"

            "htmlPlain" =? LRLanguage
            |> Import "htmlPlain" "@codemirror/lang-html"

            "htmlLanguage" =? LRLanguage
            |> Import "htmlLanguage" "@codemirror/lang-html"

            "htmlCompletionSource" => AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult
            |> Import "htmlCompletionSource" "@codemirror/lang-html"

            "htmlCompletionSourceWith" => LangHtml.HtmlCompletionSourceConfig?config ^-> (AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult)
            |> Import "htmlCompletionSourceWith" "@codemirror/lang-html"

            //
            // @codemirror/lang-css
            //
            "defineCSSCompletionSource" => (T<obj> ^-> T<bool>) ^-> AutoComplete.CompletionSource
            |> Import "defineCSSCompletionSource" "@codemirror/lang-css"

            "cssCompletionSource" =? AutoComplete.CompletionSource
            |> Import "cssCompletionSource" "@codemirror/lang-css"

            "cssLanguage" =? LRLanguage
            |> Import "cssLanguage" "@codemirror/lang-css"

            "css" => T<unit> ^-> LanguageSupport
            |> Import "css" "@codemirror/lang-css"

            //
            // @codemirror/lang-json
            //
            "jsonParseLinter" => T<unit> ^-> (EditorView ^-> !| Lint.Diagnostic)
            |> Import "jsonParseLinter" "@codemirror/lang-json"

            "jsonLanguage" =? LRLanguage
            |> Import "jsonLanguage" "@codemirror/lang-json"

            "json" => T<unit> ^-> LanguageSupport
            |> Import "json" "@codemirror/lang-json"

            //
            // @codemirror/lang-markdown
            //
            "commonmarkLanguage" =? Language
            |> Import "commonmarkLanguage" "@codemirror/lang-markdown"

            "markdownLanguage" =? Language
            |> Import "markdownLanguage" "@codemirror/lang-markdown"

            "insertNewlineContinueMarkup" =? StateCommand
            |> Import "insertNewlineContinueMarkup" "@codemirror/lang-markdown"

            "deleteMarkupBackward" =? StateCommand
            |> Import "deleteMarkupBackward" "@codemirror/lang-markdown"

            "markdownKeymap" =? !| KeyBinding
            |> Import "markdownKeymap" "@codemirror/lang-markdown"

            "markdown" => !?MarkdownConfig?config ^-> LanguageSupport
            |> Import "markdown" "@codemirror/lang-markdown"

            //
            // @codemirror/lang-rust
            //
            "rustLanguage" =? LRLanguage
            |> Import "rustLanguage" "@codemirror/lang-rust"

            "rust" => T<unit> ^-> LanguageSupport
            |> Import "rust" "@codemirror/lang-rust"

            //
            // @codemirror/lang-xml
            //
            "completeFromSchema" => !| LangXml.ElementSpec * !| LangXml.AttrSpec ^-> AutoComplete.CompletionSource
            |> Import "completeFromSchema" "@codemirror/lang-xml"

            "xmlLanguage" =? LRLanguage
            |> Import "xmlLanguage" "@codemirror/lang-xml"

            "xml" => !?LangXml.XMLConfig?conf ^-> LanguageSupport
            |> Import "xml" "@codemirror/lang-xml"

            "autoCloseTagsXml" =? Extension
            |> Import "autoCloseTags" "@codemirror/lang-xml"

            //
            // @codemirror/lang-yaml
            //
            "yamlLanguage" =? LRLanguage
            |> Import "yamlLanguage" "@codemirror/lang-yaml"

            "yaml" => T<unit> ^-> LanguageSupport
            |> Import "yaml" "@codemirror/lang-yaml"

            "yamlFrontmatter" => YamlFrontmatterConfig?config ^-> LanguageSupport
            |> Import "yamlFrontmatter" "@codemirror/lang-yaml"

            //
            // @codemirror/lang-php
            //
            "phpLanguage" =? LRLanguage
            |> Import "phpLanguage" "@codemirror/lang-php"

            "php" => !?PhpConfig?config ^-> LanguageSupport
            |> Import "php" "@codemirror/lang-php"

            //
            // @codemirror/lang-go
            //
            "goLanguage" =? LRLanguage
            |> Import "goLanguage" "@codemirror/lang-go"

            "go" => T<unit> ^-> LanguageSupport
            |> Import "go" "@codemirror/lang-go"

            "snippetsGo" =? !| AutoComplete.Completion
            |> Import "snippets" "@codemirror/lang-go"

            "localCompletionSourceGo" => AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult
            |> Import "localCompletionSource" "@codemirror/lang-go"

            //
            // @codemirror/lang-sass
            //
            "sassLanguage" =? LRLanguage
            |> Import "sassLanguage" "@codemirror/lang-sass"

            "sassCompletionSource" => AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult
            |> Import "sassCompletionSource" "@codemirror/lang-sass"

            "sass" => !?SassConfig?config ^-> LanguageSupport
            |> Import "sass" "@codemirror/lang-sass"

            //
            // @codemirror/lang-liquid
            //
            "liquidCompletionSource" => !?LiquidCompletionConfig?config ^-> (AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult)
            |> Import "liquidCompletionSource" "@codemirror/lang-liquid"

            "closePercentBraceLiquid" =? Extension
            |> Import "closePercentBrace" "@codemirror/lang-liquid"

            "liquid" => !?LiquidCompletionConfig?config ^-> LanguageSupport
            |> Import "liquid" "@codemirror/lang-liquid"

            "liquidLanguage" =? LRLanguage
            |> Import "liquidLanguage" "@codemirror/lang-liquid"

            //
            // @codemirror/lang-lezer
            //
            "lezerLanguage" =? LRLanguage
            |> Import "lezerLanguage" "@codemirror/lang-lezer"

            "lezer" => T<unit> ^-> LanguageSupport
            |> Import "lezer" "@codemirror/lang-lezer"

            //
            // @codemirror/lang-cpp
            //
            "cppLanguage" =? LRLanguage
            |> Import "cppLanguage" "@codemirror/lang-cpp"

            "cpp" => T<unit> ^-> LanguageSupport
            |> Import "cpp" "@codemirror/lang-cpp"

            //
            // @codemirror/lang-jinja
            //
            "jinjaCompletionSource" => JinjaCompletionConfig?config ^-> (AutoComplete.CompletionContext ^-> AutoComplete.CompletionResult)
            |> Import "jinjaCompletionSource" "@codemirror/lang-jinja"

            "closePercentBraceJinja" =? Extension
            |> Import "closePercentBrace" "@codemirror/lang-jinja"

            "jinjaLanguage" =? LRLanguage
            |> Import "jinjaLanguage" "@codemirror/lang-jinja"

            "jinja" => !?JinjaCompletionConfig?config ^-> LanguageSupport
            |> Import "jinja" "@codemirror/lang-jinja"

            //
            // @codemirror/lang-angular
            //
            "angularLanguage" =? LRLanguage
            |> Import "angularLanguage" "@codemirror/lang-angular"

            "angular" => !?AngularConfig?config ^-> LanguageSupport
            |> Import "angular" "@codemirror/lang-angular"

            //
            // @codemirror/lang-vue
            //
            "vueLanguage" =? LRLanguage
            |> Import "vueLanguage" "@codemirror/lang-vue"

            "vue" => !?VueConfig?config ^-> LanguageSupport
            |> Import "vue" "@codemirror/lang-vue"

            //
            // @codemirror/lang-wast
            //
            "wastLanguage" =? LRLanguage
            |> Import "wastLanguage" "@codemirror/lang-wast"

            "wast" => T<unit> ^-> LanguageSupport
            |> Import "wast" "@codemirror/lang-wast"

            //
            // @codemirror/lang-less
            //
            "lessLanguage" =? LRLanguage
            |> Import "lessLanguage" "@codemirror/lang-less"

            "lessCompletionSource" =? AutoComplete.CompletionSource
            |> Import "lessCompletionSource" "@codemirror/lang-less"

            "less" => T<unit> ^-> LanguageSupport
            |> Import "less" "@codemirror/lang-less"

            //
            // @codemirror/theme-one-dark
            //
            "color" =? ThemeColor
            |> Import "color" "@codemirror/theme-one-dark"

            "oneDarkTheme" =? Extension
            |> Import "oneDarkTheme" "@codemirror/theme-one-dark"

            "oneDarkHighlightStyle" =? HighlightStyle
            |> Import "oneDarkHighlightStyle" "@codemirror/theme-one-dark"

            "oneDark" =? Extension
            |> Import "oneDark" "@codemirror/theme-one-dark"

            //
            // @codemirror/lsp-client
            //
            "serverCompletion" => LSPClient.ServerCompletionConfig?config ^-> Extension
            |> Import "serverCompletion" "@codemirror/lsp-client"

            "serverCompletionSource" =? AutoComplete.CompletionSource
            |> Import "serverCompletionSource" "@codemirror/lsp-client"

            "hoverTooltips" => HoverTooltipOptions?config ^-> Extension
            |> Import "hoverTooltips" "@codemirror/lsp-client"

            "formatDocument" =? Command
            |> Import "formatDocument" "@codemirror/lsp-client"

            "formatKeymap" =? !| KeyBinding
            |> Import "formatKeymap" "@codemirror/lsp-client"

            "renameSymbol" =? Command
            |> Import "renameSymbol" "@codemirror/lsp-client"

            "renameKeymap" =? !| KeyBinding
            |> Import "renameKeymap" "@codemirror/lsp-client"

            "showSignatureHelp" =? Command
            |> Import "showSignatureHelp" "@codemirror/lsp-client"

            "nextSignature" =? Command
            |> Import "nextSignature" "@codemirror/lsp-client"

            "prevSignature" =? Command
            |> Import "prevSignature" "@codemirror/lsp-client"

            "signatureKeymap" =? !| KeyBinding
            |> Import "signatureKeymap" "@codemirror/lsp-client"

            "signatureHelp" => LSPClient.SignatureHelpConfig?config ^-> Extension
            |> Import "signatureHelp" "@codemirror/lsp-client"

            "jumpToDefinition" =? Command
            |> Import "jumpToDefinition" "@codemirror/lsp-client"

            "jumpToDeclaration" =? Command
            |> Import "jumpToDeclaration" "@codemirror/lsp-client"

            "jumpToTypeDefinition" =? Command
            |> Import "jumpToTypeDefinition" "@codemirror/lsp-client"

            "jumpToImplementation" =? Command
            |> Import "jumpToImplementation" "@codemirror/lsp-client"

            "jumpToDefinitionKeymap" =? !| KeyBinding
            |> Import "jumpToDefinitionKeymap" "@codemirror/lsp-client"

            "findReferences" =? Command
            |> Import "findReferences" "@codemirror/lsp-client"

            "closeReferencePanel" =? Command
            |> Import "closeReferencePanel" "@codemirror/lsp-client"

            "findReferencesKeymap" =? !| KeyBinding
            |> Import "findReferencesKeymap" "@codemirror/lsp-client"

            "languageServerSupport" => LSPClient.LSPClientClass * T<string> * !? T<string> ^-> Extension
            |> Import "languageServerSupport" "@codemirror/lsp-client"

            //
            // @codemirror/merge
            //
            "diff" => T<string> * T<string> * !? Merge.DiffConfig ^-> !| Merge.Change
            |> Import "diff" "@codemirror/merge"

            "presentableDiff" => T<string> * T<string> * !? Merge.DiffConfig ^-> !| Merge.Change
            |> Import "presentableDiff" "@codemirror/merge"

            "getChunks" => EditorState ^-> Merge.ChunksResult
            |> Import "getChunks" "@codemirror/merge"

            "goToNextChunk" =? StateCommand
            |> Import "goToNextChunk" "@codemirror/merge"

            "goToPreviousChunk" =? StateCommand
            |> Import "goToPreviousChunk" "@codemirror/merge"

            "unifiedMergeView" => Merge.UnifiedMergeConfig ^-> !| (Extension + StateField.[DecorationSet])
            |> Import "unifiedMergeView" "@codemirror/merge"

            "updateOriginalDoc" =? StateEffectType.[Merge.UpdateOriginalDoc]
            |> Import "updateOriginalDoc" "@codemirror/merge"

            "originalDocChangeEffect" => EditorState * ChangeSet ^-> StateEffect.[Merge.UpdateOriginalDoc]
            |> Import "originalDocChangeEffect" "@codemirror/merge"

            "getOriginalDoc" => EditorState ^-> State.Text
            |> Import "getOriginalDoc" "@codemirror/merge"

            "acceptChunk" => EditorView * !? T<int> ^-> T<bool>
            |> Import "acceptChunk" "@codemirror/merge"

            "rejectChunk" => EditorView * !? T<int> ^-> T<bool>
            |> Import "rejectChunk" "@codemirror/merge"

            "uncollapseUnchanged" =? StateEffectType.[T<int>]
            |> Import "uncollapseUnchanged" "@codemirror/merge"
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.CodeMirror" [
                CodeMirror
                HoverTooltipOptions
                ThemeColor
                VueConfig
                AngularConfig
                JinjaCompletionConfig
                LiquidCompletionConfig
                SassConfig
                PhpConfig
                YamlFrontmatterConfig
                CompletionPathResult
                JavaScriptConfig   
                MarkdownConfig

                //
                // CodeMirror State
                //
                EditorState
                State.Text
                TextIterator
                Line
                MapMode
                ChangeDesc
                ChangeSpec
                ChangeSet
                SelectionRange
                EditorSelection
                Transaction
                StateField
                StateFieldSpec
                AnnotationType
                Annotation
                ChangeByRangeResult
                ChangeByRangeOutput
                AnchorHeadConfig
                TransactionSpec
                FacetReader
                CharCategory
                EditorStateConfig
                Facet
                FacetConfig
                RangeSetUpdate
                RangeCursor
                State.Range
                RangeComparator
                SpanIterator
                RangeSet
                RangeValue
                StateEffectSpec
                StateEffect
                StateEffectType
                Compartment
                RangeSetBuilder
                StateCommandTarget
                Prec

                //
                // CodeMirror View
                //
                EditorView
                TooltipsOptions
                Viewport
                DocumentPaddingType
                DOMPos
                Coords
                ScrollStrategy
                ScrollIntoViewOptions
                ViewUpdate
                MouseSelectionStyle
                View.Rect
                WidgetType
                Direction
                MeasureRequest
                Decoration
                PluginValue
                ViewPlugin
                PluginSpec
                BlockType
                BlockInfo
                BidiSpan
                ScrollTarget
                EditorViewConfig
                ThemeOptions
                MatchDecoratorConfig
                MatchDecorator
                RectangularSelectionOptions
                CrosshairCursorOptions
                SelectionConfig
                GutterMarker
                RectangleMarker
                Offset
                TooltipView
                Tooltip
                Panel
                SpecialCharConfig
                LayerMarker
                LayerConfig
                PanelConfig
                DialogConfig
                GutterConfig
                LineNumberConfig
                ShowDialogResult
                GuttersConfig

                //
                // CodeMirror Language
                //
                Regions
                Language
                LRLanguageSpec
                LRLanguage
                LanguageSupport
                HighlightStyleOptions
                HighlightStyle
                RangeConfig
                FoldConfig
                FoldGutterConfig
                KeyBinding
                IndentContextOptions
                LineAtResult
                IndentContext
                SyntaxHighlightingOptions
                TreeIndentContext
                RangeSpan
                MatchResult
                Config
                LanguageDescriptionSpec
                LanguageDescription
                DocRange
                DelimitedIndentOptions
                ContinuedIndentOptions
                BidiIsolatesOptions
                DocInputClass
                ParseContextClass
                Sublanguage
                StreamLanguage
                StreamParser
                StringStream

                //
                // CodeMirror Lint
                //
                Lint.Severity  
                Lint.Action  
                Lint.Diagnostic  
                Lint.LintConfig  
                Lint.LintGutterConfig  

                //
                // CodeMirror Search
                //
                Search.SearchQueryConfig
                Search.SearchQuery  
                Search.HighlightOptions  
                Search.SearchConfig  
                Search.RegExpMatchValue  
                Search.RegExpCursorOptions  
                Search.RegExpCursor  
                Search.SearchCursor  

                //
                // CodeMirror Command
                //
                CodeMirrorCommand.BlockComment  
                CodeMirrorCommand.CommentTokens  
                CodeMirrorCommand.HistoryConfig  

                //
                // CodeMirror AutoComplete
                //
                AutoComplete.AbortListenerOptions  
                AutoComplete.TokenBeforeResult  
                AutoComplete.MatchBeforeResult  
                AutoComplete.CompletionInfoObject  
                AutoComplete.CompletionSection  
                AutoComplete.Completion  
                AutoComplete.CompletionContext  
                AutoComplete.CompletionResult  
                AutoComplete.AddToOptions  
                AutoComplete.PositionInfoResult  
                AutoComplete.CompletionConfig  
                AutoComplete.SnippetContext

                //
                // CodeMirror Collab
                //
                Collab.CollabConfig
                Collab.Update
                Collab.UpdateOver

                //
                // CodeMirror lang SQL
                //
                LangSQL.SQLDialectSpec  
                LangSQL.SQLDialect  
                LangSQL.SQLNamespace  
                LangSQL.SQLConfig  

                //
                // CodeMirror lang HTML
                //
                LangHtml.TagSpec  
                LangHtml.NestedLang  
                LangHtml.NestedAttr  
                LangHtml.HtmlCompletionSourceConfig  
                LangHtml.HtmlConfig

                //
                // CodeMirror lang XML
                //
                LangXml.AttrSpec  
                LangXml.ElementSpec  
                LangXml.XMLConfig  

                //
                // CodeMirror LSP Client
                //
                LSPClient.LSPClientClass  
                LSPClient.WorkspaceMapping  
                LSPClient.WorkspaceFile  
                LSPClient.WorkspaceFileUpdate  
                LSPClient.Workspace  
                LSPClient.LSPClientConfig  
                LSPClient.Transport  
                LSPClient.LSPPlugin  
                LSPClient.SignatureHelpConfig
                LSPClient.ServerCompletionConfig

                //
                // CodeMirror Merge
                //
                Merge.Chunk  
                Merge.Change  
                Merge.DiffConfig  
                Merge.CollapseUnchangedConfig  
                Merge.MergeConfig  
                Merge.DirectMergeConfig  
                Merge.UnifiedMergeConfig  
                Merge.MergeView  
                Merge.UpdateOriginalDoc  
                Merge.ChunksResult  
            ]
        ]

[<Sealed>]
type CodeMirrorExtension() =
    interface IExtension with
        member ext.Assembly = Definition.Assembly

[<assembly: Extension(typeof<CodeMirrorExtension>)>]
do ()
