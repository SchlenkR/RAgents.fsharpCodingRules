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

Define a custom operator only in a module that is opened on purpose, such as a DSL or combinator module, and only as a one-line alias of a named function that callers can use instead.

## Look for

Look for custom operator definitions containing logic, operators exported through general domain modules, or redefinitions of standard operators with a different meaning:

```fsharp
let (|+|) (left: Map<_, _>) right =
    Map.fold (fun merged key value -> Map.add key value merged) left right
```

Inspect `static member` operators as well as `let` definitions.

## Check the context

1. Find a named function with the same semantics, including operand order and overwrite policy.
2. Inspect how the operator module is opened. Is it a deliberate DSL/combinator choice or an implicit addition to ordinary domain scope?
3. Check whether this is conventional arithmetic on a numeric/vector type or an established library contract.

## Report when

Report domain logic available only behind an operator, an operator that is more than a thin alias, or a surprising replacement for standard operator semantics. A named function supplies a searchable, readable API; optional operator syntax should delegate to it. State which behavior needs a name.

## Suggest

Expose the operation by name first:

```fsharp
let mergeRight (left: Map<_, _>) right =
    Map.fold (fun merged key value -> Map.add key value merged) left right
```

Use the named function in ordinary domain code. If the operator serves a deliberate combinator API, keep it as a one-line alias in that API's explicitly opened module.

## Do not report

Leave an operator alias in a deliberately selected parser module:

```fsharp
module ParserOperators =
    let inline (<|>) left right = Parser.orElse left right
```

Its named operation remains available and the module makes the syntax opt-in. Also exclude ordinary arithmetic operators on numeric types and documented operators required by an established library.
