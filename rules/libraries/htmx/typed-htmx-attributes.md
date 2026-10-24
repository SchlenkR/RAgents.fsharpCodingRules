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

## Check

1. Look for attribute names starting with `hx-` written as strings, such as `attr "hx-post"` or `Attr.create "hx-target"`.
2. Is an element id written as a literal both where the element is rendered and in an `hx-target` selector?
3. Are trigger values free strings where the library offers typed values, such as `HxTrigger` in Giraffe.ViewEngine.Htmx?

## Report

The raw attribute or duplicated literal and the library helper or shared definition.

## Do not report

- Attributes of htmx extensions that the library does not cover, written once in a helper function.
- Helpers from another library that the project uses on purpose.

## Examples (illustrative, not exhaustive)

```fsharp
button [ attr "hx-post" "/cart/items"; attr "hx-target" "#cart" ] [ str "Add" ] // finding

// correction
let cartId = "cart"

button [ _hxPost Urls.cartItems; _hxTarget $"#{cartId}" ] [ str "Add" ]
div [ _id cartId ] [ Cart.view cart ]
```

No finding: `attr "hx-ext" "sse"` inside the one helper that renders server-sent event regions.
