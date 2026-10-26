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

Write a sequence of steps that return `Result`, `Option`, or `Task<Result>` as `result { }`, `option { }`, or `taskResult { }` with `let!`, not as nested matches that pass every `Error` on by hand.

## Look for

Look for nested matches that return every error unchanged and continue the next step only from the success branch:

```fsharp
let register request =
    task {
        match Email.tryCreate request.email with
        | Error error -> return Error error
        | Ok email ->
            match! Users.tryInsert email with
            | Error error -> return Error error
            | Ok user -> return Ok user.id
    }
```

The corresponding `Some`/`None` staircase is another candidate.

## Check the context

1. Check each non-success branch for fallback, logging, transformation, or other meaningful behavior beyond propagation.
2. Resolve the result/option/task types and select an existing compatible builder; rule `choose-the-ce-by-result-type` governs the builder choice.
3. Align different error types at their individual steps with `mapError`, preserving error data and short-circuit order.

## Report when

Report a staircase whose branches only propagate failure/absence and whose successful path is a linear dependency chain. The builder can make that path explicit without repeating plumbing. Identify which branches do no work besides propagation; matching alone is not a violation.

## Suggest

Bind each dependent step in the compatible computation expression:

```fsharp
let register request =
    taskResult {
        let! email = Email.tryCreate request.email
        let! user = Users.tryInsert email
        return user.id
    }
```

Use `result` for synchronous result steps or `option` for absence propagation. Keep each operation's evaluation order and its error mapping visible at that step.

## Do not report

Leave a match that selects recovery behavior:

```fsharp
let load key =
    task {
        match! Cache.tryLoad key with
        | Ok value -> return value
        | Error _ -> return! loadFromSource key
    }
```

The failure branch performs a fallback instead of passing the error through. Also preserve distinct error handling or absence policies that a simple bind would erase.
