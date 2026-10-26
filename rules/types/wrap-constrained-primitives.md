---
id: wrap-constrained-primitives
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Scott Wlaschin, Designing with types: single case union types: https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/"
---

# Wrap constrained primitives and identifiers in single-case unions

Give constrained primitives private validated wrappers and give confusable entity identifiers distinct single-case unions with total construction.

## Look for

Look for primitive fields or parameters carrying a repeated range, format, or identity rule. Also inspect wrappers whose cases are public or whose constructors throw instead of validating:

```fsharp
let reserve (seats: int) (customerId: System.Guid) (orderId: System.Guid) =
    if seats <= 0 then
        invalidArg (nameof seats) "seats must be positive"

    seats, customerId, orderId
```

Any caller can pass invalid seats or swap the two identifiers while satisfying the signature.

## Check the context

Establish the actual invariant and where the value travels. Search for an existing domain wrapper before adding another. Distinguish constrained values from unrestricted primitives, and identifiers that could be confused from local implementation details. Inspect every construction path: hiding the case is useful only if constructors enforce the invariant. Check whether an existing wrapper adds neither validation nor protection against confusion.

## Report when

Report a primitive crossing domain operations with a demonstrated invariant or identity confusion risk, a wrapper that callers can construct invalidly, or an unnecessary wrapper with no such role. Name the specific invalid value or interchangeable identity. A new type should enforce a meaningful distinction rather than just repeat the field name.

## Suggest

Validate constrained values once and use distinct total identifier constructors:

```fsharp
type SeatCount = private SeatCount of int

module SeatCount =
    let tryCreate value =
        if value > 0 then
            Ok(SeatCount value)
        else
            Error "seats must be positive"

type CustomerId = CustomerId of System.Guid
type OrderId = OrderId of System.Guid

let reserve (seats: SeatCount) (customerId: CustomerId) (orderId: OrderId) = seats, customerId, orderId
```

Construct at the boundary and handle the returned error. Members or companion functions are both permitted constructor placements. Remove a needless wrapper when it guarantees nothing and no caller needs the distinction.

## Do not report

Keep unconstrained free text as a primitive:

```fsharp
type Note = { text: string }
```

There is no format rule or competing identity here. Also permit raw DTO fields converted at entry and values whose meaning never escapes one function; do not demand wrappers solely because a primitive appears.
