---
id: test-name-states-the-assertions
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Assert what the test name says, nothing else

Assert in a test exactly what its name states, and move every other check into a test with its own name. Assertions the name does not mention make unrelated tests fail together after a change and hide what a test is for.

## Check

1. For every new or changed test: does each assertion belong to the behavior the name describes?
2. Does a branch fail the test to check success along the way, such as `| Error _ -> failtest "should succeed"`?

## Report

The extra assertion and the test it belongs to, or `Expect.wantOk` for the hidden success check.

## Do not report

- Assertions that only unpack the value the named assertion needs.

## Examples (illustrative, not exhaustive)

```fsharp
test "rejects an order without lines" {
    let result = Order.tryCreate []
    Expect.equal result (Error NoLines) "rejected"
    Expect.equal auditLog.Count 0 "nothing logged" // finding: not in the name
}

// correction
test "rejects an order without lines" { Expect.equal (Order.tryCreate []) (Error NoLines) "rejected" }

test "logs nothing for a rejected order" {
    Order.tryCreate [] |> ignore
    Expect.equal auditLog.Count 0 "nothing logged"
}
```

No finding: `let order = Expect.wantOk (Order.tryCreate lines) "created"` before the named assertion.
