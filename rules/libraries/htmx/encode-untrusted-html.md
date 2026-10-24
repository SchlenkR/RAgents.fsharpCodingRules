---
id: encode-untrusted-html
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "htmx documentation, security: https://htmx.org/docs/#security"
---

# Encode untrusted text in views

Render user input and other external text with the view engine's encoded text node, such as `str` in Giraffe.ViewEngine, and keep raw HTML helpers such as `rawText` for trusted or sanitized markup. Raw input becomes executable HTML, and with htmx even an injected attribute such as `hx-get` sends requests.

## Check

1. Can user-controlled or external text reach a raw HTML helper, directly or through a wrapper function?
2. Is that text encoded, or cleaned by a dedicated HTML sanitizer, before it is inserted?

## Report

The raw insertion and the encoded text helper of the view engine.

## Do not report

- Constant markup written in the code base.
- HTML that passed a reviewed sanitizer.

## Examples (illustrative, not exhaustive)

```fsharp
let greeting (name: string) = div [] [ rawText name ] // finding: input becomes markup

// correction
let greeting (name: string) = div [] [ str name ]
```

No finding: `rawText "<br>"`, a constant written in the code.
