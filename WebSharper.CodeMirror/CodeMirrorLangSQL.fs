namespace WebSharper.CodeMirror.Definition

open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module LangSQL = 

    let ImportFromLangSQL (c: CodeModel.Class) = 
        Import c.Name "@codemirror/lang-sql" c

    let SQLDialectSpec =
        Pattern.Config "SQLDialectSpec" {
            Required = []
            Optional = [
                "keywords", T<string>
                "builtin", T<string>
                "types", T<string>
                "backslashEscapes", T<bool>
                "hashComments", T<bool>
                "slashComments", T<bool>
                "spaceAfterDashes", T<bool>
                "doubleDollarQuotedStrings", T<bool>
                "doubleQuotedStrings", T<bool>
                "charSetCasts", T<bool>
                "plsqlQuotingMechanism", T<bool>
                "operatorChars", T<string>
                "specialVar", T<string>
                "identifierQuotes", T<string>
                "caseInsensitiveIdentifiers", T<bool>
                "unquotedBitLiterals", T<bool>
                "treatBitsAsBytes", T<bool>
            ]
        }
    
    let SQLDialect =
        Class "SQLDialect"
        |> ImportFromLangSQL
        |+> Instance [
            "language" =? Language.LRLanguage
            "spec" =? SQLDialectSpec
            "extension" =? State.Extension
            "configureLanguage" => T<obj> * !? T<string> ^-> TSelf
        ]
        |+> Static [
            Constructor (T<unit>)
            "define" => SQLDialectSpec ^-> TSelf
        ]

    let SQLNamespace = 
        Pattern.Config "SQLNamespace" {
            Required = []
            Optional = [
                "self", AutoComplete.Completion.Type
                "children", TSelf
            ]
        }
        |> ImportFromLangSQL

    let SQLNamespaceType = T<obj> + SQLNamespace + !| (AutoComplete.Completion + T<string>)

    let SQLConfig =
        Pattern.Config "SQLConfig" {
            Required = []
            Optional = [
                "dialect", SQLDialect.Type
                "schema", SQLNamespaceType
                "tables", !| AutoComplete.Completion.Type
                "schemas", !| AutoComplete.Completion.Type
                "defaultTable", T<string>
                "defaultSchema", T<string>
                "upperCaseKeywords", T<bool>
                "keywordCompletion", (T<string> * T<string> ^-> AutoComplete.Completion.Type)
            ]
        }    