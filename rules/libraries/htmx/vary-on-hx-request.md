---
id: vary-on-hx-request
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "htmx documentation, caching: https://htmx.org/docs/#caching"
  - "RFC 9110, Vary: https://www.rfc-editor.org/rfc/rfc9110.html#field.vary"
---

# Name every header that selects the response in Vary

Append every request header that decides between fragment and full page, such as `HX-Request`, `HX-History-Restore-Request`, `HX-Target`, or `HX-Request-Type`, to the response's `Vary` header. Without it, browsers and proxies cache one variant under the URL and serve a bare fragment for a page load, or a full page into a fragment target.


## Look for

Look for a URL selecting different markup from request headers without corresponding `Vary` response values, or code replacing an existing `Vary` field.

```fsharp
let cart: HttpHandler =
    fun next ctx ->
        let content = Cart.view (loadCart ctx)

        if ctx.Request.IsHtmx then
            htmlView content next ctx
        else
            htmlView (Layout.page "Cart" [ content ]) next ctx
```

## Check the context

1. Trace every header used by the selection logic, including helpers: `HX-Request`, history restore, `HX-Target`, or version-specific `HX-Request-Type` as applicable.
2. Inspect response middleware and final headers on every variant. Another layer may already append the needed values.
3. Check the actual cache policy. `no-cache` permits storage with revalidation; it is not equivalent to `no-store`.

## Report when

Report a cacheable variant missing a selecting header from `Vary`, or a write that discards existing variation such as `Accept-Encoding`. Name the differing requests and missing header. A cache may otherwise reuse the wrong representation at the same URL.

## Suggest

Append each selecting header before either branch, preserving existing values. For the example's single selector:

```fsharp
let cart: HttpHandler =
    fun next ctx ->
        ctx.Response.Headers.Append("Vary", "HX-Request")
        let content = Cart.view (loadCart ctx)

        if ctx.Request.IsHtmx then
            htmlView content next ctx
        else
            htmlView (Layout.page "Cart" [ content ]) next ctx
```

Append the history/target/request-type headers too when they influence selection. Correct full-page history selection separately under rule `fragment-from-page-view`; `Vary` alone does not fix the chosen markup.

## Do not report

Keep one response shape that does not vary with request headers:

```fsharp
let cartRows: HttpHandler =
    fun next ctx -> htmlView (Cart.rows (loadCart ctx)) next ctx
```

No header-driven representation choice needs a cache key here. Responses consistently marked `Cache-Control: no-store` or `Vary: *`, and headers already provided by middleware, are also exempt from a missing-header finding.
