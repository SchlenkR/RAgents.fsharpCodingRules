---
id: use-option-combinators
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Transform options with combinators, not by testing and reading

Use `Option.map`, `Option.bind`, `Option.defaultValue`, `Option.defaultWith`, or a `match` instead of testing `IsSome` and then reading `.Value`, and instead of writing such a combinator out as a match.

## Look for

Look for testing `IsSome`/`IsNone` before `.Value` or `Option.get`, matches that exactly rebuild map/bind/default, and `Option.map` producing an unintended nested option:

```fsharp
let couponText =
    if order.coupon.IsSome then
        $"{order.coupon.Value.percent} percent off"
    else
        "none"
```

## Check the context

1. Confirm the checked and extracted option is the same stable value; repeated property reads or effects may require binding it once.
2. Compare branch work: is it just transformation/defaulting, or does each branch perform a distinct action?
3. Preserve evaluation timing. An effectful or expensive fallback needs `Option.defaultWith`, since `Option.defaultValue` evaluates its argument eagerly.
4. For nested options, inspect consumers to establish that the two layers do not represent distinct domain states.

## Report when

Report a split presence test/read or a hand-written map, bind, or default that a standard combinator expresses directly. Report `Option.map` with an option-returning function only when consumers require one flattened absence channel. Explain the operation being expressed rather than treating every option match as a violation.

## Suggest

Express the projection and constant fallback directly:

```fsharp
let couponText =
    order.coupon
    |> Option.map (fun coupon -> $"{coupon.percent} percent off")
    |> Option.defaultValue "none"
```

Use `Option.bind` when the transformation returns an option and flattening is intended. Use a match for substantive branch behavior or to capture the payload once.

## Do not report

Leave a match whose branches perform different work:

```fsharp
match order.coupon with
| Some coupon -> notifyPartner coupon
| None -> logMissingCoupon ()
```

These branches express actions rather than rebuilding a combinator. Also preserve intentional nested-option domain distinctions and lazy fallback behavior.
