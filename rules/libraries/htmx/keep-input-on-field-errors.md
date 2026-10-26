---
id: keep-input-on-field-errors
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
---

# Re-render a form with the submitted values

Render a form that failed validation with the submitted editable values, including selections and checkbox state, while clearing secrets and regenerating server-controlled values. Users who lose their input after an error must type it again and often miss what was wrong.


## Look for

Look for validation-error branches that re-render a form from an empty/default model or from persisted data instead of the submitted request.

```fsharp
match Booking.tryCreate request with
| Ok booking -> saveAndRedirect booking next ctx
| Error errors -> htmlView (BookingForm.view BookingForm.empty errors) next ctx
```

## Check the context

1. Trace submitted values into the error view, including syntactically invalid text that typed validation could not parse.
2. Inspect selects, radio buttons, checkboxes, and safe hidden inputs as well as text fields. Missing checkbox parameters may mean an unchecked state.
3. Identify password/secret fields, file inputs, and server-controlled hidden values. Their rendering constraints differ from ordinary editable input.

## Report when

Report an error response that discards or resets ordinary submitted values, forcing re-entry or changing a selection. Identify the lost value and the default replacing it. Preserve the user's text for correction rather than reconstructing it only from a partially validated domain object.

## Suggest

Build the form model from submitted input on the error path:

```fsharp
match Booking.tryCreate request with
| Ok booking -> saveAndRedirect booking next ctx
| Error errors -> htmlView (BookingForm.view (BookingForm.ofRequest request) errors) next ctx
```

`ofRequest` must keep editable raw values and selections while excluding secrets. Encode them with the view engine; regenerate server-controlled tokens or authoritative identifiers rather than trusting arbitrary hidden input. Preserve the successful save/redirect path.

## Do not report

Keep a model that retains the email but deliberately clears the password:

```fsharp
let retryForm = { submittedForm with password = "" }
```

Here ordinary fields still reflect the submission and the secret is not returned. File-input browser restrictions and regenerated anti-forgery/security fields also need their own handling; this rule does not require echoing every request byte.
