---
id: build-lists-without-repeated-append
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Lists: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/lists"
---

# Build lists without appending to a growing list

Build a list with a list expression, or by prepending and reversing, instead of appending to it in a loop or recursion. `@` copies its whole left operand, so appending to a growing list takes quadratic time.

## Check

1. Does a loop, recursion, or fold repeatedly compute `accumulated @ items` or `List.append accumulated items`?
2. Does the left operand grow with the input?

## Report

The growing append and a linear construction that keeps the order.

## Do not report

- A single append, or appends whose left operand has a fixed size.

## Examples (illustrative, not exhaustive)

```fsharp
let duplicate (items: int list) =
    let mutable result = []

    for item in items do
        result <- result @ [ item; item ] // finding: copies result on every iteration

    result

// correction
let duplicate (items: int list) =
    [
        for item in items do
            yield item
            yield item
    ]
```

No finding: `header @ rows`, appending rows to a fixed two-line header once.
