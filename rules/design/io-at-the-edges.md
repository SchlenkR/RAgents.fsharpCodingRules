---
id: io-at-the-edges
category: design
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Mark Seemann, Impureim sandwich: https://blog.ploeh.dk/2020/03/02/impureim-sandwich/"
  - "Mark Seemann, Dependency rejection: https://blog.ploeh.dk/2017/02/02/dependency-rejection/"
---

# Keep I/O at the edges of a workflow

Structure a workflow as read, decide, write: load data, compute the decision in a pure function, then perform the effects that decision describes.

## Look for

Look for domain decisions mixed with database, HTTP, file, clock, randomness, or identifier generation. Follow callbacks passed into domain logic when they could supply a value instead:

```fsharp
let renewIfExpired (db: Db) id =
    task {
        let! subscription = db.load id

        if subscription.endsAt < System.DateTime.UtcNow then
            do! db.save { subscription with endsAt = subscription.endsAt.AddYears 1 }
    }
```

## Check the context

Identify the inputs needed for the decision and which effects depend on its result. Check whether a later read genuinely depends on an earlier decision, and whether transactions, concurrency checks, or repeated clock reads constrain the split. Extract logic without moving reads earlier or writes outside their required transaction.

## Report when

Report when a decision can receive already loaded values but instead acquires dependencies or performs effects itself. Name the separable rule and the values it needs. A pure decision can be tested without mocks; the remaining workflow still owns effect order and failure handling.

## Suggest

Return a decision from a pure function, then apply its effect:

```fsharp
let renewal (now: System.DateTime) (subscription: Subscription) =
    if subscription.endsAt < now then
        Some { subscription with endsAt = subscription.endsAt.AddYears 1 }
    else
        None

let renewIfExpired (db: Db) id =
    task {
        let! subscription = db.load id

        match renewal System.DateTime.UtcNow subscription with
        | Some renewed -> do! db.save renewed
        | None -> ()
    }
```

The clock is still sampled after loading, and saving remains conditional. Keep transaction and cancellation behavior with the workflow.

## Do not report

Leave adapters that only perform I/O as adapters:

```fsharp
let readSettings fileName = System.IO.File.ReadAllText fileName
```

There is no domain decision to extract. Also permit a workflow whose next read depends on an earlier decision; separate its pure decisions where useful without demanding one up-front batch of all reads.
