---
id: as-vs-to-conversions
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Use as for views and to for transformations

Name a conversion `asX` only when it changes the view of the same data, such as a cast or a wrapper without copying, and `toX` when it computes new data in another format, unit, or domain. The prefix tells the caller whether work happens and whether the result still shares the original.


## Look for

Look for conversions named `as...` that format, parse, copy, compute, or classify, and `to...` functions that merely expose another view of the same object.

```fsharp
let asIsoDate (date: System.DateOnly) = date.ToString "yyyy-MM-dd"
```

## Check the context

1. Read the implementation, including called helpers. Does the result share the original data, or is a new representation computed?
2. Inspect the call site for the target meaning. A vague technical suffix may still hide which format or domain is returned.
3. Check whether the name is owned by this code or fixed by an external API, interface, or established `ofX` constructor convention.

## Report when

Report an owned conversion name whose prefix contradicts its operation or whose target remains ambiguous. In the example, formatting computes text rather than exposing a view of `DateOnly`; the `as` prefix misstates that relationship. Explain the actual operation, rather than deciding from the return type alone.

## Suggest

Use the prefix matching the operation and a specific target name, updating references and signatures:

```fsharp
let toIsoDate (date: System.DateOnly) = date.ToString "yyyy-MM-dd"
```

For a view or upcast, use `asX`; for a computed representation, use `toX`. Renaming a published API needs the project's compatibility policy, rather than an unannounced breaking change.

## Do not report

Keep a view conversion that exposes the same collection through a narrower interface:

```fsharp
let asReadOnly (items: ResizeArray<'item>) =
    items :> System.Collections.Generic.IReadOnlyList<'item>
```

No copy or value transformation happens here. Keep external names such as `List.toSeq` and .NET members, and established module constructors such as `Money.ofCents`; this rule governs names the project can choose.
