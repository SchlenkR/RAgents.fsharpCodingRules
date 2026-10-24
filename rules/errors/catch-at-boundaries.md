---
id: catch-at-boundaries
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Catch exceptions only at boundaries

Write `try`/`with` only directly around a call that throws for an expected reason, such as parsing, a file, an SDK, or legacy code, and classify the exception right there; never wrap your own core logic or swallow exceptions. A handler far from its cause cannot know what failed, and a broad handler hides bugs.

## Check

1. For every new `try`/`with`: does it protect one throwing call, or a block of own code?
2. Does the handler match specific types such as `:? IOException`, or catch everything (`with _`, `with ex`) without re-raising?
3. Is the exception turned into a union case or an `Error` in the handler?

## Report

The handler and its narrow form at the throwing call, or its removal.

## Do not report

- The top-level handler of an application, request pipeline, or job that logs and applies its failure policy, including ending the process.
- `use` and `try`/`finally` for cleanup.

## Examples (illustrative, not exhaustive)

```fsharp
let import path =
    try // finding: wraps reading, parsing, and saving, and swallows everything
        File.ReadAllText path |> parseRows |> saveRows
    with _ ->
        ()

// correction
let tryReadText path =
    try
        Ok(File.ReadAllText path)
    with :? IOException as ex ->
        Error(CannotRead ex.Message)
```

No finding: the handler in `main` that logs an unhandled exception and returns exit code 1.
