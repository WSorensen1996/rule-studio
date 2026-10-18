# Rule Studio: runtime engine

The engine is the one generic plug-in that runs published rules on save.

## Assembly

| | |
|---|---|
| Project | `src/Dataverse/RuleEngine` (net462 for Dataverse, net10.0 only for unit tests) |
| Output | `bin/Release/net462/Merged.RuleStudio.RuleEngine.dll`, ILRepack-merged and signed with `RuleStudio.snk` |
| Plug-in type | `RuleStudio.Engine.RulePlugin` |
| Merged in | NCalc, Antlr4, Newtonsoft.Json, SharedContext, SharedDomain, logging/DI abstractions. Not XrmPluginCore |
| Evaluator | The sources in `src/Dataverse/Plugins/Logic/RuleStudio/Engine` are linked, not copied, so live runs and `rs_SimulateRule` share one evaluator |

It is a separate assembly because XrmSync deletes steps that are not declared in code. XrmSync
never sees it: it has no XrmPluginCore registrations and is not the assembly XrmSync deploys.
Its steps belong to the step reconciler (`src/Dataverse/Plugins/Logic/RuleStudio/Steps`).

## Deploying

1. Deploy the Plugins assembly as usual (XrmSync). It contains `rs_ReconcileSteps` and the
   publish/rollback changes that reconcile steps.
2. From the `RuleStudioApp` folder:

   ```
   dotnet build src/Dataverse/RuleEngine -c Release
   dotnet run --project DeployEngine
   ```

DeployEngine signs in with a device code, checks the environment and the `RuleStudio` solution,
uploads the merged DLL as plug-in assembly (create, or update its content; sandbox isolation,
stored in the database), creates the plug-in type when missing, adds the assembly to the
`RuleStudio` solution and calls `rs_ReconcileSteps`. Run it again after every engine change.

**Other environments:** the assembly travels in the solution; the steps do not (they are not
solution components). Call `rs_ReconcileSteps` after each import as a safety net.

## Step ownership

`StepReconciler` computes the wanted steps from the published versions of the enabled rules and
makes the engine's `sdkmessageprocessingstep` rows match them exactly (details in
docs/custom-apis.md, `rs_ReconcileSteps`). It runs at the end of `rs_PublishRelease` and
`rs_RollbackRelease`, in their transaction, when `rs_enabled` of a rule changes, and on demand through `rs_ReconcileSteps`.

- Only steps of the plug-in type `RuleStudio.Engine.RulePlugin` are created, changed or deleted.
  Do not edit them by hand: the next publish puts them back.
- Runs as SYSTEM: publishers need no privileges on plug-in steps. It
  only ever makes the engine steps match what is published.
- If the engine is not deployed, publishing still succeeds with an `ENGINE_NOT_REGISTERED`
  warning in the release's validation result.
- Enabling or disabling a rule reconciles at once: the plug-in `ReconcileOnRuleEnabled`
  runs on Update of `rs_enabled` (synchronous, post-operation).

## What one execution does

1. Maps the message and stage to the trigger (Create/Update/Delete, 20 = before save,
   40 = after save). Other messages and stages do nothing.
2. **Recursion guard:** above execution depth 6 (`EngineSettings.MaxDepth`, below the platform's
   8) it throws a readable error: before save the save fails, after save the system job fails.
3. Loads the live rules of the table/event/stage as the system user (cache below).
4. Builds the record: the `PreImage` merged with the Target. Update rules with filtered columns,
   none of which are in the Target, are left out (not run, not logged).
5. Runs the chain by priority, then rule key. Each rule reads, writes and calls functions as its
   **Run as** identity: consecutive rules with the same identity are evaluated
   together, and later rules see earlier changes.
6. **Before save:** a block throws `InvalidPluginExecutionException` with the rule's message;
   otherwise the changes are set on the Target, whatever the identity. On Delete, field changes
   are ignored (a block still stops the delete).
   **After save:** the changes are written with one Update per identity (system rules' columns
   as system, the others as the calling user). That update runs the engine again for Update
   rules on the table, which the recursion guard bounds. A failing update marks the rules that
   set fields as failed and flags them. On Delete there is nothing to update.
7. Writes the execution log (below).

**Cache:** a loaded chain is reused for 30 seconds per organization, table, event
and stage. After that only the version ids and snapshot hashes are queried again; snapshots
whose hash is already known are not read or parsed again. A publish therefore takes effect in
at most 30 seconds on a warm sandbox worker.

## Comparing and writing values

Conditions compare normalized values (`Engine/Values.cs`, `Engine/Operators.cs`):

- **Text** compares as text, ignoring case: `"007"` is not `"7"`, and `"10"` sorts before `"9"`.
  A text is read as a number only when the other side is a number.
- **Numbers, money and choices** compare as numbers. **Yes/No** accepts true/false and 1/0.
- **Dates:** two dates with times compare instants. A value without a time (`"2026-01-01"`)
  compares calendar days in UTC, so `equals "2026-01-01"` matches any time that day.
- **Multi-select choice** is a set:
  - `equals`: the same options, in any order;
  - `contains`: every given option is selected;
  - `inList`: at least one selected option is in the list;
  - `changed`: the set differs from the pre-image.

  Ordered operators and Starts with are rejected at publish.
- **Lookups** compare ids; an id as text matches the reference.
- **Empty** values: `isEmpty` matches null and `""`; `notEquals` on an empty field matches; ordered
  operators and Between never match an empty field; an empty Between bound is open.
- **Child aggregates:**
  - Any, All and None-of count matching child rows; the trace has `actual` (matches) and `total`.
  - All over no child rows is true ("0 of 0").
  - Count without children is 0; Sum, Min, Max and Average without children are empty.

Expressions (NCalc) ignore case in built-in function names (`round`, `Round`), round half away
from zero like Set field, and compare text ignoring case. An empty field makes the formula's
result empty (NCalc's own rule), so Set field then clears the column.

A custom function that takes longer than its timeout (`rs_timeoutms`) fails with
`FUNCTION_TIMEOUT`. Its result is discarded, because a synchronous call cannot be stopped halfway.
No timeout means no limit, and mocked calls in test runs never time out.

Publish rejects fixed values an operator can never use: Between with the first value larger than
the second, Contains or Starts with on an empty text, and a Yes/No column compared with anything
other than true/false/1/0.

Set field converts the value to the column's type (`Engine/Coerce.cs`):

- Whole numbers round half away from zero (2.5 → 3).
- A number outside the column's minimum and maximum fails the rule with a readable message, and so does
  text longer than the column allows.
- Multi-select gets an option collection.
- A lookup value keeps its own table, which must be one of the column's targets. A bare id is accepted
  only for a lookup that points to one table.
- An empty value clears the column.

## Execution logging

One `rs_executionlog` row per rule run (`rs_istest` = false), written as the system user.
Effective level = the version's `rs_loglevel`, else Errors only.

| Level | Rows | `rs_trace` |
|---|---|---|
| Off | none | |
| Errors only | failed and blocked runs | header and `error` (`level` summary) |
| Summary | every run, skipped ones included | header and `error` |
| Full trace | every run | everything; cut from the end above ~90% of the column, with `truncated` |

After-save failures are always logged and get `rs_adminflag` = Yes, whatever the level.
The row also carries rule, version, table, record id, event, stage, outcome,
duration, attempts (retries included), error (code: message, max 4,000 characters),
`rs_correlationid` = the plug-in context's CorrelationId (shared by every rule of one save, and
by the after-save run of that save), and the effective log level.

**Limits:**

- **Blocked saves are not logged in `rs_executionlog`.** Before-save rows are written inside the
  save's transaction, and a block rolls the transaction back, so the engine does not write them.
  It writes each rule's outcome to the plug-in trace log (`ITracingService`) instead; it is
  kept when the environment's plug-in trace log setting is Exception or All. A non-transactional
  write would need an external service or an async hand-off; not done in v1.
- Inside the before-save transaction a failed log write cannot be swallowed: Dataverse has
  already aborted the transaction, so the save fails. The engine keeps every column within its
  length to make that unlikely. After save, a failed log write is traced and skipped.
- Skipped rules for unchanged filtered columns are never logged; disabled rules are not loaded.

## Tests

`ConditionMatrixTests.cs` runs every operator on every column type, `ChangedAndGroupTests.cs` the
Changed operator and nested groups, `ChildAggregateTests.cs` every child aggregate (with the
FetchXML the engine sends) and `SetFieldWriteTests.cs` the write conversions. They use the in-memory
`EngineHarness` with real column metadata. `ChainPolicyTests.cs` covers order, stop processing, block
and the failure policies and retry caps per stage, and `LoggingTests.cs` the log level × outcome ×
stage matrix and the log row. `EngineRuntimeTests.cs` runs the plug-in in XrmMockup on every trigger:
before and after save, Delete, filtered columns, a failing after-save update, the depth limit and
disabled rules. `RetentionTests.cs` covers the two bulk-delete jobs and which rows they delete.
`FieldPathAndExpressionTests.cs`, `FunctionTests.cs` and `LookupDefinitionTests.cs` cover the value
sources; `ValidationTests.cs`, `ReleaseFlowTests.cs`, `PromotionTests.cs` and `SecurityTests.cs` the
publish checks, releases, export/import and roles.

The page logic has unit tests in `src/Dataverse/WebResources/tests`. They cover the editor model and field
paths, the plain-words readout, the save changeset and the timeline's chain order. `tests/support/load.mjs`
bundles a TypeScript module with esbuild and replaces the Web API modules with stubs.

`test/LiveTests` is an opt-in suite against a real environment: real saves, after-save updates
and FetchXML aggregates. It is skipped unless `RULESTUDIO_LIVE_URL` is set; see its README.
`test/Tests/RuleStudio/StepPlannerTests.cs` covers the wanted-step computation and the plan;
`RulePluginLogicTests.cs` covers trigger mapping, record building, selection, run-as batches,
stop/block, apply and log shaping. The test project references the engine with the alias
`engine`, because the engine contains its own copy of the evaluator types. Dataverse itself
(steps, sandbox, transactions) is not exercised by these tests.
