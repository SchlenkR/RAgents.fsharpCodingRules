---
id: verify-the-path-not-only-ok
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Verify which dependency was called, not only that it worked

Assert which dependency a workflow called and with which arguments, for example through a recording fake, instead of only checking that the result is `Ok`. A test that only sees `Ok` stays green when the workflow calls the wrong operation or passes the wrong ID.

## Check

1. For every new test of a workflow with dependencies: does it check the calls or their arguments, or only the result?
2. Could the workflow take a different branch and still return the same result?

## Report

The test and the call it should verify.

## Do not report

- Pure functions, whose result is all there is.

## Examples (illustrative, not exhaustive)

```fsharp
test "assigns the existing customer" {
    let result = assignOrder fakeStore order
    Expect.isOk result "assigned" // finding: green with the wrong customer, too
}

// correction
test "assigns the existing customer" {
    let store = RecordingStore()
    assignOrder store order |> ignore
    Expect.equal store.Assigned [ order.id, existingCustomerId ] "assigned to the existing customer"
}
```

No finding: a test of `Price.gross` that compares only the returned amount.
