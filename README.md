# F# coding rules for AI agents

AI agents write a lot of F# now, and much of it reads like C# in another syntax: service classes without state, interfaces with one implementation, mutable variables, exceptions for every failure, and comments that narrate the next line. Models have seen far more C# than good F#. This repository collects coding rules that steer agents, and the models reviewing their work, towards idiomatic F#.

The rules are opinions. Some are common F# practice, some are personal taste, some only matter with one library. That is fine: every rule stands on its own, opposing positions sit side by side, and you pick the rules you agree with. Then add your own.

- [RULES.md](RULES.md): every rule in one line
- [rules/](rules/): the rule files, by category
- [prompts/](prompts/): prompts for writing code, reviewing code, and testing rules
- [CONTRIBUTING.md](CONTRIBUTING.md): how to add a rule or a counter-position

The rules grew out of the F# conventions behind [RAgents](https://github.com/SchlenkR/RAgents), but they are plain Markdown and work with any agent: Claude Code, Codex, GitHub Copilot, Cursor, or a script of your own.

## What is in here

A few coarse categories, each a folder under [rules/](rules/):

- [formatting](rules/formatting/): layout only, mostly enforced by Fantomas.
- [naming](rules/naming/): names, conversions, and the few comments that remain.
- [scope](rules/scope/): where bindings live: nesting, shadowing, qualified access, file order.
- [types](rules/types/): domain modeling with records, unions, options, and units of measure.
- [functions](rules/functions/): pattern matching, pipelines, operators, parameters, local mutation.
- [errors](rules/errors/): `Result` versus exceptions, and where failures are caught.
- [design](rules/design/): when to use a class or an interface, where I/O lives, how F# meets C#.
- [libraries](rules/libraries/): rules that only apply with one library, so far [htmx](rules/libraries/htmx/) and [FsToolkit.ErrorHandling](rules/libraries/fstoolkit-errorhandling/).

Where opinions split, a rule lists its `alternatives` and the index says "Pick one". Three pairs exist so far, one per level:

- Formatting: [brackets on lines of their own](rules/formatting/multiline-brackets-aligned.md) or [brackets at the end of the line](rules/formatting/multiline-brackets-stroustrup.md).
- Scope: [nest a helper in its only consumer](rules/scope/nest-single-use-bindings.md) or [keep helpers flat at module level](rules/scope/top-level-helpers.md).
- Design: [members on the type](rules/design/members-for-own-data.md) or [functions in a companion module](rules/design/functions-in-companion-module.md).

## How a rule is built

Every rule is one Markdown file with one concern. It is written as a task a reviewer can carry out, and it doubles as an instruction for an agent that writes code:

```markdown
---
id: confine-mutation
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Don Syme, Notes on learning F#: https://github.com/dsyme/dsyme-presentations/..."
---

# Confine mutation to the block that builds a value

The rule in one sentence. Its reason in one sentence.

## Check

1. Numbered steps: what to look for, and which evidence decides.

## Report

The shape of a finding: the violation and its correction.

## Do not report

- Permitted exceptions, stated so that they can be decided.

## Examples (illustrative, not exhaustive)

A violation marked `// finding`, its correction, and a near-miss that is fine.
```

The frontmatter fields:

- `id`: equals the file name; never changes once published.
- `category`: equals the folder, for example `types` or `libraries/htmx`.
- `check`: `semantic` needs judgment and goes to a reviewer; `tool` is enforced by a formatter or linter, so reviewers skip it while writing agents still follow it.
- `files`: globs of the files the rule applies to.
- `packages`: library rules only; the rule applies when the project references one of these NuGet packages.
- `alternatives`: optional; rules that take the opposite position.
- `sources`: optional; where the rule comes from.

Why this shape:

- **One concern per file.** A reviewer answers a narrow question better than it applies a style guide, and a single rule can be loaded, tested, and replaced on its own. Google's AutoCommenter got its least useful comments from guidelines that mixed several concerns or had subjective exceptions ([paper](https://arxiv.org/abs/2405.13565)); Graphite recommends one specific concern per rule ([docs](https://graphite.com/docs/ai-review-customization)).
- **Imperative and observable.** "Do X", plus the evidence that shows a violation. Short imperative directives work; vague quality directives such as "be more accurate" only add noise ([GitHub](https://docs.github.com/en/copilot/tutorials/customize-code-review)).
- **A reason.** One sentence of why lets a model handle cases the examples do not show.
- **Exceptions instead of hedging.** Precision comes from an explicit "Do not report" list, not from "only report if you are sure". OpenAI tests each review rule with a triggering change, a safe counterexample, and an unrelated change ([post](https://developers.openai.com/blog/custom-code-review-rules-for-codex)).
- **Contrasting examples.** A violation with its correction shows the safe path, a near-miss shows the boundary. Examples are marked illustrative so a reviewer does not hunt for that one pattern only.
- **Short.** Under 170 words of prose per rule, checked by the script. Instruction following degrades as instructions pile up ([IFScale](https://arxiv.org/abs/2507.11538), [When Instructions Multiply](https://aclanthology.org/2025.findings-emnlp.896/)), so writing agents load the one-line index and open a full rule only when they need it.
- **Tools first.** Whatever Fantomas or FSharpLint checks deterministically is a `tool` rule; models handle the judgment calls.
- **Generic review instructions once.** Scope, output format, and "no finding is a normal result" live in [prompts/review.md](prompts/review.md), not in every rule.

## Pick your rules

1. Copy `rules/`, or only the categories you want, into your repository, for example to `docs/fsharp-rules/rules/`.
2. Delete what you disagree with, keep one rule of each alternative pair, and adapt the rest. It is your copy.
3. Copy `scripts/check.fsx` as well and regenerate your index with `dotnet fsi check.fsx docs/fsharp-rules`.
4. Point your agent at the index, as described below.

If you sharpen a rule or write a new one, send it back as a pull request.

## Use the rules

**When an agent writes code**, it reads [RULES.md](RULES.md) first and opens a full rule before it touches that rule's subject. Paste the block from [prompts/write.md](prompts/write.md) into the instructions your agent reads: `AGENTS.md` for Codex and others, `CLAUDE.md` for Claude Code, `.github/instructions/fsharp.instructions.md` for GitHub Copilot, or a rule in `.cursor/rules/` for Cursor.

**When a model reviews code**, give each reviewer one rule, or a few closely related ones, together with [prompts/review.md](prompts/review.md) and the change. Requests like these work in an agent session:

```text
Review my uncommitted F# changes against rules/errors/result-for-decisions.md. Follow prompts/review.md.

Review src/Billing.fs against each semantic rule in rules/types/, one rule at a time,
and list the findings per rule id.

Start one subagent per semantic rule in rules/design/. Each reviews this branch against main
with prompts/review.md and its rule. Then merge the findings and drop duplicates.

Implement the import described in issue 12. Follow RULES.md, and read the full rule file
for every rule whose subject you touch. Afterwards, check your diff against those rules.
```

**When you test a rule**, run it against small synthetic changes with [prompts/test-rule.md](prompts/test-rule.md):

```text
Test rules/scope/nest-single-use-bindings.md with prompts/test-rule.md. Write three small cases:
a violation, a near-miss from its "Do not report" list, and an unrelated change. Run each case
in a fresh subagent and tell me which verdict was wrong.
```

A wrong verdict means the rule is unclear or the case is ambiguous. Fix whichever is wrong.

## Contributing

New rules, counter-positions, library folders, and sharper wording are all welcome. Read [CONTRIBUTING.md](CONTRIBUTING.md), copy [rule-template.md](rule-template.md), and run

```sh
dotnet fsi scripts/check.fsx
```

It validates every rule (frontmatter, sections, word limit, alternatives, references between rules) and regenerates [RULES.md](RULES.md). CI runs the same script with `--verify`.

## Sources

F#:

- Microsoft: [F# style guide](https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/), with formatting guidelines, coding conventions, and component design guidelines
- Scott Wlaschin: [Domain Modeling Made Functional](https://pragprog.com/titles/swdddf/domain-modeling-made-functional/) and [F# for Fun and Profit](https://fsharpforfunandprofit.com/)
- Don Syme: [design notes](https://github.com/dsyme/dsyme-presentations/tree/master/design-notes) on operators, currying, and learning F#
- Kit Eason: [Stylish F# 6](https://link.springer.com/book/10.1007/978-1-4842-7205-3)
- Isaac Abraham: [F# in Action](https://www.manning.com/books/f-sharp-in-action)
- Mark Seemann: [Impureim sandwich](https://blog.ploeh.dk/2020/03/02/impureim-sandwich/) and [Dependency rejection](https://blog.ploeh.dk/2017/02/02/dependency-rejection/)
- [Fantomas](https://fsprojects.github.io/fantomas/), [FSharpLint](https://fsprojects.github.io/FSharpLint/), and the [G-Research F# formatting conventions](https://github.com/G-Research/fsharp-formatting-conventions)

Writing rules for models:

- Anthropic: [Claude Code memory and instructions](https://code.claude.com/docs/en/memory)
- GitHub: [Customizing Copilot code review](https://docs.github.com/en/copilot/tutorials/customize-code-review)
- OpenAI: [Custom code review rules for Codex](https://developers.openai.com/blog/custom-code-review-rules-for-codex)
- Graphite: [AI review customization](https://graphite.com/docs/ai-review-customization)
- Cursor: [Rules](https://cursor.com/docs/rules)
- [AI-Assisted Assessment of Coding Practices in Modern Code Review](https://arxiv.org/abs/2405.13565) (Google AutoCommenter, 2024)
- [How Many Instructions Can LLMs Follow at Once?](https://arxiv.org/abs/2507.11538) (2025)

## License

[MIT](LICENSE)
