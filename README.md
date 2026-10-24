# F# coding rules for AI agents

AI agents write a lot of F# now, and much of it reads like C# in another syntax: service classes without state, interfaces with one implementation, mutable variables, exceptions for every failure, and comments that narrate the next line. This repository collects coding rules that steer agents, and the models reviewing their work, towards idiomatic F#.

The rules are opinions. Some are common F# practice, some are personal taste, some only matter with one library. That is fine: every rule stands on its own, opposing positions sit side by side, and you pick the rules you agree with. Then add your own.

- [RULES.md](RULES.md): every rule in one line, generated from the rule files
- [rules/](rules/): the rule files, by category

The rules grew out of the F# conventions behind [RAgents](https://github.com/SchlenkR/RAgents), but they are plain Markdown and work with any agent: Claude Code, Codex, GitHub Copilot, Cursor, or a script of your own.

## What is in here

A few coarse categories, each a folder under [rules/](rules/):

- [formatting](rules/formatting/): layout only, mostly enforced by Fantomas.
- [naming](rules/naming/): names, conversions, and the few comments that remain.
- [scope](rules/scope/): where bindings live: nesting, shadowing, qualified access, file order.
- [types](rules/types/): domain modeling with records, unions, options, and units of measure.
- [functions](rules/functions/): pattern matching, pipelines, operators, parameters, local mutation.
- [errors](rules/errors/): `Result` versus exceptions, input errors, and where failures are caught.
- [async](rules/async/): `task` and `async`, awaiting, running in parallel, retries.
- [design](rules/design/): when to use a class or an interface, where I/O lives, how F# meets C#.
- [tests](rules/tests/): what a test asserts, how it is named, and which cases it covers.
- [libraries](rules/libraries/): rules that only apply with one library, so far [htmx](rules/libraries/htmx/) and [FsToolkit.ErrorHandling](rules/libraries/fstoolkit-errorhandling/).

Where opinions split, a rule lists its `alternatives` and the index marks the pair with "Pick one": brackets on lines of their own or at the end of the line, a helper nested in its only consumer or flat at module level, members on the type or functions in a companion module, and more.

## How a rule is built

Every rule is one Markdown file with one concern. It is written as a task a reviewer can carry out, and it doubles as an instruction for an agent that writes code:

```markdown
---
id: confine-mutation
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
sources:
  - "Don Syme, Notes on learning F#: https://github.com/dsyme/dsyme-presentations/blob/master/design-notes/notes-on-learning-fsharp-1.md"
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

- `id`: kebab-case, equals the file name; never changes once published, because other projects copy rules by id.
- `category`: equals the folder, for example `types` or `libraries/htmx`.
- `check`: `semantic` needs judgment and goes to a reviewer; `tool` is enforced by a formatter, a linter, or a plain search, which the rule names. Reviewers skip tool rules, writing agents still follow them.
- `files`: a one-line array of double-quoted globs of the files the rule applies to.
- `packages`: library rules only; the rule applies when the project references one of these NuGet packages.
- `alternatives`: optional; rules that take the opposite position. Both rules list each other.
- `sources`: optional; books, docs, or posts the rule comes from, one per line as `"Title: URL"`.

The first sentence below the title becomes the rule's line in [RULES.md](RULES.md), so it has to stand on its own. A reference to another rule is written as ``rule `rule-id` ``. Example code is formatted the way Fantomas formats it with the repository's [.editorconfig](.editorconfig), except where the violation itself is about layout. Examples are fragments: they annotate types they do not define, and they use invented names.

Why this shape:

- **One concern per file.** A reviewer answers a narrow question better than it applies a style guide, and a single rule can be loaded, tested, and replaced on its own. The AutoCommenter study at Google traced unhelpful comments, among other causes, to guidelines that mixed several topics or depended on contextual exceptions ([paper](https://arxiv.org/abs/2405.13565)); Graphite recommends one specific concern per rule ([docs](https://graphite.com/docs/ai-review-customization)).
- **Imperative and observable.** "Do X", plus the evidence that shows a violation. Short imperative directives work; vague quality directives such as "be more accurate" only add noise ([GitHub](https://docs.github.com/en/copilot/tutorials/customize-code-review)).
- **A reason.** One sentence of why lets a model handle cases the examples do not show.
- **Exceptions instead of hedging.** Precision comes from an explicit "Do not report" list, not from "only report if you are sure". OpenAI tests each review rule with a triggering change, a safe counterexample, and an unrelated change ([post](https://developers.openai.com/blog/custom-code-review-rules-for-codex)).
- **Contrasting examples.** A violation with its correction shows the safe path, a near-miss shows the boundary. Examples are marked illustrative so a reviewer does not hunt for that one pattern only.
- **Short.** Under 170 words of prose per rule, checked by the script together with the sections and the three parts of the examples. Instruction following degrades as instructions pile up ([IFScale](https://arxiv.org/abs/2507.11538), [When Instructions Multiply](https://aclanthology.org/2025.findings-emnlp.896/)), so writing agents load the one-line index and open a full rule only when they need it.
- **Tools first.** Whatever Fantomas or FSharpLint checks deterministically is a `tool` rule; models handle the judgment calls.
- **Generic review instructions in the prompt.** Scope, output format, and "no finding is a normal result" are the same for every rule, so they belong in your review prompt, not in each rule file.

## Pick your rules

1. Copy `rules/`, or only the categories you want, into your repository, for example to `docs/fsharp-rules/rules/`.
2. Delete what you disagree with and keep one rule of each alternative pair. Remove deleted ids from the `alternatives` of the rules you keep, and adapt references to deleted rules. It is your copy.
3. Copy `scripts/check.fsx` as well and regenerate your index with `dotnet fsi check.fsx docs/fsharp-rules`.
4. Take over the formatter and linter settings that your chosen tool rules name, such as the Fantomas bracket style.
5. Point your agent at the index, as described below.

If you sharpen a rule or write a new one, send it back as a pull request.

## Use the rules

The rules are plain Markdown, so you only tell your agent where they are. For example:

**When an agent writes code**, put a short block into the instructions it reads: `AGENTS.md` for Codex and others, `CLAUDE.md` or `.claude/rules/` for Claude Code, `.github/instructions/fsharp.instructions.md` for GitHub Copilot, or `.cursor/rules/` for Cursor.

```text
F# code in this repository follows the rules in docs/fsharp-rules/rules/. Before you write F#,
read docs/fsharp-rules/RULES.md. Before you change code in a rule's subject, read the full rule
file, including "Do not report". Run Fantomas instead of formatting by hand. When you are done,
check your diff against every rule whose subject you touched.
```

**When a model reviews code**, give each reviewer one rule, or a few closely related ones:

```text
Review my uncommitted F# changes against rules/errors/result-for-decisions.md. Work through its
Check steps, treat "Do not report" as permitted exceptions, and report only what the change
introduces, with file, line, and correction. No finding is a normal result.

Start one subagent per semantic rule in rules/design/. Each reviews this branch against main
with its rule in the same way. Then merge the findings and drop duplicates.
```

**When you test a rule**, check that a reviewer finds its violations and stays quiet on what it permits:

```text
Test rules/scope/nest-single-use-bindings.md. Write three small cases with invented names:
a violation, a near-miss from its "Do not report" list, and an unrelated change. Review each
case in a fresh subagent against the rule and tell me which verdict was wrong.
```

A wrong verdict can come from the reviewer, the rule, or the case. Repeat it with the same context first; if it stays wrong, fix the rule or the case.

## Contributing

Every F# developer has rules in their head that no style guide writes down. New rules, counter-positions, library folders, and sharper wording are all welcome:

1. Start from an existing rule of the same category, keep its sections, and name the file after its `id`.
2. Disagree with a rule? Write the counter-position as its own rule and link both through `alternatives`; the index then shows them as "Pick one".
3. Keep the top level coarse. A new library folder under `rules/libraries/` only needs a `README.md` that names the library.
4. Use invented names in examples; never paste code from your employer or a client.
5. Test the rule with the three cases above.
6. Run `dotnet fsi scripts/check.fsx` with the .NET 10 SDK. It validates every rule (frontmatter, sections, examples, word limit, alternatives, references) and regenerates [RULES.md](RULES.md); commit both. CI runs the same script with `--verify`.

In the pull request, say where the rule comes from: a book, a library's docs, or your own experience with agents getting it wrong. All three are valid.

## Sources

F#:

- Microsoft: [F# style guide](https://learn.microsoft.com/en-us/dotnet/fsharp/style-guide/), with formatting guidelines, coding conventions, and component design guidelines
- Scott Wlaschin: [Domain Modeling Made Functional](https://pragprog.com/titles/swdddf/domain-modeling-made-functional/) and [F# for Fun and Profit](https://fsharpforfunandprofit.com/)
- Don Syme: [design notes](https://github.com/dsyme/dsyme-presentations/tree/master/design-notes) on operators, currying, and learning F#
- Kit Eason: [Stylish F# 6](https://link.springer.com/book/10.1007/978-1-4842-7205-3)
- Isaac Abraham: [F# in Action](https://www.manning.com/books/f-sharp-in-action)
- Mark Seemann: [Impureim sandwich](https://blog.ploeh.dk/2020/03/02/impureim-sandwich/), [Dependency rejection](https://blog.ploeh.dk/2017/02/02/dependency-rejection/), and [Partial application is dependency injection](https://blog.ploeh.dk/2017/01/30/partial-application-is-dependency-injection/)
- htmx: [documentation](https://htmx.org/docs/), and the F# libraries [Giraffe.Htmx](https://github.com/bit-badger/Giraffe.Htmx), [Oxpecker.Htmx](https://github.com/Lanayx/Oxpecker), and [Falco.Htmx](https://github.com/falcoframework/Falco.Htmx)
- [FsToolkit.ErrorHandling](https://demystifyfp.gitbook.io/fstoolkit-errorhandling/)
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
