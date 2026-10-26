---
id: await-owned-tasks
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Task expressions: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/task-expressions"
---

# Await every task whose work you own

Await or return every task whose completion belongs to the current operation, instead of discarding it with `ignore` or `let _ =`.

## Look for

Look for calls returning `Task` or `Async` whose values are ignored, bound to `_`, or started without an observable completion path:

```fsharp
let notify (send: unit -> System.Threading.Tasks.Task) = send () |> ignore
```

Inspect signatures: a similarly named synchronous API may perform all work before returning.

## Check the context

1. Determine whether completion and failure belong to this operation. Follow any handoff to a queue, supervisor, or caller.
2. Distinguish a running task from a cold `Async` value: ignoring the latter may mean the intended work never starts.
3. Check background/event contracts for an actual error observer and lifetime owner. A comment saying "fire and forget" does not establish either.

## Report when

Report discarded owned work when the caller should await its effect or observe its failure and no other owner does so. For a task, the operation can finish before the effect; for an unstarted `Async`, the effect may never run. Name the missing ownership/completion relationship.

## Suggest

Include completion in the returned operation:

```fsharp
let notify (send: unit -> System.Threading.Tasks.Task) = task { do! send () }
```

Alternatively return the task directly so the caller owns awaiting it. For background work, propose an explicit supervised handoff rather than adding another unobserved start. Update callers to observe the returned operation.

## Do not report

Leave an explicit return of the owned task:

```fsharp
let notify (send: unit -> System.Threading.Tasks.Task) = send ()
```

The caller receives completion and failure. Also exclude work handed to a supervisor that owns its lifetime, and void-returning callbacks only when the framework's contract provides the required supervision/error handling.
