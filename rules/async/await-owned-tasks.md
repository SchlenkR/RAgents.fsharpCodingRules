---
id: await-owned-tasks
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Task expressions: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/task-expressions"
---

# Await every task whose work you own

Await or return every task whose completion belongs to the current operation, instead of discarding it with `ignore` or `let _ =`. A discarded task lets the caller finish before its effects happen, and its exception is never observed.

## Check

1. Is a call that returns `Task` or `Async` discarded, piped to `ignore`, or started without anyone awaiting it?
2. If the work is meant to run in the background: does a supervisor own it and observe its completion and failure?

## Report

The discarded task and its awaited or returned form.

## Do not report

- Work handed explicitly to a supervisor, such as a hosted background queue.
- Event callbacks whose framework requires a void return and handles errors.

## Examples (illustrative, not exhaustive)

```fsharp
let notify (send: unit -> Task) = send () |> ignore // finding: the caller cannot observe completion

// correction
let notify (send: unit -> Task) = task { do! send () }
```

No finding: `let notify (send: unit -> Task) = send ()`, which hands the task to the caller.
