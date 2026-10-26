---
id: comments-say-why
category: naming
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Comment only what the code cannot say

Write a comment only for what names and types cannot express, such as a constraint of an external system or the reason for a surprising decision, in one or two plain lines. Comments that narrate the code go stale, and they are the most visible trace of generated code.


## Look for

Look for added comments that repeat nearby names or operations, long explanations inside functions, section labels, commented-out code, and TODOs without a tracked issue.

```fsharp
// Get all unpaid invoices
let unpaid = invoices |> List.filter _.paidAt.IsNone
```

## Check the context

1. Read the code without the comment. Is any necessary fact lost, or does the name already express it?
2. Inspect whether a stronger name, type, or focused test can carry the information instead.
3. For a constraint or surprising decision, check whether the comment explains the reason in one or two plain lines and whether that reason is documented elsewhere.

## Report when

Report narration, stale or redundant information, abandoned code, untracked TODOs, or unnecessary prose longer than the selected brief-comment policy allows. Quote the redundant idea and explain whether deletion or an improved name preserves the useful information. Review only comments touched by the change.

## Suggest

Delete a comment whose information is already in the code:

```fsharp
let unpaid = invoices |> List.filter _.paidAt.IsNone
```

If the comment carries an essential external constraint, keep a brief reason next to the affected code. If it explains a domain concept that the code repeatedly needs, suggest expressing that concept in a name or type instead of merely shortening the prose.

## Do not report

Keep a short explanation of an external constraint:

```fsharp
// The payment provider rejects amounts with more than two decimals.
let providerAmount = System.Math.Round(total, 2)
```

The rounding expression cannot explain the provider's restriction. Public-library API documentation, tool directives such as `// fsharplint:disable`, and untouched existing comments are also outside this check; do not remove information merely because it appears in a comment.
