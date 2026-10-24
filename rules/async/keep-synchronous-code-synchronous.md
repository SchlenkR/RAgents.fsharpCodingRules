---
id: keep-synchronous-code-synchronous
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Keep synchronous code synchronous

Return a plain value from a function that awaits nothing, and wrap it into a task only where a signature requires one, with `Task.FromResult` or inside an existing `task`. A `task` without any await allocates a state machine, hides that the function is pure, and makes every caller await what it could compute directly.

## Check

1. Look for `task { return ... }` and `async { return ... }` that contain no `let!`, `do!`, `match!`, or `return!`.
2. Does an interface or framework really require a task here?

## Report

The expression and its synchronous form.

## Do not report

- Implementations of an interface or framework delegate that requires a task.
- Test doubles that stand in for asynchronous dependencies.

## Examples (illustrative, not exhaustive)

```fsharp
let total (lines: InvoiceLine list) = // finding: nothing is awaited
    task { return lines |> List.sumBy _.amount }

// correction
let total (lines: InvoiceLine list) = lines |> List.sumBy _.amount
```

No finding: a fake `loadCustomer` in a test that returns `Task.FromResult customer`.
