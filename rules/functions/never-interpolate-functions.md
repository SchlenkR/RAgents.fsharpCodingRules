---
id: never-interpolate-functions
category: functions
check: semantic
files: ["**/*.{fs,fsx}", "**/*.fsproj"]
---

# Apply a function before you interpolate it

Interpolate only values, apply a function before its result goes into a string, and treat warning FS3884 as an error.

## Look for

Look for interpolated expressions whose resolved type is a function, including URL helpers that still need an argument. Also check build warning FS3884 and its project-wide configuration:

```fsharp
let baseUrl locale = $"/shop/{locale}"
let searchLink locale = $"{baseUrl}/search"
```

The identifier is a function despite its value-like name.

## Check the context

1. Resolve the interpolated expression's inferred type, including partially applied calls and aliases. Identify the missing argument.
2. Check whether rendering the function itself is deliberately required for diagnostics or is accidentally replacing expected data.
3. Inspect `.fsproj` and inherited build settings for FS3884: an existing all-warnings-as-errors policy already covers it. Distinguish an absent policy from an actual interpolation bug.

## Report when

Report a function rendered where the string needs its computed result; closure text is not the intended URL or message. Show the omitted application and its required argument. Separately report an uncovered FS3884 policy when this rule is selected; do not duplicate a policy already inherited from build configuration.

## Suggest

Apply the function before interpolation:

```fsharp
let searchLink locale = $"{baseUrl locale}/search"
```

Where warning 3884 is not already an error, preserve existing settings when adding it:

```xml
<WarningsAsErrors>$(WarningsAsErrors);3884</WarningsAsErrors>
```

String concatenation can also make a missing string-producing application a type error, provided it preserves the intended formatting.

## Do not report

Leave a resolved value with meaningful string rendering:

```fsharp
let searchLink (baseUri: System.Uri) = $"{baseUri}/search"
```

Here `baseUri` is a value, not an unapplied helper. Do not infer a function type solely from a name or report warning configuration when inherited settings already enforce it.
