---
id: assert-exact-error-values
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Assert the exact error, not just that one occurred

Compare a failing result with the exact error value it must contain, instead of only checking that some error came back. A test that accepts any error stays green when the code fails for the wrong reason.

## Check

1. Look for `Expect.isError`, `Expect.isNone`, or `Result.isError` checks that are not followed by a check of the value.
2. Does the test compare the value of the error, not a field name or message text inside it?

## Report

The vague check and its exact comparison.

## Do not report

- Errors whose value is deliberately unspecified, such as an exception message from a third-party library.

## Examples (illustrative, not exhaustive)

```fsharp
test "rejects a negative amount" {
    Expect.isError (Money.tryCreate -1m) "rejected" // finding: any error passes
}

// correction
test "rejects a negative amount" { Expect.equal (Money.tryCreate -1m) (Error(AmountNegative -1m)) "rejected" }
```

No finding: `Expect.isNone (Map.tryFind "missing" prices) "absent"`, where `None` is the whole answer.
