---
id: annotate-only-where-needed
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [annotate-public-signatures]
---

# Annotate types only where inference needs it

Write a type annotation only where the compiler needs it, not for clarity or out of habit, and decide by removing it and building.

## Look for

Look for parameter and return annotations on `let` functions whose bodies appear to determine the types already:

```fsharp
let length (text: string) : int = String.length text
```

The call to `String.length` already constrains both the input and result types.

## Check the context

1. Resolve calls, record labels, and union cases that constrain the type. Member access alone may require an annotation.
2. In a scratch copy or isolated checkout, remove candidate annotations together and build the affected project; caller annotations can mask dependencies between them.
3. Compare inferred signatures and relevant callers. Successful compilation alone does not prove an unchanged contract: annotations may constrain generalization, overloads, or units of measure.

## Report when

Report annotations that the compiler does not need and whose removal preserves the intended signature and behavior under this selected inference preference. Point to the concrete call or case that determines the type. Do not claim inference works without checking it.

## Suggest

Remove the redundant annotations while retaining needed constraints:

```fsharp
let length text = String.length text
```

Retain the parameter type when removing it changes the intended generic or overloaded contract.

## Do not report

Keep an annotation needed to resolve a member access:

```fsharp
let shout (text: string) = text.ToUpperInvariant()
```

Without the parameter type, the receiver is unresolved. Record and union field declarations, required member signatures, and annotations that preserve a unit of measure are also outside this finding.
