---
id: comments-say-why
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Comment only what the code cannot say

Write a comment only for what names and types cannot express, such as a constraint of an external system or the reason for a surprising decision, in one or two plain lines. Comments that narrate the code go stale, and they are the most visible trace of generated code.

## Check

1. For every added comment: does it repeat the name, the type, or the next lines?
2. Is it longer than two lines, a section header inside a function, commented-out code, or a TODO without a ticket?
3. Could a better name, a type, or a test carry the information instead?

## Report

The comment and its fix: delete it, or move its information into a name, a type, or a test.

## Do not report

- `///` documentation on the public API of a library.
- Tool directives such as `#nowarn` or `// fsharplint:disable`.
- Comments in lines the change does not touch.

## Examples (illustrative, not exhaustive)

```fsharp
// Get all unpaid invoices // finding: repeats the code
let unpaid = invoices |> List.filter (fun invoice -> invoice.paidAt.IsNone)

// The payment provider rejects amounts with more than two decimals.
let amount = Math.Round(total, 2)
```

No finding: the comment on `Math.Round` above; the constraint is invisible in the code.
