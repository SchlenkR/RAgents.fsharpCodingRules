---
id: multiline-brackets-aligned
category: formatting
check: tool
files: ["**/*.{fs,fsx,fsi}"]
alternatives: [multiline-brackets-stroustrup]
sources:
  - "Fantomas configuration, fsharp_multiline_bracket_style: https://fsprojects.github.io/fantomas/docs/end-users/Configuration.html"
  - "Microsoft F# formatting guidelines, avoid formatting that is sensitive to name length: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/formatting"
---

# Put multiline brackets on lines of their own

Write a record, anonymous record, or list either on one line or with the opening bracket on its own line, the content indented one level, and the closing bracket on its own line. Fantomas 8 formats this by default as `fsharp_multiline_bracket_style = aligned`; unlike the older cramped style, no field aligns to the bracket, so renaming a field shifts no other line.


## Look for

Fantomas checks multiline records, anonymous records, lists, and arrays whose delimiters or elements use another layout. With the aligned alternative selected, a block opening on the binding line is a candidate:

```fsharp
let customer = {
    name = "Ada Lovelace"
    email = "ada@example.com"
    city = "London"
    isActive = true
}
```

## Check the context

1. Confirm this alternative is selected instead of rule `multiline-brackets-stroustrup`.
2. Check `.editorconfig` for `fsharp_multiline_bracket_style = aligned` and run the project's Fantomas version.
3. Distinguish one-line expressions and copy-and-update syntax from ordinary multiline blocks. Check generated-file exclusions before applying formatting.

## Report when

This is a tool rule: Fantomas reports layout that differs from the selected aligned output. Semantic reviewers skip bracket-placement findings. The preference avoids field alignment dependent on name length; the formatter decides whether the expression fits on one line and how its continuation is indented.

## Suggest

Use the configured formatter to put the opening and closing delimiters on their own lines:

```fsharp
let customer =
    {
        name = "Ada Lovelace"
        email = "ada@example.com"
        city = "London"
        isActive = true
    }
```

For copy-and-update, keep `{ customer with` together and let Fantomas arrange the updates and closing delimiter. Restrict unrelated layout churn according to rule `formatter-owns-layout`.

## Do not report

Keep expressions the formatter leaves on one line:

```fsharp
let origin = { x = 0; y = 0 }
```

The multiline preference does not require expanding small values. Generated or excluded files are also exempt; a project selecting the opposing alternative must configure that alternative instead of applying both rules.
