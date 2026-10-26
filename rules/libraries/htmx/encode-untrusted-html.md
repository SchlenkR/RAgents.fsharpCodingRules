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


## Look for

Look for user input, stored external content, or third-party text reaching raw HTML helpers, including through view wrappers. In Giraffe.ViewEngine, a raw node is a candidate:

```fsharp
let greeting (name: string) = div [] [ rawText name ]
```

## Check the context

1. Trace the value's origin; text loaded from storage can still be untrusted user input.
2. Resolve the helper to establish whether it encodes text or inserts markup unchanged.
3. For intentional rich HTML, inspect the sanitizer and allowed tags/attributes. The policy must address active content, including htmx attributes, rather than merely remove script tags.

## Report when

Report external text reaching a raw insertion without suitable encoding or a reviewed HTML sanitizer. Identify the source-to-sink path. The view can interpret text as elements and attributes instead of displaying it as data; htmx adds behavior to injected attributes too.

## Suggest

Use the view engine's encoded text helper for ordinary text:

```fsharp
let greeting (name: string) = div [] [ str name ]
```

For a rich-text contract, retain only markup allowed by the reviewed sanitizer before raw insertion. Do not use a sanitizer merely to avoid the encoded helper for a plain name or message, and do not assume HTML-text encoding makes values safe for every attribute or URL context.

## Do not report

Keep trusted constant markup with no external interpolation:

```fsharp
let separator = rawText "<br>"
```

The markup is controlled by the code base. Sanitized rich HTML is also permitted when the actual sanitizer and its policy are established. A wrapper name such as `safeHtml` is insufficient by itself; inspect what it guarantees.
