---
id: option-not-null
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Use option for missing values, not null

Represent a value that may be missing as `option`, or `voption` on hot paths, and convert .NET nulls where they enter, with `Option.ofObj` or `Option.ofNullable`. Null passes through F# code unnoticed until it crashes far from where it came from.

## Check

1. Look for `null` literals, `isNull` checks, and `Unchecked.defaultof` in domain code.
2. For every value from a .NET API that can return null, such as `Environment.GetEnvironmentVariable`, `FirstOrDefault`, or a deserializer: is it converted to option where it enters?
3. Look for `[<AllowNullLiteral>]` on new F# types and empty strings or `-1` standing for missing values.

## Report

The null or magic value and its option form at the boundary.

## Do not report

- Interop calls that must pass null to a .NET API.
- Nullable reference types (`string | null`) at the boundary, when the project enables them.

## Examples (illustrative, not exhaustive)

```fsharp
let apiKey = Environment.GetEnvironmentVariable "API_KEY" // finding: may be null
let isConfigured = not (isNull apiKey)

// correction
let apiKey = Environment.GetEnvironmentVariable "API_KEY" |> Option.ofObj
```

No finding: `new Timer(callback, null, dueTime, period)`, where .NET expects a null state.
