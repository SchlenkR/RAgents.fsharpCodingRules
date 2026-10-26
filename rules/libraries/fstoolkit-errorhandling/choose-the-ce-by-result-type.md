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

Write a function that returns `Task<Result<_, _>>` as `taskResult`, a synchronous chain of results as `result`, and a task of your own state union as `task` with `match!`, binding a `Result` with `let!` or `return!` instead of wrapping it.

## Look for

Look for a computation expression manually wrapping/unwrapping the same result type at each step, or a synchronous `Result` returned as a value inside `taskResult`:

```fsharp
let changeEmail (request: ChangeEmailRequest) =
    task {
        let! account = Accounts.load request.accountId
        return account |> Account.tryChangeEmail request.email
    }
```

Also inspect `Option` lifting and completed-task construction where FsToolkit helpers are available.

## Check the context

1. Resolve the complete return type and project builder version: `Task<Result<_, _>>`, `Result<_, _>`, or a domain state union need different handling.
2. Identify which steps should short-circuit and which deliberately inspect or preserve a nested result.
3. Check whether binding needs `let!`, `do!`, or `return!`; lift an option through `Result.requireSome` only when missing means that error.
4. For `Task.singleton`, confirm the helper is already available and no framework-specific task construction behavior is required.

## Report when

Report a mismatch between the workflow result type and its builder when the matching builder removes manual wrapping while preserving the intended error path. Also report accidentally returning an unbound result/option inside the builder, and use the available completed-task helper under this library preference. A `Result` carried as intentional domain data is not an error to short-circuit.

## Suggest

Use the matching builder and delegate the final result:

```fsharp
let changeEmail (request: ChangeEmailRequest) =
    taskResult {
        let! account = Accounts.load request.accountId
        return! account |> Account.tryChangeEmail request.email
    }
```

Use `result` for synchronous result workflows, and `Result.requireSome` to supply a real missing-value error. Where FsToolkit's `Task.singleton` is in scope, prefer it to `Task.FromResult` for an ordinary completed task.

## Do not report

Leave task workflows returning their own state union:

```fsharp
let capture order =
    task {
        match! Payments.capture order with
        | Captured receipt -> return Captured receipt
        | Declined reason -> return Declined reason
    }
```

These cases are domain outcomes rather than `Result` success/error channels. Also preserve deliberate branch-specific recovery and intentional nested results.
