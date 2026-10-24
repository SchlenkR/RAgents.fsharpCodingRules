---
id: trigger-every-failure-case
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Trigger every failure case in a test

Give every case of a function's error union a test that produces it, in addition to the successful path. An error case that no test triggers can stop being reachable or change its meaning without anyone noticing.

## Check

1. For every new or changed error union: does each case name appear in a test that provokes it?
2. Is the successful path tested as well?

## Report

The error cases without a triggering test.

## Do not report

- Cases for failures of infrastructure that the tests replace, such as a lost database connection, when another test covers the handler for them.

## Examples (illustrative, not exhaustive)

```fsharp
type SignUpError =
    | EmailMissing
    | EmailTaken
    | PasswordTooShort

testList
    "sign up"
    [
        test "creates an account" { ... }
        test "rejects a missing email" { ... }
        test "rejects a taken email" { ... } // finding: no test for PasswordTooShort
    ]

// correction
testList
    "sign up"
    [
        test "creates an account" { ... }
        test "rejects a missing email" { ... }
        test "rejects a taken email" { ... }
        test "rejects a short password" { ... }
    ]
```

No finding: a case for an unavailable database that a separate handler test covers.
