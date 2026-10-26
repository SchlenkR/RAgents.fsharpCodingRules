---
id: members-for-own-data
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
alternatives: [functions-in-companion-module]
---

# Put on a type what needs only its own data

Define construction, projections, and state transitions that use only a type's own data as members, and put I/O and workflows over several types into modules.

## Look for

Look for module functions that only inspect one domain value, and members that instead perform I/O or coordinate collections and other domain types:

```fsharp
type Invoice = { amounts: decimal list }

module Invoice =
    let total (invoice: Invoice) = invoice.amounts |> List.sum
```

The projection needs only `Invoice` data. Constructors and pure transitions are similar candidates.

## Check the context

Trace dependencies rather than counting parameters. Does the operation need infrastructure, another type's internals, or a collection of instances? Check whether callers intentionally partially apply curried parameters for pipelines. Inspect extensions separately: an infrastructure extension in another module need not move into the domain definition. Check callers and signature files when moving the operation.

## Report when

Report a projection or construction operation separated from its own type without a pipeline or layering reason. Conversely, report a domain member that acquires infrastructure dependencies or coordinates a workflow. State the actual data or effects that determine placement. This selected preference makes own-data operations available through member completion while keeping workflows outside the data type.

## Suggest

Place the pure projection on its type and update uses:

```fsharp
type Invoice =
    {
        amounts: decimal list
    }

    member this.Total = this.amounts |> List.sum

let amountDue (invoice: Invoice) = invoice.Total
```

Move I/O and multi-type coordination into module functions that receive their dependencies. Preserve validation and effect order during the move. This rule is the alternative to rule `functions-in-companion-module`.

## Do not report

Keep effectful operations in a module:

```fsharp
module Invoice =
    let sendReminder (send: string -> unit) recipient = send recipient
```

The operation depends on an external effect rather than one invoice's data. Also retain companion functions whose curried parameters serve pipelines, such as `Invoice.addLine line`, and infrastructure extensions defined outside the domain layer.
