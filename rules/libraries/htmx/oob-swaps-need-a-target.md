---
id: oob-swaps-need-a-target
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "htmx, hx-swap-oob: https://htmx.org/attributes/hx-swap-oob/"
  - "htmx reference, events: https://htmx.org/reference/"
---

# Send out-of-band fragments only to targets that exist

Send an out-of-band fragment only when every page that can trigger the request contains an element with its target id, and report results the user must see only through such a fragment. htmx drops an out-of-band element without a matching target and only raises `htmx:oobErrorNoTarget`, so the message silently never appears.

## Check

1. For every `hx-swap-oob` fragment: which pages can send this request, and does each of them render the target id?
2. With a strategy other than `outerHTML` or `true`: does the code expect the wrapper element itself to arrive? htmx strips it and inserts only its children.

## Report

The fragment and the page without its target.

## Do not report

- Targets rendered by the shared layout of every page.

## Examples (illustrative, not exhaustive)

```fsharp
let removed (item: Item) = // finding: only the basket page renders #notices
    [
        Basket.row item
        div [ _id "notices"; _hxSwapOob "beforeend" ] [ str $"{item.name} removed" ]
    ]

// correction: every page renders the target in the shared layout
let page (content: XmlNode list) =
    html [] [ body [] (div [ _id "notices" ] [] :: content) ]
```

No finding: an out-of-band update of a header counter that the layout renders on every page.
