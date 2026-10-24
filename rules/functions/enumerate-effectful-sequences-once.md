---
id: enumerate-effectful-sequences-once
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Sequences: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/sequences"
---

# Enumerate effectful sequences once

Materialize a sequence that reads files, queries a database, or has other effects into a list or array when more than one consumer needs its elements. A `seq` runs its work again on every enumeration and may return different elements each time.

## Check

1. Is a sequence backed by I/O, randomness, or mutable state enumerated more than once, for example by `Seq.length` and `Seq.sum`?
2. Must the consumers see the same elements, and is the sequence finite and small enough to materialize?

## Report

The repeated enumeration and the materialized value the consumers share.

## Do not report

- Pure sequences whose repeated cost is acceptable.
- Deliberate fresh reads, and streaming code that consumes the sequence once.

## Examples (illustrative, not exhaustive)

```fsharp
let summarize (path: string) =
    let amounts = File.ReadLines path |> Seq.map decimal
    { count = Seq.length amounts; amount = Seq.sum amounts } // finding: reads the file twice

// correction
let summarize (path: string) =
    let amounts = File.ReadLines path |> Seq.map decimal |> Seq.toArray
    { count = amounts.Length; amount = Array.sum amounts }
```

No finding: two traversals of a list that is already in memory.
