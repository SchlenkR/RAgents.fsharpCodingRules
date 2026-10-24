---
id: start-independent-awaits-together
category: async
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Microsoft, Task expressions: https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/task-expressions"
---

# Start independent awaits together

Bind independent asynchronous calls with `and!`, which current FSharp.Core supports in `task` and FsToolkit in `taskResult`, or start their tasks before the first `let!`, instead of awaiting them one after the other. Sequential awaits of calls that do not depend on each other add up their waiting times for nothing.

## Check

1. Look for consecutive `let!` where the second call uses nothing from the first.
2. Can the calls run at the same time, or do they share a connection or context that forbids it?

## Report

The sequential awaits and their `and!` form.

## Do not report

- Calls on one connection or context that does not support concurrent use, such as an EF Core `DbContext`.
- Calls whose order matters for their side effects.

## Examples (illustrative, not exhaustive)

```fsharp
task {
    let! customer = loadCustomer customerId
    let! orders = loadOrders customerId // finding: waits for customer without using it
    return summarize customer orders
}

// correction
task {
    let! customer = loadCustomer customerId
    and! orders = loadOrders customerId
    return summarize customer orders
}
```

No finding: `let! order = loadOrder orderId` followed by `let! customer = loadCustomer order.customerId`.
