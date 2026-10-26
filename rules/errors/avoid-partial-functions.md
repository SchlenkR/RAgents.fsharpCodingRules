---
id: avoid-partial-functions
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "FSharpLint FL0066 NoPartialFunctions: https://github.com/fsprojects/FSharpLint/blob/master/docs/content/how-tos/rules/FL0066.md"
---

# Avoid functions that throw on missing data

Use `tryHead`, `tryFind`, `tryItem`, `tryExactlyOne`, `Map.tryFind`, or a match instead of `List.head`, `List.find`, `Seq.exactlyOne`, `Map.find`, `Option.get`, or `.Value` on an option when the input can break their preconditions. These functions throw a generic exception far from the decision about what a missing or ambiguous value means.


## Look for

Look for partial reads such as `List.head`, `List.find`, `Map.find`, `Seq.exactlyOne`, indexers, `Option.get`, or option `.Value`, including wrappers around them.

```fsharp
let customer =
    customers |> List.find (fun customer -> customer.id = order.customerId)
```

## Check the context

1. Trace the input back to its producer. Can ordinary input be empty, missing, out of range, or ambiguous?
2. Inspect guards and branches for the complete precondition: `exactlyOne` needs exactly one element; an index needs both bounds.
3. Decide what absence means at this call site. Distinguish a caller-handled outcome from a broken invariant.

## Report when

Report a partial operation whose precondition can fail for ordinary input and whose missing or ambiguous case needs a decision. Identify the possible input and the exception-producing call. FSharpLint FL0066 can locate candidates, but the reviewer decides whether the precondition is established.

## Suggest

Use the matching `try` operation or a pattern match and handle the absent case explicitly. Here an unknown customer becomes a domain error:

```fsharp
match customers |> List.tryFind (fun customer -> customer.id = order.customerId) with
| Some customer -> Ok customer
| None -> Error(UnknownCustomer order.customerId)
```

Update callers for the result type; do not silently substitute an arbitrary customer.

## Do not report

Leave access whose full precondition is established on the same path:

```fsharp
let firstOrDefault fallback values =
    match values with
    | [] -> fallback
    | _ -> List.head values
```

The non-empty branch proves this read safe. Also permit partial reads in tests where a missing value should fail the test. A vaguely named upstream validator is insufficient evidence without inspecting its contract.
