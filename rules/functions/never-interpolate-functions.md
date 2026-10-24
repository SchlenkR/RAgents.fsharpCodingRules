---
id: never-interpolate-functions
category: functions
check: semantic
files: ["**/*.{fs,fsx}", "**/*.fsproj"]
---

# Apply a function before you interpolate it

Interpolate only values, apply a function before its result goes into a string, and treat warning FS3884 as an error. An interpolated function compiles and renders its closure name, such as `Program+link@12/search`, which looks plausible in a URL and fails only at run time.

## Check

1. Does the build report FS3884, "This expression is a function value"?
2. For every interpolated name: is it a function in this code base, such as a URL builder that takes a locale or an ID?
3. Is FS3884 configured as an error in the project file?

## Report

The interpolation and the applied call, or a concatenation, which the compiler type-checks.

## Do not report

- Values whose `ToString` is meaningful, such as `Uri` or `DateOnly`.

## Examples (illustrative, not exhaustive)

```fsharp
let searchLink locale = $"{baseUrl}/search" // finding: baseUrl takes a locale

// correction
let searchLink locale = $"{baseUrl locale}/search"
```

No finding: `$"{baseUri}/search"` where `baseUri` is a value of type `Uri`.
