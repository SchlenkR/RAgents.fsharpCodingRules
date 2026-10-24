---
id: operators-as-named-aliases
category: functions
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
sources:
  - "Don Syme, On teaching operators: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/on-teaching-operators.md"
  - "Microsoft F# component design guidelines, operator definitions: https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/component-design-guidelines"
---

# Define operators only as aliases of named functions

Define a custom operator only in a module that is opened on purpose, such as a DSL or combinator module, and only as a one-line alias of a named function that callers can use instead. An operator symbol cannot be searched or pronounced, and readers who do not know the module cannot guess its meaning.

## Check

1. For every new operator, `let (<+>) ...` or `static member (+)`: does a named function with the same behavior exist, and is the operator a one-line alias of it?
2. Is the operator in a module opened deliberately, not in general domain code?
3. Look for operators that redefine standard symbols with another meaning, such as `|>` or `>>`.

## Report

The operator and the named function that replaces it or stands behind it.

## Do not report

- Arithmetic operators on numeric types such as `Money` or a vector.
- Documented operators of established libraries, such as `>=>` in Giraffe.

## Examples (illustrative, not exhaustive)

```fsharp
let (|+|) (left: Map<_, _>) right = // finding
    Map.fold (fun merged key value -> Map.add key value merged) left right

// correction
let mergeRight (left: Map<_, _>) right =
    Map.fold (fun merged key value -> Map.add key value merged) left right
```

No finding: `let inline (<|>) left right = Parser.orElse left right` in a parser module.
