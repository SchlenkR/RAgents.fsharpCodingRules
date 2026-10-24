---
id: keep-input-on-field-errors
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
---

# Re-render a form with the submitted values

Render a form that failed validation again with every value the user submitted, including selections, checkboxes, and hidden inputs, and leave only password fields empty. Users who lose their input after an error must type it again and often miss what was wrong.

## Check

1. For every error branch of a form handler: does the form view receive the submitted values, or an empty or default model?
2. Do selects, radio buttons, checkboxes, and hidden inputs keep their submitted state, not only text fields?

## Report

The error branch and the form model built from the request.

## Do not report

- Password and other secret fields, which are never sent back.

## Examples (illustrative, not exhaustive)

```fsharp
match Booking.tryCreate request with
| Ok booking -> saveAndRedirect booking next ctx
| Error errors -> htmlView (BookingForm.view BookingForm.empty errors) next ctx // finding: the form comes back empty

// correction
match Booking.tryCreate request with
| Ok booking -> saveAndRedirect booking next ctx
| Error errors -> htmlView (BookingForm.view (BookingForm.ofRequest request) errors) next ctx
```

No finding: a sign-up form that shows the email again but leaves the password field empty.
