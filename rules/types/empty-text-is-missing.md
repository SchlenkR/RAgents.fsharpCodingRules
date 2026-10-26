---
id: empty-text-is-missing
category: types
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Treat empty text as missing where it enters

Convert null, empty, and whitespace-only text into one option representation at entry when the domain treats all three as missing.

## Look for

Look for `Option.ofObj` and null-only checks on text from database rows, forms, files, or APIs. Compare checks on the same field across consumers:

```fsharp
let middleName = Option.ofObj row.MiddleName

let displayName firstName =
    match middleName with
    | Some name -> $"{firstName} {name}"
    | None -> firstName
```

An empty or whitespace-only name passes through as a present value.

## Check the context

Establish what absence means for this field. Inspect schema constraints, input normalization, and callers that test null, emptiness, or whitespace independently. Check whether empty and missing are intentionally distinct, such as an explicit empty replacement versus no update. A database constraint against only `''` does not necessarily exclude whitespace-only values.

## Report when

Report inconsistent or incomplete normalization when the domain equates these representations of absence. Identify a concrete accepted empty value and the consumer that treats it as meaningful. Normalizing once prevents different modules from giving the same field different meanings.

## Suggest

Normalize at the boundary and let consumers handle the option:

```fsharp
let middleName =
    if System.String.IsNullOrWhiteSpace row.MiddleName then
        None
    else
        Some row.MiddleName
```

Keep nonempty text unchanged unless trimming is a separate domain requirement. A query can perform equivalent normalization, but `NULLIF(column, '')` alone handles neither every whitespace value nor every agreed absence representation. Reuse the same conversion at other entry points.

## Do not report

Keep empty and missing separate when the operation defines different meanings:

```fsharp
type TextUpdate = { replacement: string option }

let applyUpdate current update =
    update.replacement |> Option.defaultValue current
```

`Some ""` clears the text, while `None` keeps it unchanged. Also allow `Option.ofObj` when verified upstream constraints exclude both empty and whitespace-only strings.
