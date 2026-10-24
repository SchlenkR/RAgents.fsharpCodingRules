---
id: name-by-result
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Name functions after what they return

Name a function after its result or its effect and a value after its content, so that the call site tells what comes back and from which side. Names of activities such as `process`, `handle`, or `applyDefaults` send the reader into the implementation.

## Check

1. For every new function: does its name state the result (`toInvoiceLines`, `unpaidInvoices`) or the effect (`sendReminder`), or only an activity (`process`, `handle`, `manage`, `doWork`)?
2. For every name of a difference or a set (missing, unknown, extra, invalid): does it say relative to what?
3. Does every boolean read as a question (`isOverdue`, `hasLines`, `canShip`)?

## Report

The name and a name that states the result, the side, or the question.

## Do not report

- Names a framework requires, such as `main` or `Configure`.
- Short names in small lambdas and generic code (`fun x -> ...`, `'a`, `f`).

## Examples (illustrative, not exhaustive)

```fsharp
let applyDefaults rows = ... // finding: says what happens, not what comes out
let toSendableRows rows = ... // correction

let missingColumns = ... // finding: missing in the file or in the schema?
let columnsMissingInFile = ... // correction
```

No finding: `InvoiceNumber.parse`, where the module name already names the result.
