---
id: recreate-tasks-for-retries
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Async programming in F#: https://learn.microsoft.com/en-us/dotnet/fsharp/tutorials/async"
---

# Start a fresh task for every retry

Pass a function that starts the task to a retry loop and call it for every attempt.

## Look for

Look for a retry branch or loop awaiting the same task instance that was obtained before the first attempt:

```fsharp
let retry (operation: System.Threading.Tasks.Task<Result<int, string>>) =
    task {
        match! operation with
        | Ok value -> return Ok value
        | Error _ -> return! operation
    }
```

Trace task values passed through parameters and aliases, not only local bindings.

## Check the context

1. Locate the call that starts the actual operation. Is that call inside each attempt, or is only its returned task reused?
2. Check that a proposed factory starts fresh work rather than returning a cached task.
3. Preserve attempt count, cancellation, backoff, and failure classification. Confirm that repeating the operation is allowed, particularly for writes with externally visible effects.

## Report when

Report when an intended retry reuses a completed task instead of invoking the operation again. Awaiting the same task returns its stored outcome; it cannot repeat the request. Identify both the task creation site and the supposed retry. Do not use this rule to introduce retries where none were intended.

## Suggest

Take a factory and call it for each attempt:

```fsharp
let retry (start: unit -> System.Threading.Tasks.Task<Result<int, string>>) =
    task {
        match! start () with
        | Ok value -> return Ok value
        | Error _ -> return! start ()
    }
```

This example retains two total attempts. Update the caller to pass the operation-producing function; adding `fun () -> existingTask` would retain the bug.

## Do not report

Leave reuse when consumers deliberately need one operation's result:

```fsharp
let shared = loadCustomer customerId
let first = summarizeCustomer shared
let second = describeCustomer shared
```

Sharing a task is appropriate when no second execution is intended. Also exclude cold `Async` computations that are deliberately run anew for each attempt; verify the helper actually reruns them.
