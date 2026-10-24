---
id: reraise-keeps-stack
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "FSharpLint FL0073 FavourReRaise: https://github.com/fsprojects/FSharpLint/blob/master/docs/content/how-tos/rules/FL0073.md"
---

# Rethrow with reraise

Rethrow the exception being handled with `reraise ()`, or with `ExceptionDispatchInfo.Capture(ex).Throw()` inside computation expressions where `reraise` does not compile, and wrap it only to add context, passing the original as inner exception. `raise ex` restarts the stack trace at the handler, so the log no longer shows where the failure happened.

## Check

1. Look for `raise ex` inside a `with` handler that refers to the handled exception; FSharpLint reports it as FL0073.
2. For every new exception thrown inside a handler: does it pass the original as inner exception?

## Report

The rethrow and its `reraise ()`, `ExceptionDispatchInfo`, or inner-exception form.

## Do not report

- A new exception unrelated to the handled one, thrown after the failure was classified.

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

// correction inside task { }, where reraise fails with FS0413
task {
    try
        do! Db.insertAsync order
    with ex ->
        logger.LogError(ex, "insert failed")
        ExceptionDispatchInfo.Capture(ex).Throw()
}
```

No finding: `raise (InvalidOperationException($"import of {path} failed", ex))`.
