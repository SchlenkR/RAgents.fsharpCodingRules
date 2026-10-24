---
id: result-expressions-over-match-staircases
category: libraries/fstoolkit-errorhandling
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["FsToolkit.ErrorHandling"]
sources:
  - "FsToolkit.ErrorHandling, taskResult computation expression: https://demystifyfp.gitbook.io/fstoolkit-errorhandling/fstoolkit.errorhandling/taskresult/ce"
---

# Bind results with a computation expression, not a match staircase

Write a sequence of steps that return `Result`, `Option`, or `Task<Result>` as `result { }`, `option { }`, or `taskResult { }` with `let!`, not as nested matches that pass every `Error` on by hand. The successful path reads top to bottom, and each error leaves at its step.

## Check

1. Look for `| Error e -> Error e` or `| Error e -> return Error e` followed by a nested match on the next step.
2. Are differing error types aligned with `mapError` at the step, not by wrapping the whole expression?

## Report

The staircase and its computation expression form.

## Do not report

- A match that does something other than pass the error on, such as a fallback.
- Which expression fits which return type; rule `choose-the-ce-by-result-type` covers that.

## Examples (illustrative, not exhaustive)

```fsharp
let register request =
    task {
        match Email.tryCreate request.email with // finding: staircase
        | Error error -> return Error error
        | Ok email ->
            match! Users.tryInsert email with
            | Error error -> return Error error
            | Ok user -> return Ok user.id
    }

// correction
let register request =
    taskResult {
        let! email = Email.tryCreate request.email
        let! user = Users.tryInsert email
        return user.id
    }
```

No finding: `match! Cache.tryLoad key with | Ok value -> ... | Error _ -> loadFromSource key`.
