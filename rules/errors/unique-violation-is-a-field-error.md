---
id: unique-violation-is-a-field-error
category: errors
check: semantic
files: ["**/*.{fs,fsx,sql}"]
---

# Report a unique-index collision as a field error

Check for an existing row with the same filter that the unique index uses before an insert, return a field error on a collision, and map the unique violation that the insert can still raise to the same error. Otherwise a known rule of the data ends as an exception and an error page.


## Look for

Look for user-input uniqueness checks followed by an insert, especially an active-only lookup when the index also includes deleted rows, or an insert whose unique collision escapes as a server error.

```fsharp
task {
    match! Accounts.tryFindActiveByEmail email with
    | Some _ -> return Error EmailTaken
    | None ->
        let! accountId = Accounts.insert email
        return Ok accountId
}
```

## Check the context

1. Read the actual unique index or constraint, including predicates, composite keys, and normalization/collation. Compare its coverage with the lookup.
2. Inspect the insert adapter for the race where another writer inserts after the precheck.
3. Identify the provider exception and specific constraint that represent this input collision. Other unique constraints and infrastructure failures must remain distinguishable.

## Report when

Report a precheck that permits a collision covered by the index, or an unclassified insert collision for user-correctable input. Name the conflicting row condition or race. A precheck improves feedback but cannot enforce uniqueness under concurrent writers.

## Suggest

Match the precheck to the index and map its collision at the insert boundary:

```fsharp
let tryInsert email =
    task {
        try
            let! accountId = Accounts.insert email
            return Ok accountId
        with error when isEmailUniqueViolation error ->
            return Error EmailTaken
    }

let signUp email =
    task {
        match! Accounts.tryFindByEmail email with
        | Some _ -> return Error EmailTaken
        | None -> return! tryInsert email
    }
```

`isEmailUniqueViolation` must identify the actual provider code and email constraint, rather than any database exception. Keep deleted-account-specific messaging if the workflow distinguishes it, while still classifying the insert race as a field error.

## Do not report

Keep an insert whose data has no user-input uniqueness requirement:

```fsharp
let recordAudit event = Audit.insert event
```

Do not invent a field error for an audit identifier collision or an unavailable database. Also keep an existing lookup whose predicate matches a filtered index and whose insert already maps that constraint correctly.
