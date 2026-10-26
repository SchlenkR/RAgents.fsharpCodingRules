---
id: views-take-intent-parameters
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
---

# Give view functions parameters that state the intent

Give a view function parameters that say what the caller wants, such as a small union for how it is updated, instead of lists of raw attributes or nodes that it splices into its markup. Raw attributes let every caller change the component in its own way, so the component can no longer guarantee its structure.


## Look for

Look for domain view functions accepting arbitrary attribute/node lists only so callers can alter behavior or structure, especially lists used to request an out-of-band variant.

```fsharp
let pager (extraAttributes: XmlAttribute list) (pageNumber: int) =
    div (_id "pager" :: extraAttributes) [ str (string pageNumber) ]

let update = pager [ _hxSwapOob "true" ] pageNumber
```

## Check the context

1. Inspect the view and all callers. What choices do the raw lists actually represent?
2. Distinguish a domain component owning its structure from a generic element wrapper or a layout intentionally accepting child content.
3. Check whether callers can override invariant ids, targets, or behavior, and whether a small intent parameter expresses the real supported choices.

## Report when

Report a raw parameter used as an unbounded switch for a component's finite domain behavior. Identify the caller intent and structure it can currently override. Attribute-list parameters alone are not a violation; the component's responsibility decides whether pass-through is appropriate.

## Suggest

Express the supported behavior explicitly and let the component choose its attributes:

```fsharp
[<RequireQualifiedAccess>]
type PagerUpdate =
    | InPlace
    | OutOfBand

let pager update (pageNumber: int) =
    let attributes =
        match update with
        | PagerUpdate.InPlace -> [ _id "pager" ]
        | PagerUpdate.OutOfBand -> [ _id "pager"; _hxSwapOob "true" ]

    div attributes [ str (string pageNumber) ]
```

Use a simpler explicit parameter when only one meaningful choice exists; do not invent a general component-configuration framework.

## Do not report

Keep a thin wrapper whose purpose is to forward caller-owned attributes:

```fsharp
let icon attributes name = span attributes [ str name ]
```

This wrapper does not promise a domain component's fixed behavior or structure. Layout functions accepting page content as nodes are also valid; their content parameter is the composition contract rather than a hidden behavior switch.
