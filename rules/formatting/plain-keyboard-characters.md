---
id: plain-keyboard-characters
category: formatting
check: tool
files: ["**/*.{fs,fsx,fsi}"]
---

# Write punctuation you can type

Use plain keyboard punctuation in code, comments, and string literals: `-`, `->`, `...`, and straight quotes instead of typographic dashes, arrows, ellipses, and curly quotes. Typographic characters are a typical trace of generated text, break searches, and look identical to characters nobody can type.

## Check

1. Search the changed lines, for example with `rg "[\x{2013}\x{2014}\x{2190}-\x{21FF}\x{2026}\x{2018}\x{2019}\x{201C}\x{201D}]"`.
2. Look for non-breaking spaces and zero-width characters in identifiers and strings.

## Report

The character and its keyboard replacement.

## Do not report

- Letters of natural languages, such as é, ü, or ß.
- Resource files and user-facing texts that deliberately use typography.
- Test data about Unicode handling.

## Examples (illustrative, not exhaustive)

```fsharp
// Retries the request — then gives up // finding: em dash
// Retries the request - then gives up // correction

let arrow = "draft → sent" // finding
let arrow = "draft -> sent" // correction
```

No finding: `let greeting = "Grüß Gott"`.
