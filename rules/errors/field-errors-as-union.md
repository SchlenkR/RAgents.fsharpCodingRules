---
id: field-errors-as-union
category: errors
check: semantic
files: ["**/*.{fs,fsx}"]
---

# Report input errors as a union, map them to fields in the view

Report invalid input as a union with one case per situation, and map each case to its field and message in exactly one exhaustive match in the view. Free-text errors with field names tie the domain to the markup, and a new case then reaches the user without a message.


## Look for

Look for domain validation errors carrying markup field names and ready-made messages, or several views independently classifying the same error.

```fsharp
type BookingError = InvalidField of field: string * message: string

let missingDate = InvalidField("date", "Please choose a date.")
```

## Check the context

1. Trace the error from validation to rendering. Is it a domain situation, a transport DTO, or an already rendered view model?
2. Inspect every mapping to field names and messages, including errors discovered after loading data, such as an occupied name.
3. Check whether adding a domain case makes the mapping non-exhaustive, or slips through a wildcard/free-text branch.

## Report when

Report domain input errors coupled to field names/messages, missing mappings, or duplicated presentation classification. Explain which domain situation is being encoded as text. One exhaustive view mapping makes new cases visible at compile time and lets views control their own field names and wording.

## Suggest

Represent situations as cases and keep the field/message mapping at the view boundary:

```fsharp
[<RequireQualifiedAccess>]
type BookingError =
    | DateMissing
    | DateInPast
    | TooManyGuests of maximum: int

let fieldError error =
    match error with
    | BookingError.DateMissing -> "date", "Please choose a date."
    | BookingError.DateInPast -> "date", "This date has passed."
    | BookingError.TooManyGuests maximum -> "guests", $"At most {maximum} guests."
```

Include post-lookup input rejections in the same domain error vocabulary; localize messages after classifying the case.

## Do not report

Keep workflow failures that do not belong to a form field:

```fsharp
[<RequireQualifiedAccess>]
type BookingFailure =
    | InvalidInput of BookingError list
    | ServiceUnavailable
```

`ServiceUnavailable` describes the whole operation and can be rendered at page level. A view model containing a field and message after the exhaustive mapping is also valid; it is already presentation data.
