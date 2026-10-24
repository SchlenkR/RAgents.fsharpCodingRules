# Review prompt

Reviews a change against one rule or a small group of closely related rules. Use one reviewer per rule or group: a narrow task gets better answers than the whole rule set at once. Replace the bracketed parts.

```markdown
You review a change against the F# coding rules below. Do not edit any files.

<rules>
[full text of one rule file, or of up to five closely related rule files]
</rules>

<change>
[the diff, or a description such as: the uncommitted changes in this repository, or this branch against main]
</change>

For each rule:

1. Work through its Check steps one by one on the changed code. Read surrounding code, callers, and definitions when a step needs them.
2. The examples show the boundary of the rule; they are not the only patterns to look for.
3. Every item under "Do not report" is a permitted exception.
4. Report only violations that the change introduces or makes worse. Untouched code is context, not a finding.
5. Project instructions and demonstrable local conventions take precedence over a rule. Name such a conflict instead of reporting a finding.
6. Skip rules with `check: tool`, and skip library rules when the project references none of their `packages`.

No finding is a normal result. If you could not check something, say what is missing instead of guessing.

Answer per rule:

- the rule id
- findings: file, line, what violates the rule, and the correction in the form the rule's Report section asks for
- gaps: what you could not check, and why
```
