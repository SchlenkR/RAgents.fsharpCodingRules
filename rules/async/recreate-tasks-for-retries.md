---
id: recreate-tasks-for-retries
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Async programming in F#: https://learn.microsoft.com/en-us/dotnet/fsharp/tutorials/async"
---

# Start a fresh task for every retry

Pass a function that starts the task to a retry loop and call it for every attempt. A task runs once from the moment it is created, so awaiting the same instance again only repeats its old result.

## Check

1. Does a retry branch await a `Task` instance that was created before the first attempt?
2. Is the operation started inside a function that the loop calls for every attempt?

## Report

The reused task and the factory call that starts a new attempt.

## Do not report

- Several consumers awaiting the result of one operation on purpose.
- `Async` computations, which start again every time they run.

## Examples (illustrative, not exhaustive)

```fsharp
let retry (operation: Task<Result<int, string>>) =
    task {
        match! operation with
        | Ok value -> return Ok value
        | Error _ -> return! operation // finding: no new attempt
    }

// correction
let retry (start: unit -> Task<Result<int, string>>) =
    task {
        match! start () with
        | Ok value -> return Ok value
        | Error _ -> return! start ()
    }
```

No finding: two consumers awaiting one shared request.
