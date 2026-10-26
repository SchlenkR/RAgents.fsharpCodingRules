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


## Look for

Look for infrastructure exceptions converted into error cases that callers only wrap, forward, or log. Also inspect several error cases that receive exactly the same reaction everywhere.

```fsharp
type SaveError = DatabaseUnavailable of exn

let save order =
    try
        Ok(Db.insert order)
    with error ->
        Error(DatabaseUnavailable error)

let submit order =
    match save order with
    | Ok() -> ()
    | Error(DatabaseUnavailable error) -> logFailure error
```

## Check the context

1. Follow each error case through callers until it is consumed. Find a different action, such as a retry decision, a field message, or a response status.
2. Inspect the operation's outer exception policy before removing a wrapper. Check that it already logs and fails the operation appropriately.
3. Distinguish internal application code from library APIs whose unknown callers may need the result. Do not conclude "only logged" from a single visible caller.

## Report when

Report a case whose inspected consumers only transport or log it and whose exception can reach the established outer policy. Identify the redundant propagation path. For indistinguishable cases, suggest merging only when neither decisions nor required diagnostics need the distinction.

## Suggest

Let the infrastructure exception propagate to the operation's existing failure boundary:

```fsharp
let save order = Db.insert order

let submit order =
    try
        save order
    with error ->
        logFailure error
```

The example places the policy in `submit` only if it is that boundary; otherwise remove the inner catch there as well. Preserve failure behavior and update callers when changing the return type.

## Do not report

Keep a result whose caller makes a distinct user-facing decision:

```fsharp
match signUp request with
| Ok account -> showAccount account
| Error EmailAlreadyTaken -> showEmailError "This email is already registered."
```

Here the error selects a field response. Mapping errors to HTTP responses at a process boundary and public-library contracts for unknown callers also carry real decisions.
