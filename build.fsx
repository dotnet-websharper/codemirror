#load "tools/includes.fsx"
open IntelliFactory.Build

let bt =
    BuildTool().PackageId("IntelliFactory.WebSharper.CodeMirror", "2.5")

let resourceFiles =
    [
        "CodeMirror/lib/codemirror.js"
        "CodeMirror/lib/codemirror.css"
        "CodeMirror/mode/meta.js"
        "CodeMirror/mode/apl/apl.js"
        "CodeMirror/mode/asterisk/asterisk.js"
        "CodeMirror/mode/clike/clike.js"
        "CodeMirror/mode/clojure/clojure.js"
        "CodeMirror/mode/cobol/cobol.js"
        "CodeMirror/mode/coffeescript/coffeescript.js"
        "CodeMirror/mode/commonlisp/commonlisp.js"
        "CodeMirror/mode/css/css.js"
        "CodeMirror/mode/d/d.js"
        "CodeMirror/mode/diff/diff.js"
        "CodeMirror/mode/ecl/ecl.js"
        "CodeMirror/mode/erlang/erlang.js"
        "CodeMirror/mode/gas/gas.js"
        "CodeMirror/mode/gfm/gfm.js"
        "CodeMirror/mode/go/go.js"
        "CodeMirror/mode/groovy/groovy.js"
        "CodeMirror/mode/haml/haml.js"
        "CodeMirror/mode/haskell/haskell.js"
        "CodeMirror/mode/haxe/haxe.js"
        "CodeMirror/mode/htmlembedded/htmlembedded.js"
        "CodeMirror/mode/htmlmixed/htmlmixed.js"
        "CodeMirror/mode/http/http.js"
        "CodeMirror/mode/javascript/javascript.js"
        "CodeMirror/mode/jinja2/jinja2.js"
        "CodeMirror/mode/less/less.js"
        "CodeMirror/mode/livescript/livescript.js"
        "CodeMirror/mode/lua/lua.js"
        "CodeMirror/mode/markdown/markdown.js"
        "CodeMirror/mode/mirc/mirc.js"
        "CodeMirror/mode/ntriples/ntriples.js"
        "CodeMirror/mode/ocaml/ocaml.js"
        "CodeMirror/mode/pascal/pascal.js"
        "CodeMirror/mode/perl/perl.js"
        "CodeMirror/mode/php/php.js"
        "CodeMirror/mode/pig/pig.js"
        "CodeMirror/mode/properties/properties.js"
        "CodeMirror/mode/python/python.js"
        "CodeMirror/mode/q/q.js"
        "CodeMirror/mode/r/r.js"
        "CodeMirror/mode/rpm/changes/changes.js"
        "CodeMirror/mode/rpm/spec/spec.js"
        "CodeMirror/mode/rst/rst.js"
        "CodeMirror/mode/ruby/ruby.js"
        "CodeMirror/mode/rust/rust.js"
        "CodeMirror/mode/sass/sass.js"
        "CodeMirror/mode/scheme/scheme.js"
        "CodeMirror/mode/shell/shell.js"
        "CodeMirror/mode/sieve/sieve.js"
        "CodeMirror/mode/smalltalk/smalltalk.js"
        "CodeMirror/mode/smarty/smarty.js"
        "CodeMirror/mode/sparql/sparql.js"
        "CodeMirror/mode/sql/sql.js"
        "CodeMirror/mode/stex/stex.js"
        "CodeMirror/mode/tcl/tcl.js"
        "CodeMirror/mode/tiddlywiki/tiddlywiki.js"
        "CodeMirror/mode/tiki/tiki.js"
        "CodeMirror/mode/turtle/turtle.js"
        "CodeMirror/mode/vb/vb.js"
        "CodeMirror/mode/vbscript/vbscript.js"
        "CodeMirror/mode/velocity/velocity.js"
        "CodeMirror/mode/verilog/verilog.js"
        "CodeMirror/mode/xml/xml.js"
        "CodeMirror/mode/xquery/xquery.js"
        "CodeMirror/mode/yaml/yaml.js"
        "CodeMirror/mode/z80/z80.js"
        "CodeMirror/addon/comment/comment.js"
        "CodeMirror/addon/dialog/dialog.css"
        "CodeMirror/addon/dialog/dialog.js"
        "CodeMirror/addon/display/placeholder.js"
        "CodeMirror/addon/edit/closebrackets.js"
        "CodeMirror/addon/edit/closetag.js"
        "CodeMirror/addon/edit/continuecomment.js"
        "CodeMirror/addon/edit/continuelist.js"
        "CodeMirror/addon/edit/matchbrackets.js"
        "CodeMirror/addon/edit/trailingspace.js"
        "CodeMirror/addon/fold/foldcode.js"
        "CodeMirror/addon/fold/foldgutter.js"
        "CodeMirror/addon/fold/brace-fold.js"
        "CodeMirror/addon/fold/indent-fold.js"
        "CodeMirror/addon/fold/xml-fold.js"
        "CodeMirror/addon/hint/html-hint.js"
        "CodeMirror/addon/hint/javascript-hint.js"
        "CodeMirror/addon/hint/pig-hint.js"
        "CodeMirror/addon/hint/python-hint.js"
        "CodeMirror/addon/hint/show-hint.css"
        "CodeMirror/addon/hint/show-hint.js"
        "CodeMirror/addon/hint/xml-hint.js"
        "CodeMirror/addon/lint/coffeescript-lint.js"
        "CodeMirror/addon/lint/javascript-lint.js"
        "CodeMirror/addon/lint/json-lint.js"
        "CodeMirror/addon/lint/lint.js"
        "CodeMirror/addon/lint/lint.css"
        "CodeMirror/addon/merge/merge.css"
        "CodeMirror/addon/merge/merge.js"
        "CodeMirror/addon/merge/dep/diff_match_patch.js"
        "CodeMirror/addon/mode_/loadmode.js"
        "CodeMirror/addon/mode_/multiplex.js"
        "CodeMirror/addon/mode_/overlay.js"
        "CodeMirror/addon/runmode/colorize.js"
        "CodeMirror/addon/runmode/runmode.js"
        "CodeMirror/addon/search/match-highlighter.js"
        "CodeMirror/addon/search/search.js"
        "CodeMirror/addon/search/searchcursor.js"
        "CodeMirror/addon/selection/active-line.js"
        "CodeMirror/addon/selection/mark-selection.js"
    ]

let main =
    bt.WebSharper.Extension("IntelliFactory.WebSharper.CodeMirror")
        .SourcesFromProject()
        .Embed(resourceFiles)

let website =
    bt.WebSharper.Library("Website")
        .SourcesFromProject()
        .References(fun r ->
            [
                r.Assembly("System.Web")
                r.Project main
            ])

let web =
    bt.WebSharper.HostWebsite("Web")
        .References(fun r ->
            [
                r.Project main
                r.Project website
                r.NuGet("WebSharper").At(["/tools/net45/IntelliFactory.Xml.dll"]).Reference()
            ])

bt.Solution [

    main
    website
    web

    bt.NuGet.CreatePackage()
        .Add(main)
        .Description("CodeMirror bindings for WebSharper")

]
|> bt.Dispatch
