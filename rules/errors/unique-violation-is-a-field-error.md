---
id: unique-violation-is-a-field-error
category: errors
check: semantic
files: ["**/*.{fs,fsx,sql}"]
---

# Report a unique-index collision as a field error

Check for an existing row with the same filter that the unique index uses before an insert, return a field error on a collision, and map the unique violation that the insert can still raise to the same error. Otherwise a known rule of the data ends as an exception and an error page.

## Check

1. Does the lookup before the insert skip rows that the index still covers, such as soft-deleted rows when the index includes them?
2. Is a unique violation raised by the insert caught right there and turned into the field error (rule `catch-at-boundaries`)?

## Report

The mismatched lookup or the unmapped violation, and the field error.

## Do not report

- Inserts into tables without a unique constraint on user input.

## Examples (illustrative, not exhaustive)

```fsharp
match! Accounts.tryFindActiveByEmail email with // finding: deleted accounts still hold their email in the index
| Some _ -> return Error EmailTaken
| None -> return! Accounts.tryInsert email

// correction
match! Accounts.tryFindByEmail email with
| Some account when account.isDeleted -> return Error EmailOfDeletedAccount
| Some _ -> return Error EmailTaken
| None -> return! Accounts.tryInsert email
```

No finding: a lookup whose filter matches the index exactly, with the violation mapped at the insert.
