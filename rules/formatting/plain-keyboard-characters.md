---
id: plain-keyboard-characters
category: formatting
check: tool
files: ["**/*.{fs,fsx,fsi}"]
---

# Write punctuation you can type

Use plain keyboard punctuation in code, comments, and string literals: `-`, `->`, `...`, and straight quotes instead of typographic dashes, arrows, ellipses, and curly quotes. Typographic characters are a typical trace of generated text, break searches, and look identical to characters nobody can type.


## Look for

Use a plain-text scan for typographic dashes, arrows, ellipses, curly quotes, non-breaking spaces, and zero-width characters in changed source lines. This negative example intentionally contains an arrow:

```fsharp
let status = "draft → sent"
```

A suitable `rg` character-class search can find the punctuation ranges named in the rule; inspect invisible whitespace separately.

## Check the context

1. Identify the exact code point and whether it is punctuation, a natural-language letter, or intentional Unicode data.
2. Check whether a resource, user-facing typography requirement, or Unicode test needs that character.
3. Before replacing a string character, inspect consumers: a literal may encode a protocol value or test fixture rather than ordinary prose.

## Report when

This is a tool rule: a plain `rg` scan reports prohibited punctuation in ordinary project-owned source text, followed by checking the explicit exemptions. Semantic reviewers skip this mechanical character check. Report the character and its replacement; do not classify umlauts or other natural-language letters as forbidden typography.

## Suggest

Use the corresponding keyboard punctuation for ordinary status text:

```fsharp
let status = "draft -> sent"
```

Use `-`, `...`, and straight quotes for the analogous punctuation. Do not silently alter protocol values, preserved external text, or Unicode fixtures; establish whether the source text is subject to this preference first.

## Do not report

Keep natural-language letters and explicit Unicode test data:

```fsharp
let greeting = "Grüß Gott"
let unicodeArrowForEncodingTest = "\u2192"
```

The first string contains letters, and the second deliberately exercises a Unicode code point. Resource files and user-facing text that intentionally use typography are also exempt; the preference targets incidental source punctuation.
