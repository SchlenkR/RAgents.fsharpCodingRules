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

## Check

1. For every multiline record, list, array, or anonymous record: does any element share a line with an opening or closing bracket?
2. For copy-and-update expressions: does `{ value with` stay together and the closing bracket stand on its own line?

## Report

The bracket placement and the block form.

## Do not report

- Expressions that fit on one line.
- Files the formatter does not touch, such as generated code.

## Examples (illustrative, not exhaustive)

```fsharp
let customer = { name = "Ada Lovelace" // finding: fields aligned to the bracket
                 email = "ada@example.com"
                 city = "London"
                 isActive = true }

// correction
let customer =
    {
        name = "Ada Lovelace"
        email = "ada@example.com"
        city = "London"
        isActive = true
    }

let renamed =
    { customer with
        name = "Ada King-Noel"
        email = "ada.king-noel@example.com"
        city = "Ockham"
    }
```

No finding: `let origin = { x = 0; y = 0 }`.
