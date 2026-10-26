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


## Look for

Look for a navigable URL that always returns a bare region, or full-page and fragment branches that build the same region with separate markup functions.

```fsharp
let cart: HttpHandler = fun next ctx -> htmlView (cartRows (loadCart ctx)) next ctx
```

## Check the context

1. Inspect links, history updates, routes, and client configuration. Can this URL be opened or restored as a full page?
2. Check the installed htmx version. In htmx 2, account for history-restore requests; in htmx 4, inspect `HX-Request-Type: full`. A generic htmx flag alone does not establish a fragment request.
3. Trace rendering of the region from both paths. If targets select different fragments at the same URL, inspect `HX-Target` handling too.

## Report when

Report a navigable request receiving only a fragment, a fragment request receiving the wrong region, or duplicated rendering of the same region that can drift. Name the relevant request and the missing selection/shared view. Do not infer the endpoint's navigation contract solely from it using `GET`.

## Suggest

Use one region view and select its enclosing page according to the actual request contract. This example uses the htmx 2/Giraffe.Htmx history helpers:

```fsharp
let cart: HttpHandler =
    fun next ctx ->
        let content = Cart.view (loadCart ctx)

        if ctx.Request.IsHtmx && not ctx.Request.IsHtmxRefresh then
            htmlView content next ctx
        else
            htmlView (Layout.page "Cart" [ content ]) next ctx
```

For htmx 4, use its full/partial request distinction instead. Apply rule `vary-on-hx-request` to the headers actually selecting the response.

## Do not report

Keep a dedicated non-navigable action returning its updated row:

```fsharp
let updateQuantity: HttpHandler =
    fun next ctx -> htmlView (Basket.row (saveQuantity ctx)) next ctx
```

This is permitted when the route is a form action whose contract is a row response and no history/navigation path requests a page there. Its function name alone is not proof of that contract.
