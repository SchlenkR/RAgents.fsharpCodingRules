---
id: nest-single-use-bindings
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [top-level-helpers]
sources:
  - "FSharpLint FL0091 FavourNestedFunctions: https://github.com/fsprojects/FSharpLint/blob/master/docs/content/how-tos/rules/FL0091.md"
---

# Nest single-use bindings in their consumer

Declare a helper used by only one function inside that function, after its dependencies and before its users.

## Look for

Look for module-level functions or values, including `let private`, that appear to have one consumer. Also inspect helpers buried in deep subexpressions:

```fsharp
let private normalize (key: string) = key.Trim().ToLowerInvariant()

let lookup table key = table |> Map.tryFind (normalize key)
```

The candidate includes the consumer; a search result in one diff is not the full reference count.

## Check the context

Find references across the project, tests, and other files. Check whether the binding is an intended library API. For values, determine whether moving them would repeat expensive work or change effect timing, identity, or sharing. For nested helpers, identify the local dependencies and the earliest place in the consumer's main body where those dependencies are available.

## Report when

Report a module-level binding whose sole intended consumer can own it without changing evaluation or reuse requirements. Also report a nested helper placed deeper than necessary when lifting it within that consumer preserves dependencies. A small scope shows where the helper matters; `private` alone does not express that relationship.

## Suggest

Place the helper near the top of its consumer's body:

```fsharp
let lookup table key =
    let normalize (key: string) = key.Trim().ToLowerInvariant()

    table |> Map.tryFind (normalize key)
```

Move it after any locals it needs and before its first use. If a second consumer appears, lift the shared helper to module level. This rule is the alternative to rule `top-level-helpers`.

## Do not report

Keep a helper shared by distinct operations at module level:

```fsharp
let normalize (key: string) = key.Trim().ToLowerInvariant()

let lookup table key = table |> Map.tryFind (normalize key)
let remove table key = table |> Map.remove (normalize key)
```

There are two consumers. Also keep intentionally public library operations, deliberately test-accessible helpers, and values such as compiled regular expressions or lookup tables that must be constructed once rather than on every call.
