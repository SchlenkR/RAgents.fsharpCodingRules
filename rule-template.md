---
id: rule-id-in-kebab-case
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Author, Title: https://example.com/source"
---

# Imperative title that states the rule

The rule in one sentence, written as an instruction with the safe alternative. The reason in one sentence: what goes wrong without the rule.

## Check

1. What to look for in the changed code, as a question a reviewer can answer from the code.
2. Which evidence decides between violation and permitted case.

## Report

The violation and its correction, for example: the binding and its nested form.

## Do not report

- A permitted case, stated so that it can be decided from the code.
- Another permitted case.

## Examples (illustrative, not exhaustive)

```fsharp
let violation = ... // finding: why this violates the rule

// correction
let correction = ...
```

No finding: a near-miss that looks similar but is permitted.
