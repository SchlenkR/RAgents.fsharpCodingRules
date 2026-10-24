---
id: reraise-keeps-stack
category: errors
check: tool
files: ["**/*.{fs,fsx}"]
sources:
  - "FSharpLint FL0073 FavourReRaise: https://github.com/fsprojects/FSharpLint/blob/master/docs/content/how-tos/rules/FL0073.md"
---

# Rethrow with reraise

Rethrow the exception being handled with `reraise ()`, not with `raise ex`, and wrap it in a new exception only to add context, passing the original as the inner exception. `raise ex` restarts the stack trace at the handler, so the log no longer shows where the failure happened.

## Check

1. Look for `raise ex`, `raise e`, or `raise (ex)` inside a `with` handler that refers to the handled exception.
2. For every new exception thrown inside a handler: does it pass the original as inner exception?

## Report

The rethrow and its `reraise ()` or inner-exception form. FSharpLint reports the first case as FL0073.

## Do not report

- Throwing a new, unrelated exception after a failure was classified.
- `reraise` outside a handler, which the compiler already rejects.

## Examples (illustrative, not exhaustive)

```fsharp
try
    Db.insert order
with ex ->
    logger.LogError(ex, "insert failed")
    raise ex // finding: the stack trace restarts here

// correction
try
    Db.insert order
with ex ->
    logger.LogError(ex, "insert failed")
    reraise ()
```

No finding: `with :? IOException as ex -> raise (InvalidOperationException($"import of {path} failed", ex))`.
