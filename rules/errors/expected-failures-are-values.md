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

## Check

1. For every new `failwith`, `failwithf`, `raise`, or `invalidOp`: can a user or caller trigger the condition in normal use, and should they be told?
2. For every `try`/`with` around own code: does it catch an exception the same code base raises for a business case?

## Report

The exception and the `Result` or `Option` signature that replaces it.

## Do not report

- Broken invariants and bugs that no caller can handle, such as an unreachable branch.
- `invalidArg` for programmer errors at a library's public entry point.

## Examples (illustrative, not exhaustive)

```fsharp
let withdraw amount account =
    if amount > account.balance then
        failwith "insufficient funds" // finding: an expected outcome

    { account with balance = account.balance - amount }

// correction
let withdraw amount account =
    if amount > account.balance then
        Error InsufficientFunds
    else
        Ok { account with balance = account.balance - amount }
```

No finding: `invalidArg (nameof pageSize) "must be positive"` in a library's public function.
