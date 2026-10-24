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

Mark helpers that only serve their own module `private`, and give a library with many internals a signature file that states its public surface. Every public function is an API that someone may start to use, which turns each later change of it into a breaking one.

## Check

1. For every new public function or value: does code outside its module use it, or is it meant to?
2. For every library project: is the public surface chosen on purpose, through `private`, `internal`, or `.fsi` files?

## Report

The helper and its `private` or signature-file form.

## Do not report

- Functions that tests call directly, in projects that test them on purpose.
- Scripts.

## Examples (illustrative, not exhaustive)

```fsharp
let normalize (key: string) = key.Trim().ToLowerInvariant() // finding: only lookup uses it

let lookup table key = table |> Map.tryFind (normalize key)

// correction
let private normalize (key: string) = key.Trim().ToLowerInvariant()
```

No finding: a public `parse` function that other modules call.
