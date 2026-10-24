---
id: members-for-own-data
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
alternatives: [functions-in-companion-module]
---

# Put on a type what needs only its own data

Define construction (`TryCreate`), projections (`Value`, `Total`), and state transitions that use only the type's own data as members of the type, and put functions that need I/O, combine several types, or work on collections into a module. Members appear in dot completion where the value is, and modules stay the place for workflows.

## Check

1. For every new module function whose only input is one value of a type from this code base: does it use anything but that value's data?
2. For every new member: does it need I/O, other types' internals, or a collection of instances?

## Report

The function or member and where it belongs.

## Do not report

- Functions in a companion module that need curried parameters for pipelines, such as `Invoice.addLine line`.
- Extension members that add infrastructure concerns in another module.

## Examples (illustrative, not exhaustive)

```fsharp
module Invoice =
    let total (invoice: Invoice) = invoice.lines |> List.sumBy _.amount // finding: needs only the invoice

// correction
type Invoice =
    {
        lines: InvoiceLine list
    }

    member this.Total = this.lines |> List.sumBy _.amount
```

No finding: `Invoice.sendReminder (mail: MailClient) invoice` in a module, because it needs I/O.
