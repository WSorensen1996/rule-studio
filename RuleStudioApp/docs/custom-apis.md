# Rule Studio: Custom APIs

Custom APIs are registered in code (XrmPluginCore) in the XrmSync-managed plug-in assembly
(`src/Dataverse/Plugins`). They are unbound actions and run as the **calling user**, so the
caller's security role decides what they may do: a Rule Viewer gets a privilege error.

The web resource client is `src/Dataverse/WebResources/src/shared/releaseApi.ts`.

## `rs_ValidateRelease`

Runs the publish checks on the draft of each rule. Saves nothing.

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `RuleIds` | String, required | JSON array of `rs_rule` ids, e.g. `["…","…"]` |
| out | `ValidationResult` | String | `rs_validationresult` document (json-formats.md §3) |

## `rs_PublishRelease`

Validates again, then publishes all drafts as one release. Everything happens in the Custom
API's transaction: the whole release goes live or nothing changes.

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `RuleIds` | String, required | JSON array of `rs_rule` ids |
| in | `Name` | String, required | Release label (`rs_release.rs_name`) |
| in | `Description` | String, optional | `rs_release.rs_description` |
| out | `ValidationResult` | String | Same document as `rs_ValidateRelease` |
| out | `ReleaseId` | Guid | The new `rs_release`; the empty GUID when validation failed |

**When validation fails** (`outcome` is `failed`), nothing is written and `ReleaseId` is empty.
The call itself succeeds, so the dialog shows the issues. Unexpected errors (a row deleted
meanwhile, missing privileges) throw, and Dataverse rolls everything back.

**On success**, for each rule:

1. The draft version is compiled into `rs_compiledsnapshot` (json-formats.md §5) and its
   SHA-256 goes into `rs_snapshothash`. `rs_costlevel`, `rs_releaseid`, `rs_publishedon` and
   `rs_publishedby` are set, and `rs_status` becomes Published.
2. The version that was live before becomes Superseded.
3. The rule's `rs_publishedversionid` points at the new version and `rs_draftversionid` is cleared.

The release row gets status Live, the publish time and user, and the validation result.

4. **Engine steps are reconciled** (`rs_ReconcileSteps` below), in the same transaction, so the
   rules start running on the next save. If the engine plug-in is not deployed yet, the publish
   still succeeds: the result gets an `ENGINE_NOT_REGISTERED` warning (outcome `passed` becomes
   `passedWithWarnings`) and the release's `rs_validationresult` is updated with it. A table that
   has no step for the event adds a `STEP_NOT_SUPPORTED` warning the same way.

**Privileges:** step reconciliation runs as SYSTEM, so publishers need no
privileges on plug-in steps. Only engine steps are touched.

## `rs_RollbackRelease`

Puts every rule of a live release back on the version that was live before it, recorded as a
new release. Everything happens in the Custom API's transaction: all rules go back or nothing
changes. Web resource: `rollback-release` dialog.

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `ReleaseId` | Guid, required | The `rs_release` to roll back; must be Live |
| out | `ReleaseId` | Guid | The new rollback `rs_release` |

**Blocked** (throws, nothing changes) when the release is not Live, or when a rule version it
published is no longer the rule's `rs_publishedversionid`. The message names the rules a later
release changed: roll back the later release first.

**For each rule version V in the release:**

1. The previous version is the rule's Superseded version with the highest `rs_versionnumber` below V's, with an
   `rs_compiledsnapshot`, skipping versions whose release is Rolled back. Every new draft gets the next number and a rule
   has one draft at a time, so number order is publish order.
2. If there is one, it becomes Published again and `rs_publishedversionid` points at it. Its
   snapshot, hash and `rs_releaseid` are left as they were, so it still shows under the release
   that first published it (history is not rewritten). If there is none, the rule was first
   published by this release: `rs_publishedversionid` is cleared and the rule stops running.
3. V becomes Superseded. `rs_draftversionid` is not touched.

A new `rs_release` named "Rollback of <name>" is created with status Live, the publish time and
user, `rs_rolledbackreleaseid` set to the target. It has no
`rs_validationresult` and no linked versions. The target release becomes Rolled back.

**Not validated again:** the restored snapshots were valid when they were published.

**Engine steps** are reconciled at the end, in the same transaction (as SYSTEM, like
publishing). Reconcile warnings are not returned by this API; run `rs_ReconcileSteps` to see them.

## `rs_RestoreVersion`

Copies a rule version into a new draft of the same rule. Runs in the Custom API's transaction.
Web resource: `restore-version` dialog, opened from the rule editor's version history.

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `RuleVersionId` | Guid, required | The `rs_ruleversion` to restore |
| in | `ReplaceDraft` | Boolean, optional | Default false. Must be true when the rule already has a draft |
| out | `RuleVersionId` | Guid | The new draft `rs_ruleversion` |

**When the rule already has a draft:** with `ReplaceDraft` false the call throws ("This rule
already has a draft…"); with true the old draft becomes Discarded and is kept in history.
Restoring the current draft itself throws.

**The new draft** gets the next `rs_versionnumber` of the rule (highest + 1), status Draft,
source Authored, `rs_derivedfromversionid` = the restored version, change note "Restored from
vN", and the trigger and runtime fields of the source. Its condition groups (parent links
remapped), conditions, actions and input mappings (remapped to the new condition and action
ids) are deep-copied. Snapshot, hash, cost, release and publish fields are not copied. Groups
that scope an aggregate are not copied, as the editor does not author them yet. The rule's
`rs_draftversionid` points at the new draft.

## `rs_SimulateRule`

> Optional Boolean parameters that the caller leaves out reach the plug-in as false (seen live 2026-10-17), so every flag is phrased so that false is the default.

Runs rules against a real or sample record with the rule evaluator and returns the traces.
**Saves nothing:** the evaluator never writes, functions with side effects are
always mocked, and no `rs_executionlog` row is written. Runs as the calling user, so it reads
only the rules and records the caller may read. Web resource: `test-trace` page (opened from
the rule editor's ⚗ Test rule and the execution timeline's Test this chain).

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `RuleIds` | String, optional | JSON array of `rs_rule` ids. When set, `Table`, `Event` and `Stage` are ignored |
| in | `Table` | String, optional | Target table logical name; required when `RuleIds` is not given |
| in | `Event` | String, optional | `create`, `update`, `delete` or the `rs_event` choice value; required with `Table` |
| in | `Stage` | String, optional | `beforeSave`, `afterSave` or the `rs_stage` choice value; required with `Table` |
| in | `PublishedOnly` | Boolean, optional | Default false: evaluate each rule's draft where it has one, else its published version. True: published versions only |
| in | `RecordId` | Guid, optional | A real record of the table; loaded with all columns |
| in | `SampleValues` | String, optional | JSON object of column → value, applied over the record (fixed-value format, json-formats.md) |
| in | `CallFunctions` | Boolean, optional | Default false: every function is mocked. True: functions without side effects are called for real. Side-effect functions are always mocked |
| in | `LookupDefinitionId` | Guid, optional | Lookup test mode (below). `Table` is the record's table; `RuleIds`, `Event`, `Stage`, `PublishedOnly` and `CallFunctions` are ignored |
| out | `Trace` | String | JSON array of `rs_trace` documents (json-formats.md §4), one per rule in chain order; in lookup test mode a `{ "lookup": LookupTrace, "error"?: { code, message, attempt } }` object |

**Lookup test mode** (lookup definitions page, ⚗ Test lookup): evaluates one lookup definition's
match criteria against the record built from `RecordId` and/or `SampleValues` of `Table`, with
its no-match and multi-match policies. A Block or Error policy is returned as `error`, not thrown.

**Which rules run:**

- **`RuleIds`:** exactly those rules, disabled ones included (they were picked on purpose). They
  must share one table, event and stage, taken from their evaluated versions; otherwise the call
  throws and names the two triggers. A rule with no draft or published version to test, or with
  an incomplete trigger, throws.
- **`Table`/`Event`/`Stage`:** every active rule whose evaluated version uses the trigger, in
  chain order. Disabled rules are included as `skipped` with
  `ruleDisabled`. No matching rule throws.

**The record:** with `RecordId` the real record is loaded and sample values overwrite its
columns. Without it the record starts empty. On Update the sample values are the changed
columns (for filtered-column skipping) and the real record is the pre-image; without sample
values no rule is skipped for unchanged columns. On Create there is no pre-image. An unknown
column, an unreadable record or a `SampleValues` that is not a JSON object throws.

**The traces** are full traces with `test.mockedFunctions` set and `rule` filled in (`name`,
`draft` = the draft was evaluated). Rule failures are reported in the trace (`outcome` failed,
`error`), not thrown; a block shows as `outcome` blocked and later rules as skipped with
`stoppedByEarlierRule`.

## `rs_ReconcileSteps`

Makes the engine plug-in's steps (`sdkmessageprocessingstep` rows of the plug-in type
`RuleStudio.Engine.RulePlugin`) match the published versions of the enabled rules exactly.
Publish and rollback call the same logic; call this API after deploying the engine
or importing a solution, or whenever steps look wrong. Enabling or disabling a rule
reconciles automatically (plug-ins on Update of `rs_enabled`). See docs/engine.md.

| Direction | Name | Type | Notes |
|---|---|---|---|
| out | `Summary` | String | e.g. `Engine steps: 1 created, 0 updated, 2 deleted, 3 unchanged.` followed by any warnings |

**Rules considered:** active rules with a published version, `rs_enabled` not No. The trigger comes from the published `rs_compiledsnapshot`.

**Steps wanted:** one per table, message (Create/Update/Delete) and stage. Before save:
PreOperation (20), synchronous. After save: PostOperation (40), asynchronous with
`asyncautodelete`. Update steps filter on the union of the rules' filtered columns, or on all
columns when any rule of that chain has none. Update and Delete steps get a pre-image `PreImage`
with all columns. Steps are named `Rule Studio: <table> <message> <PreOperation|PostOperation>`.

**Changes made:** missing steps are created, differing ones updated (name, filtering attributes,
auto-delete, missing pre-image), and engine steps that no rule needs, or duplicates, are deleted.
Steps of any other plug-in type are never touched. Steps are not added to a solution: every
environment builds its own from its published rules.

**Engine not deployed:** nothing changes and `Summary` says so. Runs as SYSTEM in the Custom
API's transaction.

## `rs_ExportRules`

Returns the published rules as one portable bundle (json-formats.md §6). Changes
nothing; runs as the calling user. Web resource: `environments` page (⬇ Export published rules).

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `RuleKeys` | String, optional | JSON array of rule keys; absent exports every active rule with a published version |
| out | `Bundle` | String | `{ version, exportedOn, source, rules: PortableRule[], lookups: PortableLookup[] }`, rules sorted by key, only the lookups they use |

An unknown key throws. Rules without a published version are left out.

## `rs_ImportRules`

Checks or applies a bundle with the three-way merge. Runs as the
calling user in the Custom API's transaction. Web resource: `environments` page.

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `Bundle` | String, required | The output of `rs_ExportRules` |
| in | `Mode` | String, optional | `check` (default, changes nothing) or `apply` |
| out | `Report` | String | `ImportReport` (json-formats.md §6) |

**Per rule key** (incoming = hash of the bundle's rule, current = portable hash of this
environment's published version, base = `rs_rule.rs_lastdeployedhash`):

| Status | When | Apply |
|---|---|---|
| `unchanged` | current = incoming | nothing; base set to incoming |
| `create` | the key does not exist here and nothing was deployed | rule created, draft written, published |
| `update` | current = base (or the rule exists without a published version and was never deployed) | new draft (source Imported, `rs_basehash` = incoming), published |
| `keepTarget` | incoming = base, current differs | kept; back-merge to Git |
| `conflict` | all three differ | **blocks the whole import** |
| `targetOnly` | published here, not in the bundle | kept; back-merge to Git |

**Blocks** (outcome `blocked`, nothing changes): any conflict, a function the bundle calls that is
not registered here (`IMPORT_FUNCTION_MISSING`, matched by Custom API name), a lookup neither in
the bundle nor here (`IMPORT_LOOKUP_MISSING`), a duplicate or missing rule key. An existing draft of
a rule that will be updated is discarded and kept in history (warning `IMPORT_DRAFT_DISCARDED`).

**Apply** creates or updates the bundle's lookup definitions (matched by name), writes every
`create`/`update` rule as a draft and publishes them as one release "Import <time> (n rules)"
through `rs_PublishRelease`'s logic (validation, steps). If validation fails, the call throws and
everything rolls back. Afterwards `rs_lastdeployedhash` = incoming for every applied or unchanged
rule, and the published result is re-hashed: a mismatch adds a warning.

## `rs_ConfigureLogRetention`

Schedules log retention: cancels the waiting jobs whose name starts with "Rule Studio
log retention" and submits two daily recurring bulk-delete jobs on `rs_executionlog`. Runs as the
calling user, who needs the Bulk Delete privilege (System Administrator). Web resource:
execution-logs page, "Retention & administration".

| Direction | Name | Type | Notes |
|---|---|---|---|
| in | `Days` | Integer, optional | Absent or 0: 30. Rows older than this are deleted, except failed and blocked runs |
| in | `ErrorDays` | Integer, optional | Absent or 0: 90. Failed and blocked runs older than this are deleted |
| out | `Summary` | String | The schedule and how many earlier jobs were cancelled |

Both values must be 1–3650. The admin flag (`rs_adminflag`) does not protect a row from deletion;
clear or act on flags within the retention period (log detail: "Clear admin flag").

## Sample custom functions

Shipped in the management plug-in as examples for the Guide and the tests; registered in
`rs_customfunction` as "Sample – Calculate discount" and "Sample – Stamp contacts".

| Custom API | Inputs | Outputs | Behaviour |
|---|---|---|---|
| `rs_SampleCalculateDiscount` | `Revenue` Decimal, `Employees` Integer | `Success`, `Value` Decimal, `Message` | 10 % of revenue from 100 employees, else 5 %. Negative input → Success false. No side effects, safe to retry |
| `rs_SampleStampContacts` | `AccountId` EntityReference (account), `Text` String | `Success`, `Value` Integer (contacts updated), `Message` | Writes Text into `description` of the account's active contacts. Side effects, not safe to retry |

## Standard custom functions

Ready-made functions for rule authors, and examples of how to write your own
(`Logic/RuleStudio/Functions`, registered in `Registrations/RuleStudio`). They ship in the
solution as Custom APIs; register each one on the Custom functions page to use it in rules. All
inputs are optional at the API level so the function can answer with a readable message, and all
return `Success`, `Value` and `Message`. The validation functions treat an empty value as valid:
check "is filled in" with a separate condition.

| Custom API | Inputs | Value | Register as |
|---|---|---|---|
| `rs_ValidateEmail` | `Email` String | Boolean: the address has a valid form | Yes/no output; no side effects, safe to retry |
| `rs_ValidatePhone` | `Phone` String, `CountryCode` String (ISO, e.g. `DK`) | Boolean; the message gives the number as `+4512345678` | Yes/no output; no side effects, safe to retry |
| `rs_ValidateVatNumber` | `VatNumber` String, `CountryCode` String | Boolean: format, and check digits for AT, BE, CH, DE, DK, FI, FR, IT, NL, NO, PL and SE. Offline: it does not ask VIES whether the number is registered | Yes/no output; no side effects, safe to retry |
| `rs_CreateTask` | `RegardingTable` + `RegardingId` String, `Subject` String, `Description` String, `DueInDays` Integer, `OwnerId` EntityReference (user or team) | EntityReference: the new task | Lookup output; side effects, not safe to retry |
| `rs_AssignTo` | `RecordTable` + `RecordId` String, `OwnerId` EntityReference (user or team) | Boolean | Yes/no output; side effects, safe to retry |
| `rs_CallWebhook` | `Url` String, `Payload` String, `AuthorizationVariable` String | Integer: the HTTP status code; Success only for 2xx | Number output; side effects, not safe to retry |

- **Records** are given as a table name and an id, so any table works. For the record the rule
  runs on, map the table as a fixed value and the id from its primary key column. That record has
  no id yet in Create · Before save: create tasks after save. To change the owner of the record
  the rule runs on, a Set field action on `ownerid` is simpler than `rs_AssignTo`.
- **`rs_CallWebhook`** posts `Payload` (sent as JSON when it starts with `{` or `[`, else as text)
  to an https URL. `Url` may instead be the schema name of an environment variable that holds the
  URL, so it differs per environment. `AuthorizationVariable` names an environment variable whose
  value is sent as the `Authorization` header, which keeps secrets out of rules. A call gives up
  after 30 seconds. Dataverse plug-ins cannot call private network addresses.
- Functions with side effects are never run by Test & trace; the validation functions run there
  when "Mock all functions" is off.
