---
id: one-setup-per-test
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Give every setup its own test

Write one test per setup, and generate a named test per input from a list when the same assertion applies to several known inputs. A test that runs several setups stops at the first failure and hides which of the others still pass.


## Look for

Look for one test invoking the same operation with several independent inputs, resetting a fixture between assertions, or looping through unrelated scenarios inside one named test.

```fsharp
test "maps delivery options" {
    Expect.equal (Delivery.ofCode "S") (Some Standard) "standard"
    Expect.equal (Delivery.ofCode "E") (Some Express) "express"
    Expect.equal (Delivery.ofCode "X") None "unknown"
}
```

## Check the context

1. Determine whether these calls form one stateful scenario or independent examples of a mapping.
2. Inspect setup dependencies: could each input run in a fresh fixture without relying on the previous assertion?
3. Check the test runner's reporting unit. A loop inside one test still stops on the first assertion failure; generated named tests are separate units.

## Report when

Report multiple independent setups bundled into one test. Identify the separately meaningful scenarios hidden after the first possible failure. Keep the request focused on test isolation and diagnostics, rather than counting assertions mechanically.

## Suggest

Generate one named test for each known input and expected result:

```fsharp
testList
    "delivery options"
    [
        for code, expected in [ "S", Some Standard; "E", Some Express; "X", None ] do
            test $"maps delivery code {code}" { Expect.equal (Delivery.ofCode code) expected "mapped" }
    ]
```

For stateful fixtures, create the fixture inside each generated test body so cases do not share mutable setup. Split scenarios that need different setup rather than forcing them into one parameterized table.

## Do not report

Keep several assertions describing the result of one setup:

```fsharp
test "creates money with the supplied amount and currency" {
    let money = Money.create 12m "EUR"
    Expect.equal money.amount 12m "amount"
    Expect.equal money.currency "EUR" "currency"
}
```

Both assertions inspect the same created value. A sequence of operations needed to establish one state transition is also one scenario; do not split it into dependent tests.
