---
id: one-setup-per-test
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Give every setup its own test

Write one test per setup, and generate a named test per input from a list when the same assertion applies to several known inputs. A test that runs several setups stops at the first failure and hides which of the others still pass.

## Check

1. Does one test build several independent setups or call the code under test with several unrelated inputs?
2. Would a list of cases with one named test each express the same checks?

## Report

The bundled test and its split or generated form.

## Do not report

- Several assertions about one result of one setup.

## Examples (illustrative, not exhaustive)

```fsharp
test "maps delivery options" { // finding: three setups in one test
    Expect.equal (Delivery.ofCode "S") (Some Standard) "standard"
    Expect.equal (Delivery.ofCode "E") (Some Express) "express"
    Expect.equal (Delivery.ofCode "X") None "unknown"
}

// correction
testList
    "delivery options"
    [
        for code, expected in [ "S", Some Standard; "E", Some Express; "X", None ] do
            test $"maps {code} to {expected}" { Expect.equal (Delivery.ofCode code) expected "mapped" }
    ]
```

No finding: a test that checks the amount and the currency of one created `Money` value.
