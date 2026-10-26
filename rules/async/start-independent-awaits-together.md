---
id: start-independent-awaits-together
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Task expressions: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/task-expressions"
---

# Start independent awaits together

Bind independent asynchronous calls with `and!`, which current FSharp.Core supports in `task` and FsToolkit in `taskResult`, or start their tasks before the first `let!`, instead of awaiting them one after the other.

## Look for

Look for consecutive awaits where the second operation is only started after the first finishes, although its inputs do not use the first result:

```fsharp
task {
    let! customer = loadCustomer customerId
    let! orders = loadOrders customerId
    return summarize customer orders
}
```

If both tasks were created earlier, sequentially awaiting their handles may already overlap the work.

## Check the context

1. Find actual start points, data dependencies, and dependencies through side effects.
2. Check connection/context sharing, ordering requirements, concurrency limits, and whether the second call is permitted when the first fails.
3. Verify the builder/compiler supports the proposed `and!`, including its error and cancellation behavior. `Task<Result>` concurrency may change which effects run and which error is returned.

## Report when

Report serialized start of independent operations when concurrent execution is allowed and the added waiting time serves no required ordering or failure policy. Lack of a direct variable reference alone is insufficient evidence. State why both starts are safe and identify any intended change from not starting the second operation after a first-operation failure.

## Suggest

With a compatible task builder, start the independent operations together:

```fsharp
task {
    let! customer = loadCustomer customerId
    and! orders = loadOrders customerId
    return summarize customer orders
}
```

Alternatively create both tasks before awaiting either, while ensuring every owned task's completion/failure remains observed. Preserve an explicit failure policy for the concurrent work.

## Do not report

Leave awaits where the second call depends on the first result:

```fsharp
task {
    let! order = loadOrder orderId
    let! customer = loadCustomer order.customerId
    return summarizeOrder customer order
}
```

Also exclude calls sharing a connection or context that forbids concurrency, required effect ordering, and already-started tasks whose work overlaps.
