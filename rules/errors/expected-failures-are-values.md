---
id: expected-failures-are-values
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Scott Wlaschin, Against Railway-Oriented Programming, when should you use Result: https://fsharpforfunandprofit.com/posts/against-railway-oriented-programming/"
  - "Scott Wlaschin, Domain Modeling Made Functional, chapter 5"
---

# Return expected failures as values, not exceptions

Return a business failure that callers handle, such as invalid input, an unknown key, or a violated rule, as `Result` or `Option` instead of `failwith`, `raise`, or `invalidOp`. An exception for an expected outcome is invisible in the signature and forces callers into `try`/`with` for a normal decision.


## Look for

Look for `failwith`, `failwithf`, `raise`, or `invalidOp` on paths caused by ordinary user input or a rejected business operation. Follow catches around your own code that recover from those exceptions.

```fsharp
let withdraw amount account =
    if amount > account.balance then
        failwith "insufficient funds"

    { account with balance = account.balance - amount }
```

## Check the context

1. Determine whether valid interaction can reach the condition, rather than a programming defect.
2. Inspect the caller's reaction: can it ask for different input, display a field error, or choose another business path?
3. Check which information that reaction needs. Use `Option` for undifferentiated absence and `Result` for a specific rejected outcome.

## Report when

Report when normal interaction raises an exception for a condition the caller handles as a decision. Identify that caller and outcome. The exception hides the expected branch from the function's type and makes the caller catch failures that belong in normal control flow.

## Suggest

Expose the rejection as a value and update callers to match it:

```fsharp
let withdraw amount account =
    if amount > account.balance then
        Error InsufficientFunds
    else
        Ok { account with balance = account.balance - amount }
```

Keep sufficient error information for the actual decision. Do not convert unrelated infrastructure exceptions into the same business case.

## Do not report

Keep exceptions for established invariants that cannot fail through ordinary input:

```fsharp
let currentJob state =
    match state with
    | Running job -> job
    | Stopped -> invalidOp "The scheduler called currentJob while stopped."
```

This exception is appropriate only if the inspected scheduler contract guarantees the running state; if callers legitimately ask while stopped, absence needs a value. Public-library argument checks for programmer misuse, such as `invalidArg`, are also permitted.
