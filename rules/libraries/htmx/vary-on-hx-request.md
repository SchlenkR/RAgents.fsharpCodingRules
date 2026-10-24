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

## Check

1. For every handler that branches on htmx request headers: does every response of that URL name each of these headers in `Vary`?
2. Is `Vary` appended to rather than overwritten, so that values such as `Accept-Encoding` survive?

## Report

The handler, the missing header names, and where to append them.

## Do not report

- Responses sent with `Cache-Control: no-store`, and `Vary: *`.
- Endpoints that return the same markup for every request.

## Examples (illustrative, not exhaustive)

```fsharp
let cart: HttpHandler = // finding: fragment and page share one cache entry
    fun next ctx -> htmlView (cartContent ctx) next ctx

// correction
let varyOn (header: string) : HttpHandler =
    fun next ctx ->
        ctx.Response.Headers.Append("Vary", header)
        next ctx

let cart: HttpHandler =
    varyOn "HX-Request"
    >=> varyOn "HX-History-Restore-Request"
    >=> fun next ctx -> htmlView (cartContent ctx) next ctx
```

No finding: a `hx-post` endpoint that always returns the same fragment.
