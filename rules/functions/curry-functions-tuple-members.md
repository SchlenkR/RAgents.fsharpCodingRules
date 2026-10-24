---
id: curry-functions-tuple-members
category: functions
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Don Syme, On currying: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/on-currying.md"
  - "Microsoft F# component design guidelines: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
---

# Curry module functions and tuple members

Give functions in modules curried parameters, ordered from the least to the most varying with the piped data last, and give members of types that .NET code calls tupled parameters. Curried functions pipe and partially apply; tupled members allow overloads and named or optional arguments and look normal from C#.

## Check

1. For every new module function with tupled parameters `(a, b)`: is it meant for C# callers or overloading? If not, curry it.
2. Does the value that is usually piped, such as the collection or the state, come last?
3. For every new member of a type used from C#: are its parameters tupled?

## Report

The signature and its curried or tupled form.

## Do not report

- A tuple that is one value, such as a coordinate `(x, y)`.
- Functions passed to .NET APIs that expect a `Func`.

## Examples (illustrative, not exhaustive)

```fsharp
let addDays (days: int, date: DateOnly) = date.AddDays days // finding: cannot be partially applied as addDays 30

// correction
let addDays (days: int) (date: DateOnly) = date.AddDays days
let due = invoiceDate |> addDays 30
```

No finding: `member this.Resize(width: int, height: int)` on a class that C# code calls.
