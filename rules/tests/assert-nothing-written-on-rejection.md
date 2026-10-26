---
id: assert-nothing-written-on-rejection
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Prove that a rejected input writes nothing

Give every workflow that writes a test showing that a rejected input leaves storage untouched, in addition to the test of the error value. A test that only checks the error stays green when the workflow writes first and fails afterwards.


## Look for

Look for rejection tests of workflows that can write storage but assert only the returned error. Follow the workflow to each store it can modify.

```fsharp
test "rejects a booking in the past" {
    let store = FakeStore()
    let result = book store pastDate
    Expect.equal result (Error DateInPast) "rejected"
}
```

## Check the context

1. Establish the rejection path and which writes could occur before it returns, including audit/outbox stores when the contract requires them to remain unchanged.
2. Inspect other tests for a fresh setup that verifies unchanged storage on that path.
3. Check transactions and validated input types. A rollback claim needs a testable boundary; merely calling a validator first is not a type-level guarantee.

## Report when

Report a writable workflow's rejection path without evidence that the required stores remain unchanged. Name the writes the error-only assertion cannot detect. Restrict the request to the actual rejection contract: a deliberate rejection audit may be allowed even when booking storage must remain untouched.

## Suggest

Add a separate test of the storage consequence, alongside the exact-error test:

```fsharp
test "writes no booking for a date in the past" {
    let store = FakeStore()
    let before = store.Bookings |> Seq.toList
    book store pastDate |> ignore
    Expect.equal (store.Bookings |> Seq.toList) before "bookings unchanged"
}
```

Use seeded storage when deletion or mutation of existing entries is possible. Check every store the rejection contract protects; an empty newly created store only proves that no new booking remains.

## Do not report

Keep a workflow whose write function accepts only an already validated command and cannot reject that input through the tested business rule:

```fsharp
let saveValidatedBooking (command: ValidBooking) = BookingStore.insert command
```

This exception requires the type's construction and write signature to establish the boundary. An existing separate negative test with its own setup also satisfies the rule; it need not repeat the error assertion.
