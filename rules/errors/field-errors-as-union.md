---
id: field-errors-as-union
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Report input errors as a union, map them to fields in the view

Report invalid input as a union with one case per situation, and map each case to its field and message in exactly one exhaustive match in the view. Free-text errors with field names tie the domain to the markup, and a new case then reaches the user without a message.

## Check

1. Do error types carry field names or message texts (`InvalidField of field: string * message: string`)?
2. Is there exactly one exhaustive match that maps each case to its field and text?
3. Are input errors that only appear after loading data, such as an already taken name, cases of the same union?

## Report

The free-text error and the union with its single mapping.

## Do not report

- Failures without a field, such as an unavailable service; they belong to the workflow's failure type.

## Examples (illustrative, not exhaustive)

```fsharp
type BookingError = InvalidField of field: string * message: string // finding: domain knows fields and texts

// correction
[<RequireQualifiedAccess>]
type BookingError =
    | DateMissing
    | DateInPast
    | TooManyGuests of maximum: int

let fieldError (error: BookingError) =
    match error with
    | BookingError.DateMissing -> "date", "Please choose a date."
    | BookingError.DateInPast -> "date", "This date has passed."
    | BookingError.TooManyGuests maximum -> "guests", $"At most {maximum} guests."
```

No finding: `ServiceUnavailable` in the workflow's failure type, shown as a message for the whole page.
