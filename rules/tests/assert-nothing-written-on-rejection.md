---
id: assert-nothing-written-on-rejection
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Prove that a rejected input writes nothing

Give every workflow that writes a test showing that a rejected input leaves storage untouched, in addition to the test of the error value. A test that only checks the error stays green when the workflow writes first and fails afterwards.

## Check

1. For every new workflow that writes data: is there a test with rejected input that checks the store afterwards?
2. Does that test check every store the workflow can write to?

## Report

The workflow and the missing negative test.

## Do not report

- Workflows that validate everything before any write, where a type proves it, such as a validated command that is the only input to the write.

## Examples (illustrative, not exhaustive)

```fsharp
test "rejects a booking in the past" {
    let store = FakeStore()
    let result = book store pastDate
    Expect.equal result (Error DateInPast) "rejected" // finding: never checks the store
}

// correction
test "writes nothing for a booking in the past" {
    let store = FakeStore()
    book store pastDate |> ignore
    Expect.isEmpty store.Bookings "nothing written"
}
```

No finding: a test for the error value next to this one, each with its own name.
