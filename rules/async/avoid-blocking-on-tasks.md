---
id: avoid-blocking-on-tasks
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Asynchronous programming scenarios: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios"
---

# Await asynchronous work without blocking

Wait for a task with `let!` or `do!` instead of `.Result`, `.Wait()`, `GetAwaiter().GetResult()`, or `Async.RunSynchronously`. A blocked thread cannot run the continuation it waits for, which leads to deadlocks and thread-pool starvation under load.

## Check

1. Look for `.Result`, `.Wait()`, `GetAwaiter().GetResult()`, and `Async.RunSynchronously` outside the program's entry point.
2. Is a synchronous boundary really unavoidable, and is the wait isolated there?

## Report

The blocking wait and its `let!` or `do!` form.

## Do not report

- The entry point of a console program or script with no asynchronous caller.
- Reading `.Result` of a task that has already completed, for example after `Task.WhenAll`.

## Examples (illustrative, not exhaustive)

```fsharp
let length (request: Task<string>) =
    task {
        return request.Result.Length // finding: blocks while the request runs
    }

// correction
let length (request: Task<string>) =
    task {
        let! text = request
        return text.Length
    }
```

No finding: `main` calling `(run args).GetAwaiter().GetResult()` once.
