---
id: wrap-constrained-primitives
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Scott Wlaschin, Designing with types: single case union types: https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/"
---

# Wrap constrained primitives and identifiers in single-case unions

Give a primitive that carries a domain rule its own single-case union with a private case and a validating constructor that returns `Result`, and give an entity identifier its own union with a total constructor. A validated wrapper guarantees its rule wherever it appears, and an order ID cannot be passed where a customer ID is expected.

## Check

1. For every new `string`, `int`, `decimal`, or `Guid` field or parameter: does it carry a format or range rule, or could it be confused with another value of that type?
2. Does every wrapper with a rule have a `private` case and a constructor that validates instead of throwing?
3. Look for wrappers without rule or confusion risk; they only add ceremony.

## Report

The primitive and its wrapper, or the needless wrapper.

## Do not report

- DTOs at the boundary; they convert on entry.
- Values that never leave one function.
- Whether the constructor is a member or a companion-module function.

## Examples (illustrative, not exhaustive)

```fsharp
let sendInvoice (email: string) invoice = ... // finding: any string is accepted

// correction
type EmailAddress =
    private
    | EmailAddress of string

    member this.Value =
        let (EmailAddress text) = this
        text

    static member TryCreate(text: string) =
        if text.Contains '@' then
            Ok(EmailAddress text)
        else
            Error $"'{text}' is not an email address"

type CustomerId = CustomerId of Guid
```

No finding: `type Note = { text: string }` for free text without rules.
