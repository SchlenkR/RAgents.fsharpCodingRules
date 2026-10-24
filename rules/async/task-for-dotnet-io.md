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

Write asynchronous application code that calls .NET APIs as `task { }`, pass a `CancellationToken` explicitly where work must be cancellable, and use `async { }` only where its cold start or implicit cancellation is needed. `task` needs no conversion at the many APIs that return `Task` and adds less overhead.

## Check

1. For every new `async { }`: does it mostly await `Task`-returning APIs through `Async.AwaitTask`?
2. For every new `task { }` that must be cancellable: does it receive a token and pass it to each call?
3. Does code rely on `async` being cold, for example by running the same computation twice?

## Report

The expression and its `task` form, or the missing token.

## Do not report

- Libraries that expose `Async` deliberately to F# callers.
- `async` used for retries or other re-running of the same computation.

## Examples (illustrative, not exhaustive)

```fsharp
let loadProfile (client: HttpClient) (userId: int) = // finding: only awaits Task APIs
    async {
        let! json = client.GetStringAsync $"/users/{userId}" |> Async.AwaitTask
        return parseProfile json
    }

// correction
let loadProfile (client: HttpClient) (userId: int) (cancellation: CancellationToken) =
    task {
        let! json = client.GetStringAsync($"/users/{userId}", cancellation)
        return parseProfile json
    }
```

No finding: `async` in a retry helper that runs the same computation again.
