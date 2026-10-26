---
id: typed-htmx-attributes
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "Giraffe.Htmx README, view engine: https://github.com/bit-badger/Giraffe.Htmx"
  - "Oxpecker.Htmx README: https://github.com/Lanayx/Oxpecker/blob/develop/src/Oxpecker.Htmx/README.md"
  - "Falco.Htmx README: https://github.com/falcoframework/Falco.Htmx"
---

# Use the htmx helpers of your view library

Write htmx attributes with the helpers of the installed library, such as `_hxGet` in Giraffe.ViewEngine.Htmx, `.hxGet(...)` in Oxpecker.Htmx, or `Hx.get` in Falco.Htmx, and take URLs and target ids from one definition. A raw `"hx-get"` string compiles with any typo, and an id written twice drifts apart.


## Look for

Look for string-built `hx-` attribute names, repeated URL/target-id literals, and free-string values where the installed helper library already offers named values.

```fsharp
button [ attr "hx-post" "/cart/items"; attr "hx-target" "#cart" ] [ str "Add" ]
div [ _id "cart" ] [ Cart.view cart ]
```

## Check the context

1. Identify the installed view library and version. Check which helpers and named trigger/swap values it actually supplies.
2. Resolve wrappers before treating an attribute as raw: a project helper may already centralize its spelling.
3. Follow target ids and routes to their rendered elements and endpoint definitions. Distinguish duplicated definitions from deliberate references to shared values.

## Report when

Report an available library helper bypassed with a raw attribute name, a named library value replaced by an incidental string, or independently duplicated target/route definitions. Explain the drift or misspelling the helper/shared definition prevents. A typed attribute name does not guarantee its selector or URL is correct.

## Suggest

Use the installed library's helpers and one definition for the shared target and route. In Giraffe.ViewEngine.Htmx:

```fsharp
let cartId = "cart"

button [ _hxPost Urls.cartItems; _hxTarget $"#{cartId}" ] [ str "Add" ]
div [ _id cartId ] [ Cart.view cart ]
```

Use that library's named trigger/swap values where provided. Oxpecker and Falco have their own APIs; do not import a second view framework just to copy this syntax.

## Do not report

Keep a centralized attribute helper for a custom extension the library does not cover:

```fsharp
let chartRange days = attr "hx-chart-range" (string days)
```

There is no installed helper to use for that extension attribute. Existing deliberate helpers from another installed library are also valid when they provide the same contract; inspect their implementation instead of deciding from a naming convention.
