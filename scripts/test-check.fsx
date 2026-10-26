open System
open System.Diagnostics
open System.IO

type CheckResult = { code: int; output: string }

let check root verify =
    let start = ProcessStartInfo("dotnet")
    start.ArgumentList.Add "fsi"
    start.ArgumentList.Add(Path.Combine(__SOURCE_DIRECTORY__, "check.fsx"))
    start.ArgumentList.Add root

    if verify then
        start.ArgumentList.Add "--verify"

    start.RedirectStandardOutput <- true
    start.RedirectStandardError <- true
    start.UseShellExecute <- false

    use child = Process.Start start
    let output = child.StandardOutput.ReadToEndAsync()
    let errors = child.StandardError.ReadToEndAsync()
    child.WaitForExit()

    { code = child.ExitCode; output = output.Result + errors.Result }

let sample =
    """---
id: demo
category: functions
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Demo rule

Keep the chosen value immutable.

## Look for

Look for a reassigned value with no subsequent updates.

```fsharp
let mutable value = 1
```

## Check the context

Inspect writes throughout the value's scope.

## Report when

Report when no later write needs a mutable binding.

## Suggest

Use an immutable binding.

```fsharp
let value = 1
```

## Do not report

Leave mutation that builds a local result.

```fsharp
let value =
    let mutable total = 0
    for item in items do
        total <- total + item
    total
```
"""

let withoutExample (heading: string) (text: string) =
    let start = text.IndexOf(heading, StringComparison.Ordinal)
    let opening = text.IndexOf("```", start, StringComparison.Ordinal)
    let closing = text.IndexOf("```", opening + 3, StringComparison.Ordinal)
    text.Remove(opening, closing + 3 - opening)

let cases =
    [
        "required sections", sample, None
        "optional finding", sample + "\n## Finding\n\nName the unused mutability and its scope.\n", None
        "long context", sample.Replace("Inspect writes", String.replicate 200 "Evidence " + "Inspect writes"), None
        "heading inside code", sample.Replace("let value = 1", "## This heading belongs to the code example"), None
        "reference inside code", sample.Replace("let value = 1", "rule `not-a-rule`"), None
        "missing section", sample.Replace("## Report when", "### Report when"), Some "sections must be in order"
        "misordered sections",
        sample.Replace("## Check the context", "## Suggest").Replace("## Suggest\n\nUse", "## Check the context\n\nUse"),
        Some "sections must be in order"
        "missing candidate", withoutExample "## Look for" sample, Some "'## Look for' needs a non-empty code example"
        "missing correction", withoutExample "## Suggest" sample, Some "'## Suggest' needs a non-empty code example"
        "missing counterexample",
        withoutExample "## Do not report" sample,
        Some "'## Do not report' needs a non-empty code example"
        "empty example",
        sample.Replace("let mutable value = 1", ""),
        Some "'## Look for' needs a non-empty code example"
        "code without prose",
        sample.Replace("Use an immutable binding.", ""),
        Some "'## Suggest' needs explanatory prose"
        "empty context",
        sample.Replace("Inspect writes throughout the value's scope.", ""),
        Some "'## Check the context' needs explanatory prose"
        "unclosed fence", sample + "\n```fsharp\nlet other = 2\n", Some "unbalanced code fences"
        "wrong id", sample.Replace("id: demo", "id: other"), Some "id must be 'demo'"
        "unknown reference",
        sample.Replace("Inspect writes", "Follow rule `not-a-rule` and inspect writes"),
        Some "referenced rule 'not-a-rule' does not exist"
    ]

let scratch =
    Path.Combine(Path.GetTempPath(), $"fsharp-rule-check-{Guid.NewGuid():N}")

let fixture name (content: string) =
    let root = Path.Combine(scratch, name)
    let directory = Path.Combine(root, "rules", "functions")
    Directory.CreateDirectory directory |> ignore
    File.WriteAllText(Path.Combine(directory, "demo.md"), content)
    root

try
    for name, content, expectedProblem in cases do
        let root = fixture name content
        let index = Path.Combine(root, "RULES.md")
        File.WriteAllText(index, "existing index")
        let result = check root false

        match expectedProblem with
        | Some problem ->
            if result.code = 0 || not (result.output.Contains problem) then
                failwith $"{name}: expected '{problem}', got exit {result.code}: {result.output}"

            if File.ReadAllText index <> "existing index" then
                failwith $"{name}: an invalid rule overwrote the index"
        | None ->
            if result.code <> 0 then
                failwith $"{name}: expected a valid rule: {result.output}"

            if not ((File.ReadAllText index).Contains "Keep the chosen value immutable.") then
                failwith $"{name}: the generated index lost the summary"

            let verified = check root true

            if verified.code <> 0 then
                failwith $"{name}: a generated index did not verify: {verified.output}"

    let root = fixture "stale index" sample
    let index = Path.Combine(root, "RULES.md")
    File.WriteAllText(index, "stale index")
    let result = check root true

    if result.code = 0 || not (result.output.Contains "RULES.md is outdated") then
        failwith $"verify accepted a stale index: {result.output}"

    if File.ReadAllText index <> "stale index" then
        failwith "verify modified the stale index"

    printfn $"{cases.Length + 1} validator cases passed"
finally
    if Directory.Exists scratch then
        Directory.Delete(scratch, true)
