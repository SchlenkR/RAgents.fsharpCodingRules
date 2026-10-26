---
id: keep-module-initialization-pure
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft F# coding conventions, use classes to contain values that have side effects: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/conventions"
---

# Keep module initialization free of I/O

Perform I/O in functions that startup code calls explicitly rather than in module-level values.

## Look for

Look for module-level bindings that read files or environment variables, contact services, open connections, or start work. Follow helper calls that may hide these effects:

```fsharp
module Settings =
    let text = System.IO.File.ReadAllText "settings.json"
```

A function-shaped helper can still run during initialization when a module-level value immediately calls it.

## Check the context

Determine when the binding is evaluated and who controls its failure and lifetime. Is the module the application's actual startup entry point, or a reusable module initialized when first accessed? Trace wrappers, eager task creation, and connection constructors. Pure allocation and lookup tables built from literals do not become I/O merely because they run at module initialization.

## Report when

Report effectful initialization in a reusable module when callers cannot choose when to run it or handle failure at the call. Identify the effect and the initializer that triggers it. Initialization failures can surface through type initialization rather than the intended startup path; opening resources there also hides their ownership.

## Suggest

Make the effect explicit and call it from startup:

```fsharp
module Settings =
    let load fileName = System.IO.File.ReadAllText fileName

[<EntryPoint>]
let main _ =
    let settings = Settings.load "settings.json"
    System.Console.WriteLine settings
    0
```

Preserve the intended number of reads. If settings should be loaded once, pass the resulting value to consumers rather than rereading it on every access. Put resource disposal and startup failure handling with the caller that owns them.

## Do not report

Keep pure module data:

```fsharp
module Tax =
    let rates = Map.ofList [ "standard", 0.19m; "reduced", 0.07m ]
```

This creates a deterministic value without external dependencies. I/O performed deliberately by the application's startup entry point is also permitted; moving unrelated helpers into that module does not make their initialization harmless.
