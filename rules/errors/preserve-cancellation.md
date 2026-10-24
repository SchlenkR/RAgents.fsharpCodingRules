---
id: preserve-cancellation
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Let cancellation remain cancellation

Let `OperationCanceledException` propagate instead of turning it into a business error, a fallback value, or a retry. Callers cancel to stop work, so treating cancellation as a failure keeps shutdown or an abandoned request running.

## Check

1. Can a catch-all handler (`with ex ->`, `with _ ->`) intercept `OperationCanceledException` or `TaskCanceledException`?
2. Does the handler let cancellation through before it translates or retries ordinary failures?

## Report

The handler that swallows cancellation and its narrower form.

## Do not report

- The owner of an operation that ends normally after cancelling its own token.

## Examples (illustrative, not exhaustive)

```fsharp
let load (read: unit -> Task<Result<string, string>>) =
    task {
        try
            return! read ()
        with ex ->
            return Error ex.Message // finding: also catches cancellation
    }

// correction
let load (read: unit -> Task<Result<string, string>>) =
    task {
        try
            return! read ()
        with :? IOException as ex ->
            return Error ex.Message
    }
```

No finding: a background worker whose outer loop ends quietly when its own shutdown token is cancelled.
