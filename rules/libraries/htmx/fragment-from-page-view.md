---
id: fragment-from-page-view
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "htmx documentation, history support and request headers: https://htmx.org/docs/"
  - "htmx 4, HX-Request-Type header: https://four.htmx.org/reference/headers/HX-Request-Type/"
  - "Giraffe.Htmx README, server side: https://github.com/bit-badger/Giraffe.Htmx"
---

# Render a fragment with the function that renders it in the page

Return a fragment only to htmx requests for a part of the page, render it with the same view function the full page uses for that region, and return the full page to every other request. A fragment built separately drifts from what a reload shows, and a fragment sent to a full-page request renders without layout.

## Check

1. For every handler that returns a fragment: do normal requests get the full page, and so do htmx requests that ask for one, which are history restores in htmx 2 (`HX-History-Restore-Request`) and `HX-Request-Type: full` in htmx 4?
2. If one URL returns different fragments for different targets: does it tell them apart by `HX-Target`?
3. Does the full page call the same view function for that region?

## Report

The handler or view and the shared view function with request-based selection.

## Do not report

- Endpoints whose URL never reaches the address bar, such as form actions without `hx-push-url`.

## Examples (illustrative, not exhaustive)

```fsharp
let cart: HttpHandler = fun next ctx -> htmlView (cartRows (loadCart ctx)) next ctx // finding: a reload shows bare rows

// correction, htmx 2 with Giraffe.Htmx
let cart: HttpHandler =
    fun next ctx ->
        let content = Cart.view (loadCart ctx)

        if ctx.Request.IsHtmx && not ctx.Request.IsHtmxRefresh then
            htmlView content next ctx
        else
            htmlView (Layout.page "Cart" [ content ]) next ctx
```

No finding: a `hx-post` action that returns one updated row.
