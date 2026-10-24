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

## Check

1. For every multiline record, list, array, or anonymous record: does the opening bracket stand alone on a line, or share a line with the first element?
2. Does the closing bracket stand on its own line, aligned with the binding?

## Report

The bracket placement and the Stroustrup form.

## Do not report

- Expressions that fit on one line.
- Files the formatter does not touch, such as generated code.

## Examples (illustrative, not exhaustive)

```fsharp
let customer = // finding: opening bracket alone on the next line
    {
        name = "Ada Lovelace"
        email = "ada@example.com"
        city = "London"
        isActive = true
    }

// correction
let customer = {
    name = "Ada Lovelace"
    email = "ada@example.com"
    city = "London"
    isActive = true
}
```

No finding: `let origin = { x = 0; y = 0 }`.
