---
id: test-name-states-the-assertions
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Assert what the test name says, nothing else

Assert in a test exactly what its name states, and move every other check into a test with its own name. Assertions the name does not mention make unrelated tests fail together after a change and hide what a test is for.


## Look for

Look for tests whose assertions cover more behavior than the name promises, and verbose success/failure matches used only to unpack the value for a later assertion.

```fsharp
test "rejects an order without lines" {
    let audit = FakeAudit()
    let result = submitOrder audit []
    Expect.equal result (Error NoLines) "rejected"
    Expect.equal audit.Entries.Count 0 "nothing logged"
}
```

## Check the context

1. Relate every assertion to the behavior stated by the test name.
2. Separate assertions needed to obtain the subject from independent behavioral guarantees. Extracting `Ok` can be a prerequisite rather than a second scenario.
3. Check whether the proposed split needs fresh fixtures, so one test does not depend on another or share recorded state.

## Report when

Report an independently meaningful assertion that the name does not cover, or a manual success-checking match obscuring the named assertion. Identify the extra guarantee and give it its own descriptive test. Multiple assertions are allowed when they jointly express the named behavior.

## Suggest

Give the error and logging contract separate tests, each with its own setup:

```fsharp
test "rejects an order without lines" {
    let audit = FakeAudit()
    Expect.equal (submitOrder audit []) (Error NoLines) "rejected"
}

test "logs nothing for an order without lines" {
    let audit = FakeAudit()
    submitOrder audit [] |> ignore
    Expect.equal audit.Entries.Count 0 "nothing logged"
}
```

Where the only extra check unpacks success, use the assertion library's value-extraction helper, such as `Expect.wantOk`.

## Do not report

Keep success extraction required to inspect the named property:

```fsharp
test "keeps the supplied order reference" {
    let order = Expect.wantOk (Order.tryCreate reference lines) "created"
    Expect.equal order.reference reference "reference retained"
}
```

The preliminary success requirement obtains the value under test. It does not independently specify another business effect or storage contract.
