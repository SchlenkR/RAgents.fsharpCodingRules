---
id: scope-owned-resources
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Resource management: the use keyword: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/resource-management-the-use-keyword"
---

# Keep a resource alive for the work that uses it

Dispose an owned resource with `use` or `use!`, and keep that scope around every sequence, task, or closure that reads the resource. Deferred work that leaves the `use` scope runs after disposal and reads a closed resource.

## Check

1. Does every newly created `IDisposable` or `IAsyncDisposable` have a `use` or an explicit owner?
2. Does a returned `seq`, closure, or task read the resource after the `use` scope that disposes it has ended?

## Report

The resource and the scope that must enclose its use.

## Do not report

- Resources borrowed from the caller or owned by a container.
- Ownership transferred on purpose to the caller, documented by the function's name or type.

## Examples (illustrative, not exhaustive)

```fsharp
let contents (path: string) =
    use reader = File.OpenText path
    seq { yield reader.ReadToEnd() } // finding: reader is disposed before enumeration

// correction
let contents (path: string) =
    seq {
        use reader = File.OpenText path
        yield reader.ReadToEnd()
    }
```

No finding: `File.ReadLines path`, which owns its reader for the whole enumeration.
