---
id: use-option-combinators
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Transform options with combinators, not by testing and reading

Use `Option.map`, `Option.bind`, `Option.defaultValue`, `Option.defaultWith`, or a `match` instead of testing `IsSome` and then reading `.Value`, and instead of writing such a combinator out as a match. Test-then-read splits one decision into two that can drift apart, and a hand-written map hides that nothing else happens.

## Check

1. Look for `IsSome`, `IsNone`, or `Option.isSome` followed by `.Value` or `Option.get`.
2. Look for matches that rebuild a combinator: `Some v -> Some(f v) | None -> None`, `Some v -> f v | None -> None`, or `Some v -> v | None -> fallback`.
3. Look for an `option<option<_>>` from `Option.map` with an option-returning function, where `Option.bind` belongs.

## Report

The code and its combinator.

## Do not report

- Matches whose branches do different real work, such as sending a mail or writing a log.

## Examples (illustrative, not exhaustive)

```fsharp
let couponText = // finding: test, then read
    if order.coupon.IsSome then
        $"{order.coupon.Value.percent} percent off"
    else
        "none"

// correction
let couponText =
    order.coupon
    |> Option.map (fun coupon -> $"{coupon.percent} percent off")
    |> Option.defaultValue "none"
```

No finding: `match order.coupon with Some coupon -> notifyPartner coupon | None -> logMissingCoupon ()`.
