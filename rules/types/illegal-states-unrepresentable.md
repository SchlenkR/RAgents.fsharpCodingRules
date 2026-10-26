---
id: illegal-states-unrepresentable
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Scott Wlaschin, Making illegal states unrepresentable: https://fsharpforfunandprofit.com/posts/designing-with-types-making-illegal-states-unrepresentable/"
  - "Scott Wlaschin, Domain Modeling Made Functional, chapter 5"
---

# Make illegal states unrepresentable

Choose domain data shapes that exclude forbidden combinations rather than relying on flags, optional fields, or string keys to agree at runtime.

## Look for

Look for status or boolean fields next to dependent options, options of which only one may be populated, and string-keyed maps representing a fixed schema:

```fsharp
type Payment = { isPaid: bool; paidAt: System.DateTime option }

let paidWithoutDate = { isPaid = true; paidAt = None }
```

Also scan runtime guards such as `payment.isPaid && payment.paidAt.IsNone`; they reveal a relationship the type does not encode.

## Check the context

Determine the valid states from domain behavior and constructors. Are fields actually dependent or independently optional? Enumerate at least one forbidden combination admitted by the current type. For a map, verify that keys are a known finite schema rather than user-defined entries. Distinguish domain state from read-only query projections that intentionally mirror storage.

## Report when

Report a domain type that admits a specific contradictory state and can express its legitimate cases directly. Explain the impossible value and the invariant it breaks. For fixed string keys, show the missing or misspelled key the map permits. A union or record should carry the actual invariant rather than merely rename existing flags.

## Suggest

Give each state only the data that exists in it:

```fsharp
[<RequireQualifiedAccess>]
type Payment =
    | Unpaid
    | Paid of paidAt: System.DateTime
```

Use a union for exclusive alternatives, adding an explicit absence case when both fields may legitimately be absent. Replace fixed-key maps with named record fields, using options only where absence is valid. Update construction and matching sites rather than preserving a separate status flag.

## Do not report

Keep genuinely independent options:

```fsharp
type SearchFilter = { customer: string option; fromDate: System.DateOnly option }
```

Either, both, or neither filter may be selected. Also permit read-only table projections and maps whose key set is genuinely dynamic; no hidden state relationship has been established there.
