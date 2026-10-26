---
id: multiline-brackets-stroustrup
category: formatting
check: tool
files: ["**/*.{fs,fsx,fsi}"]
alternatives: [multiline-brackets-aligned]
sources:
  - "Fantomas configuration, fsharp_multiline_bracket_style: https://fsprojects.github.io/fantomas/docs/end-users/Configuration.html"
---

# Open multiline brackets at the end of the line

Write a record, anonymous record, or list either on one line or with the opening bracket at the end of the binding line, the content indented one level, and the closing bracket on its own line. Fantomas enforces this with `fsharp_multiline_bracket_style = stroustrup`; it saves a line and an indentation level per block and reads like the brace style of most C-family languages.


## Look for

Fantomas checks multiline records, anonymous records, lists, and arrays against the selected delimiter style. With Stroustrup selected, an opening delimiter on its own line is a candidate:

```fsharp
let customer =
    {
        name = "Ada Lovelace"
        email = "ada@example.com"
        city = "London"
        isActive = true
    }
```

## Check the context

1. Confirm this alternative is selected instead of rule `multiline-brackets-aligned`.
2. Check `.editorconfig` for `fsharp_multiline_bracket_style = stroustrup` and use the project's Fantomas version.
3. Check whether the expression fits on one line or belongs to an excluded/generated file. Do not infer the selected style from one old block alone.

## Report when

This is a tool rule: have Fantomas report deviations from the configured Stroustrup output. Semantic reviewers skip delimiter-style findings. This is a layout preference rather than a correctness defect; its configured opening delimiter shares the binding line, with the contents indented and the closing delimiter on its own line.

## Suggest

Run Fantomas with the selected style:

```fsharp
let customer = {
    name = "Ada Lovelace"
    email = "ada@example.com"
    city = "London"
    isActive = true
}
```

Let the formatter handle lists, arrays, anonymous records, and continuation indentation consistently. Keep unrelated formatting outside a functional diff according to rule `formatter-owns-layout`.

## Do not report

Keep an expression that fits on one line:

```fsharp
let origin = { x = 0; y = 0 }
```

There is no multiline delimiter decision here. Generated/excluded files are also exempt. A project selecting aligned brackets uses that alternative alone; the catalogue offering both is not an instruction to alternate styles within a file.
