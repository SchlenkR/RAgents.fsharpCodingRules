## What this changes

- [ ] new rule
- [ ] counter-position to an existing rule (both list each other in `alternatives`)
- [ ] change to an existing rule
- [ ] other

## Checklist

- [ ] One concern per rule; the first sentence states the rule, the second its reason.
- [ ] Check steps describe evidence a reviewer can find in the code.
- [ ] "Do not report" lists the permitted cases.
- [ ] Examples show a violation, its correction, and a near-miss, with invented names.
- [ ] Tested with the three cases from `prompts/test-rule.md`.
- [ ] `dotnet fsi scripts/check.fsx` passes and `RULES.md` is regenerated.
