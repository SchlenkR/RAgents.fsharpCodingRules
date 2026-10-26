---
id: keep-synchronous-code-synchronous
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Keep synchronous code synchronous

Return a plain value from a function that awaits nothing, and wrap it into a task only where a signature requires one, with `Task.FromResult` or inside an existing `task`.

## Look for

Look for `task { return ... }` or `async { return ... }` containing no awaits or delegated asynchronous work:

```fsharp
let total (lines: InvoiceLine list) =
    task { return lines |> List.sumBy _.amount }
```

Check the whole expression for `let!`, `do!`, `match!`, and `return!`, including nested helpers.

## Check the context

1. Resolve calls in the body: no await does not prove purity, and blocking I/O remains synchronous I/O.
2. Inspect interfaces, delegates, published APIs, test doubles, and callers for a required asynchronous signature.
3. For `Async`, check whether callers rely on delayed execution or rerunning the computation. For both forms, check exception timing and handlers before proposing synchronous evaluation.

## Report when

Report an unnecessary asynchronous wrapper when the operation is synchronous and its callers do not require the wrapper's contract, timing, or failure channel. It advertises an asynchronous boundary and forces callers to await a plain calculation. Do not claim a precise allocation penalty without evidence from the compiler/runtime in use.

## Suggest

Expose the calculation synchronously:

```fsharp
let total (lines: InvoiceLine list) = lines |> List.sumBy _.amount
```

Update internal callers and keep evaluation under the intended failure handler. Where an adapter must return a task, wrap the value with `Task.FromResult` or use it inside an existing `task` at that boundary. This is a signature change, not merely deletion of braces.

## Do not report

Leave a synchronous result adapted to a required asynchronous dependency in a test:

```fsharp
let loadCustomer customerId =
    System.Threading.Tasks.Task.FromResult(Map.tryFind customerId customers)
```

When the consuming contract requires a task, this shape is appropriate. Also exclude framework/interface implementations and intentional cold `Async` computations.
