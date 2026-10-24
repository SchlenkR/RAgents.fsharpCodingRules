---
id: fragment-from-page-view
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "htmx documentation, history support and request headers: https://htmx.org/docs/"
  - "Giraffe.Htmx README, server side: https://github.com/bit-badger/Giraffe.Htmx"
---

# Render a fragment with the function that renders it in the page

Return a fragment only to htmx requests that are not history restores, render it with the same view function the full page uses for that region, and return the full page to every other request. A fragment built separately drifts from what a reload shows, and a fragment sent to a normal request renders without layout.

## Check

1. For every handler that returns a fragment: does it check the htmx request header and exclude history restore requests (`HX-History-Restore-Request`, in Giraffe.Htmx `IsHtmxRefresh`)?
2. Where several regions of one page are updated: does it check `HX-Target`?
3. Does the full page call the same view function for that region?

## Report

The handler or view and the shared view function with request-based selection.

## Do not report

- Endpoints whose URL never reaches the address bar, such as form actions without `hx-push-url`.

## Examples (illustrative, not exhaustive)

```fsharp
let cart: HttpHandler = fun next ctx -> htmlView (cartRows (loadCart ctx)) next ctx // finding: a reload shows bare rows

// correction
let cart: HttpHandler =
    fun next ctx ->
        let content = Cart.view (loadCart ctx)

        if ctx.Request.IsHtmx && not ctx.Request.IsHtmxRefresh then
            htmlView content next ctx
        else
            htmlView (Layout.page "Cart" [ content ]) next ctx
```

No finding: a `hx-post` action that returns one updated row.
