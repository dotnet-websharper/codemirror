# WebSharper.CodeMirror

WebSharper bindings for the [CodeMirror 6](https://codemirror.net/6/) modular code editor.

---

## Bound NPM Packages

This project provides bindings for the following CodeMirror 6 modules:

### Core
- `codemirror` (core setup module)
- `@codemirror/state`
- `@codemirror/view`
- `@codemirror/language`
- `@codemirror/commands`
- `@codemirror/search`
- `@codemirror/autocomplete`
- `@codemirror/lint`
- `@codemirror/collab`

### Language Modes
- `@codemirror/lang-angular`
- `@codemirror/lang-cpp`
- `@codemirror/lang-css`
- `@codemirror/lang-go`
- `@codemirror/lang-html`
- `@codemirror/lang-java`
- `@codemirror/lang-javascript`
- `@codemirror/lang-jinja`
- `@codemirror/lang-json`
- `@codemirror/lang-less`
- `@codemirror/lang-lezer`
- `@codemirror/lang-liquid`
- `@codemirror/lang-markdown`
- `@codemirror/lang-php`
- `@codemirror/lang-python`
- `@codemirror/lang-rust`
- `@codemirror/lang-sass`
- `@codemirror/lang-sql`
- `@codemirror/lang-vue`
- `@codemirror/lang-wast`
- `@codemirror/lang-xml`
- `@codemirror/lang-yaml`
- `@codemirror/language-data`

### Themes
- `@codemirror/theme-one-dark`

---

## Usage Example

```fsharp
open WebSharper.CodeMirror

EditorView(
    EditorViewConfig(
        Doc = "const x = 1;",
        Parent = JS.Document.Body,
        Extensions = [|
            CodeMirror.BasicSetup // from core
            CodeMirror.Javascript(JavaScriptConfig(Typescript = true)) // from @codemirror/lang-javascript
            CodeMirror.OneDark // from @codemirror/theme-one-dark
        |]
    )
)
```