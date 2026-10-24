open System
open System.IO
open System.Text.RegularExpressions

type Rule =
    {
        path: string
        id: string
        category: string
        check: string
        alternatives: string list
        references: string list
        title: string
        core: string
        problems: string list
    }

let root =
    fsi.CommandLineArgs
    |> Array.skip 1
    |> Array.tryFind (fun arg -> not (arg.StartsWith "--"))
    |> Option.defaultValue (Path.Combine(__SOURCE_DIRECTORY__, ".."))
    |> Path.GetFullPath

let rulesDir = Path.Combine(root, "rules")
let indexPath = Path.Combine(root, "RULES.md")
let maxProseWords = 170

let categoryOrder =
    [ "formatting"; "naming"; "scope"; "types"; "functions"; "errors"; "design" ]

let expectedSections =
    [
        "## Check"
        "## Report"
        "## Do not report"
        "## Examples (illustrative, not exhaustive)"
    ]

let toRelativePath (path: string) =
    Path.GetRelativePath(root, path).Replace('\\', '/')

let parseRule (path: string) =
    let parseValues (value: string) =
        let quoted = Regex.Matches(value, "\"([^\"]*)\"")

        match value.Trim() with
        | "" -> []
        | "[]" -> []
        | _ when quoted.Count > 0 -> [ for m in quoted -> m.Groups[1].Value ]
        | trimmed -> [ trimmed.Trim('[', ']') ]

    let parseFields (header: string list) =
        let fields =
            let mutable fields = []

            for line in header do
                let item = Regex.Match(line, @"^\s+-\s+(.*)$")
                let field = Regex.Match(line, @"^([a-z-]+):\s*(.*)$")

                match fields with
                | (key, values) :: rest when item.Success ->
                    fields <- (key, values @ parseValues item.Groups[1].Value) :: rest
                | _ when field.Success -> fields <- (field.Groups[1].Value, parseValues field.Groups[2].Value) :: fields
                | _ -> ()

            fields

        Map.ofList fields

    let proseLines (body: string list) =
        let rec collect isInCode (lines: string list) =
            match lines with
            | [] -> []
            | line :: rest when line.TrimStart().StartsWith "```" -> collect (not isInCode) rest
            | _ :: rest when isInCode -> collect isInCode rest
            | line :: rest -> line :: collect isInCode rest

        collect false body

    let lines = File.ReadAllLines path |> Array.toList

    let header, body =
        match lines with
        | "---" :: rest when rest |> List.contains "---" ->
            let header = rest |> List.takeWhile (fun line -> line <> "---")
            header, rest |> List.skip (header.Length + 1)
        | _ -> [], lines

    let fields = parseFields header

    let field key =
        fields |> Map.tryFind key |> Option.defaultValue []

    let prose = proseLines body
    let id = Path.GetFileNameWithoutExtension path

    let category =
        Path.GetRelativePath(rulesDir, Path.GetDirectoryName path).Replace('\\', '/')

    let title =
        prose
        |> List.tryFind (fun line -> line.StartsWith "# ")
        |> Option.map (fun line -> line.Substring 2)

    let core =
        prose
        |> List.skipWhile (fun line -> not (line.StartsWith "# "))
        |> List.skip 1
        |> List.tryFind (fun line -> line.Trim() <> "")
        |> Option.map (fun line -> Regex.Match(line, @"^(.+?[.!?])(\s|$)").Groups[1].Value)
        |> Option.filter (fun sentence -> sentence <> "")

    let sections = prose |> List.filter (fun line -> line.StartsWith "## ")

    let proseWords =
        prose
        |> List.filter (fun line -> not (line.StartsWith "#"))
        |> List.sumBy (fun line -> Regex.Matches(line, @"\S+").Count)

    let problems =
        [
            if header.IsEmpty then
                "missing frontmatter"
            if field "id" <> [ id ] then
                $"id must be '{id}', the file name"
            if field "category" <> [ category ] then
                $"category must be '{category}', the folder"
            match field "check" with
            | [ "semantic" ]
            | [ "tool" ] -> ()
            | _ -> "check must be 'semantic' or 'tool'"
            if (field "files").IsEmpty then
                "files needs at least one glob"
            if category.StartsWith "libraries/" && (field "packages").IsEmpty then
                "library rules need packages"
            if title.IsNone then
                "missing '# Title'"
            if core.IsNone then
                "missing rule sentence below the title"
            if sections <> expectedSections then
                let expected = String.Join(", ", expectedSections)
                $"sections must be exactly: {expected}"
            if proseWords > maxProseWords then
                $"{proseWords} words without code, the limit is {maxProseWords}"
        ]

    {
        path = path
        id = id
        category = category
        check = field "check" |> List.tryHead |> Option.defaultValue ""
        alternatives = field "alternatives"
        references =
            [
                for m in Regex.Matches(String.Join("\n", prose), @"rule `([a-z0-9-]+)`") -> m.Groups[1].Value
            ]
        title = title |> Option.defaultValue id
        core = core |> Option.defaultValue ""
        problems = problems
    }

let rules =
    Directory.GetFiles(rulesDir, "*.md", SearchOption.AllDirectories)
    |> Array.filter (fun path -> Path.GetFileName path <> "README.md")
    |> Array.sort
    |> Array.map parseRule
    |> Array.toList

let crossProblems =
    let ruleById = rules |> List.map (fun rule -> rule.id, rule) |> Map.ofList

    [
        for id, count in rules |> List.countBy (fun rule -> rule.id) do
            if count > 1 then
                $"rule id '{id}' is used {count} times"
        for rule in rules do
            for alternative in rule.alternatives do
                match ruleById |> Map.tryFind alternative with
                | None -> $"{rule.id}: alternative '{alternative}' does not exist"
                | Some other when not (other.alternatives |> List.contains rule.id) ->
                    $"{rule.id}: '{alternative}' must list '{rule.id}' as alternative too"
                | Some _ -> ()

            for reference in rule.references do
                if not (ruleById.ContainsKey reference) then
                    $"{rule.id}: referenced rule '{reference}' does not exist"
    ]

let index =
    let categoryRank (category: string) =
        match categoryOrder |> List.tryFindIndex (fun known -> known = category) with
        | Some rank -> rank, category
        | None -> categoryOrder.Length, category

    let categoryTitle (category: string) =
        let readme = Path.Combine(rulesDir, category, "README.md")

        let title =
            if File.Exists readme then
                File.ReadAllLines readme
                |> Array.tryFind (fun line -> line.StartsWith "# ")
                |> Option.map (fun line -> line.Substring 2)
                |> Option.defaultValue category
            else
                category

        if category.StartsWith "libraries/" then
            $"Library: {title}"
        else
            title

    let ruleLine rule =
        let link = $"[{rule.title}]({toRelativePath rule.path})"
        let tool = if rule.check = "tool" then " (formatter or linter)" else ""

        let alternatives =
            match rule.alternatives with
            | [] -> ""
            | ids ->
                let names = String.Join(", ", ids |> List.map (fun id -> $"`{id}`"))
                $" Pick one: alternative to {names}."

        $"- {link} - {rule.core}{tool}{alternatives}"

    let sections =
        rules
        |> List.groupBy (fun rule -> rule.category)
        |> List.sortBy (fun (category, _) -> categoryRank category)
        |> List.map (fun (category, categoryRules) ->
            [
                $"## {categoryTitle category}"
                ""
                for rule in categoryRules do
                    ruleLine rule
                ""
            ])
        |> List.concat

    let lines =
        [
            "# Rule index"
            ""
            "Generated by `dotnet fsi scripts/check.fsx` from the rule files. Do not edit by hand."
            ""
            "One line per rule: its core sentence. Agents that write code load this file and open a rule file when"
            "they touch its subject or need its exceptions. Reviewers always get the full rule file."
            ""
            yield! sections
        ]

    String.Join("\n", lines).TrimEnd() + "\n"

let isVerify = fsi.CommandLineArgs |> Array.contains "--verify"

let problems =
    [
        for rule in rules do
            for problem in rule.problems do
                $"{toRelativePath rule.path}: {problem}"
        yield! crossProblems
        if isVerify && (not (File.Exists indexPath) || File.ReadAllText indexPath <> index) then
            "RULES.md is outdated, run: dotnet fsi scripts/check.fsx"
    ]

if not isVerify then
    File.WriteAllText(indexPath, index)

for problem in problems do
    eprintfn $"{problem}"

printfn $"{rules.Length} rules, {problems.Length} problems"
exit (if problems.IsEmpty then 0 else 1)
