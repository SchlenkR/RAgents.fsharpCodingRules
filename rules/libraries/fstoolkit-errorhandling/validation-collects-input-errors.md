---
id: validation-collects-input-errors
category: libraries/fstoolkit-errorhandling
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["FsToolkit.ErrorHandling"]
sources:
  - "FsToolkit.ErrorHandling, validation computation expression and its gotchas: https://demystifyfp.gitbook.io/fstoolkit-errorhandling/fstoolkit.errorhandling/index/ce"
---

# Collect input errors with validation, fail fast in workflows

Use `validation { }` with `let!` and `and!` where all errors of independent fields should be reported together, typically when building a domain value from user input, and use `result` or `taskResult` with sequential `let!` everywhere else.

## Look for

Look for constructors checking independent input fields with sequential result binds, and validation expressions splitting independent checks across successive bind groups:

```fsharp
let tryCreateCustomer input =
    result {
        let! name = Name.tryCreate input.name
        let! email = Email.tryCreate input.email
        return { name = name; email = email }
    }
```

Also inspect workflow `validation`/`and!` groups that may run operations despite another step failing.

## Check the context

1. Determine the contract: should one response include all independent field errors, or must later workflow actions stop after failure?
2. Trace dependencies between checks. Independence means a check can meaningfully run without another check's successful output.
3. Check error types and grouping: `validation` accumulates within a `let! ... and! ...` group; later dependent binds cannot collect errors from checks that never run.
4. Inspect effects. Independent pure validation is different from starting concurrent workflow I/O.

## Report when

Report fail-fast construction when independent field errors must be returned together, or accumulating workflow steps that should be fail-fast. Explain the required error policy and the checks/actions it affects. The presence of several fields alone does not establish a requirement to collect errors.

## Suggest

Group independent field validation with the validation builder:

```fsharp
let tryCreateCustomer input =
    validation {
        let! name = Name.tryCreate input.name
        and! email = Email.tryCreate input.email
        return { name = name; email = email }
    }
```

This intentionally changes the error result from the first error to collected errors; update the consuming contract accordingly. Use sequential `result`/`taskResult` binds for workflows whose later actions require earlier success.

## Do not report

Leave a dependent workflow sequential:

```fsharp
let charge customerId =
    taskResult {
        let! customer = Customers.tryLoad customerId
        let! receipt = Payments.tryCharge customer.card
        return receipt
    }
```

Charging requires a successfully loaded customer and should not run after failure. Also leave dependent validation, such as a check requiring a successfully parsed value, after that successful bind.
