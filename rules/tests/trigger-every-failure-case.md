---
id: trigger-every-failure-case
category: tests
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Trigger every failure case in a test

Give every case of a function's error union a test that produces it, in addition to the successful path. An error case that no test triggers can stop being reachable or change its meaning without anyone noticing.


## Look for

Look for new error-union cases or new branches producing existing cases, then inspect tests that exercise the function. A test whose setup always uses a valid password cannot prove the short-password branch:

```fsharp
[<RequireQualifiedAccess>]
type SignUpError =
    | EmailMissing
    | EmailTaken
    | PasswordTooShort

test "rejects a missing email" {
    Expect.equal (signUp "" "long-password") (Error SignUpError.EmailMissing) "email missing"
}
```

## Check the context

1. Map the function's reachable error cases to actual test inputs and dependency responses, not just case-name mentions.
2. Inspect all relevant tests, including shared/generated cases and a successful path, before claiming one is missing.
3. For cases shared by several functions, identify which cases this function can produce. Establish whether separate boundary tests cover infrastructure failures.

## Report when

Report a reachable error case without a test that provokes and verifies it, or a missing successful path. Name the branch and setup needed. Constructing `Error PasswordTooShort` as an expected value without invoking the branch does not establish coverage.

## Suggest

Add the missing case with otherwise valid input, so the intended rejection is reached:

```fsharp
test "rejects a short password" {
    Expect.equal (signUp "ada@example.com" "x") (Error SignUpError.PasswordTooShort) "password rejected"
}
```

Configure uniqueness lookup dependencies to allow that email if needed. Add equivalent triggering tests for any other uncovered cases and retain a valid successful setup. Keep one setup per test (rule `one-setup-per-test`).

## Do not report

Keep a missing-email case already covered by a generated test that calls the function:

```fsharp
testList
    "missing emails"
    [
        for email in [ ""; " " ] do
            test $"rejects missing email '{email}'" {
                Expect.equal (signUp email "long-password") (Error SignUpError.EmailMissing) "email missing"
            }
    ]
```

The case is genuinely exercised for each input. Infrastructure exceptions replaced in unit tests may be covered by separate adapter/handler tests; identify that evidence before requesting duplicates.
