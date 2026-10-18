# Rule Studio: UI page standard

Every Rule Studio web resource page uses the same top-of-page layout, so the model-driven app
looks like one product. The building blocks live in `src/shared/rule-studio.css` (section
"Page standard") and `src/shared/standard.ts`. `tests/ui-standard.test.mjs` fails the build
when a page falls back to the old patterns.

## Page types

| Type | Pages | Top of page, in order |
|---|---|---|
| Record page | `rule-editor`, `register-function`, `new-lookup-definition`, `release-detail`, `log-detail` | toolbar (← first) → summary bar → full-width title → sections |
| List page | `rule-library`, `releases`, `execution-logs`, `execution-timeline`, `test-trace`, `custom-functions`, `lookup-definitions`, `environments`, `guide` | toolbar (← first) → title · description (one line) → filters / cards |
| Dialog | `publish-release`, `rollback-release`, `restore-version` | host title (Dataverse) → one-line intro → content |
| Dashboard | `overview` | toolbar (←) → counts → cards |

A record shown inside a list page (the detail panel on `custom-functions` and
`lookup-definitions`) uses the record pattern: summary bar + title.

## Records open full-page

Rule Studio opens a record the way a model-driven app opens a form: in the main area, in place
of the list, with the sitemap still on the left. Only short confirm-and-act steps are dialogs.

- **Open a record with `openRecord(page, params)`** (`src/shared/nav.ts`). Each record page names
  its list in `RECORD_PAGES`. `openDialog()` is only for the pages in the Dialog row above.
- **← Back is the first command** in every page's `.rs-commandbar`, `id="cmdBack"`, started by
  `applyStandard()` (`src/shared/back.ts`). It looks like the model-driven app's: a 42 × 32 button
  with the Fluent left arrow, then a divider.
  - opened from another Rule Studio page → back to that page (browser history);
  - a record opened from a link or a bookmark → its list;
  - a list page opened from the sitemap → no Back button (it is removed, not greyed out; a bar
    that held only Back goes too).
- **Forms have Save and Save & close** (`id="cmdSaveClose"`): Save & close saves and goes back.
- **Unsaved changes:** a form registers `setLeaveGuard({ dirty, save })`. Back then asks first:
  *Save and leave*, *Leave without saving* or *Stay*. A new record nobody has touched leaves
  without asking.
- **A list reloads when you come back to it**, so there is no refresh-after-close code.
- Record title: `New …` for a new record, otherwise the record's name; the summary bar names the
  record type (e.g. "Custom function").

```html
<div class="rs-commandbar" role="toolbar" aria-label="Custom function commands">
  <button type="button" class="btn cmd-back" id="cmdBack" aria-label="Back" title="Back">←</button>
  <button type="button" class="btn" id="save" data-author>💾 Save</button>
  <button type="button" class="btn" id="cmdSaveClose" data-author>Save &amp; close</button>
</div>
```

The test blocks a page (other than a dialog) without a command bar whose first command is Back,
an `openDialog()` call for a record page, and a form without Save & close.

## Rules

- **No breadcrumb labels.** The sitemap already says where you are.
- **Status lives in the summary bar**, above the title: chips first, then key facts
  (`key · trigger`). The title gets the full width.
- **Section and card headers are one line:** number · title · description, chips on the right.
- **Descriptions are one line.** Long text ends with "…"; the full text shows as a tooltip,
  only when it is cut (`applyStandard()`).
- **Every page calls `applyStandard()`** once, at module start.
- The toolbar (`.rs-commandbar`) and everything inside the form stay as they are.
- **Activate / Deactivate is a header command**, never a switch in the form. A record that can
  be switched off (a rule, a custom function) gets one button in `.rs-commandbar`, next to the
  other commands, with `id="cmdActivate"`. Its text names the action: `⏸ Deactivate` when the
  record is active, `▶ Activate` when it is not. Status chips say **Active** / **Inactive**
  (never Enabled / Disabled). Dialogs do not carry the setting: a new record starts active.
  The test blocks on/off switches named *enabled* or *active* and a `cmdActivate` outside the bar.

## Markup

List page:

```html
<div class="rs-pagehead"><h1>Rule library</h1><p class="rs-desc">Each rule targets one Dataverse table and event.</p></div>
```

Record (form or detail panel):

```html
<div class="rs-summary" aria-label="Rule summary">
  <div class="rs-summary-pills" id="ruleStatus"><!-- pill() chips --></div>
  <p class="rs-summary-text" id="ruleSub">TEST.R1 · Pricing · account / Update / Before save</p>
</div>
<div class="rs-title"><h1>…title or title input…</h1></div>
```

Section header:

```html
<div class="section-title"><span class="secindex">1</span>
  <div class="grow"><h2 class="h3">Rule identity &amp; trigger</h2><p class="rs-desc">Which Dataverse record event wakes this rule up?</p></div>
</div>
```

Dialog intro (first element in `.dialog-body`):

```html
<p class="rs-intro" id="intro">A lookup definition finds one row in a table and returns one of its columns.</p>
```

Page script:

```ts
import { applyStandard } from "../../shared/standard";

applyStandard();
```

## Classes

| Class | Use |
|---|---|
| `rs-pagehead` | List page head: `h1` + `rs-desc` on one line (wraps under 800 px) |
| `rs-pagehead stacked` | Page head with a two-line summary under the `h1` (wraps, max 780 px); for pages that need a short explanation |
| `rs-desc` | One-line muted description with ellipsis + tooltip |
| `rs-summary`, `rs-summary-pills`, `rs-summary-text` | Record summary bar above the title |
| `rs-title` | Full-width record title (`h1` page, `h2` detail panel), optional `rs-desc` under it |
| `rs-intro` | One-line dialog intro |
| `section-title`, `card-head` | One-line headers; put `h2` + `rs-desc` in a `.grow` |

## Removed (blocked by the test)

`eyebrow`, `form-heading*`, `section-desc`, `sub`.

## Checking a change

```bash
cd src/Dataverse/WebResources && npx tsc --noEmit && npx eslint src && npm test && npm run build
```

After a deploy, Dataverse may serve the old page until a hard refresh (Ctrl+Shift+R).

## Field help

Non-trivial fields explain themselves in a tooltip. Hover over the field (or tab into it) for
2 seconds and a short, plain-language explanation appears. Esc, a click or scrolling hides it.

- **One list of texts:** `src/shared/fieldHelp.ts` (`FIELD_HELP`). Each key is reused wherever
  the same field appears (e.g. `ruleTable`, `ruleStage` on the editor, test & trace and timeline).
- **One bubble:** `src/shared/tooltip.ts`, started by `applyStandard()`; styled by `.rs-tip`.
- **Text style:** what the field is, what goes in it, what happens. One to two short sentences.
  Existing grey hints under fields stay.

Tag a control in HTML:

```html
<input class="input" id="vPriority" type="number" data-help="priority">
<!-- checkbox/switch: tag the wrapper -->
<div class="switchline" data-help="stopProcessing"><input class="switch" type="checkbox" id="vStop">…</div>
```

Tag a control built in code:

```ts
import { withHelp } from "../../shared/tooltip";

field("Level", withHelp(level, "messageLevel"));
```

`tests/field-help.test.mjs` fails when a page control has no `data-help` (unless listed as
self-explanatory in its `TRIVIAL` set, e.g. names, descriptions, search boxes) or when a key
is not in `FIELD_HELP`.

## Page help

Background information (how something works, why it is built that way) is never an
always-visible box. It sits behind a small **?** icon at the far right of the header it belongs
to: the command bar for page-level help (`.rs-commandbar`, last child, far right), a section or card header (`.section-title`, `.card-head`)
or a dialog intro (`.rs-introhead`). Click opens it; a click elsewhere or Esc closes it.

```html
<div class="rs-commandbar" role="toolbar">…buttons…
  <details class="rs-help"><summary aria-label="About this page" title="About this page">?</summary>
    <div class="rs-help-pop"><strong>Chain = table · event · stage</strong>Rules on the same trigger run by priority …</div></details></div>

<!-- dialog: wrap the intro so the icon sits on its right -->
<div class="rs-introhead"><p class="rs-intro">…</p><details class="rs-help">…</details></div>
```

- **One icon per header**, last child of the header. Several points go in one popover (`<p>`).
- **Only static explanations move here.** Notices that show live state, warnings or errors
  (`.notice` with an `id`, set from code) stay visible.
- Script: `src/shared/pageHelp.ts`, started by `applyStandard()`; styles: `.rs-help`, `.rs-help-pop`.

`tests/field-help.test.mjs` fails when a page has a static `.notice.info` box (one without an `id`).
