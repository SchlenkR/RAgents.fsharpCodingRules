# Testing a rule

A rule works when a reviewer finds its violations and stays quiet on what it permits. Before you open a pull request, test a new or changed rule with at least three small cases:

1. a violation, which should produce a finding,
2. a near-miss from the rule's "Do not report" list, which should produce none,
3. an unrelated change, which should produce none.

Run each case in a fresh session, repeat it a few times, and use more than one model if you can. A wrong verdict means the rule text is unclear or the case is ambiguous. Fix the one that is wrong.

```markdown
You review a change against one F# coding rule. Do not edit any files.

<rule>
[full text of the rule file]
</rule>

<change>
[a small synthetic diff or file, with invented names]
</change>

Work through the rule's Check steps. Every item under "Do not report" is a permitted exception, and no finding is a normal result.

Answer with "finding" or "none" on the first line. Then give one line per finding: the line, the violation, the correction, and the Check step that found it.
```
