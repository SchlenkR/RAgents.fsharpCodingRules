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

Structure a workflow as read, decide, write: load the data, compute the decision in a pure function that receives values, then perform the effects the decision describes. A pure decision can be tested without mocks and read without knowing the infrastructure.

## Check

1. For every new function that both decides and performs I/O (database, HTTP, files, clock, randomness): can the decision move into a pure function that receives the loaded data?
2. Look for I/O functions passed deep into domain logic where the value could be loaded first.
3. Look for `DateTime.Now`, `DateTime.UtcNow`, `Guid.NewGuid()`, and `Random` inside domain functions.

## Report

The mixed function and its split into a pure decision and a thin function that loads and writes.

## Do not report

- Workflows where a second read depends on the first decision; keep each part small.
- Adapters whose only job is I/O.

## Examples (illustrative, not exhaustive)

```fsharp
let renewIfExpired (db: Db) id = // finding: the decision sits between I/O calls
    task {
        let! subscription = db.load id

        if subscription.endsAt < DateTime.UtcNow then
            do! db.save { subscription with endsAt = subscription.endsAt.AddYears 1 }
    }

// correction
let renewal (now: DateTime) subscription =
    if subscription.endsAt < now then
        Some { subscription with endsAt = subscription.endsAt.AddYears 1 }
    else
        None

let renewIfExpired (db: Db) id =
    task {
        let! subscription = db.load id

        match renewal DateTime.UtcNow subscription with
        | Some renewed -> do! db.save renewed
        | None -> ()
    }
```

No finding: a repository function that only runs one query.
