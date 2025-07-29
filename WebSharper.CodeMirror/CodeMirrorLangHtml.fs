namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module LangHtml =
    let TagSpec =
        Pattern.Config "TagSpec" {
            Required = []
            Optional = [
                "attrs", T<obj>
                "globalAttrs", T<bool>
                "children", !| T<string>
            ]
        }
        |> Import "TagSpec" "@codemirror/lang-html"

    let NestedLang =
        Pattern.Config "NestedLang" {
            Required = [
                "tag", T<string>
                "parser", T<obj>
            ]
            Optional = [
                "attrs", T<obj> ^-> T<bool>
            ]
        }

    let NestedAttr =
        Pattern.Config "NestedAttr" {
            Required = [
                "name", T<string>
                "parser", T<obj>
            ]
            Optional = [
                "tagName", T<string>
            ]
        }

    let HtmlCompletionSourceConfig =
        Pattern.Config "HtmlCompletionSourceConfig" {
            Required = []
            Optional = [
                "extraTags", T<obj>
                "extraGlobalAttributes", T<obj>
            ]
        }

    let HtmlConfig =
        Pattern.Config "HtmlConfig" {
            Required = []
            Optional = [
                "matchClosingTags", T<bool>
                "selfClosingTags", T<bool>
                "autoCloseTags", T<bool>
                "extraTags", T<obj>
                "extraGlobalAttributes", T<obj>
                "nestedLanguages", !| NestedLang
                "nestedAttributes", !| NestedAttr
            ]
        }