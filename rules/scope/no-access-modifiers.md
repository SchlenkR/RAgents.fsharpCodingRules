---
id: no-access-modifiers
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
alternatives: [hide-implementation-details]
---

# Leave access modifiers out

Write functions, values, types, and modules without `private` or `internal`, and keep `private` only on the case of a validated single-case union. Modules and file order already structure F# code, while modifiers lock functions away from tests and scripts and have to be removed again when a second consumer appears.

## Check

1. Look for `let private`, `let internal`, `module private`, `module internal`, `type private`, and `member private` in new code.
2. Is the modifier on the case of a validated union (rule `wrap-constrained-primitives`)?

## Report

The modifier and its removal.

## Do not report

- Private cases of validated single-case unions.
- Published libraries whose public surface is part of their contract.

## Examples (illustrative, not exhaustive)

```fsharp
let private normalize (key: string) = key.Trim().ToLowerInvariant() // finding

// correction
let normalize (key: string) = key.Trim().ToLowerInvariant()
```

No finding: `type EmailAddress = private EmailAddress of string`.
