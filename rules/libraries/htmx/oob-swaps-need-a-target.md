---
id: oob-swaps-need-a-target
category: libraries/htmx
check: semantic
files: ["**/*.{fs,fsx}"]
packages: ["Giraffe.ViewEngine.Htmx", "Oxpecker.Htmx", "Falco.Htmx"]
sources:
  - "htmx, hx-swap-oob: https://htmx.org/attributes/hx-swap-oob/"
  - "htmx reference, events: https://htmx.org/reference/"
---

# Send out-of-band fragments only to targets that exist

Send an out-of-band fragment only when every page that can trigger the request contains its target, and provide an in-band fallback for required feedback when that cannot be guaranteed. A missing target prevents the update from reaching the user; `htmx:oobErrorNoTarget` alone does not display the message.


## Look for

Look for responses containing `hx-swap-oob` or a view helper that emits it. Follow every page capable of triggering that response. A page with only a row target is a candidate:

```html
<body>
  <button hx-delete="/basket/42" hx-target="#row-42">Remove</button>
  <div id="row-42">Example item</div>
</body>
```

Its response attempts to append feedback somewhere else:

```html
<div id="notices" hx-swap-oob="beforeend"><p>Item removed.</p></div>
```

## Check the context

1. Resolve the OOB target: the fragment's id or an explicit selector in the swap value.
2. Inspect shared layouts and conditional rendering for every triggering page. Is that target present when the response arrives?
3. Inspect the strategy. With insertion strategies such as `beforeend`, the wrapper is stripped; check that required ids or styling belong to retained children.
4. Determine whether required feedback has an ordinary response fallback if the OOB target is unavailable.

## Report when

Report an OOB response without its target on a reachable page, or feedback that relies solely on that missing target. Identify the triggering page and unresolved target. A missing-target event does not by itself present the required message to the user. Also report code expecting a stripped wrapper to remain in the DOM.

## Suggest

Render the target in the shared layout of every page using the action:

```html
<body>
  <div id="notices"></div>
  <button hx-delete="/basket/42" hx-target="#row-42">Remove</button>
  <div id="row-42">Example item</div>
</body>
```

If a shared target is inappropriate, send required feedback in the normal response target instead. Keep the swap strategy and retained child markup consistent.

## Do not report

Keep an OOB update when every triggering layout establishes its target:

```html
<header><span id="basket-count">2</span></header>
```

A response replacing `basket-count` with `hx-swap-oob="true"` has a matching element. The target may come from the layout rather than the local page function; inspect that layout before claiming it is absent.
