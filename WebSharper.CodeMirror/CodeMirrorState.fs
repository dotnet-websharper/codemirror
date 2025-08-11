namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module State = 

    let ImportFromState (c: CodeModel.Class) = 
        Import c.Name "@codemirror/state" c

    let Extension = T<obj> + !| T<obj>

    let EditorState = Class "EditorState"

    let TextIterator =
        Class "TextIterator"
        |> ImportFromState
        |+> Instance [
            "next" => !? T<int> ^-> TSelf
            "value" =@ T<string>
            "done" =@ T<bool>
            "lineBreak" =@ T<bool>
        ]

    let Line =
        Class "Line"
        |> ImportFromState
        |+> Instance [
            "from" =? T<int>
            "to" =? T<int>
            "number" =? T<int>
            "text" =? T<string>
            "length" =? T<int>
        ]

    let Text = 
        Class "Text"
        |> ImportFromState
        |+> Instance [
            "length" =? T<int>
            "lines" =? T<int>
            "children" =? !? (!| TSelf)

            
            "lineAt" => T<int> ^-> Line
            "line" => T<int> ^-> Line
            "replace" => T<int> * T<int> * TSelf ^-> TSelf
            "append" => TSelf ^-> TSelf
            "slice" => T<int> * !? T<int> ^-> TSelf
            "sliceString" => T<int> * !? T<int> * !? T<string> ^-> T<string>
            "eq" => TSelf ^-> T<bool>
            "iter" => !? T<int> ^-> TextIterator
            "iterRange" => T<int> * !? T<int> ^-> TextIterator
            "iterLines" => !? T<int> * !? T<int> ^-> TextIterator
            "toString" => T<unit> ^-> T<string>
            "toJSON" => T<unit> ^-> !| T<string>
        ]
        |+> Static [
            "of" => !| T<string> ^-> TSelf
            "empty" =? TSelf
        ]

    let MapMode =
        Pattern.EnumStrings "MapMode" [
            "Simple"
            "TrackDel"
            "TrackBefore"
            "TrackAfter"
        ]
        |> ImportFromState

    let ChangeDesc =
        Class "ChangeDesc"
        |> ImportFromState
        |+> Instance [
            "length" =? T<int>
            "newLength" =? T<int>
            "empty" =? T<bool>

            "iterGaps" => (T<int> * T<int> * T<int> ^-> T<unit>) ^-> T<unit>

            "iterChangedRanges" => 
                (T<int> * T<int> * T<int> * T<int> ^-> T<unit>) 
                * !? T<bool> 
                ^-> T<unit>

            "invertedDesc" =? TSelf
            "composeDesc" => TSelf ^-> TSelf
            "mapDesc" => TSelf * !? T<bool> ^-> TSelf

            "mapPos" => T<int> * !? T<int> * !? MapMode ^-> (T<int> + T<unit>)
            "touchesRange" => T<int> * !? T<int> ^-> (T<bool> + T<string>)
            "toJSON" => T<unit> ^-> !| T<int>
        ]
        |+> Static [
            "fromJSON" => T<obj>?json ^-> TSelf
        ]

    let ChangeSpec =
        Pattern.Config "ChangeSpec" {
            Required = [
                "from", T<int>
            ]
            Optional = [
                "to", T<int>
                "insert", T<string> + Text.Type
            ]
        }
        |> ImportFromState

    let ChangeSet =
        Class "ChangeSet"        

    let ChangeSpecType = ChangeSpec + ChangeSet + !| ChangeSpec

    ChangeSet
        |> ImportFromState
        |=> Inherits ChangeDesc
        |+> Instance [
            "apply" => Text.Type ^-> Text.Type
            "mapDesc" => ChangeDesc * !? T<bool> ^-> ChangeDesc
            "invert" => Text.Type ^-> ChangeSet
            "compose" => ChangeSet ^-> ChangeSet
            "map" => ChangeDesc * !? T<bool> ^-> ChangeSet
            "iterChanges" => 
                (T<int> * T<int> * T<int> * T<int> * Text.Type ^-> T<unit>)
                * !? T<bool> ^-> T<unit>
            "desc" =? ChangeDesc
            "toJSON" => T<unit> ^-> T<obj>
        ]
        |+> Static [
            "of" => ChangeSpecType * T<int> * !? T<string> ^-> ChangeSet
            "empty" => T<int> ^-> ChangeSet
            "fromJSON" => T<obj> ^-> ChangeSet
        ]
        |> ignore

    let SelectionRange =
        Class "SelectionRange"
        |> ImportFromState
        |+> Instance [
            "from" =? T<int>
            "to" =? T<int>
            "anchor" =? T<int>
            "head" =? T<int>
            "empty" =? T<bool>
            "assoc" =? T<int>
            "bidiLevel" =? T<int>
            "goalColumn" =? T<int>

            "map" => ChangeDesc * !? T<int> ^-> TSelf
            "extend" => T<int> * !? T<int> ^-> TSelf
            "eq" => TSelf * !? T<bool> ^-> T<bool>
            "toJSON" => T<unit> ^-> T<obj>
        ]
        |+> Static [
            "fromJSON" => T<obj> ^-> TSelf
        ]

    let EditorSelection =
        Class "EditorSelection"
        |> ImportFromState
        |+> Instance [
            "ranges" =? !| SelectionRange
            "mainIndex" =? T<int>
            "map" => ChangeDesc * !? T<int> ^-> TSelf
            "eq" => TSelf * !? T<bool> ^-> T<bool>
            "main" =? SelectionRange

            "asSingle" => T<unit> ^-> TSelf
            "addRange" => SelectionRange * !? T<bool> ^-> TSelf
            "replaceRange" => SelectionRange * !? T<int> ^-> TSelf
            "toJSON" => T<unit> ^-> T<obj>
        ]
        |+> Static [
            "fromJSON" => T<obj> ^-> TSelf
            "single" => T<int> * !? T<int> ^-> TSelf
            "create" => !| SelectionRange * !? T<int> ^-> TSelf
            "cursor" => T<int> * !? T<int> * !? T<int> * !? T<int> ^-> SelectionRange
            "range" => T<int> * T<int> * !? T<int> * !? T<int> ^-> SelectionRange
        ]

    let Transaction =
        Class "Transaction"

    let StateFieldDeclare = Class "StateField"

    let StateFieldSpec =
        Generic - fun value ->
            Pattern.Config "StateFieldSpec" {
                Required = [
                    "create", EditorState ^-> value
                    "update", value * Transaction ^-> value
                ]
                Optional = [
                    "compare", value * value ^-> T<bool>
                    "provide", StateFieldDeclare.[value] ^-> Extension
                    "toJSON", value * EditorState ^-> T<obj>
                    "fromJSON", T<obj> * EditorState ^-> value
                ]
            }

    let StateField =
        Generic - fun value ->
            StateFieldDeclare
            |> ImportFromState
            |+> Instance [
                "init" => (EditorState ^-> value) ^-> Extension
                "extension" =? Extension
            ]
            |+> Static [
                "define" => StateFieldSpec.[value] ^-> TSelf.[value]
            ]

    let AnnotationType = Class "AnnotationType"
    let Annotation = Class "Annotation"

    do
        (Generic - fun t ->
        AnnotationType
        |> ImportFromState
        |+> Instance [
            "of" => t?value ^-> Annotation.[t]
        ])
        |> ignore

    do
        (Generic - fun t ->
        Annotation
        |> ImportFromState
        |+> Instance [
            "type" =? AnnotationType.[t]
            "value" =? t
        ]
        |+> Static [
            "define" => T<unit> ^-> AnnotationType.[t]
        ])
        |> ignore

    let ChangeByRangeResult =
        Pattern.Config "ChangeByRangeResult" {
            Required = [
                "range", SelectionRange.Type
            ]
            Optional = [
                "changes", ChangeSpecType
                "effects", StateField.[T<obj>] + !| StateField.[T<obj>]
            ]
        }

    let ChangeByRangeOutput =
        Pattern.Config "ChangeByRangeOutput" {
            Required = [
                "changes", ChangeSet.Type
                "selection", EditorSelection.Type
                "effects", !| StateField.[T<obj>]
            ]
            Optional = []
        }

    let AnchorHeadConfig =
        Pattern.Config "AnchorHeadConfig" {
            Required = [
                "anchor", T<int>
            ]
            Optional = [
                "head", T<int>
            ]
        }

    let TransactionSpec =
        Pattern.Config "TransactionSpec" {
            Required = []
            Optional = [
                "changes", ChangeSpecType
                "selection", EditorSelection + AnchorHeadConfig
                "effects", StateField.[T<obj>] + !| StateField.[T<obj>]
                "annotations", Annotation.[T<obj>] + !| Annotation.[T<obj>]
                "userEvent", T<string>
                "scrollIntoView", T<bool>
                "filter", T<bool>
                "sequential", T<bool>
            ]
        }
        |> ImportFromState

    let FacetReader =
        Generic - fun output -> 
        Pattern.Config "FacetReader" {
            Required = []
            Optional = [
                "tag", output.Type
            ]
        }
        |> ImportFromState

    let CharCategory =
        Pattern.EnumStrings "CharCategory" [
            "Word"
            "Space"
            "Other"
        ]
        |> ImportFromState

    let EditorStateConfig =
        Pattern.Config "EditorStateConfig" {
            Required = []
            Optional = [
                "doc", T<string> + Text.Type
                "selection", EditorSelection + AnchorHeadConfig
                "extensions", Extension
            ]
        }
        |> ImportFromState

    let Slot = FacetReader.[T<obj>] + StateField.[T<obj>] + T<string>

    let FacetDeclare = Class "Facet"

    let FacetConfig =
        Generic -- fun input output ->
            Pattern.Config "FacetConfig" {
                Required = []
                Optional = [
                    "combine", !| input ^-> output
                    "compare", output * output ^-> T<bool>
                    "compareInput", input * input ^-> T<bool>
                    "static", T<bool>
                    "enables", Extension + (FacetDeclare.[input, output] ^-> Extension)
                ]
            }
            |> ImportFromState

    let Facet =
        Generic -- fun input output ->
            FacetDeclare
            |> ImportFromState
            |+> Instance [
                "reader" =? FacetReader.[output]
                "of" => input ^-> Extension
                "compute" =>
                    !| Slot * (EditorState ^-> input) ^-> Extension
                "computeN" =>
                    !| Slot * (EditorState ^-> !| input) ^-> Extension

                Generic - fun t ->
                    "from" => StateField.[t] ^-> Extension

                Generic - fun t ->
                    "from" => StateField.[t] * (t ^-> input) ^-> Extension

                "tag" =? output
            ]
            |+> Static [
                "define" => !? FacetConfig.[input, output] ^-> TSelf.[input, output]
            ]

    let Range =
        Generic - fun t ->
            Class "Range"
            |> ImportFromState
            |+> Instance [
                "from" =? T<int>
                "to" =? T<int>
                "value" =? t
            ]

    let RangeSetUpdate =
        Generic - fun t ->
            Pattern.Config "RangeSetUpdate" {
                Required = []
                Optional = [
                    "add", !| Range.[t]
                    "sort", T<bool>
                    "filter", T<int> * T<int> * t ^-> T<bool>
                    "filterFrom", T<int>
                    "filterTo", T<int>
                ]
            }
            |> ImportFromState

    let RangeCursor =
        Generic - fun t ->
            Pattern.Config "RangeCursor" {
                Required = []
                Optional = [
                    "next", T<unit> ^-> T<unit>
                    "value", t.Type
                    "from", T<int>
                    "to", T<int>
                ]
            }
            |> ImportFromState

    let RangeComparator =
        Generic - fun t ->
            Pattern.Config "RangeComparator" {
                Required = [
                    "compareRange", T<int> * T<int> * !| t * !| t ^-> T<unit>
                    "comparePoint", T<int> * T<int> * t * t ^-> T<unit>
                ]
                Optional = [
                    "boundChange", T<int> ^-> T<unit>
                ]
            }
            |> ImportFromState

    let SpanIterator =
        Generic - fun t ->
            Pattern.Config "SpanIterator" {
                Required = [
                    "span", T<int> * T<int> * !| t * T<int> ^-> T<unit>
                    "point", T<int> * T<int> * t * !| t * T<int> * T<int> ^-> T<unit>
                ]
                Optional = []
            }
            |> ImportFromState

    let RangeSet =
        Generic - fun t ->
            Class "RangeSet"
            |> ImportFromState
            |+> Instance [
                "size" =? T<int>

                Generic - fun u ->
                    "update" => RangeSetUpdate.[u] ^-> t

                "map" => ChangeDesc ^-> TSelf.[t]
                "between" => T<int> * T<int> * (T<int> * T<int> * t ^-> T<unit> + T<bool>) ^-> T<unit>
                "iter" => !? T<int> ^-> RangeCursor.[t]
            ]
            |+> Static [
                Generic - fun t ->
                    "iterStatic" => !| TSelf.[t] * !? T<int> ^-> RangeCursor.[t]

                Generic - fun t ->
                    "compare" => !| TSelf.[t] * !| TSelf.[t] * ChangeDesc * RangeComparator.[t] * !? T<int> ^-> T<unit>
                
                Generic - fun t ->
                    "eq" => !| TSelf.[t] * !| TSelf.[t] * !? T<int> * !? T<int> ^-> T<bool>

                Generic - fun t ->
                    "spans" => !| TSelf.[t] * T<int> * T<int> * SpanIterator.[t] * !? T<int> ^-> T<int>

                Generic - fun t ->
                    "of" => (Range.[t] + !| Range.[t]) * !? T<bool> ^-> TSelf.[t] 

                Generic - fun t ->
                    "join" => !| TSelf.[t] ^-> TSelf.[t]
                "empty" =? TSelf.[T<obj>]
            ]

    let RangeValue =
        Class "RangeValue"
        |> ImportFromState
        |+> Instance [
            "eq" => TSelf ^-> T<bool>
            "startSide" =? T<int>
            "endSide" =? T<int>
            "mapMode" =? MapMode
            "point" =? T<bool>
            "range" => T<int> * !? T<int> ^-> Range.[TSelf]
        ]

    let StateEffectSpec =
        Generic - fun value ->
            Pattern.Config "StateEffectSpec" {
                Required = []
                Optional = [
                    "map", value * ChangeDesc ^-> value
                ]
            }

    let StateEffect = Class "StateEffect"

    let StateEffectType =
        Generic - fun value ->
            Class "StateEffectType"
            |> ImportFromState
            |+> Instance [
                "map" =? (T<obj> * ChangeDesc ^-> T<obj>)
                "of" => value ^-> StateEffect.[value]
            ]

    do
        (Generic - fun value ->
        StateEffect
        |> ImportFromState
        |+> Instance [
            "value" =? value

            "map" => ChangeDesc ^-> (TSelf.[value])

            Generic - fun t ->
                "is" => StateEffectType.[t] ^-> T<bool>
        ]
        |+> Static [
            Generic - fun v ->
                "define" => !? StateEffectSpec.[v] ^-> StateEffectType.[v]

            "mapEffects" => (!| TSelf.[T<obj>]) * ChangeDesc ^-> !| TSelf.[T<obj>]

            "reconfigure" =? StateEffectType.[Extension]
            "appendConfig" =? StateEffectType.[Extension]
        ])
        |> ignore

    let Compartment =
        Class "Compartment"
        |> ImportFromState
        |+> Instance [
            "of" => Extension ^-> Extension
            "reconfigure" => Extension ^-> StateEffect.[T<obj>]
            "get" => EditorState ^-> Extension
        ]

    let RangeSetBuilder =
        Generic - fun t ->
            Class "RangeSetBuilder"
            |> ImportFromState
            |+> Instance [
                "add" => T<int> * T<int> * t ^-> T<unit>
                "finish" => RangeSet.[t]
            ]
            |+> Static [
                Constructor (T<unit>)
            ]

    let StateCommandTarget =
        Pattern.Config "StateCommandTarget" {
            Required = [
                "state", EditorState.Type
                "dispatch", Transaction ^-> T<unit>
            ]
            Optional = []
        }

    let StateCommand = StateCommandTarget ^-> T<bool>

    let Prec =
        Pattern.Config "Prec" {
            Required = [
                "highest", Extension ^-> Extension
                "high", Extension ^-> Extension
                "default", Extension ^-> Extension
                "low", Extension ^-> Extension
                "lowest", Extension ^-> Extension
            ]
            Optional = []
        }
        |> ImportFromState

    Transaction
        |> ImportFromState
        |+> Instance [
            "startState" =? EditorState
            "changes" =? ChangeSet
            "selection" =? EditorSelection
            "effects" =? !| StateField.[T<obj>]
            "scrollIntoView" =? T<bool>

            "newDoc" =? Text
            "newSelection" =? EditorSelection
            "state" =? EditorState

            Generic - fun t ->
                "annotation" => AnnotationType.[t] ^-> t

            "docChanged" =? T<bool>
            "reconfigured" =? T<bool>
            "isUserEvent" => T<string> ^-> T<bool>
        ]
        |+> Static [
            "time" =? AnnotationType.[T<int>]
            "userEvent" =? AnnotationType.[T<string>]
            "addToHistory" =? AnnotationType.[T<bool>]
            "remote" =? AnnotationType.[T<bool>]
        ]
        |> ignore

    let languageDataFunc = (EditorState * T<int> * T<int> ^-> !| T<obj>)
    let changeFilterFunc = (Transaction ^-> T<bool> + !| T<int>)
    let transactionFilter = (Transaction ^-> TransactionSpec + !| TransactionSpec)
    let transactionExtender = (Transaction ^-> T<string>)

    EditorState
        |> ImportFromState
        |+> Instance [
            "doc" =? Text
            "selection" =? EditorSelection
            "tabSize" =? T<int>
            "lineBreak" =? T<string>
            "readOnly" =? T<bool>

            Generic - fun t ->
                "field" => StateField.[t] * !? T<bool> ^-> t

            "update" => !+ TransactionSpec ^-> Transaction
            "replaceSelection" => (T<string> + Text) ^-> TransactionSpec
            "changeByRange" => (SelectionRange ^-> ChangeByRangeResult) ^-> ChangeByRangeOutput
            "changes" => !? ChangeSpecType ^-> ChangeSet
            "toText" => T<string> ^-> Text
            "sliceDoc" => !? T<int> * !? T<int> ^-> T<string>

            Generic - fun output ->
                "facet" => FacetReader.[output] ^-> output

            "toJSON" => !? T<obj> ^-> T<obj>

            Generic - fun t ->
                "languageDataAt" => T<string> * T<int> * !? T<int> ^-> !| t

            "charCategorizer" => T<int> ^-> (T<string> ^-> CharCategory)
            "wordAt" => T<int> ^-> SelectionRange + T<unit>
            "phrase" => T<string> *+ T<obj> ^-> T<string>
        ]
        |+> Static [
            "fromJSON" => T<obj> * !? EditorStateConfig * !? T<obj> ^-> EditorState
            "create" => !? EditorStateConfig ^-> EditorState
            "allowMultipleSelections" =? Facet.[T<bool>, T<bool>]
            "tabSizeStatic" =? Facet.[T<int>, T<int>]
            "lineSeparator" =? Facet.[T<string>, T<string>]
            "readOnlyStatic" =? Facet.[T<bool>, T<bool>]
            "phrases" =? Facet.[T<obj>, T<obj[]>]
            "languageData" =? Facet.[languageDataFunc, !| languageDataFunc]
            "changeFilter" =? Facet.[changeFilterFunc, !| changeFilterFunc]
            "transactionFilter" =? Facet.[transactionFilter, !| transactionFilter]
            "transactionExtender" =? Facet.[transactionExtender, !| transactionExtender]
        ]
        |> ignore