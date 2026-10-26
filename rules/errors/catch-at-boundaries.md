---
id: catch-at-boundaries
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Catch exceptions only at boundaries

Write `try`/`with` only directly around a call that throws for an expected reason, such as parsing, a file, an SDK, or legacy code, and classify the exception right there; never wrap your own core logic or swallow exceptions. A handler far from its cause cannot know what failed, and a broad handler hides bugs.


## Look for

Look for `try`/`with` around several stages of application logic, especially catch-all branches returning success, defaults, or unclassified errors.

```fsharp
let import fileName =
    try
        System.IO.File.ReadAllText fileName |> parseRows |> saveRows
    with _ ->
        ()
```

## Check the context

1. List the operations protected by the handler and which exceptions each can raise.
2. Inspect the caller: does it need a decision for one expected boundary failure, or should an exception reach the operation's outer failure policy?
3. Check whether parsing bugs, storage failures, and cancellation are all being treated like the one anticipated failure.

## Report when

Report when a handler catches core logic failures it cannot classify, swallows failures, or translates unrelated exceptions into one expected outcome. Explain which failures it confuses or hides. A handler being broad is acceptable at the application's actual failure boundary, where the policy is to log and terminate that operation.

## Suggest

If unreadable input is a caller-handled outcome, classify only that boundary call and let the workflow handle the result:

```fsharp
let tryReadText fileName =
    try
        Ok(System.IO.File.ReadAllText fileName)
    with :? System.IO.IOException as error ->
        Error(CannotRead error.Message)
```

Keep parsing and saving outside this handler. If nobody handles unreadable input separately, remove the inner catch and let the outer policy handle the exception instead (rule `result-for-decisions`).

## Do not report

Keep an actual process boundary that logs a failure and returns a failure exit code:

```fsharp
let main args =
    try
        run args
        0
    with error ->
        logFailure error
        1
```

This handler owns the application's final failure policy. `use` and `try`/`finally` for cleanup also remain valid; they do not classify failures as successful results.
