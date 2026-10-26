---
id: abstract-on-second-use
category: design
check: semantic
files: ["**/*.{fs,fsx,fsi}"]
---

# Generalize only for a second real use

Generalize domain code for reuse only when a second real use exists, and let small incidental blocks repeat until then.

## Look for

Look for explicit type parameters, interfaces, optional switches, configuration settings, union cases, or shared helpers that anticipate another use. Inspect the consumers as well as the declaration:

```fsharp
type Store<'entity, 'key> = { load: 'key -> 'entity option; save: 'entity -> unit }

let findCustomer (store: Store<Customer, CustomerId>) id = store.load id
```

This is a candidate if all constructed stores and consumers specialize the abstraction to customers.

## Check the context

Find actual instantiations, implementations, callers, and requirements across the project. Trace configuration reads and union construction/matching before calling a setting or case hypothetical; a legitimate domain state need not occur in current tests. Check whether switches have a meaningful second setting and whether helper callers implement the same domain operation or merely resemble each other. Separate explicit generalization from ordinary compiler-inferred polymorphism. Check whether the type is a public library contract for external consumers.

## Report when

Report machinery that serves only hypothetical consumers and makes the concrete operation harder to understand. Show the unused degree of freedom and the actual use that remains. For a shared helper, explain which incidental differences its parameters accommodate; repeated syntax alone does not establish a shared concept.

## Suggest

Keep the real operation concrete until another requirement supplies the missing design evidence:

```fsharp
type CustomerStore = { load: CustomerId -> Customer option; save: Customer -> unit }

let findCustomer (store: CustomerStore) id = store.load id
```

Update construction sites and annotations together. Remove unused switches, settings, or cases only after checking their consumers and contracts.

## Do not report

Leave abstractions serving distinct existing uses in place:

```fsharp
let repeat count value = List.replicate count value

let labels = repeat 3 "pending"
let retries = repeat 3 0
```

Here the uses already require different element types, and polymorphism is inferred. Also exclude domain modeling, helpers isolating a decision or effect (rule `io-at-the-edges`), and library APIs deliberately designed for unknown callers.
