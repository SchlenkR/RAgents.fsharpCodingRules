---
id: option-not-null
category: types
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Use option for missing values, not null

Represent missing domain values as option, or value option on measured hot paths, and convert .NET absence at the boundary.

## Look for

Look for `null`, `isNull`, `Unchecked.defaultof`, `AllowNullLiteral`, and magic absent values such as `-1` or empty text in domain code. Trace .NET APIs that may return null:

```fsharp
let readDisplayName () =
    let name = System.Environment.GetEnvironmentVariable "DISPLAY_NAME"
    name

let displayName = readDisplayName ()
```

The nullable value leaves its entry point without an explicit domain representation of absence.

## Check the context

Read the external API's null contract and follow the value into domain consumers. Distinguish a real missing value from a valid sentinel-shaped value, a required invariant, or a framework argument. `Unchecked.defaultof<int>` is zero rather than null, so its use alone proves nothing. Inspect nullable value types and the project's nullable-reference settings at interop boundaries.

## Report when

Report nullable or magic missing values that escape into domain code without an explicit absence type. Identify the source and the consumer forced to know its sentinel convention. Do not turn a broken required invariant into an optional value merely to avoid failure.

## Suggest

Convert where the external value enters:

```fsharp
let readDisplayName () =
    System.Environment.GetEnvironmentVariable "DISPLAY_NAME" |> Option.ofObj

let displayName = readDisplayName () |> Option.defaultValue "Guest"
```

Use `Option.ofNullable` for nullable value types. When the domain also treats empty or whitespace text as missing, apply rule `empty-text-is-missing` rather than assuming `Option.ofObj` handles them. Preserve absence through callers until a default or other decision is actually required.

## Do not report

Keep null where an interop contract explicitly uses it:

```fsharp
let formatAmount (amount: decimal) = amount.ToString("0.00", null)
```

Here .NET uses a null format provider to select the current culture; no missing domain value escapes. Nullable reference annotations such as `string | null` are also permitted at a boundary when enabled, provided the core receives an explicit absence representation.
