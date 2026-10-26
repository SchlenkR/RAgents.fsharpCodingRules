---
id: assert-exact-error-values
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Assert the exact error, not just that one occurred

Compare a failing result with the exact error value it must contain, instead of only checking that some error came back. A test that accepts any error stays green when the code fails for the wrong reason.


## Look for

Look for tests accepting any `Error`, such as `Expect.isError` or `Result.isError`, and matches that discard an error payload even though the test names a particular rejection.

```fsharp
test "rejects a negative amount" { Expect.isError (Money.tryCreate -1m) "rejected" }
```

## Check the context

1. Identify the exact error that the setup should provoke, including relevant payload values.
2. Inspect the rest of the test for an exact assertion; a broad preliminary check may be followed by the required comparison.
3. Determine whether the error contract is stable and owned by the project. Distinguish a complete `None` outcome from a `Result` whose error carries information.

## Report when

Report a test naming a particular rejection that would pass for a different error. State an alternative erroneous outcome the assertion would also accept. Comparing only a message or field name is insufficient when the domain error case and payload are the actual contract.

## Suggest

Compare the result with the expected case and payload:

```fsharp
test "rejects a negative amount" { Expect.equal (Money.tryCreate -1m) (Error(AmountNegative -1m)) "rejected" }
```

If the result's successful type prevents direct equality, match and compare the error value explicitly, failing the test on success. Use the project's existing assertion library.

## Do not report

Keep an absence assertion when `None` is the entire expected result:

```fsharp
test "finds no price for an unknown key" { Expect.isNone (Map.tryFind "missing" prices) "absent" }
```

There is no omitted error payload to inspect. Also permit deliberately unspecified external details, such as a third-party exception message; test the project's stable classification rather than pinning that external wording.
