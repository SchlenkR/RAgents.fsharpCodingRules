---
id: preserve-cancellation
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Let cancellation remain cancellation

Let `OperationCanceledException` propagate instead of turning it into a business error, a fallback value, or a retry. Callers cancel to stop work, so treating cancellation as a failure keeps shutdown or an abandoned request running.


## Look for

Look for catch-all handlers, fallback paths, and retry loops around cancellable operations that translate every exception into an ordinary outcome.

```fsharp
let load (read: unit -> System.Threading.Tasks.Task<Result<string, string>>) =
    task {
        try
            return! read ()
        with error ->
            return Error error.Message
    }
```

## Check the context

1. Follow the awaited calls and identify whether they can raise `OperationCanceledException` or its subclass `TaskCanceledException`.
2. Trace that exception through wrappers and retries. Does it escape before the general translation or retry branch?
3. If a handler intentionally consumes cancellation, identify who owns the token and whether that handler ends the owned operation.

## Report when

Report when cancellation becomes a business error, a fallback value, or another attempt, so callers cannot observe that work was cancelled. Identify the intercepting handler and the action that continues work or changes its outcome. An exception message containing "cancelled" is not a substitute for cancellation semantics.

## Suggest

Narrow the catch to the failure the caller actually handles:

```fsharp
let load (read: unit -> System.Threading.Tasks.Task<Result<string, string>>) =
    task {
        try
            return! read ()
        with :? System.IO.IOException as error ->
            return Error error.Message
    }
```

This deliberately stops translating unrelated exceptions too. If the failure policy needs a broader catch, exclude cancellation explicitly and preserve its exception when propagating it (rule `reraise-keeps-stack`).

## Do not report

Keep the owner of a worker ending normally for its own shutdown token:

```fsharp
let runWorker (shutdown: System.Threading.CancellationToken) =
    task {
        try
            do! work shutdown
        with :? System.OperationCanceledException when shutdown.IsCancellationRequested ->
            ()
    }
```

This boundary ends the owned worker. The same handler inside a reusable child operation would need to propagate cancellation to its caller instead.
