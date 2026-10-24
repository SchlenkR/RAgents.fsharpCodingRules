---
id: result-for-decisions
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Scott Wlaschin, Against Railway-Oriented Programming: https://fsharpforfunandprofit.com/posts/against-railway-oriented-programming/"
---

# Return Result only for failures a caller acts on

Use a `Result` error case only when some caller matches on it and does something different, and let failures nobody can handle, such as an unavailable database, raise exceptions caught once at the top. An error case that only travels up to be logged costs wrappers and match branches without carrying a decision.

## Check

1. For every new error case: does some match on it do more than log, wrap, or pass it on?
2. Look for `try`/`with` that turns an infrastructure exception into `Error` just to return it.
3. Look for error types wrapping the lower layer's error type (`StorageError of DbError`, `Failed of exn`).
4. Do several cases get the same reaction everywhere? Merge them, unless logs must distinguish them.

## Report

The error case and its replacement: a propagating exception, or a case a caller handles.

## Do not report

- Libraries whose unknown callers may decide.
- Mapping to a response at a process boundary, such as an HTTP status.

## Examples (illustrative, not exhaustive)

```fsharp
type SaveError = DatabaseUnavailable of exn // finding: every caller only logs it

let save order =
    try
        Ok(Db.insert order)
    with ex ->
        Error(DatabaseUnavailable ex)

let save order = Db.insert order // correction: the top-level handler logs the exception
```

No finding: `Error EmailAlreadyTaken`, which the sign-up page shows next to the field.
