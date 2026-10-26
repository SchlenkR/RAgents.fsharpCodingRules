---
id: name-by-result
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Name functions after what they return

Name a function after its result or its effect and a value after its content, so that the call site tells what comes back and from which side. Names of activities such as `process`, `handle`, or `applyDefaults` send the reader into the implementation.


## Look for

Look for vague activity names such as `process`, `handle`, or `applyDefaults`, directional set names without a named side, and booleans that do not read as a question.

```fsharp
let applyDefaults rows = rows |> List.map Row.withDefaults

let missingColumns fileColumns schemaColumns =
    Set.difference schemaColumns fileColumns
```

## Check the context

1. Read the function and a representative call site. What value comes back, or which effect occurs?
2. For differences and missing/extra/unknown values, establish the direction of comparison from the actual operands.
3. Check whether the containing module or type already supplies the subject, and whether a framework fixes the name.

## Report when

Report an owned name that leaves the result, effect, comparison side, or boolean question unclear at its call site. Explain the concrete ambiguity. Do not rename every verb: an effect such as `sendReminder` already states what the call does, and a module can supply context that need not be repeated.

## Suggest

Name the result and the side established by the implementation:

```fsharp
let rowsWithDefaults rows = rows |> List.map Row.withDefaults

let columnsMissingInFile fileColumns schemaColumns =
    Set.difference schemaColumns fileColumns
```

Use a question such as `isOverdue`, `hasLines`, or `canShip` for booleans. Update callers and signature files. Avoid promising a stronger result, such as "validated", unless the function actually establishes it.

## Do not report

Keep a concise name whose module supplies the result's subject:

```fsharp
module InvoiceNumber =
    let parse text = System.Int32.TryParse text
```

Callers already read `InvoiceNumber.parse`. Framework-mandated names such as `main` or `Configure`, and short variables in small lambdas or generic combinators, are also permitted when their local role is clear.
