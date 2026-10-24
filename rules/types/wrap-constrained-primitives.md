---
id: wrap-constrained-primitives
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Scott Wlaschin, Designing with types: single case union types: https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/"
---

# Wrap constrained primitives in single-case unions

Give a primitive that carries a domain rule or identifies one kind of entity its own single-case union with a private constructor and a `TryCreate` that returns `Result`. Then every existing value has passed validation, and an order ID cannot be passed where a customer ID is expected.

## Check

1. For every new field or parameter of type `string`, `int`, `decimal`, or `Guid`: does it have a format or range rule, or could it be confused with another value of the same type?
2. For every wrapper: is the case `private`, and does construction validate instead of throwing or accepting anything?
3. Look for wrappers without any rule or confusion risk; they only add ceremony.

## Report

The primitive and its wrapper, or the needless wrapper and its removal.

## Do not report

- DTOs and serialization types at the boundary; they convert on entry.
- Values that never leave one function.

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
```

No finding: `type Note = { text: string }` for free text without rules.
