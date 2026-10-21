# Rule Studio — User Guide

Rule Studio lets you build business rules for Dataverse without code. This guide follows the app from top to bottom, in the same order as the menu on the left: **Workspace**, **Building blocks**, **Operations** and **Reference**.

## 1. Start here

**TLDR:** A rule says "when this record is saved, check this, then do that". You build it as a draft, test it, publish it, and watch it run.

### Five words to know

- **Rule:** when a record is saved (the trigger), check something (IF), then do something (THEN / ELSE). Example: when a quote is saved and the total is over 10,000, block the save.
- **Draft:** a version you are still working on. Drafts never run in production.
- **Live:** the published version. This is the one that runs.
- **Chain:** all rules that start on the same table, event and stage. They run one after the other.
- **Release:** one publish. It can be rolled back as a whole.

### The journey of a rule

Every rule takes the same path, and this guide follows it:

1. **Create** the rule in the **Rule library**.
2. **Build** it in the **Rule editor**: trigger, IF, THEN / ELSE.
3. **Check the order** of rules in the **Execution timeline**.
4. **Test** it in **Test & trace**. Nothing is saved.
5. **Publish** it in **Releases & versions**.
6. **Watch** it run in **Execution logs**, and roll back if needed.
7. **Move** it to Test and Prod with **Environments**.

Rules can use **building blocks**: custom functions, lookups and expressions. Want to know what happens under the hood? See **How Rule Studio works**.

### Who can do what

- **Rule Studio Author:** can create, edit, test, publish, roll back and import.
- **Rule Studio Viewer:** read only. Buttons that change things are greyed out.
- Give either role together with Basic User.

### Every page works the same way

- **← Back** is the first button on every page. It returns to where you came from.
- **Records open full-page**, like any Power Apps form. Only short confirm steps (publish, rollback, restore) are pop-ups.
- **Unsaved changes:** leaving a form with unsaved changes asks first: Save and leave, Leave without saving, or Stay.
- **Field help:** hover over a field (or tab into it) for 2 seconds. A short tooltip explains it. Press Esc, click or scroll to hide it.
- **Status chips** (Live, Draft, Inactive …) sit in the bar above a record's title.

## 2. Overview

*Workspace › Overview*

**TLDR:** The home page. A quick summary of your rules and what needs attention. It only reads data.

- **Four counts:** total rules, live rules, drafts in progress, and rules not published (with how many are inactive).
- **Continue working:** your most recently edited drafts. Click one to open it, or use "All drafts".
- **Trigger landscape:** a bar chart of how many rules run on each table, event and stage.
- **Needs attention:** runs and failures in the last 7 days, and the latest failures with their error.
- **Recent releases:** the latest releases and their status.

**Next:** open the Rule library to find or create a rule.

## 3. Rule library

*Workspace › Rule library*

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
- **Version / status:**
  - **Live vN:** this version runs in production.
  - **Draft vN:** unpublished changes are waiting.
  - **Live + Draft:** both chips show. Live runs; the draft is the next version.
  - **Rolled back vN:** the last release was rolled back, so nothing runs.
  - **Inactive:** the rule is deactivated, so it never runs.
- **Open →:** opens the rule in the rule editor.

**Next:** click **+ New rule** or **Open →** to build the rule.

## 4. Rule editor

*Opens from the Rule library*

**TLDR:** The form where you build one rule. It reads top to bottom like a sentence: **when** this happens (trigger), **if** this is true (conditions), **then** do this (actions).

### Top of the form

- **Summary bar:** status chips (for example Draft v6 · Live v5), then key · trigger.
- **Title:** the rule name. Click it to edit.
- **Tabs:** Rule builder (the form) and Version history & rollback.

### Buttons

- **Save draft:** saves your changes as a draft. Nothing runs yet.
- **Save & close:** saves the draft and goes back.
- **Test rule:** opens Test & trace with this rule picked.
- **Publish:** makes the draft the live version. From then on it runs in production.
- **Discard draft:** throws the draft away. The live version stays.
- **Delete rule:** removes the whole rule. Only allowed when the rule is not live. If the rule only has a draft, Discard draft becomes Delete rule.
- **Copy rule:** makes a new rule with the same setup, named "Copy of …".
- **View live:** shows the live version, read-only. Click **Back to draft** to keep editing.
- **Timeline:** shows all rules that run on the same table, in order.
- **Version history:** jumps to the Version history tab.

### Step 1 · When: identity & trigger

What the rule is, and which save starts it.

- **Unique rule key:** a short ID, like PRICING.DISCOUNT. It stays the same in every environment.
- **Description:** a short note on what the rule does.
- **Target table:** the table whose records start the rule, for example quote.
- **Event:** Create, Update or Delete.
- **Execution stage:** **Before save** can change or block the save. **After save** runs once the record is saved and cannot block it.
- **Triggering columns:** Update only. The rule runs only when one of these columns changes. Empty means any change.
- **Activate / Deactivate** (in the header): an inactive rule never runs, even when published. Applies as soon as you save.

### Step 2 · If: conditions

What must be true for the actions to run. No conditions means the rule always runs.

- Each row reads: **field → operator → value**. Example: Total amount → Greater than → 10000.
- **Field:** a field on the record, or a field on a parent record (for example the quote's account).
- **Child records:** for related lists (for example quote lines) pick Any, All, None-of, or a number from Count, Sum, Min, Max or Average.
- **Operator:** Equals, Not equals, Greater/Less than, Contains, Starts with, Is empty, In list, Between, or Changed (this save changes the field).
- **Value source:** where the value comes from:
  - **Fixed:** you type it.
  - **Field path:** another field.
  - **Lookup:** a value from another table. See **Lookup definitions**.
  - **Function:** the result of a custom function. See **Custom functions**.
  - **Expression:** a small formula. See **Expressions**.
- **Groups:** combine rows with AND (all must be true) or OR (one is enough). Groups can be nested.

### Step 3 · Then / else: actions

What the rule does. Actions run top to bottom.

- **THEN:** runs when the conditions are true.
- **ELSE:** optional. Runs when the conditions are false.
- **Action types:**
  - **Set field:** gives a field a new value.
  - **Block save:** stops the save and shows your error message. Before save only.
  - **Call function:** runs a custom function and maps its inputs.
  - **Log message:** writes a message to the execution log.
- After save cannot change the saved record directly. A Set field there makes a follow-up update.

### Step 4 · Order, safety & runtime

How the rule behaves when it runs.

- **Priority:** run order in the chain. Lower numbers run first.
- **On failure:** Block stops the save with an error. Log and continue logs the error and lets the save go on.
- **Run as:** Calling user uses the user's permissions. System uses full permissions.
- **Retry count / Retry delay:** how often to try again after a failure, and how long to wait between tries (ms).
- **Execution log level:** Off, Errors only, Summary, or Full trace. Default means Errors only.
- **Stop processing after match:** when this rule matches, later rules in the chain are skipped.
- **Version change note:** what changed in this version. Shows in the history.

### Step 5 · Latency & frequency

How the live rule performs, based on recent production logs.

- **Average logged latency:** average run time in ms.
- **Observed executions:** how many recent runs were logged.
- **Recent durations:** one bar per run. Hover a bar for outcome, time and date.
- Only logged runs count. With Errors only, successful runs are not counted.

### Version history & rollback tab

- Every saved and published version is kept. History is never overwritten.
- **Restore as draft:** copies an older version into a new draft.
- **Create draft from live:** starts a new draft from the live version.

**Next:** check where the rule sits among the other rules in the Execution timeline.

## 5. Execution timeline

*Workspace › Execution timeline*

**TLDR:** Shows which rules run when a record is saved, and in what order.

### How a chain works

- A **chain** is all rules with the same **table + event + stage**. Example: quote · Update · Before save.
- When that save happens, every rule in the chain runs, one after the other.
- **Order:** lowest priority number first. Same priority? Then by rule key (A–Z).

### Buttons

- **Test this chain:** opens Test & trace with this table, event and stage picked.
- **Refresh:** reloads the rules.

### Filters

- **Target table, Event, Stage:** pick the chain to show. Only combinations that have rules are listed.

### What each card shows

Cards run in reading order (left to right, then the next row), joined by arrows. A rule with an unpublished draft is shown as its draft.

- **#Priority:** the run order number.
- **State chip:**
  - **Enabled:** the published rule runs.
  - **Draft:** an unpublished draft; it runs once published. **Draft · new here** or **Draft · changed** when the draft adds the rule to this chain or changes its priority, stop flag or filtered columns.
  - **Inactive:** the rule is deactivated. It is skipped.
  - **Not reached:** an earlier rule stops processing, so this one never runs.
- **Rule name**, then the version ("(draft)" when it is the draft).
- **Filtered columns:** the columns that start the rule on Update. "Any column" means any change.
- **Rule key**, and a **Stop processing** chip if the rule stops later rules.
- **■ Stop marker:** drawn where processing stops. Rules after it are not reached.
- Click a card to open the rule in the editor.

### Rules below the chain

Dashed cards marked **Draft removes** are published rules whose draft moves them off this trigger.

**Next:** test the chain before you publish.

## 6. Test & trace

*Workspace › Test & trace*

**TLDR:** Try rules on a record before they go live, and see step by step what they would do. **It never changes anything in your environment.**

### Is it safe? Yes.

- **Nothing is saved.** No records are created, changed or deleted.
- Set field only changes a copy in memory. You see the result; Dataverse does not.
- **No execution log rows** are written for tests.
- **Functions with side effects are never called.** They always get a fake (mocked) result.
- By default **all** functions are mocked. Turn off "Mock all functions" to really call functions that only read.
- The test runs with **your permissions**. You only see rules and records you may read.
- Drafts are tested without publishing them.

### How to open it

- From the menu: **Test & trace**.
- From the rule editor: **Test rule** (that rule is picked).
- From the execution timeline: **Test this chain** (that chain is picked).

### Step 1 · What to test

Pick one of two modes:

- **Table / event / stage chain:** the whole chain, exactly as a real save runs it. Inactive rules show as skipped.
- **Selected rules:** tick the rules to test. They must share the same table, event and stage. Inactive rules still run here, because you picked them on purpose.

Then choose:

- **Rule version to evaluate:**
  - **Draft where available:** uses your unpublished changes. Rules without a draft use their live version.
  - **Published only:** exactly what runs in production today.
- **Mock all functions:** on = no function is really called. Off = read-only functions are called for real. Side-effect functions are always mocked.

### Step 2 · Record

Give the rules data to work with.

- **Real record (optional):** search by name and pick a record. Its values are loaded. It is only read, never changed.
- **Sample values:** add column + value pairs on top of the record. Use them to try "what if" values.
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
- **Skipped:** the rule did not run, with the reason: inactive, or none of its triggering columns changed.
- **Stopped:** an earlier rule stopped processing or blocked the save.

Open a line to see each condition with its actual value, and what each action did.

**3. Before / after field values**

Every field the rules changed: the value before, the value after, and which rule wrote it.

### Chain preview (right side)

The rules that will be tested, in order. In Draft mode your draft replaces the live version.

### Tips

- Test a draft before you publish it. It is free and safe.
- To check a filtered rule on Update, add a sample value for one of its triggering columns.
- The test uses the same engine as production: same data, same result.

**Next:** when the test looks right, publish it in **Releases & versions**. If the rule needs a function or a lookup, set those up first in Building blocks.

## 7. Custom functions

*Building blocks › Custom functions*

**TLDR:** A custom function is code a developer has written (a Dataverse Custom API). Register it here once, and rule authors can pick it from a dropdown. Registering does not create code: the Custom API must already exist.

### Buttons

- **+ Register function:** opens a new custom function form.
- **Refresh:** reloads the list.

### Pick a function

Choose a function from the **Custom function** dropdown (inactive ones say so). Its details fill the page, with these chips in the summary bar:

- **Pure / Side effects:** Pure only reads. Side effects changes data or calls other systems.
- **Retry-safe / One attempt:** whether a failed rule may run it again.
- **Inactive:** cannot be picked in new rules, and rules that call it fail. Use Activate / Deactivate in the header.

### The selected function

- **Custom API logical name, Output type, Timeout.**
- **Typed inputs:** each parameter with its type, whether it is required, and its table (for lookups).
- **Uniform result contract:** every function returns success, value and message.
- **Behaviour notes:** how tests and retries treat it.
- **Usage:** which rules use it. A function in use cannot be deleted.
- **Edit registration:** opens the function's form.

### Custom function form

Commands: **← Back**, **Save**, **Save & close**.

- **Display name:** the name authors see.
- **Custom API logical name:** the real Custom API. Use the search button to pick one.
- **Output type:** what the function returns (text, number, yes/no and so on).
- **Timeout (ms):** how long it may run. Empty = default.
- **Description:** what it does.
- **Input parameters:** name, type, required, and table for lookups.
- **Has side effects / Safe to retry:** the switches described above.

### Standard functions

Rule Studio ships six ready-made functions. To use one: **+ Register function**, search for the Custom API, and set the switches as listed.

- **rs\_ValidateEmail:** is this a valid e-mail address? Yes/no. Pure, safe to retry.
- **rs\_ValidatePhone:** is this a valid phone number? Give CountryCode (e.g. DK) for numbers without +. Yes/no. Pure, safe to retry.
- **rs\_ValidateVatNumber:** is this a valid VAT number? Checks format and check digits offline; it does not ask VIES. Yes/no. Pure, safe to retry.
- **rs\_CreateTask:** creates a task, optionally regarding a record, with a due date and owner. Side effects, not safe to retry.
- **rs\_AssignTo:** makes a user or team the owner of a record. Side effects, safe to retry.
- **rs\_CallWebhook:** posts a payload to an https URL (or the URL in an environment variable) and returns the HTTP status. Side effects, not safe to retry.

**Example:** keep a Yes/No column "VAT number valid" up to date. Trigger: account, Update, Before save, triggering column = the VAT field. Action: **Set field** "VAT number valid" to function **rs\_ValidateVatNumber** with VatNumber from the VAT field.

Empty values count as valid, so pair a validation with "is filled in" when the field is required. Details: [Custom APIs](custom-apis.md#standard-custom-functions).

### Sample functions

- **rs\_SampleCalculateDiscount:** revenue + employees → discount. Pure and safe to retry.
- **rs\_SampleStampContacts:** writes text on an account's contacts. Has side effects, so it is mocked in tests and tried only once.
- Register both again after a data reset.

## 8. Lookup definitions

*Building blocks › Lookup definitions*

**TLDR:** A lookup finds a value in another table while a rule runs. Define it once, use it in any rule.

**Example:** a Discount table has one row per customer type and country, with a discount %. The lookup "Discount for quote" says: find the row where Customer type = the quote's customer type and Country = the quote's country, and give back Discount %. A rule can then set the quote's discount to that value.

The values are read from Dataverse every time the rule runs. Change a row in the Discount table, and the rule uses the new value straight away.

### Buttons

- **+ Define lookup:** opens a new lookup definition form.
- **Refresh:** reloads the list.
- **Edit definition / Test lookup / Delete:** act on the selected lookup. Delete is off while the lookup is in use.

### Pick a lookup

- Choose a lookup from the **Lookup definition** dropdown (name · table).
- Its details fill the page below.

### The selected lookup

- **Table with the answers:** the table that holds the rows (the Discount table).
- **Return column:** the value the lookup gives back (Discount %).
- **Match criteria:** what a row must meet. All must be true. No criteria = every row matches.
- **How the fields connect:** a picture of the record being saved on the left and the table with the answers on the right. Blue lines must match; the green line is the answer that comes back.
- **If no rows match:** Block, Use default (shows the default value), or Skip.
- **If multiple rows match:** Most specific, First by sort (shows the sort column), or Error.
- **Usage:** where it is used. A lookup in use cannot be deleted.

### Lookup definition form

Commands: **← Back**, **Save**, **Save & close**, and **Delete** for a saved lookup (not when it is in use).

- **Lookup name** and **Description.**
- **Record being saved:** the table of the record the rule runs on (quote).
- **Table with the answers:** the table with the rows to search (Discount).
- **Return column:** what to give back.
- **Sort column:** optional. Used with First by sort.
- **No match / Multiple matches:** the policies above. Use default shows a Default value field.
- **Match criteria:** one row is added for you. Each row reads "column in the table with the answers – equals – field on the record being saved". Remove it to match every row.
- **How the fields connect:** the same picture as on the list. It updates while you build.

## 9. Expressions

*Used in the Rule editor*

**TLDR:** An expression is a small formula, like in Excel. Use it as a value source in conditions and actions.

- Fields go in brackets: `[creditlimit] * 0.1`.
- Parent fields: `[parentcustomerid:account.creditlimit]`.
- Built-in math: if, in, round, min, max, abs, pow. Names ignore case. round rounds half away from zero (2.5 → 3).
- Custom functions can be called by name.
- **Empty fields:** an empty field makes the whole result empty, and Set field then clears the column. `[quantity] * [unitprice]` with no quantity gives empty, not 0. To use 0, write `if([quantity] = null, 0, [quantity]) * [unitprice]`, or add the condition "Quantity is filled in".
- Publishing checks every expression. Errors block the release.

**Next:** with the building blocks in place, publish the rule.

## 10. Releases & versions

*Operations › Releases & versions*

**TLDR:** Every publish creates a release: a bundle of rule versions that went live together, and can be undone together.

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
- **Inspect:** opens the release details (rules, versions, validation result).
- **Rollback:** opens the rollback dialog.

### Publish dialog

1. Give the release a **name** and optional **description**.
2. Check the **versions included**.
3. Click **Validate.** It checks fields, types, paths, expressions, functions and conflicts.
4. **Publish release** turns on only when validation passes. All drafts go live together, or none do.

### Rollback dialog

- Each rule goes back to the version that was live before.
- The rollback is saved as a new release. The old one is marked Rolled back.
- Drafts are not touched.
- If something blocks the rollback, the reason is shown and Confirm is off.

### Restore version (from a rule's history)

- Copies an older version into a new draft with the next version number.
- The live version keeps running until you publish the draft.

**Good to know:** there is no approval step inside one environment. Approvals happen in the Dev → Test → Prod pipeline.

**Next:** watch the rule run in the Execution logs.

## 11. Execution logs

*Operations › Execution logs*

**TLDR:** A record of rules that ran: when, the result, and how long it took. Use it to find problems.

### Buttons

- **Test a rule:** opens Test & trace.
- **Refresh.**

### Filters

- **Rule, Table, From, To.**
- **Outcome:** any, or only failures and blocks.
- **Runs:** production, test runs, or both.
- **Correlation ID:** everything from one save.
- **Admin flag only:** runs marked for follow-up.
- **Apply / Clear.**

### Each row shows

Time, rule, outcome, trigger, duration, attempts, mode (production or test) and correlation ID. Click **Inspect** to open the log detail.

### Log detail

- Rule and version, outcome, record, duration, time and trigger.
- Run details: correlation ID, attempts, admin flag, log level, mode.
- The error, if any.
- The full trace: conditions, actions and field changes (depends on log level).

### What gets logged

Each rule sets its own log level in the rule editor (Step 4).

- **Off:** nothing.
- **Errors only:** failures and blocked saves. The default.
- **Summary:** outcome and duration.
- **Full trace:** every detail.
- After-save failures are always logged and flagged for an admin.
- Blocked saves are not stored as log rows (the save is undone). They go to the plug-in trace log.

### Retention

- **Keep logs (days):** default 30.
- **Keep failed and blocked runs (days):** default 90.
- **Schedule retention jobs:** creates two daily cleanup jobs. Needs System Administrator.

**Next:** when the rules work, move them on to the next environment.

## 12. Environments

*Operations › Environments*

**TLDR:** Move published rules from one environment to another (for example Dev to Test) as one file.

### Step 1 · Export

- **↓ Export published rules:** downloads every published rule, plus the lookups it uses, as one file.
- Functions and lookups are referred to by name, so the file works in any environment.

### Step 2 · Import

1. **↑ Choose bundle…:** pick the file.
2. **Run check:** compares the file with this environment. It changes nothing.
3. **Apply import:** publishes the changed rules as one release.

Custom functions must already be registered in the target environment.

### Check result

- One row per rule key: the result and what will happen.
- Lookup changes, and a side-by-side diff (incoming vs current).

### How conflicts work

- **Not changed here** since the last import: the incoming rule is applied.
- **Changed only here:** kept. Copy that change back to Git.
- **Changed in both places:** conflict. The whole import stops. Fix it in Git and check again.
- Rules that only exist here are kept.

## 13. How Rule Studio works

*Behind the scenes*

**TLDR:** One plug-in, the engine, runs every published rule. On a save it loads the chain for that table, event and stage, and builds the full record from the pre-image plus the changes. It then runs the rules in order, and either writes the result into the save or blocks it.

### From rule to engine: what publishing does

What you build in the editor is stored as rows: rule version, conditions, actions. The engine never reads those rows.

- **Compile:** on publish, each version is compiled into one JSON **snapshot** on the rule version. This is what the engine runs. The live version is therefore fixed: editing the draft can't change what runs.
- **Validate:** before anything goes live, the publish checks fields, types, paths, hop limits, expressions, functions and fixed values. If one rule fails, the whole release is stopped.
- **Register steps:** Rule Studio then makes the plug-in steps in Dataverse match the published rules exactly.
  - It registers **one step per table + event + stage**, not one per rule.
  - **Before save** registers as PreOperation (20) and runs synchronously.
  - **After save** registers as PostOperation (40) and runs asynchronously, with its system jobs auto-deleted.
  - **Update steps filter** on the union of the chain's triggering columns. If any rule in the chain has none, the step fires on every change.
  - **Update and Delete steps** get a pre-image.
  - Steps no published rule needs are removed. This happens again on rollback and on activate/deactivate. Steps edited by hand are put back on the next publish.
- **Cache:** the engine keeps a loaded chain in memory for **30 seconds**. After that it only checks the version ids and snapshot hashes, and re-reads a snapshot only if it changed. A publish or rollback is live within about 30 seconds.

### What happens on one save

1. **Map the trigger:** message + stage become, for example, quote · Update · Before save. Other messages do nothing.
2. **Recursion guard:** above execution depth **6** (Dataverse's own limit is 8) the engine stops with a readable error.
3. **Load the chain:** the live, active rules for this trigger. They are read as system, so permissions never hide a rule.
4. **Build the record:** the pre-image with the Target (the changed columns) on top. See the next part.
5. **Drop untouched rules:** an Update rule with triggering columns, none of which are in the Target, is left out. It isn't run or logged.
6. **Run the chain:** by priority, then rule key. Each rule:
   - evaluates its condition tree (AND/OR groups, nested);
   - runs THEN or ELSE, top to bottom;
   - **stages** its field changes, and commits them only when the whole rule succeeds. A rule that fails halfway leaves no half-done changes behind.
   - Later rules read the committed changes of earlier ones.
7. **Stop early:** when a rule with "Stop processing after match" matches, or a rule blocks.
8. **Apply the result:**
   - **Before save:** the changes are set on the Target, so they go into the same save. There is no extra update and no extra trigger. A block throws an error with the rule's message, and the save is rolled back.
   - **After save:** the changes are written as one Update per identity (see "Who the rule runs as"). That Update can trigger Update rules again, and the recursion guard limits that.
   - **Delete:** field changes are ignored, but a block still stops the delete.
9. **Log:** one execution log row per rule run, depending on its log level.

### The record images: before, changes and the full picture

Dataverse only sends the columns that change, in the **Target**. A rule like "Total > 10,000" also needs Total when only Discount changed. The engine therefore merges:

`full record = pre-image (all columns, as before the save) + Target (the new values) on top`

| Event | Pre-image | Target | What the rule sees |
| --- | --- | --- | --- |
| Create | — | All set columns | Only the new values |
| Update | The record before the save | The changed columns | The full record after the change |
| Delete | The record before the delete | — | The record as it was |

- **Changed** compares the merged value with the pre-image. "Status changed" is true only when this save sets Status to a different value.
- **Multi-select choice:** Changed compares the set of options, so their order doesn't matter.
- **Each rule sees its own changes first**, then earlier rules' changes, then the record.
- In **Test & trace**, the real record you pick plays the pre-image and your sample values play the Target. That's why sample values count as changed columns.

### Before save vs after save

| | Before save | After save |
| --- | --- | --- |
| Dataverse stage | PreOperation, synchronous | PostOperation, asynchronous (system job) |
| Inside the user's transaction | Yes | No |
| Set field | Written into the same save, at no extra cost | A follow-up Update |
| Block | Yes, the save is rolled back | No, and failures never stop the chain |
| The user waits | Yes, so keep it fast | No |
| Retries | At most 2, at most 1 s apart | At most 10, up to 60 s apart |
| Failure | Block or Log and continue | Always logged and flagged for an admin |

- **Rule of thumb:** use before save to validate and to set fields on the same record. Use after save for work on other records, such as tasks, webhooks or assignments.
- **Plug-in time limit:** the 2-minute limit covers the whole chain. That's why paths, rows and retries have caps.

### Following relations: field paths

A **field path** starts at the record being saved and follows relations one **hop** at a time.

- **Parent hop (N:1):** follows a lookup column up.
  - `quote → account (customerid) → contact (primarycontactid) → emailaddress1`
  - Each hop reads one record, by id, with only the column it needs.
  - If a lookup is empty, the path ends with an empty value and the rule doesn't fail.
  - A lookup that points to another table than the path expects (for example a customer that is a contact rather than an account) also gives empty.
- **Child hop (1:N):** follows a relation down, to many rows: `quote → quotedetail (quoteid)`.
  - A child path is a list, so a condition must say how to use it:
    - **Any / All / None-of:** check a condition per child row. The trace shows "matched X of Y".
    - **Count / Sum / Min / Max / Average:** one number, computed **by Dataverse** in one aggregate FetchXML query. The rows are never loaded.
  - A child path without an aggregate is rejected at publish.
- **Mixed paths:** parent hops first, then child hops, then more hops. For example: `quote → account → opportunities → opportunityproducts → amount`.
  - The engine follows the parent hops record by record.
  - From the first child hop on, the rest becomes **one FetchXML query** with inner link-entities.
- **Empty sets:**
  - All over no rows is true ("0 of 0"), and Any is false.
  - Count gives 0, while Sum, Min, Max and Average give empty.
- **Unsaved record:** on Create the record has no id yet, so child paths give no rows.
- **Read once per save:** each parent record and column is read once and cached for the whole chain, so ten rules reading `account.creditlimit` cost one read. Child queries run per use.
- **Expressions** use the same paths: `[parentcustomerid:account.creditlimit]`.

| Limit | Value | Checked |
| --- | --- | --- |
| Hops per path | 5 | At publish and at run time |
| Child rows per path (and lookup rows matched) | 4,999 | At run time; going over fails the rule |

### How a lookup finds its answer

Example: "Discount for quote" returns `Discount %` from the Discount table. The match is the quote's customer type and country.

**1 · Read the values from the record being saved.** Each criterion gets its value from a field path (which can hop, like `quote → account → country`), a fixed value or an expression.

**2 · Build one query against the table with the answers.** Every criterion becomes one filter, and all filters must hold (AND).

- The query only returns **active rows**, and only the columns it needs.
- It asks for one row more than the limit, to detect when it's crossed.

| Criterion | Filter on the table with the answers |
| --- | --- |
| Equals | `column = value` |
| In list | `column in (values)` |
| Range | `min column ≤ value` and `max column > value`. An empty bound is open, so the upper bound is exclusive and ranges can chain (0–10,000, 10,000–50,000 …) without overlapping |
| Is empty | `column is empty` |
| + Empty matches all | Also matches rows where the column is empty (a wildcard row, such as "any country") |

Two notes:

- If the record's own value is empty, Equals and In list match rows where the column is empty.
- An empty value against a range matches only rows whose bounds are empty.

**3 · Pick one row.**

| Rows found | Result |
| --- | --- |
| 0 | The **No match** policy: **Block** fails the rule with "No row … matches", **Use default** returns the default value, and **Skip** skips the condition or action that used the lookup |
| 1 | Its return column |
| More than 1 | The **Multiple matches** policy: **Most specific** takes the row with the fewest wildcards, then the first by sort column; **First by sort** takes the first row by sort column; **Error** fails the rule |

**Most specific in practice:**

| Customer type | Country | Discount % |
| --- | --- | --- |
| Gold | DK | 15 |
| Gold | *(empty = any)* | 10 |
| *(empty = any)* | *(empty = any)* | 0 |

- A Gold quote from Denmark matches all three rows, and **15** wins because it has 0 wildcards.
- Gold from Sweden gets **10**, and every other quote gets **0**.
- That's a full discount matrix with a fallback, and no rule logic.

**4 · Fresh every time.** The rows are read when the rule runs, not at publish. Changing a row in the Discount table takes effect on the next save. Changing the **definition** (criteria, columns, policies) takes effect when the rules that use it are published again, because they're compiled into the snapshot.

**5 · Traceable.** The trace and the full-trace log show how many rows matched, which row won, how many wildcards it had, and which policy applied. **Test lookup** on the Lookup definitions page runs exactly this against a real record.

### Who the rule runs as

- **Calling user:** the rule reads, queries, calls functions and (after save) writes with the user's own permissions.
  - A record the user can't read gives an empty value, and does not cause an error.
  - Lookups and child aggregates only see the rows the user may see.
- **System:** full permissions, whatever the user's role.
- **Mixing them in one chain:**
  - Rules next to each other with the same identity are evaluated together, and later rules still see earlier changes.
  - Before save, all changes go into the one save.
  - After save, the changes are written as one Update per identity: the system rules' columns as system, and the rest as the calling user.

### Retries and failures

- **Retry:** a failed attempt is thrown away completely, along with its staged changes, and the rule runs again from the start after the retry delay. Caps: before save at most 2 retries, at most 1 s apart. After save at most 10, up to 60 s apart.
- **No retry when:**
  - the rule blocked on purpose (Block save, or a lookup with No match = Block);
  - the attempt already really called a function marked **not safe to retry**, so a task or webhook is never sent twice.
- **After the last attempt:**
  - **Before save, On failure = Block:** the save is stopped with "RULE.KEY: message".
  - **Before save, Log and continue:** the failure is logged, and the chain and the save go on.
  - **After save:** it can't block, so it is always logged, flagged for an admin, and the chain goes on.
- **Function timeout:** a call that takes longer than its timeout counts as failed and its result is discarded. A synchronous call can't be stopped halfway, so the time is still spent.
- **Values are converted on write:**
  - whole numbers round half away from zero;
  - text that's too long, or a number outside the column's range, fails with a readable message;
  - a lookup value must point to one of the column's tables.

### What gets logged, and when

- **Rows:** one `rs_executionlog` row per rule run, written as system.
- **Correlation ID:** every rule of one save shares the Dataverse **correlation ID**, and so does the after-save run of that save. Filter on it in Execution logs to see one save from start to end.
- **Blocked saves leave no log row:** the block rolls back the save's transaction, including the log row. The outcome goes to the **plug-in trace log** instead, which is kept when the environment's plug-in trace setting is Exception or All.
- **Not logged:** skipped rules (no triggering column changed) and inactive rules. Inactive rules aren't even loaded.

## 14. Reference

*Reference › Documentation*

**TLDR:** This guide, inside Rule Studio, plus background for admins and developers.

### This page

- **Contents (left):** every section. Click one to jump to it.
- **Sections (right):** the same text as `RuleStudioApp/docs/user-guide.md`.
- To update it: edit that file, run `node guide.build.mjs` in the WebResources project, then build and deploy.

### Where the data lives

Rule Studio stores everything in its own Dataverse tables: rules, rule versions, conditions, actions, custom functions, lookup definitions, releases and execution logs. `RuleStudioApp/DeployDataModel/Program.cs` defines the full model.

| Dataverse table | Used for |
| --- | --- |
| `rs_rule`, `rs_ruleversion` | Rules, live versions and drafts |
| `rs_lookupdefinition` | Lookup definitions |
| `rs_executionlog` | Run and failure history |
| `rs_release` | Release history |

### For developers: page standard

Every page follows one layout standard; see `RuleStudioApp/docs/ui-standard.md`.

| Page type | Pages |
| --- | --- |
| Record form | Rule editor, Custom function, Lookup definition, Release details, Execution detail |
| List page | Rule library, Releases, Execution logs, Execution timeline, Test & trace, Custom functions, Lookup definitions, Environments, Documentation |
| Dialog | Publish, Rollback, Restore |
| Dashboard | Overview |

- Tests fail the build if a page uses old layout patterns (`ui-standard.test.mjs`) or a new field has no help text.
- All field help texts live in one shared file, so a field shows the same text on every page.
