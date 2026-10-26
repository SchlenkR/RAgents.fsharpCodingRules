---
id: task-for-dotnet-io
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Async programming in F#: https://learn.microsoft.com/en-us/dotnet/fsharp/tutorials/async"
  - "Microsoft, Task expressions: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/task-expressions"
---

# Use task for I/O in .NET applications

Write asynchronous application code that calls .NET APIs as `task { }`, pass a `CancellationToken` explicitly where work must be cancellable, and use `async { }` only where its cold start or implicit cancellation is needed.

## Look for

Look for application `async` expressions that mostly convert task-returning .NET calls through `Async.AwaitTask`. Also inspect cancellable task workflows for omitted or dropped tokens:

```fsharp
let loadProfile (client: System.Net.Http.HttpClient) userId cancellation =
    async {
        let! json =
            client.GetStringAsync($"/users/{userId}", cancellation) |> Async.AwaitTask

        return parseProfile json
    }
```

## Check the context

1. Inspect callers for reliance on `Async` being cold, rerunnable, implicitly cancellable, or part of an intentional F#-facing API.
2. Check whether a task-returning contract fits the application boundary without changing required scheduling or cancellation behavior.
3. For cancellation findings, trace the operation's token through each cancellable API. Establish the actual cancellation requirement rather than adding a parameter by habit.

## Report when

Report task interop wrappers when no `Async` semantics are needed and the application uses task-based .NET APIs. Separately report a missing token where owned work is required to stop on cancellation. Name the token source and the call that drops it. Do not replace a cold retry computation with one reusable task.

## Suggest

Use the task form with the explicit cancellation contract:

```fsharp
let loadProfile (client: System.Net.Http.HttpClient) userId cancellation =
    task {
        let! json = client.GetStringAsync($"/users/{userId}", cancellation)
        return parseProfile json
    }
```

Adapt callers from the cold `Async` contract to task creation at the intended start point. If only token propagation is missing, fix that independently instead of changing the asynchronous abstraction too.

## Do not report

Leave intentionally rerunnable cold work:

```fsharp
let loadProfileAgain (client: System.Net.Http.HttpClient) userId =
    async {
        let! json = client.GetStringAsync($"/users/{userId}") |> Async.AwaitTask
        return parseProfile json
    }
```

When callers deliberately run this same computation for each retry, converting it to a stored task changes behavior. Also exclude deliberate `Async` library APIs and workflows requiring its implicit cancellation semantics.
