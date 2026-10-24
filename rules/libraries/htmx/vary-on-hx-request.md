---
id: vary-on-hx-request
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "htmx documentation, caching: https://htmx.org/docs/#caching"
---

# Send Vary: HX-Request when a URL serves fragments and pages

Add `Vary: HX-Request` to every response of a URL that returns a fragment or a full page depending on the htmx request headers. Without it, browsers and proxies cache one variant under the URL and serve a bare fragment for a page load, or a full page into a fragment target.

## Check

1. For every handler that branches on `HX-Request` or `HX-Target`: does each of its responses send `Vary` naming that header?
2. Is the header set in one place, such as a shared handler or middleware, rather than in some branches only?

## Report

The handler and where to add the header.

## Do not report

- Responses sent with `Cache-Control: no-store`.
- Endpoints that return the same markup for every request.

## Examples (illustrative, not exhaustive)

```fsharp
let cart: HttpHandler = // finding: fragment and page share one cache entry
    fun next ctx -> htmlView (cartContent ctx) next ctx

// correction
let cart: HttpHandler =
    setHttpHeader "Vary" "HX-Request"
    >=> fun next ctx -> htmlView (cartContent ctx) next ctx
```

No finding: a `hx-post` endpoint that always returns the same fragment.
