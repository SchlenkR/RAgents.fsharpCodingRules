---
id: build-lists-without-repeated-append
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Lists: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/lists"
---

# Build lists without appending to a growing list

Build a list with a list expression, or by prepending and reversing, instead of appending to it in a loop or recursion.

## Look for

Look for a loop, recursive call, or fold that appends to the list accumulated so far:

```fsharp
let duplicate items =
    let mutable result = []

    for item in items do
        result <- result @ [ item; item ]

    result
```

Also look for `List.append accumulated next`, including through a named helper.

## Check the context

1. Trace which operand is the accumulated result and whether its length grows on each iteration.
2. Check the intended element order and whether computing the appended elements has effects.
3. Distinguish copying a growing prefix from one fixed-size append or prepending a small chunk.

## Report when

Report when repeated appends copy the growing left operand. Each append traverses that prefix, so building an input-sized result this way can take quadratic work. Cite the recurrence or loop update that makes the left side grow.

## Suggest

Build elements directly in their final order:

```fsharp
let duplicate items =
    [
        for item in items do
            yield item
            yield item
    ]
```

For recursive construction, prepend and reverse once when that preserves order and evaluation behavior. Keep effects in the same sequence as the original.

## Do not report

Leave a single append of a fixed header unchanged:

```fsharp
let lines = header @ rows
```

This does not repeatedly copy a growing result. Likewise, repeatedly prepending a fixed-size chunk is outside this pattern; assess its ordering separately.
