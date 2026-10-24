---
id: views-take-intent-parameters
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
---

# Give view functions parameters that state the intent

Give a view function parameters that say what the caller wants, such as a small union for how it is updated, instead of lists of raw attributes or nodes that it splices into its markup. Raw attributes let every caller change the component in its own way, so the component can no longer guarantee its structure.

## Check

1. Does a new view function take `XmlAttribute list`, `XmlNode list`, or attribute strings only to pass them into its markup?
2. Do callers use these lists to switch behavior, such as adding an out-of-band swap?

## Report

The raw parameter and the intent parameter that replaces it.

## Do not report

- Thin wrappers around one element whose job is to pass attributes through, such as an icon helper.
- Layout functions that take their content as nodes.

## Examples (illustrative, not exhaustive)

```fsharp
let pager (extraAttributes: XmlAttribute list) (page: Page) = ... // finding: callers splice in anything

// correction
[<RequireQualifiedAccess>]
type PagerUpdate =
    | InPlace
    | OutOfBand

let pager (update: PagerUpdate) (page: Page) = ...
```

No finding: `Layout.page (title: string) (content: XmlNode list)`.
