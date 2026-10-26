---
id: hide-implementation-details
category: scope
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
alternatives: [no-access-modifiers]
sources:
  - "Microsoft F# coding conventions, access control: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions"
  - "Microsoft, Signature files: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/signature-files"
---

# Hide what is not part of a module's purpose

Mark module-only helpers private and choose a library's public surface deliberately, using signature files when many internals need hiding.

## Look for

Look for public bindings that appear to serve only another operation in the same module. Inspect libraries whose public functions mirror every implementation detail:

```fsharp
module Keys =
    let normalize (key: string) = key.Trim().ToLowerInvariant()

    let lookup table key = table |> Map.tryFind (normalize key)
```

`normalize` is a candidate helper; its presence beside `lookup` does not prove that no external caller needs it.

## Check the context

Find callers across modules, tests, and signature files, and inspect the intended API. Unknown consumers of a published library require compatibility evidence, not just repository search results. Check whether tests deliberately call helpers directly. For a library with many internals, inspect whether accessibility or signatures already express its selected public surface.

## Report when

Report a helper exposed by default although its inspected purpose and consumers are module-local. For a library surface, identify concrete internal operations accidentally exposed rather than demanding a signature file solely because none exists. Deliberate visibility keeps callers from depending on details that need to change with the implementation.

## Suggest

Restrict a module-local helper while retaining its public consumer:

```fsharp
module Keys =
    let private normalize (key: string) = key.Trim().ToLowerInvariant()

    let lookup table key = table |> Map.tryFind (normalize key)
```

For a library with many internals, expose the intended operations in `.fsi` files and compile each signature before its implementation. Use `internal` when callers within the assembly need the operation. Check compatibility before narrowing a published API. This rule is the alternative to rule `no-access-modifiers`.

## Do not report

Keep intentionally consumed operations visible:

```fsharp
module Keys =
    let normalize (key: string) = key.Trim().ToLowerInvariant()

module Import =
    let normalizedKeys keys = keys |> List.map Keys.normalize
```

`normalize` serves another module. Also exclude scripts and helpers that the project's testing approach deliberately tests directly.
