# Contributing

Every F# developer has rules in their head that no style guide writes down. Bring them in: a new rule, a counter-position to an existing one, rules for a library you use, or a sharper wording of a rule that misfires.

## What makes a good rule

- **One concern.** If the rule needs "and also", it is two rules.
- **An instruction with its safe alternative.** "Use X instead of Y", not "avoid bad Y".
- **A reason.** One sentence on what goes wrong without the rule.
- **Observable.** A reviewer must be able to decide from the code whether the rule is violated. "Write clean functions" is not a rule; "a function that both decides and performs I/O" is.
- **Explicit exceptions.** Every case where the rule does not apply belongs under "Do not report". Do not hedge in the rule text instead.
- **Contrasting examples.** A violation marked `// finding`, its correction, and a near-miss that is fine. Use invented names; never paste code from your employer or a client.
- **Short.** At most 170 words of prose, code not counted. The script checks this.
- **F#-specific where possible.** Generic advice that applies to every language is welcome only when agents get it wrong in F# in particular.

## Rule file format

Copy [rule-template.md](rule-template.md) into the folder of its category and name it after its `id`.

Frontmatter:

- `id`: kebab-case, equals the file name. Do not rename published rules; other projects copy them by id.
- `category`: the folder below `rules/`, for example `errors` or `libraries/htmx`.
- `check`: `semantic` if a reviewer has to judge, `tool` if Fantomas, FSharpLint, or a plain search enforces it. Name the tool or setting in the rule.
- `files`: a one-line array of double-quoted globs, such as `["**/*.{fs,fsx}"]`.
- `packages`: required for library rules, the NuGet packages the rule applies to.
- `alternatives`: optional, ids of rules that take the opposite position. Both rules must list each other.
- `sources`: optional, books, docs, or posts the rule comes from, one per line as `"Title: URL"`.

Body, in this order and with exactly these headings:

1. `# Title`: imperative, states the rule.
2. Two sentences: the rule, then its reason. The first sentence becomes the rule's line in [RULES.md](RULES.md), so it must stand on its own.
3. `## Check`: numbered steps, each a question a reviewer answers from the code.
4. `## Report`: the shape of a finding, the violation and its correction.
5. `## Do not report`: the permitted cases.
6. `## Examples (illustrative, not exhaustive)`: violation, correction, near-miss.

Refer to another rule as ``rule `rule-id` ``; the script checks that it exists. Write example code the way Fantomas formats it with the repository's [.editorconfig](.editorconfig), except where the violation itself is about layout.

## Disagree with a rule?

Write the counter-position as its own rule and link both through `alternatives`. The index then shows them as "Pick one", and every reader chooses. A short, well-argued alternative helps more people than a long discussion in a pull request.

## Categories

Keep the top level coarse: formatting, naming, scope, types, functions, errors, design. A new top-level category needs a good reason; a new library folder under `rules/libraries/` only needs a `README.md` that names the library.

## Before you open a pull request

1. Test the rule with the three cases from [prompts/test-rule.md](prompts/test-rule.md): a violation, a near-miss, and an unrelated change.
2. Run `dotnet fsi scripts/check.fsx`. It validates all rules and regenerates `RULES.md`; commit the regenerated file.
3. If you changed the script, run `dotnet tool restore` and `dotnet fantomas scripts`.

Mention in the pull request where the rule comes from: a book, a library's docs, or your own experience with agents getting it wrong. All three are valid.
