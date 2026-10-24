---
id: keep-module-initialization-pure
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft F# coding conventions, use classes to contain values that have side effects: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions"
---

# Keep module initialization free of I/O

Perform I/O such as reading files, opening connections, or loading configuration in functions that startup code calls explicitly, not in module-level values. A failing module initializer surfaces as a `TypeInitializationException` at some later first access, and no caller can control when it runs or retry it.

## Check

1. Does a module-level `let` read files or environment, contact services, start work, or create connections?
2. Can the caller decide when it runs and how a failure is handled?

## Report

The module-level value and the startup function that replaces it.

## Do not report

- Pure constants and lookup tables built from literals.
- The entry point module of an application, which runs at startup anyway.

## Examples (illustrative, not exhaustive)

```fsharp
let settings = File.ReadAllText "settings.json" // finding: I/O when the module initializes

// correction
let loadSettings (path: string) = File.ReadAllText path
```

No finding: `let vatRates = Map.ofList [ "standard", 0.19m; "reduced", 0.07m ]`.
