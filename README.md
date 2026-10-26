# F# coding rules for AI agents

AI agents write a lot of F# now, and much of it reads like C# in another syntax: service classes without state, interfaces with one implementation, mutable variables, exceptions for every failure, and comments that narrate the next line. This repository gives review agents concrete patterns to investigate, conditions for reporting them, and context-appropriate improvements. Agents that write code can follow the same rules.

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

Every rule is one Markdown file about one concern. It starts with the code a reviewer should look for, then explains how to decide whether that candidate deserves a finding. Code examples sit beside the instructions they illustrate.

For example, a resource lifetime check has this shape (see the full [scope-owned-resources](rules/scope/scope-owned-resources.md) rule):

````markdown
---
id: scope-owned-resources
category: scope
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Keep a resource alive for the work that uses it

Keep an owned resource alive until the work that reads it has completed.

## Look for

Look for a resource disposed by a local scope while returned work still captures it:

```fsharp
let contents (fileName: string) =
    use reader = System.IO.File.OpenText fileName
    seq { yield reader.ReadToEnd() }
```

## Check the context

1. Identify who owns the reader and when it is disposed.
2. Follow the returned sequence to determine when the read executes.
3. Check whether resource-dependent work is consumed before scope exit.

## Report when

Report when the read can run after disposal. Here the scope closes the reader
before the caller can enumerate the sequence.

## Suggest

If callers need lazy reads, let the sequence own its reader. Acquisition then
happens during enumeration, with a fresh reader for each enumeration:

```fsharp
let contents (fileName: string) =
    seq {
        use reader = System.IO.File.OpenText fileName
        yield reader.ReadToEnd()
    }
```

If callers need an eager snapshot, consume the read inside the original scope
instead. Choose according to the caller's contract.

## Do not report

Eager consumption completes the read before disposal:

```fsharp
let contents (fileName: string) =
    use reader = System.IO.File.OpenText fileName
    seq { yield reader.ReadToEnd() } |> Seq.toList
```

## Finding

Name the resource, its disposal boundary, and the operation that can run later.
````

The five required sections appear in that order:

- **Look for:** search instructions in prose and a candidate code example. Include consumers when the declaration alone cannot show the relevant relationship. Names and syntax may differ from the example.
- **Check the context:** the evidence to follow, such as call sites, data flow, ownership, effects, library versions, or framework contracts.
- **Report when:** the conditions that justify a finding and the reason they matter. A selected style preference is a valid reason; describe it as a preference rather than inventing a runtime defect.
- **Suggest:** a concrete correction with code and the conditions under which it applies. Explain relevant behavior changes or alternatives when no universal rewrite exists.
- **Do not report:** permitted cases, including a close code counterexample and the decisive difference.

An optional final **Finding** section can show the rule-specific evidence to include in a report. Omit it when it merely repeats the conditions and suggestion. Generic output fields, review scope, and the instruction that no finding is a normal outcome belong in the review prompt.

The frontmatter fields:

- `id`: kebab-case, equals the file name; never changes once published, because other projects copy rules by id.
- `category`: equals the folder, for example `types` or `libraries/htmx`.
- `check`: `semantic` needs judgment and goes to a reviewer; `tool` is enforced by a formatter, a linter, or a plain search, which the rule names. Reviewers skip tool rules, writing agents still follow them.
- `files`: a one-line array of double-quoted globs of the files the rule applies to.
- `packages`: library rules only; the rule applies when the project references one of these NuGet packages.
- `alternatives`: optional; rules that take the opposite position. Both rules list each other.
- `sources`: optional; books, docs, or posts the rule comes from, one per line as `"Title: URL"`.

The first sentence below the title becomes the rule's line in [RULES.md](RULES.md), so it has to stand on its own. A reference to another rule is written as ``rule `rule-id` ``. Format example code with the repository's [.editorconfig](.editorconfig), except when demonstrating a layout violation or an alternative formatter setting. Examples may be fragments; annotate types they do not define and use invented names.

Keep rules concise, but include the evidence needed to decide. There is no fixed prose word limit. The checker requires explanatory prose in each section and non-empty fenced code examples in **Look for**, **Suggest**, and **Do not report**. It checks structure, not F# correctness or reviewer accuracy.

A pattern match starts an investigation. It does not prove a violation: a local class may own a resource, a `bind` may be a straight pipeline, and a sequence may already be consumed eagerly. Conversely, follow equivalent relationships through helpers and call sites even when their spelling differs from an example. For conditions that depend on callers, show or inspect the callers instead of claiming "only one use" in a comment.

## Pick your rules

1. Copy `rules/`, or only the categories you want, into your repository, for example to `docs/fsharp-rules/rules/`.
2. Delete what you disagree with and keep one rule of each alternative pair. Remove deleted ids from the `alternatives` of the rules you keep, and adapt references to deleted rules. It is your copy.
3. Copy `scripts/check.fsx` as well and regenerate your index with `dotnet fsi check.fsx docs/fsharp-rules`.
4. Take over the formatter and linter settings that your chosen tool rules name, such as the Fantomas bracket style.
5. Point your agent at the index, as described below.

If you sharpen a rule or write a new one, send it back as a pull request.

## Context budget

The complete catalogue of 87 rules can fit in a single prompt, including all code examples:

| Content | Approximate tokens |
| --- | ---: |
| All rule files, including YAML metadata and sources | 53,000 |
| Rule bodies without YAML metadata and sources | 49,000 |
| The index (`RULES.md`) alone | 6,000 |
| One full rule on average | 600 |

These estimates use roughly four characters per token for about 213,000 characters in the rule files. Allow 50,000 to 60,000 tokens for the full catalogue, depending on the model's tokenizer; these are estimates, not model-specific token counts.

Even a mid-sized model with a 128k-token context window can therefore receive the entire selected rule set at once, leaving roughly 70k tokens for system and review instructions, code, and the response. The model's context window determines what fits; parameter count alone does not. Choose one rule from each alternative pair first, as described above. You can load the full selected rules directly, or use the index to load individual rules as needed.

## Use the rules

The rules are plain Markdown, so you only tell your agent where they are. For example:

**When an agent writes code**, put a short block into the instructions it reads: `AGENTS.md` for Codex and others, `CLAUDE.md` or `.claude/rules/` for Claude Code, `.github/instructions/fsharp.instructions.md` for GitHub Copilot, or `.cursor/rules/` for Cursor.

```text
F# code in this repository follows the rules in docs/fsharp-rules/rules/. Before you write F#,
read docs/fsharp-rules/RULES.md. Before you change code in a rule's subject, read the full rule
file, including "Do not report". Run Fantomas instead of formatting by hand. When you are done,
check your diff against every rule whose subject you touched.
```

**When a model reviews code**, load all selected rules if the context budget permits, or give each reviewer one rule or a few closely related ones:

```text
Review my uncommitted F# changes against rules/errors/result-for-decisions.md. Use Look for
to find candidates, follow Check the context, and establish the Report when conditions.
Use Do not report to check close permitted cases. Inspect relevant callers outside the diff
when needed; missing context does not prove a violation. Report only what the change introduces,
with file, line, evidence, and a context-appropriate suggestion. No finding is a normal result.

Start one subagent per semantic rule in rules/design/. Each reviews this branch against main
with its rule in the same way. Then merge the findings and drop duplicates.
```

**When you test a rule**, check that a reviewer finds its violations and stays quiet on what it permits:

```text
Test rules/scope/nest-single-use-bindings.md. Write small cases with invented names:
a violation satisfying Report when, a close permitted case from Do not report, and an unrelated
change. For contextual checks, also try missing evidence and an equivalent pattern written
differently. Review each case independently against the full rule. Check the verdict and
whether its explanation identifies the decisive evidence.
```

A wrong verdict can come from the reviewer, the rule, or the case. Repeat it with the same context first; if it stays wrong, fix the rule or the case.

## Contributing

Every F# developer has rules in their head that no style guide writes down. New rules, counter-positions, library folders, and sharper wording are all welcome:

1. Start from an existing rule of the same category, keep its sections, and name the file after its `id`.
2. Disagree with a rule? Write the counter-position as its own rule and link both through `alternatives`; the index then shows them as "Pick one".
3. Keep the top level coarse. A new library folder under `rules/libraries/` only needs a `README.md` that names the library.
4. Use invented names in examples; never paste code from your employer or a client.
5. Evaluate the rule with the cases above, checking both findings and permitted cases.
6. Run `dotnet fsi scripts/check.fsx` with the .NET 10 SDK. It validates every rule (frontmatter, section order, prose, examples, alternatives, references) and regenerates [RULES.md](RULES.md); commit both. CI runs the same script with `--verify`, checks the script formatting, and runs `dotnet fsi scripts/test-check.fsx` for validator regressions.

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
