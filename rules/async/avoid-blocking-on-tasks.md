---
id: avoid-blocking-on-tasks
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Asynchronous programming scenarios: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/async-scenarios"
---

# Await asynchronous work without blocking

Wait for a task with `let!` or `do!` instead of `.Result`, `.Wait()`, `GetAwaiter().GetResult()`, or `Async.RunSynchronously`.

## Look for

Look for `.Result`, `.Wait()`, `GetAwaiter().GetResult()`, or `Async.RunSynchronously` in code that could await asynchronous work:

```fsharp
let length (request: System.Threading.Tasks.Task<string>) = task { return request.Result.Length }
```

Resolve the receiver type: a field named `Result` is not necessarily a blocking task property.

## Check the context

1. Establish whether the task may still be incomplete at this point. Look for an earlier await of this task or a containing `WhenAll`.
2. Inspect callers and framework signatures to decide whether the asynchronous operation can propagate outward.
3. If a synchronous boundary is required, check that blocking is isolated there rather than repeated throughout the call chain.

## Report when

Report blocking on potentially incomplete work when the current operation can await or return it. Blocking ties up a thread and may deadlock when a continuation needs its context, or exhaust worker threads under load. Cite the incomplete operation and the asynchronous path available; do not assume every blocking wait necessarily deadlocks.

## Suggest

Await the result within the asynchronous operation:

```fsharp
let length (request: System.Threading.Tasks.Task<string>) =
    task {
        let! text = request
        return text.Length
    }
```

For synchronous callers, describe the necessary signature/call-chain changes. Preserve failure handling; awaiting may expose the original exception instead of a blocking API's aggregate wrapper.

## Do not report

Leave reading the result after proven successful completion:

```fsharp
let length (request: System.Threading.Tasks.Task<string>) =
    task {
        let! _ = request
        return request.Result.Length
    }
```

The read cannot block here. Also exclude the isolated entry point of a synchronous console program/script with no asynchronous caller, or a required synchronous framework boundary.
