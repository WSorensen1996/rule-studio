# Rule Studio — User Guide

This document describes the Rule Studio solution: a no-code rule engine for Dataverse / Power Apps. Each section covers one part of the app — what it shows and where its data comes from.

## 1. Overview

The Overview is the home page of Rule Studio: a quick summary of your rules and what needs attention.

**What it shows**

- **Four counts:** total rules, live rules, drafts in progress, and rules not published (with how many are inactive).
- **Continue working:** your most recently edited drafts. Click one to open it in the editor, or use "All drafts" to see them all.
- **Trigger landscape:** a bar chart of how many rules run on each table, event and stage.
- **Needs attention:** runs and failures in the last 7 days, plus the latest failures with their error message.
- **Recent releases:** the latest releases and their status.

**Where the data comes from**

| Dataverse table | Used for |
| --- | --- |
| `rs_rule`, `rs_ruleversion` | Rules, live versions and drafts |
| `rs_executionlog` | Run and failure history (test runs excluded) |
| `rs_release` | Release history |

Table display names come from Dataverse metadata. The page only reads data; it changes nothing.

## 2. UI page standard

Every Rule Studio page follows one layout standard, so the app looks like one product. The developer reference is `RuleStudioApp/docs/ui-standard.md`.

| Page type | Pages | Top of page |
| --- | --- | --- |
| Record form | Rule editor, Custom function, Lookup definition, Release details, Execution detail | Opens full-page in place of the list. Toolbar (← Back first), summary bar (status chips, key facts), full-width title, sections |
| List page | Rule library, Releases, Execution logs, Execution timeline, Test & trace, Custom functions, Lookup definitions, Environments, Guide | Toolbar, then title and description on one line |
| Dialog | Publish, Rollback, Restore | Dataverse title, then a one-line intro |
| Dashboard | Overview | Counts, then cards |

**Rules for all pages**

- No breadcrumb labels: the sitemap already shows where you are.
- Records open full-page, like a form in any model-driven app: the record takes the place of the list. Only short confirm-and-act steps (publish, rollback, restore) are pop-ups.
- **← Back** is the first button in the toolbar on every page. It returns to the page you came from; a record opened from a link goes to its list. A page you reached from the sitemap has no Back button.
- **Unsaved changes:** leaving a form with changes that are not saved asks first: Save and leave, Leave without saving, or Stay.
- Status chips sit in the summary bar above a record's title, so the title uses the full width.
- Section and card headers are one line: number, title, description, chips on the right.
- Long descriptions end with "…"; hovering shows the full text.
- A test (`ui-standard.test.mjs`) fails the build if a page uses the old patterns.

## 3. Field help tooltips

**TLDR:** Hover over a field (or tab into it) for 2 seconds and a short tooltip explains what it is and what goes in it. Example: Priority → "Lower numbers run first, so 10 runs before 100."

- **Where:** about 45 non-trivial fields on 8 pages: rule editor, conditions and actions, the custom function and lookup definition forms, the publish dialog, execution logs, test & trace and timeline.
- **Not on:** self-explanatory fields such as names, descriptions and search boxes.
- **One list of texts:** every explanation lives in one shared file, so the same field shows the same text on every page.
- **Hide it:** Esc, a click or scrolling.
- **Guarded:** a test fails the build if a new field has no help text.
- Developer details: `docs/ui-standard.md`, section "Field help".

## 4. Rule library

**TLDR:** A list of every rule. Find a rule, see its status, and open it.

### Buttons

- **+ New rule:** opens an empty rule editor.
- **Publish selected:** publishes the drafts of the ticked rules. Tick at least one rule with a draft first.
- **Refresh:** reloads the list.

### Filters

- **Search:** type part of a rule name or key.
- **Table:** show only rules that run on one table.
- **Status:** Has a draft, Live, Never published, or Inactive.
- The counter on the right shows how many rules match.

### What each row shows

- **Checkbox:** select the rule for Publish selected. The top checkbox selects all shown rules.
- **Rule:** the name, with the unique key below it.
- **Trigger:** the table, then the event and stage (for example Update · Before save). If the draft uses a different trigger than the live version, a "Draft:" line shows the new one.
- **Priority:** the run order. Lower numbers run first.
- **Version / status:** chips that show the state:
  - **Live vN:** this version runs in production.
  - **Draft vN:** unpublished changes are waiting.
  - **Live + Draft:** both chips show. Live runs; the draft is the next version.
  - **Rolled back vN:** the last release was rolled back, so nothing runs.
  - **Inactive:** the rule is deactivated, so it never runs.
- **Open →:** opens the rule in the rule editor.

**Good to know:** only published versions run. Drafts never affect production.

## 5. Rule editor

**TLDR:** The form where you build one rule: when it runs, what it checks, and what it does.

### Top of the form

- **Summary bar:** status chips (for example Draft v6 · Live v5), then key · trigger.
- **Title:** the rule name. Click it to edit.
- **Tabs:** Rule builder (the form) and Version history & rollback.
- **Field help:** hover over a field for 2 seconds to see what it means.

### Buttons

- **← Back:** returns to the page you came from. Asks first if you have unsaved changes.
- **Save draft:** saves your changes as a draft. Nothing runs yet.
- **Save & close:** saves the draft and goes back.
- **Test rule:** opens Test & trace to try the rule on a record.
- **Publish:** makes the draft the live version. From then on it runs in production.
- **Discard draft:** throws the draft away. The live version stays.
- **Delete rule:** removes the whole rule. Only allowed when the rule is not live. If the rule only has a draft, Discard draft becomes Delete rule.
- **Copy rule:** makes a new rule with the same setup, named "Copy of …".
- **View live:** shows the live version, read-only. Click **Back to draft** to keep editing.
- **Timeline:** shows all rules that run on the same table, in order.
- **Version history:** jumps to the Version history tab.

### 1. Rule identity & trigger

What the rule is, and which record event starts it.

- **Unique rule key:** a short ID, like PRICING.DISCOUNT. It stays the same in every environment.
- **Description:** a short note on what the rule does.
- **Target table:** the table whose records start the rule, for example quote.
- **Event:** Create, Update or Delete.
- **Execution stage:** Before save can change or block the save. After save runs once the record is saved and cannot block it.
- **Triggering columns:** Update only. The rule runs only when one of these columns changes. Empty means any change.
- **Activate / Deactivate** (in the header): an inactive rule never runs, even when it is published. Applies as soon as you save.

### 2. Conditions (IF)

What must be true for the actions to run. No conditions means the rule always runs.

- Each row is: **field → operator → value**. For example: Total amount → Greater than → 10000.
- **Field:** pick a field on the record, or follow a relation to a parent record (for example the quote's account).
- **Child records:** for related lists (for example quote lines) pick how to check them: Any, All, None-of, or a number from Count, Sum, Min, Max or Average.
- **Operator:** Equals, Not equals, Greater/Less than, Contains, Starts with, Is empty, In list, Between, or Changed (the save changes this field).
- **Value source:** Fixed (you type it), Field path (another field), Lookup (a row from a table), Function (a custom function), or Expression (a small formula).
- **Groups:** combine rows with AND (all must be true) or OR (one is enough). Groups can be nested.

### 3. Actions (THEN / ELSE)

What the rule does. Actions run top to bottom.

- **THEN:** runs when the conditions are true.
- **ELSE:** optional. Runs when the conditions are false.
- **Action types:**
  - **Set field:** gives a field a new value.
  - **Block save:** stops the save and shows your error message. Before save only.
  - **Call function:** runs a registered custom function and maps its inputs.
  - **Log message:** writes a message to the execution log.
- After save cannot change the saved record directly. A Set field there makes a follow-up update.

### 4. Order, safety & runtime

How the rule behaves when it runs.

- **Priority:** run order. Lower numbers run first.
- **On failure:** Block stops the save with an error. Log and continue logs the error and lets the save go on.
- **Run as:** Calling user uses the user's permissions. System uses full permissions.
- **Retry count / Retry delay:** how often to try again after a failure, and how long to wait between tries (ms).
- **Execution log level:** Off, Errors only, Summary, or Full trace. Default means Errors only.
- **Stop processing after match:** when this rule matches, later rules on the same trigger are skipped.
- **Version change note:** what changed in this version. Shows in the history.

### 5. Rule latency & frequency

How the live rule performs, based on recent production logs.

- **Average logged latency:** average run time in ms.
- **Observed executions:** how many recent runs were logged.
- **Recent durations:** a bar per run. Hover a bar for outcome, time and date.
- Counts only logged runs. With Errors only, successful runs are not counted.

### Version history & rollback tab

- Every saved and published version is kept. History is never overwritten.
- **Restore as draft:** copies an older version into a new draft.
- **Create draft from live:** starts a new draft from the live version.

## 6. Execution timeline

**TLDR:** Shows which rules run when a record is saved, and in what order.

### How a chain works

- A **chain** is all rules with the same **table + event + stage**. Example: quote · Update · Before save.
- When that save happens, every rule in the chain runs, one after the other.
- **Order:** lowest priority number first. Same priority? Then by rule key (A–Z).

### Buttons

- **Test this chain:** opens Test & trace with this table, event and stage already picked.
- **Refresh:** reloads the rules.

### Filters

- **Target table, Event, Stage:** pick the chain to show. Only combinations that have rules are listed.

### What each card shows

Cards run in reading order (left to right, then the next row), joined by arrows. A rule with an unpublished draft is shown as its draft.

- **#Priority:** the run order number.
- **State chip:**
  - **Enabled:** the published rule runs.
  - **Draft:** the card shows an unpublished draft; it runs once published. **Draft · new here** or **Draft · changed** when the draft of a published rule adds it to this chain or changes its priority, stop flag or filtered columns.
  - **Inactive:** the rule is deactivated. It is skipped.
  - **Not reached:** an earlier rule stops processing, so this one never runs.
- **Rule name**, then the version ("(draft)" when it is the draft).
- **Filtered columns:** the columns that start the rule on Update. "Any column" means any change.
- **Rule key**, and a **Stop processing** chip if the rule stops later rules.
- **■ Stop marker:** drawn where processing stops. Rules after it are not reached.
- Click a card to open the rule in the editor.

### Rules below the chain

Dashed cards below the chain, marked **Draft removes**, are published rules whose draft moves them off this trigger.

## 7. Test & trace

**TLDR:** Try rules on a record before they go live, and see step by step what they would do. **It never changes anything in your environment.**

### Is it safe? Yes.

- **Nothing is saved.** No records are created, changed or deleted.
- Actions like Set field only change a copy in memory. You see the result, Dataverse does not.
- **No execution log rows** are written for tests.
- **Functions with side effects are never called.** They always get a fake (mocked) result.
- By default **all** functions are mocked. Turn off "Mock all functions" to really call functions that are side-effect free (they only read, never write).
- The test runs with **your permissions**. You only see rules and records you are allowed to read.
- Drafts are tested without publishing them.

### How to open it

- From the sitemap: **Test & trace**.
- From the rule editor: **Test rule** (that rule is pre-selected).
- From the execution timeline: **Test this chain** (that chain is pre-selected).

### Step 1 · What to test

Pick one of two modes:

- **Table / event / stage chain:** test the whole chain, exactly as a real save would run it. Inactive rules show as skipped.
- **Selected rules:** tick one or more rules to test only those. They must share the same table, event and stage. Inactive rules still run here, because you picked them on purpose.

Then choose:

- **Rule version to evaluate:**
  - **Draft where available:** uses your unpublished changes. Rules without a draft use their live version.
  - **Published only:** tests exactly what runs in production today.
- **Mock all functions:** on = no function is really called. Off = side-effect free functions are called for real. Side-effect functions are always mocked.

### Step 2 · Record

Give the rules data to work with.

- **Real record (optional):** search by name and pick a record. Its current values are loaded. It is only read, never changed.
- **Sample values:** add column + value pairs. They are laid over the record. Use them to try "what if" values.
  - Values are JSON: `20`, `true`, `"text"`, or a lookup like `{"table":"account","id":"…"}`. Plain text also works.
- **No record picked?** The test starts from an empty record plus your sample values.
- **On Update:** the sample values count as the **changed columns**. The real record is the "before" state. Rules with triggering columns are skipped if none of their columns changed.
- **On Create:** there is no "before" state.

### Step 3 · Run and read the trace

Click **▷ Run simulation**. The result has three parts:

**1. The verdict (top)**

- **Allowed:** the record would pass this chain.
- **Blocked:** a rule would stop the save. The rule and its message are shown.
- **Failed:** a rule broke. The error is shown.

**2. Rule-by-rule trace**

One line per rule, in run order:

- **Matched:** the conditions were true. The THEN actions ran.
- **Not matched:** the conditions were false. The ELSE actions ran, if any.
- **Blocked:** the rule stopped the save.
- **Skipped:** the rule did not run. The reason is shown: rule inactive or none of its triggering columns changed.
- **Stopped:** an earlier rule stopped processing or blocked the save.

Open a line to see:

- **Condition evaluation:** each condition with the actual value and whether it was true.
- **Action results:** what each action did (field set, function called, message logged).

**3. Before / after field values**

A table of every field the rules changed: the value before, the value after, and which rule wrote it. "No fields changed" means no rule set a field.

### Chain preview (right side)

Shows the rules that will be tested, in order. In Draft mode your draft replaces the live version of that rule.

### Tips

- Test a draft before you publish it. It is free and safe.
- To check a filtered rule on Update, add a sample value for one of its triggering columns.
- Same evaluator as production: if the test passes, the live rule behaves the same on the same data.

## 8. Custom functions

**TLDR:** Custom functions are pieces of code made by developers (Dataverse Custom APIs). Register one here once, and rule authors can pick it from a dropdown.

### Buttons

- **+ Register function:** opens a new custom function form.
- **Refresh:** reloads the list.

### Left: available functions

One card per function with its name, Custom API name, and chips:

- **Pure / Side effects:** Pure only reads. Side effects changes data or calls other systems.
- **Retry-safe / One attempt:** whether a failed rule may run it again.
- **Inactive:** cannot be picked in new rules, and rules that call it fail when they run. Use Activate / Deactivate in the header.

### Right: the selected function

- **Custom API logical name, Output type, Timeout.**
- **Typed inputs:** each parameter with its type, whether it is required, and its table (for lookups).
- **Uniform result contract:** every function returns success, value and message.
- **Behaviour notes:** how tests and retries treat it.
- **Usage:** which rules use it. A function in use cannot be deleted.
- **Edit registration:** opens the function's form to change it.

### Custom function form

Opens full-page. Commands: **← Back**, **Save**, **Save & close**.

- **Display name:** the name authors see.
- **Custom API logical name:** the real Custom API. Use the search button to pick one from this environment.
- **Output type:** what the function returns (text, number, yes/no and so on).
- **Timeout (ms):** how long it may run. Empty = default.
- **Description:** what it does.
- **Input parameters:** add each input with name, type, required, and table for lookups.
- **Has side effects / Safe to retry:** the switches described above. Activate or deactivate a function from the Custom functions header.

**Good to know:** registering does not create code. The Custom API must already exist.

### Standard functions

Rule Studio ships six ready-made functions. Register the ones you want: **+ Register function**, search for the Custom API, and set the switches as listed.

- **rs\_ValidateEmail:** is this a valid e-mail address? Yes/no. Pure, safe to retry.
- **rs\_ValidatePhone:** is this a valid phone number? Give CountryCode (e.g. DK) for numbers without +. Yes/no. Pure, safe to retry.
- **rs\_ValidateVatNumber:** is this a valid VAT number? Checks format and check digits offline; it does not ask VIES. Yes/no. Pure, safe to retry.
- **rs\_CreateTask:** creates a task, optionally regarding a record, with a due date and owner. Side effects, not safe to retry.
- **rs\_AssignTo:** makes a user or team the owner of a record. Side effects, safe to retry.
- **rs\_CallWebhook:** posts a payload to an https URL (or to the URL in an environment variable) and returns the HTTP status. Side effects, not safe to retry.

Example: keep a Yes/No column "VAT number valid" up to date. Trigger: account, Update, Before save, triggering column = the VAT field. Action: **Set field** "VAT number valid" to function **rs\_ValidateVatNumber** with VatNumber from the VAT field. Views and other rules can then use that column.

Empty values count as valid, so pair a validation with "is filled in" when the field is required. Details: [Custom APIs](custom-apis.md#standard-custom-functions).

## 9. Lookup definitions

**TLDR:** A lookup finds a value in another table while a rule runs. Example: find the discount % in a discount table for the quote's customer type. Define it once, reuse it in any rule.

### Buttons

- **+ Define lookup:** opens a new lookup definition form.
- **Refresh:** reloads the list.

### Left: registered lookups

- **Search** by name or table.
- Click a lookup to see its details.
- Values are read from Dataverse each time the rule runs. They are never copied into the rule.

### Right: the selected lookup

- **Target table:** the table that holds the rows.
- **Return column:** the value the lookup gives back.
- **Match criteria:** the rules a row must meet. All must be true. No criteria = every row matches.
- **If no rows match:** Block, Use default (shows the default value), or Skip.
- **If multiple rows match:** Most specific, First by sort (shows the sort column), or Error.
- **Usage:** where it is used. A lookup in use cannot be deleted.
- **Edit definition / Test lookup / Delete.**

### Lookup definition form

Opens full-page. Commands: **← Back**, **Save**, **Save & close**.

- **Lookup name** and **Description.**
- **Evaluated for table:** the table of the record the rule runs on.
- **Lookup table:** the table with the rows to search.
- **Return column:** what to give back.
- **Sort column:** optional. Used with First by sort.
- **No match / Multiple matches:** the policies above. Use default shows a Default value field.
- **Match criteria:** add rows like "lookup column equals record field". All must hold.

## 10. Releases & versions

**TLDR:** Every publish creates a release. A release is a bundle of rule versions that went live together, and can be undone together.

### Buttons

- **Validate & publish…:** opens the publish dialog.
- **Open rule library.**
- **Refresh.**

### Counters

- **Recorded releases:** all publishes so far.
- **Live releases:** not rolled back or failed.
- **Published rule versions:** total versions across releases.

### Release history (one row per release)

- **Release:** name and description.
- **Published:** date and who published it.
- **Scope:** how many rules.
- **Validation:** the result of the publish checks.
- **State:** Live, Rolled back or Failed.
- **Inspect:** opens the release details page (rules, versions, validation result).
- **Rollback:** opens the rollback dialog.

### Publish dialog

- Give the release a **name** and optional **description**.
- Check the **versions included**.
- Click **Validate.** It checks fields, types, paths, expressions, functions and conflicts.
- **Publish release** is enabled only when validation passes. All drafts go live together, or none do.

### Rollback dialog

- Each rule goes back to the version that was live before.
- The rollback is saved as a new release. The old one is marked Rolled back.
- Drafts are not touched.
- If something blocks the rollback, the reason is shown and Confirm is off.

### Restore version (from a rule's history)

- Copies an older version into a new draft with the next version number.
- The live version keeps running until you publish the draft.

**Good to know:** there is no approval step inside one environment. Approvals happen in the Dev → Test → Prod pipeline.

## 11. Execution logs

**TLDR:** A record of rules that ran: when, the result, and how long it took. Use it to find problems.

### Buttons

- **Test a rule:** opens Test & trace.
- **Refresh.**

### Filters

- **Rule, Table, From, To.**
- **Outcome:** any, or only failures and blocks.
- **Runs:** production, test runs, or both.
- **Correlation ID:** shows everything from one save.
- **Admin flag only:** runs marked for follow-up.
- **Apply / Clear.**

### Each row shows

Time, rule, outcome, trigger, duration, attempts, mode (production or test) and correlation ID. Click **Inspect** to open the log detail page.

### Log detail

- Rule and version, outcome, record, duration, time and trigger.
- Run metadata: correlation ID, attempts, admin flag, log level, mode.
- The error, if any.
- The full trace: conditions, actions and field changes (depends on log level).

### Logging policy

- **Off:** nothing is logged.
- **Errors only:** failures and blocked saves. The default.
- **Summary:** outcome and duration.
- **Full trace:** every detail.
- A rule without its own level uses Errors only.
- After-save failures are always logged and flagged for an admin.
- Blocked saves are not stored as log rows (the save is undone). They go to the plug-in trace log.

### Retention

- **Keep logs (days):** default 30.
- **Keep failed and blocked runs (days):** default 90.
- **Schedule retention jobs:** creates two daily cleanup jobs. Needs System Administrator.

## 12. Environments

**TLDR:** Move published rules from one environment to another (for example Dev to Test) as one JSON file.

### Step 1 · Export

- **↓ Export published rules:** downloads every published rule, plus the lookups it uses, as one file.
- Functions and lookups are referenced by name, so the file works in any environment.

### Step 2 · Import

1. **↑ Choose bundle…:** pick the file.
2. **Run check:** compares the file with this environment. It changes nothing.
3. **Apply import:** publishes the changed rules as one release.

Custom functions must already be registered in the target environment.

### Check result

- One row per rule key: the result and what will happen.
- Lookup changes and a side-by-side diff (incoming vs current).

### How conflicts work

- **Not changed here** since the last import: the incoming rule is applied.
- **Changed only here:** kept. Copy that change back to Git.
- **Changed in both places:** conflict. The whole import stops. Fix it in Git and check again.
- Rules that only exist here are kept.

## 13. Documentation (Reference)

**TLDR:** This document, inside Rule Studio. Open it from the sitemap under Reference › Documentation.

### What it shows

- **Contents (left):** every section of this document. Click one to jump to it.
- **Sections (right):** one white card per section, the same text as this document.
- The page only reads; it changes nothing.

### How it is kept up to date

- The page is generated from `RuleStudioApp/docs/user-guide.md` by `guide.build.mjs` in the WebResources project.
- After editing the document: run `node guide.build.mjs`, then build and deploy as usual.

### From idea to safe rollout

1. **Find or create** the rule in the Rule library.
2. **Set the trigger:** table, event, stage, and optional triggering columns.
3. **Write the IF:** conditions, grouped with AND/OR.
4. **Write THEN and ELSE:** the actions.
5. **Test it** in Test & trace. Nothing is saved.
6. **Publish** with Validate & publish.
7. **Observe and roll back** with Execution logs and Releases.
8. **Promote** to other environments with Environments.

### Worked example: sample functions

- **rs\_SampleCalculateDiscount:** revenue + employees → discount. Pure and safe to retry.
- **rs\_SampleStampContacts:** writes text on an account's contacts. Has side effects, so it is mocked in tests and tried only once. Both samples must be registered again on Custom functions after a data reset.

### Expressions (NCalc)

- Fields in brackets: `[creditlimit] * 0.1`.
- Parent fields: `[parentcustomerid:account.creditlimit]`.
- Built-in math: if, in, round, min, max, abs, pow. Names ignore case; round rounds half away from zero (2.5 → 3).
- **Empty fields follow NCalc:** an empty field makes the result empty, and Set field then clears the column. `[quantity] * [unitprice]` with no quantity gives empty, not 0. To use 0, write it into the formula, e.g. `if([quantity] = null, 0, [quantity]) * [unitprice]`, or add the condition "Quantity is filled in".
- Custom functions by name.
- Publishing checks every expression. Errors block the release.

### Dataverse tables

Rule Studio stores everything in its own tables: rule, rule version, conditions, actions, custom functions, lookup definitions, releases and execution logs. `RuleStudioApp/DeployDataModel/Program.cs` defines the full model.

### Runtime and roles

- **One engine plug-in** runs every published rule. Its steps update automatically on publish, rollback, or enable/disable.
- **Before save** changes the record directly and can block. **After save** runs in the background.
- **Rule Studio Author:** can create, edit, test, publish, roll back and import.
- **Rule Studio Viewer:** read only.
- Give either role together with Basic User.
