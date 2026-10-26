---
id: scope-owned-resources
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Resource management: the use keyword: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword"
---

# Keep a resource alive for the work that uses it

Dispose owned resources and keep their ownership scope around all work that reads them.

## Look for

Look for created disposable resources without disposal or an explicit owner, and for resources referenced by escaping sequences, callbacks, closures, or tasks:

```fsharp
let contents fileName =
    use reader = System.IO.File.OpenText fileName
    seq { yield reader.ReadToEnd() }

let readNow fileName =
    let reader = System.IO.File.OpenText fileName
    reader.ReadToEnd()
```

The first candidate disposes too early; the second has no visible disposal.

## Check the context

Identify ownership, disposal boundaries, and when resource-dependent operations execute. Follow returned work through helpers and consumers. Check eager consumption before scope exit and ownership inside deferred computations. A task return type alone does not establish when its reads complete. For a borrowed resource or ownership transfer, verify that the responsible owner keeps it alive for the whole operation; borrowed status alone proves no lifetime guarantee.

## Report when

Report an owned resource with no disposal path, or work that can read it after disposal. Name the missing owner or both the disposal boundary and later operation. A returned sequence that captures a reader disposed on function return will fail when enumerated.

## Suggest

For lazy reads, let each enumeration own its reader; for immediate reads, dispose after completing the read:

```fsharp
let contents fileName =
    seq {
        use reader = System.IO.File.OpenText fileName
        yield reader.ReadToEnd()
    }

let readNow fileName =
    use reader = System.IO.File.OpenText fileName
    reader.ReadToEnd()
```

The lazy version moves acquisition and I/O to enumeration and opens a reader each time. If callers need one eager snapshot, materialize inside the original scope instead. Use `use!` for asynchronously acquired resources where supported, and await owned work before its resource scope ends.

## Do not report

Leave resource-dependent work consumed before disposal:

```fsharp
let contents fileName =
    use reader = System.IO.File.OpenText fileName
    seq { yield reader.ReadToEnd() } |> Seq.toList
```

The returned list contains text, not a deferred read. External ownership and deliberate transfers are permitted when the verified owner covers the entire use and remains responsible for disposal.
