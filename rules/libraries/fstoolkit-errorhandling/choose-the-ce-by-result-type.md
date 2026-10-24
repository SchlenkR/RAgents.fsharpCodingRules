---
id: choose-the-ce-by-result-type
category: libraries/fstoolkit-errorhandling
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["FsToolkit.ErrorHandling"]
sources:
  - "FsToolkit.ErrorHandling, taskResult computation expression: https://demystifyfp.gitbook.io/fstoolkit-errorhandling/fstoolkit.errorhandling/taskresult/ce"
---

# Choose the computation expression by what the function returns

Write a function that returns `Task<Result<_, _>>` as `taskResult`, a synchronous chain of results as `result`, and a task of your own state union as `task` with `match!`, binding a `Result` with `let!` or `return!` instead of wrapping it. An expression that does not match the return type forces manual wrapping and unwrapping at every step.

## Check

1. Does a `task` that returns a `Result` wrap values by hand, such as `return Ok x` after every step or `return (x |> Order.tryCreate)`?
2. Inside `taskResult`: is a `Result` from a pure function bound with `let!` or `return!`, and an `Option` lifted with `requireSome`?
3. Is `Task.FromResult` used where FsToolkit's `Task.singleton` is available?

## Report

The expression and the one that matches the return type.

## Do not report

- Functions that return their own state union instead of a `Result`; they stay `task` with `match!`.

## Examples (illustrative, not exhaustive)

```fsharp
let changeEmail (request: ChangeEmailRequest) =
    task { // finding: a task that returns Result, wrapped by hand
        let! account = Accounts.load request.accountId
        return account |> Account.tryChangeEmail request.email
    }

// correction
let changeEmail (request: ChangeEmailRequest) =
    taskResult {
        let! account = Accounts.load request.accountId
        return! account |> Account.tryChangeEmail request.email
    }
```

No finding: `task { match! Payments.capture order with ... }` for a function that returns `Captured | Declined`.
