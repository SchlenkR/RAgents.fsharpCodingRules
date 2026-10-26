---
id: one-reason-per-none
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Return a union when None has several reasons

Return named outcomes instead of option when different reasons for absence require different caller behavior or diagnostic information.

## Look for

Look for several branches producing `None`, especially lookup functions that combine not found, expiry, and exhausted limits:

```fsharp
let tryFindDiscount today code (discounts: Map<string, Discount>) =
    match Map.tryFind code discounts with
    | None -> None
    | Some discount when discount.expiresAt < today -> None
    | Some discount when discount.remaining = 0 -> None
    | Some discount -> Some discount
```

The result erases which of three conditions occurred.

## Check the context

Read callers, logs, tests, and requirements to establish whether anyone needs the distinction. Several internal branches alone do not justify a larger return type. Check whether callers repeat the lookup or infer a reason from partial data because the option lost it. If diagnostic information is already retained reliably at the appropriate boundary, do not invent an additional reporting requirement.

## Report when

Report when distinct relevant outcomes collapse into `None` before a caller can react or a required diagnostic can be produced. Name the outcomes and the actual differing reaction or diagnostic requirement. Without that evidence, multiple paths to absence can be an intentional simplification.

## Suggest

Return the relevant outcomes with any data their consumers need:

```fsharp
[<RequireQualifiedAccess>]
type DiscountLookup =
    | Valid of Discount
    | Unknown
    | Expired of expiredOn: System.DateOnly
    | UsedUp

let findDiscount today code (discounts: Map<string, Discount>) =
    match Map.tryFind code discounts with
    | None -> DiscountLookup.Unknown
    | Some discount when discount.expiresAt < today -> DiscountLookup.Expired discount.expiresAt
    | Some discount when discount.remaining = 0 -> DiscountLookup.UsedUp
    | Some discount -> DiscountLookup.Valid discount
```

Update callers to make the required distinction. Preserve branch priority when several failure conditions apply, and include only outcomes that actually matter.

## Do not report

Keep option for a single meaningful absence:

```fsharp
let tryFindPrice sku (prices: Map<string, decimal>) = Map.tryFind sku prices
```

`None` only means the key has no price. Several internal reasons may also share one option result when every caller intentionally treats them identically and no diagnostic needs to distinguish them.
