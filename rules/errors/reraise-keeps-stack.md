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


## Look for

Look for a handler that raises its caught exception again, or wraps it without preserving it as an inner exception.

```fsharp
try
    Db.insert order
with error ->
    logFailure error
    raise error
```

## Check the context

1. Resolve the raised value: is it the exception currently being handled, a wrapper for that failure, or a genuinely new exception?
2. Check whether the enclosing expression permits `reraise ()`; some computation-expression placements reject it.
3. Inspect wrappers for useful added context and an inner exception. FSharpLint FL0073 identifies direct rethrows, but aliases and wrappers also need inspection.

## Report when

Report rethrowing the caught exception with `raise`, or discarding the original exception while adding contextual wrapping. State where the original failure evidence is lost. A direct `raise error` resets the rethrow location, while preserving the original stack helps locate the cause.

## Suggest

Use `reraise ()` in a handler where it compiles:

```fsharp
try
    Db.insert order
with error ->
    logFailure error
    reraise ()
```

If the computation expression rejects `reraise`, preserve dispatch information:

```fsharp
task {
    try
        do! Db.insertAsync order
    with error ->
        logFailure error
        System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw()
}
```

When adding meaningful context instead, pass the caught exception as the wrapper's inner exception.

## Do not report

Keep a contextual wrapper that retains the cause:

```fsharp
try
    importRows fileName
with error ->
    raise (System.InvalidOperationException($"Import of {fileName} failed.", error))
```

The wrapper adds the import identity and preserves the original failure. Also exclude a new, unrelated exception raised after classification; establish that distinction from the actual handler rather than the variable's name.
