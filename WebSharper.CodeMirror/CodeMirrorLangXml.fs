namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module LangXml = 
    let AttrSpec =
        Pattern.Config "AttrSpec" {
            Required = [
                "name", T<string>
            ]
            Optional = [
                "values", !| (T<string> + AutoComplete.Completion)
                "global", T<bool>
                "completion", AutoComplete.Completion.Type
            ]
        }

    let ElementSpec =
        Pattern.Config "ElementSpec" {
            Required = [
                "name", T<string>
            ]
            Optional = [
                "children", !| T<string>
                "textContent", !| T<string>
                "top", T<bool>
                "attributes", !| (T<string> + AttrSpec) 
                "completion", AutoComplete.Completion.Type
            ]
        }    

    let XMLConfig =
        Pattern.Config "XMLConfig" {
            Required = []
            Optional = [
                "elements", !| ElementSpec
                "attributes", !| AttrSpec
                "autoCloseTags", T<bool>
            ]
        }