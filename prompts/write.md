# Instructions for agents that write F#

Paste the block below into the instructions your agent reads and adjust the paths:

- Codex and other agents: `AGENTS.md`
- Claude Code: `CLAUDE.md`, or a file under `.claude/rules/` with `paths` for F# files
- GitHub Copilot: `.github/instructions/fsharp.instructions.md` with `applyTo: "**/*.fs,**/*.fsx,**/*.fsi"` in its frontmatter
- Cursor: a rule in `.cursor/rules/` with `globs` for F# files

```markdown
## F# coding rules

F# code in this repository follows the rules in `docs/fsharp-rules/rules/`. `docs/fsharp-rules/RULES.md` lists each rule in one line.

- Before you write F#, read `docs/fsharp-rules/RULES.md`.
- Before you change code in a rule's subject, or when its one-line form leaves a question open, read the full rule file, including "Do not report".
- Rules with `check: tool` are enforced by Fantomas or FSharpLint. Run the tool instead of formatting by hand.
- The task, this file, and the demonstrable conventions of the surrounding code take precedence over a rule. When they conflict, say which one you followed.
- When you are done, check your diff against every rule whose subject you touched and fix what violates it.
```
