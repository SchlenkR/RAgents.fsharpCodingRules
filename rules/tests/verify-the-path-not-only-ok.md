---
id: verify-the-path-not-only-ok
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Verify which dependency was called, not only that it worked

Assert which dependency a workflow called and with which arguments, for example through a recording fake, instead of only checking that the result is `Ok`. A test that only sees `Ok` stays green when the workflow calls the wrong operation or passes the wrong ID.


## Look for

Look for workflow tests that return `Ok` from interchangeable fakes and assert only success, although the named behavior selects a dependency operation or identifier.

```fsharp
test "assigns the existing customer" {
    let store = FakeStore()
    let result = assignOrder store order
    Expect.isOk result "assigned"
}
```

## Check the context

1. Inspect the workflow's branches and dependency calls. Could the wrong operation or argument still produce the asserted result?
2. Read the fake: does it record calls, reject incorrect arguments, or return different observable results for different paths?
3. Inspect assertions elsewhere in the test for equivalent evidence. A recording assertion is unnecessary when a focused fake already fails on the wrong call.

## Report when

Report a named routing or dependency-use behavior that could pass through the wrong path with the current fixture. State the alternative wrong call the test accepts. Request evidence for the actual interaction contract, rather than verifying every private implementation detail.

## Suggest

Use a recording fake with setup establishing the existing customer, then assert the operation and arguments:

```fsharp
test "assigns the existing customer" {
    let store = RecordingStore(existingCustomer)
    assignOrder store order |> ignore
    Expect.equal store.Assigned [ order.id, existingCustomer.id ] "assigned to the existing customer"
}
```

If creating a customer would also be a wrong observable effect, assert that its recorded call list stays empty. Initialize the recording fake afresh for each test.

## Do not report

Keep a pure-function test whose result fully expresses the contract:

```fsharp
test "adds the configured tax" { Expect.equal (Price.gross 0.2m 10m) 12m "gross amount" }
```

There is no dependency path to verify. Also keep workflows where the fixture or result assertions already distinguish the wrong operation or arguments; do not require recording merely as a testing style.
