namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator
open State

module View = 

    let ImportFromView (c: CodeModel.Class) = 
        Import c.Name "@codemirror/view" c

    let DOMEventHandlers = T<obj>

    let EditorView =
        Class "EditorView"

    let Command = EditorView ^-> T<bool>

    let TooltipsOptions =
        Pattern.Config "TooltipsOptions" {
            Required = []
            Optional = [
                "position", T<string> 
                "parent", T<HTMLElement>
                "tooltipSpace", EditorView ^-> T<Dom.Rect>
            ]
        }

    let Viewport =
        Pattern.Config "Viewport" {
            Required = [
                "from", T<int>
                "to", T<int>
            ]
            Optional = []
        }

    let DocumentPaddingType =
        Pattern.Config "DocumentPadding" {
            Required = [
                "top", T<int>
                "bottom", T<int>
            ]
            Optional = []
        }

    let DOMPos =
        Pattern.Config "DOMPos" {
            Required = [
                "node", T<Dom.Node>
                "offset", T<int>
            ]
            Optional = []
        }

    let Coords =
        Pattern.Config "Coords" {
            Required = [
                "x", T<int>
                "y", T<int>
            ]
            Optional = []
        }

    let ScrollStrategy =
        Pattern.EnumStrings "ScrollStrategy" [
            "nearest"
            "start"
            "end"
            "center"
        ]
        |> ImportFromView

    let ScrollIntoViewOptions =
        Pattern.Config "ScrollIntoViewOptions" {
            Required = []
            Optional = [
                "y", ScrollStrategy.Type
                "x", ScrollStrategy.Type
                "yMargin", T<int>
                "xMargin", T<int>
            ]
        }

    let MeasureRequest =
        Generic - fun t ->
            Pattern.Config "MeasureRequest" {
                Required = [
                    "read", EditorView ^-> t
                ]
                Optional = [
                    "write", t * EditorView ^-> T<unit>
                    "key", T<obj>
                ]
            }
            |> ImportFromView

    let ViewUpdate =
        Class "ViewUpdate"
        |> ImportFromView
        |+> Instance [
            "view" =? EditorView
            "state" =? EditorState
            "transactions" =? !| Transaction
            "changes" =? ChangeSet
            "startState" =? EditorState
            "viewportChanged" =? T<bool>
            "viewportMoved" =? T<bool>
            "heightChanged" =? T<bool>
            "geometryChanged" =? T<bool>
            "focusChanged" =? T<bool>
            "docChanged" =? T<bool>
            "selectionSet" =? T<bool>
        ]

    let Rect =
        Pattern.Config "Rect" {
            Required = []
            Optional = [
                "left", T<float>
                "right", T<float>
                "top", T<float>
                "bottom", T<float>
            ]
        }
        |> ImportFromView

    let WidgetType = 
        Class "WidgetType"
        |> ImportFromView
        |+> Instance [
            "toDOM" => EditorView ^-> T<HTMLElement>
            "eq" => TSelf ^-> T<bool>
            "updateDOM" => T<HTMLElement> * EditorView ^-> T<bool>
            "estimatedHeight" =? T<int>
            "lineBreaks" =? T<int>
            "ignoreEvent" => T<Dom.Event> ^-> T<bool>
            "coordsAt" => T<HTMLElement> * T<int> * T<int> ^-> Rect
            "destroy" => T<HTMLElement> ^-> T<unit>
        ]

    let Direction =
        Pattern.EnumStrings "Direction" [
            "LTR"
            "RTL"
        ]
        |> ImportFromView

    let MarkDecorationSpec = T<obj>

    let WidgetDecorationSpec = T<obj>

    let ReplaceDecorationSpec = T<obj>

    let LineDecorationSpec = T<obj>

    let Decoration =
        Class "Decoration"

    let DecorationSet = RangeSet.[Decoration]

    Decoration
        |> ImportFromView
        |=> Inherits RangeValue
        |+> Instance [
            "spec" =? T<obj>
            "eq" => TSelf ^-> T<bool>
        ]
        |+> Static [

            Constructor (T<int> * T<int> * WidgetType * T<obj>)

            "mark" => MarkDecorationSpec ^-> TSelf
            "widget" => WidgetDecorationSpec ^-> TSelf
            "replace" => ReplaceDecorationSpec ^-> TSelf
            "line" => LineDecorationSpec ^-> TSelf
            "set" => (Range.[TSelf] + !| Range.[TSelf]) * !? T<bool> ^-> DecorationSet
            "none" =? DecorationSet
        ]
        |> ignore

    let PluginValue =
        Class "PluginValue"
        |> ImportFromView
        |+> Instance [
            "update" => ViewUpdate ^-> T<unit>
            "docViewUpdate" => EditorView ^-> T<unit>
            "destroy" => T<unit> ^-> T<unit>
        ]

    let ViewPlugin = Class "ViewPlugin"

    let PluginSpec =
        Generic - fun v ->
            Pattern.Config "PluginSpec" {
                Required = []
                Optional = [
                    "eventHandlers", DOMEventHandlers
                    "eventObservers", DOMEventHandlers
                    "provide", (ViewPlugin.[v, T<obj>] ^-> Extension)
                    "decorations", (v ^-> DecorationSet)
                ]
            }
            |> ImportFromView

    do
        (Generic -- fun v arg ->
        ViewPlugin
        |> ImportFromView
        |+> Instance [
            "extension" =? T<obj>
            "of" => arg ^-> Extension
        ]
        |+> Static [
            Generic -- fun v arg ->
                "define" => (EditorView * arg ^-> v) * !? PluginSpec.[v] ^-> TSelf.[v, arg]

            Generic -- fun v arg ->
                "fromClass" => T<obj> * !? PluginSpec.[v] ^-> TSelf.[v, arg]
        ])
        |> ignore

    let BlockType =
        Pattern.EnumStrings "BlockType" [
            "Text"
            "WidgetBefore"
            "WidgetAfter"
            "WidgetRange"
        ]
        |> ImportFromView

    let BlockInfo =
        Class "BlockInfo"
        |> ImportFromView
        |+> Instance [
            "from" =? T<int>
            "length" =? T<int>
            "top" =? T<int>
            "height" =? T<int>
            "type" =? (BlockType + !| TSelf)
            "to" =? T<int>
            "bottom" =? T<int>
            "widget" =? WidgetType
            "widgetLineBreaks" =? T<int>
        ]

    let BidiSpan =
        Class "BidiSpan"
        |> ImportFromView
        |+> Instance [
            "from" =? T<int>
            "to" =? T<int>
            "level" =? T<int>
            "dir" =? Direction
        ]

    let ScrollTarget =
        Class "ScrollTarget"
        |> ImportFromView
        |+> Instance [
            "range" =? SelectionRange
            "y" =? ScrollStrategy
            "x" =? ScrollStrategy
            "yMargin" =? T<int>
            "xMargin" =? T<int>
            "isSnapshot" =? T<bool>

            "map" => ChangeDesc ^-> TSelf
            "clip" => EditorState ^-> TSelf
        ]
        |+> Static [
            Constructor (SelectionRange * !? ScrollStrategy * !? ScrollStrategy * !? T<int> * !? T<int> * !? T<bool>)
        ]

    let EditorViewConfig =
        Pattern.Config "EditorViewConfig" {
            Required = []
            Optional = [
                "state", EditorState.Type
                "parent", T<obj> 
                "root", T<obj>
                "scrollTo", StateEffect.[T<obj>]
                "dispatchTransactions", (!| Transaction * EditorView ^-> T<unit>)
                "dispatch", (Transaction * EditorView ^-> T<unit>)

                // inherit from EditorStateConfig
                "doc", T<string> + Text.Type
                "selection", EditorSelection + AnchorHeadConfig
                "extensions", Extension
            ]
        }
        |> ImportFromView

    let ThemeOptions =
        Pattern.Config "ThemeOptions" {
            Required = []
            Optional = [
                "dark", T<bool>
            ]
        }

    let AttrSource = T<obj> + (EditorView ^-> T<obj>)

    let MatchDecoratorConfig = 
        Pattern.Config "MatchDecoratorConfig" {
            Required = [
                "regexp", T<RegExp>
            ]
            Optional = [
                "decoration", Decoration + (T<obj> * EditorView * T<int> ^-> Decoration)
                "decorate", (T<int> * T<int> * Decoration ^-> T<unit>) * T<int> * T<int> * T<obj> * EditorView ^-> T<unit>
                "boundary", T<RegExp>
                "maxLength", T<int>
            ]
        }

    let MatchDecorator =
        Class "MatchDecorator"
        |> ImportFromView
        |+> Instance [
            "createDeco" => EditorView ^-> RangeSet.[Decoration]
            "updateDeco" => ViewUpdate * DecorationSet ^-> DecorationSet
        ]
        |+> Static [
            Constructor (MatchDecoratorConfig)
        ]

    let RectangularSelectionOptions =
        Pattern.Config "RectangularSelectionOptions" {
            Required = []
            Optional = [
                "eventFilter", T<Dom.MouseEvent> ^-> T<bool>
            ]
        }

    let CrosshairCursorOptions =
        Pattern.Config "CrosshairCursorOptions" {
            Required = []
            Optional = [
                "key", T<string>
            ]
        }

    let SelectionConfig =
        Pattern.Config "SelectionConfig" {
            Required = []
            Optional = [
                "cursorBlinkRate", T<int>
                "drawRangeCursor", T<bool>
            ]
        }

    let GutterMarker =
        Class "GutterMarker"
        |> ImportFromView
        |=> Inherits RangeValue
        |+> Instance [
            "eq" => TSelf ^-> T<bool>
            "toDOM" => EditorView ^-> T<obj>
            "elementClass" =@ T<string>
            "destroy" => T<Dom.Node> ^-> T<unit> 
        ]

    let RectangleMarker =
        Class "RectangleMarker"
        |> ImportFromView
        |+> Instance [
            "left" =? T<int>
            "top" =? T<int>
            "width" =? T<int>
            "height" =? T<int>

            "draw" => T<unit> ^-> T<obj>
            "update" => T<HTMLElement> * TSelf ^-> T<bool>
            "eq" => TSelf ^-> T<bool>
        ]
        |+> Static [
            Constructor (T<string> * T<int> * T<int> * T<int> * T<int>)

            "forRange" => EditorView * T<string> * SelectionRange ^-> !| TSelf
        ]

    let Offset =
        Pattern.Config "Offset" {
            Required = [
                "x", T<int>
                "y", T<int>
            ]
            Optional = []
        }

    let TooltipView =
        Pattern.Config "TooltipView" {
            Required = [
                "dom", T<HTMLElement>
            ]
            Optional = [
                "offset", Offset.Type
                "getCoords", T<int> ^-> Rect
                "overlap", T<bool>
                "mount", EditorView ^-> T<unit>
                "update", ViewUpdate ^-> T<unit>
                "destroy", T<unit> ^-> T<unit>
                "positioned", Rect ^-> T<unit>
                "resize", T<bool>
            ]
        }
        |> ImportFromView

    let Tooltip =
        Pattern.Config "Tooltip" {
            Required = [
                "pos", T<int>
                "create", EditorView ^-> TooltipView
            ]
            Optional = [
                "end", T<int>
                "above", T<bool>
                "strictSide", T<bool>
                "arrow", T<bool>
                "clip", T<bool>
            ]
        }
        |> ImportFromView

    let Panel =
        Pattern.Config "Panel" {
            Required = [
                "dom", T<HTMLElement>
            ]
            Optional = [
                "mount", T<unit> ^-> T<unit>
                "update", ViewUpdate ^-> T<unit>
                "destroy", T<unit> ^-> T<unit>
                "top", T<bool>
            ]
        }
        |> ImportFromView

    let PanelConstructor = EditorView ^-> Panel

    let SpecialCharConfig =
        Pattern.Config "SpecialCharConfig" {
            Required = []
            Optional = [
                "render", (T<int> * T<string> * T<string> ^-> T<HTMLElement>)
                "specialChars", T<RegExp>
                "addSpecialChars", T<RegExp>
            ]
        }

    let LayerMarker =
        Class "LayerMarker"
        |> ImportFromView
        |+> Instance [
            "eq" => TSelf ^-> T<bool>
            "draw" => T<unit> ^-> T<HTMLElement>
            "update" => T<HTMLElement> * TSelf ^-> T<bool>
        ]

    let LayerConfig =
        Pattern.Config "LayerConfig" {
            Required = [
                "above", T<bool>
                "update", ViewUpdate * T<HTMLElement> ^-> T<bool>
                "markers", EditorView ^-> !| LayerMarker
            ]
            Optional = [
                "class", T<string>
                "updateOnDocViewUpdate", T<bool>
                "mount", T<HTMLElement> * EditorView ^-> T<unit>
                "destroy", T<HTMLElement> * EditorView ^-> T<unit>
            ]
        }
        |> ImportFromView

    let HoverTooltipSource = EditorView * T<int> * T<int> ^-> (T<unit> + Tooltip + !| Tooltip + T<Promise<_>>[T<unit> + Tooltip + !| Tooltip])

    let PanelConfig =
        Pattern.Config "PanelConfig" {
            Required = []
            Optional = [
                "topContainer", T<HTMLElement>
                "bottomContainer", T<HTMLElement>
            ]
        }
        |> ImportFromView

    let DialogConfig =
        Pattern.Config "DialogConfig" {
            Required = []
            Optional = [
                "content", (EditorView * (T<unit> ^-> T<unit>)) ^-> T<HTMLElement>
                "label", T<string>
                "input", T<obj>
                "submitLabel", T<string>
                "class", T<string>
                "focus", T<string> + T<bool>
                "top", T<bool>
            ]
        }
        |> ImportFromView

    let GutterConfig =
        Pattern.Config "GutterConfig" {
            Required = []
            Optional = [
                "class", T<string>
                "renderEmptyElements", T<bool>

                "markers", EditorView ^-> (RangeSet.[GutterMarker] + !| RangeSet.[GutterMarker])

                "lineMarker", EditorView * BlockInfo * !| GutterMarker ^-> GutterMarker
                "widgetMarker", EditorView * WidgetType * BlockInfo ^-> GutterMarker

                "lineMarkerChange", T<unit> + (ViewUpdate ^-> T<bool>)
                "initialSpacer", T<unit> + (EditorView ^-> GutterMarker)
                "updateSpacer", T<unit> + (GutterMarker * ViewUpdate ^-> GutterMarker)

                "domEventHandlers", T<obj>

                "side", T<string> //"before" | "after"
            ]
        }
        |> ImportFromView

    let LineNumberConfig =
        Pattern.Config "LineNumberConfig" {
            Required = []
            Optional = [
                "formatNumber", T<int> * EditorState ^-> T<string>
                "domEventHandlers", T<obj>
            ]
        }
        |> ImportFromView

    let ShowDialogResult = 
        Pattern.Config "ShowDialogResult" {
            Required = [
                "close", StateEffect.[T<obj>]
                "result", T<Promise<_>>[T<HTMLFormElement>]
            ]
            Optional = []
        }

    let GuttersConfig = 
        Pattern.Config "GuttersConfig" {
            Required = []
            Optional = [
                "fixed", T<bool>
            ]
        }       

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
                "any", EditorView * T<Dom.KeyboardEvent> ^-> T<bool>
                "scope", T<string>
                "preventDefault", T<bool>
                "stopPropagation", T<bool>
            ]
        }
        |> ImportFromView

    let MouseSelectionStyle =
        Pattern.Config "MouseSelectionStyle" {
            Required = [
                "get", T<Dom.MouseEvent> * T<bool> * T<bool> ^-> EditorSelection
                "update", ViewUpdate ^-> T<bool> + T<unit>
            ]
            Optional = []
        }
        |> ImportFromView

    let clipboardFilterFunc = (T<string> * EditorState ^-> T<string>)
    let scrollHandlerFunc = (EditorView * SelectionRange * T<obj> ^-> T<unit>)
    let focusChangeEffectFunc = (EditorState * T<bool> ^-> StateEffect.[T<obj>])
    let exceptionSinkFunc = (T<obj> ^-> T<unit>)
    let updateListenerFunc = (ViewUpdate ^-> T<unit>)
    let mouseSelectionStyleFunc = (EditorView * T<Dom.MouseEvent> ^-> MouseSelectionStyle)
    let dragMovesSelectionFunc = (T<Dom.MouseEvent> ^-> T<bool>)
    let clickAddsSelectionRangeFunc = dragMovesSelectionFunc
    let decorationsFunc = (DecorationSet + (EditorView ^-> DecorationSet))
    let atomicRangesFunc = (EditorView ^-> RangeSet.[T<obj>])
    let bidiIsolatedRangesFunc = decorationsFunc
    let scrollMarginsFunc = (EditorView ^-> Rect)

    EditorView
        |> ImportFromView
        |+> Instance [
            "state" =? EditorState
            "viewport" =? Viewport
            "visibleRanges" =? !| Viewport
            "inView" =? T<bool>
            "composing" =? T<bool>
            "compositionStarted" =? T<bool>
            "root" =? T<obj>
            "dom" =? T<HTMLElement>
            "scrollDOM" =? T<HTMLElement>
            "contentDOM" =? T<HTMLElement>

            "dispatch" => Transaction ^-> T<unit>
            "dispatch" => !|Transaction ^-> T<unit>
            "dispatch" => !+ TransactionSpec ^-> T<unit>

            "update" => !| Transaction ^-> T<unit>
            "setState" => EditorState ^-> T<unit>

            "themeClasses" =? T<string>

            Generic - fun t ->
                "requestMeasure" => !? MeasureRequest.[t] ^-> T<unit>

            Generic - fun t ->
                "plugin" => ViewPlugin.[t, T<obj>] ^-> t

            "documentTop" =? T<int>
            "documentPadding" =? DocumentPaddingType
            "scaleX" =? T<float>
            "scaleY" =? T<float>

            "elementAtHeight" => T<int> ^-> BlockInfo
            "lineBlockAtHeight" => T<int> ^-> BlockInfo

            "viewportLineBlocks" =? !| BlockInfo

            "lineBlockAt" => T<int> ^-> BlockInfo

            "contentHeight" =? T<int>

            "moveByChar" => SelectionRange * T<bool> * !? (T<string> ^-> (T<string> ^-> T<bool>)) ^-> SelectionRange
            "moveByGroup" => SelectionRange * T<bool> ^-> SelectionRange
            "visualLineSide" => Line * T<bool> ^-> SelectionRange
            "moveToLineBoundary" => SelectionRange * T<bool> * !? T<bool> ^-> SelectionRange
            "moveVertically" => SelectionRange * T<bool> * !? T<int> ^-> SelectionRange
            "domAtPos" => T<int> ^-> DOMPos
            "posAtDOM" => T<Dom.Node> * !? T<int> ^-> T<int>
            "posAtCoords" => Coords * !? T<bool> ^-> T<int>
            "coordsAtPos" => T<int> * !? T<int> ^-> Rect
            "coordsForChar" => T<int> ^-> Rect + T<unit>
            "defaultCharacterWidth" =? T<float>
            "defaultLineHeight" =? T<float>
            "textDirection" =? Direction

            "textDirectionAt" => T<int> ^-> Direction

            "lineWrapping" =? T<bool>            
            "bidiSpans" =? Line ^-> !| BidiSpan
            "hasFocus" =? T<bool>

            "focus" => T<unit> ^-> T<unit>            
            "setRoot" => T<obj> ^-> T<unit>
            "destroy" => T<unit> ^-> T<unit>
            "scrollSnapshot" => T<unit> ^-> StateEffect.[ScrollTarget]
            "setTabFocusMode" => !? (T<bool> + T<int>) ^-> T<unit>
        ]
        |+> Static [

            Constructor (!? EditorViewConfig)

            "scrollIntoView" => (T<int> + SelectionRange) * !? ScrollIntoViewOptions ^-> StateEffect.[T<obj>]
            "styleModule" =? Facet.[T<obj>, !| T<obj>]
            "domEventHandlers" => DOMEventHandlers ^-> Extension
            "domEventObservers" => DOMEventHandlers ^-> Extension
            "inputHandler" =? Facet.[(EditorView * T<int> * T<int> * T<string> * (T<unit> ^-> Transaction) ^-> T<bool>), !| (EditorView * T<int> * T<int> * T<string> * (T<unit> ^-> Transaction) ^-> T<bool>)]
            "clipboardInputFilter" =? Facet.[clipboardFilterFunc, !| clipboardFilterFunc]
            "clipboardOutputFilter" =? Facet.[clipboardFilterFunc, !| clipboardFilterFunc]
            "scrollHandler" =? Facet.[scrollHandlerFunc, !| scrollHandlerFunc]
            "focusChangeEffect" =? Facet.[focusChangeEffectFunc, !| focusChangeEffectFunc]
            "perLineTextDirection" =? Facet.[T<bool>, T<bool>]
            "exceptionSink" =? Facet.[exceptionSinkFunc, !| exceptionSinkFunc]
            "updateListener" =? Facet.[updateListenerFunc, !| updateListenerFunc]
            "editable" =? Facet.[T<bool>, T<bool>]
            "mouseSelectionStyle" =? Facet.[mouseSelectionStyleFunc, !| mouseSelectionStyleFunc]
            "dragMovesSelection" =? Facet.[dragMovesSelectionFunc, !| dragMovesSelectionFunc]
            "clickAddsSelectionRange" =? Facet.[clickAddsSelectionRangeFunc, !| clickAddsSelectionRangeFunc]
            "decorationsFunc" =? Facet.[decorationsFunc, !| decorationsFunc]
            "outerDecorations" =? Facet.[decorationsFunc, !| decorationsFunc]
            "atomicRanges" =? Facet.[atomicRangesFunc, !| atomicRangesFunc]
            "bidiIsolatedRanges" =? Facet.[bidiIsolatedRangesFunc, !| bidiIsolatedRangesFunc]
            "scrollMargins" =? Facet.[scrollMarginsFunc, !| scrollMarginsFunc]
            "theme" => T<obj> * !? ThemeOptions ^-> Extension
            "darkTheme" =? Facet[T<bool>, T<bool>]
            "baseTheme" => T<obj> ^-> Extension
            "cspNonce" =? Facet.[T<string>, T<string>]
            "contentAttributes" =? Facet.[AttrSource, !| AttrSource]
            "editorAttributes" =? Facet.[AttrSource, !| AttrSource]
            "lineWrappingStatic" =? Extension
            "announce" =? StateEffectType.[T<string>]
            "findFromDOM" => T<HTMLElement> ^-> TSelf
        ]
        |> ignore