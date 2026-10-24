---
id: validation-collects-input-errors
category: libraries/fstoolkit-errorhandling
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["FsToolkit.ErrorHandling"]
sources:
  - "FsToolkit.ErrorHandling, validation computation expression and its gotchas: https://demystifyfp.gitbook.io/fstoolkit-errorhandling/fstoolkit.errorhandling/index/ce"
---

# Collect input errors with validation, fail fast in workflows

Use `validation { }` with `let!` and `and!` where all errors of independent fields should be reported together, typically when building a domain value from user input, and use `result` or `taskResult` with sequential `let!` everywhere else. Failing fast in a form hides the other field errors; collecting in a workflow runs steps after one they depend on failed.

## Check

1. For every constructor of a domain value from several input fields: does it stop at the first invalid field?
2. In every `validation { }`: are all independent checks in one `let! ... and! ...` group, or does a later sequential `let!` stop collection?
3. Look for `validation` or `and!` in workflows whose steps depend on each other.

## Report

The expression and its validation or fail-fast form.

## Do not report

- Checks that depend on an earlier one, such as a password confirmation after the password.

## Examples (illustrative, not exhaustive)

```fsharp
let tryCreateCustomer input = // finding: an invalid name hides an invalid email
    result {
        let! name = Name.tryCreate input.name
        let! email = Email.tryCreate input.email
        return { name = name; email = email }
    }

// correction
let tryCreateCustomer input =
    validation {
        let! name = Name.tryCreate input.name
        and! email = Email.tryCreate input.email
        return { name = name; email = email }
    }
```

No finding: `taskResult` that loads a customer and then charges the card.
