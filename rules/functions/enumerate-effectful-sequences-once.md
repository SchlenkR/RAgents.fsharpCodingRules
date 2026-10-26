---
id: enumerate-effectful-sequences-once
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Sequences: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/sequences"
---

# Enumerate effectful sequences once

Materialize a sequence that reads files, queries a database, or has other effects into a list or array when more than one consumer needs its elements.

## Look for

Look for one lazy sequence consumed by several terminal operations when its source reads files, queries storage, uses randomness, or observes mutable state:

```fsharp
let summarize fileName =
    let amounts = System.IO.File.ReadLines fileName |> Seq.map decimal
    { count = Seq.length amounts; amount = Seq.sum amounts }
```

## Check the context

1. Resolve the source and transformations: does enumeration perform work again, or is this already a materialized collection?
2. Follow all consumers and check whether they must observe the same elements.
3. Establish that materialization is finite and reasonably bounded. Check resource lifetime and whether extra eager evaluation changes failure timing or streaming behavior.

## Report when

Report repeated enumeration when consumers require one consistent set of effectful results. In the example, counting and summing open/read the file separately and can see different contents. State the source and the consumers, rather than flagging every repeated `Seq` call.

## Suggest

Materialize once before sharing the results:

```fsharp
let summarize fileName =
    let amounts = System.IO.File.ReadLines fileName |> Seq.map decimal |> Seq.toArray
    { count = amounts.Length; amount = Array.sum amounts }
```

If materialization would be unbounded, suggest computing the required results in one pass. Make the change from streaming to eager work explicit.

## Do not report

Leave multiple traversals over an existing in-memory snapshot:

```fsharp
let summarize (amounts: decimal list) =
    { count = amounts.Length; amount = List.sum amounts }
```

These consumers read the same stored elements. Also exclude deliberately fresh reads and streams with only one enumeration.
