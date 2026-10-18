# Rule Studio: JSON formats (v1)

Five memo columns hold JSON. The web resources, the Custom APIs and the engine all read and write these columns, so each needs one agreed shape. They all build on two shared pieces: a **field path** and a **value source**. Those two also define the existing JSON columns `rs_condition.rs_fieldpath` and `rs_valuepath`, and `rs_action.rs_valuepath` and `rs_actioninputmapping.rs_valuepath`.

| Column | Written by | Read by |
|---|---|---|
| `rs_lookupdefinition.rs_matchcriteria` | new-lookup-definition dialog | lookup-definitions page, engine, publish validation |
| `rs_customfunction.rs_inputschema` | register-function dialog | custom-functions page, rule editor (input mapping), publish validation |
| `rs_release.rs_validationresult` | Validate & Publish Custom API (also returned by Validate-only) | publish-release dialog, release-detail pane |
| `rs_executionlog.rs_trace` | engine (live runs) and Simulate Custom API (test runs) | log-detail pane, test-trace page |
| `rs_ruleversion.rs_compiledsnapshot` | Validate & Publish Custom API | engine, Simulate Custom API |

Types: TypeScript in `src/Dataverse/WebResources/src/shared/formats.ts`, C# in `src/Shared/SharedDomain/RuleStudio/Formats/` (namespace `SharedDomain.RuleStudio.Formats`).

## General rules

- **Envelope:** every document is a JSON object with `"version": 1`. Readers reject versions they don't know and show "Unsupported format". The version only increases for breaking changes. Adding optional properties isn't breaking.
- **Naming:** properties are camelCase. Enumerations are camelCase strings (`"fieldPath"`, `"notMatched"`), never choice integers. The JSON then reads the same in every environment and in Git exports.
- **Dataverse names:** tables and columns are referenced by **logical name**. Display names are never stored; the UI resolves them from metadata when it renders.
- **IDs:** GUIDs are lowercase, without braces.
- **Times:** ISO 8601 in UTC (`2026-10-16T13:58:53Z`).
- **Absent means default:** optional properties are left out rather than written as `null`.

## Shared building blocks

### Field path

A field path is the route from the triggering record to a column, as a list of hops.

```json
{
  "table": "quote",
  "hops": [
    { "kind": "parent", "attribute": "customerid", "table": "account" }
  ],
  "column": "rs_segment"
}
```

| Property | Meaning |
|---|---|
| `table` | Logical name of the table the path starts from (the rule's target table) |
| `hops` | Empty for a column on the record itself |
| `hops[].kind` | `parent` follows a lookup on the current table (many-to-one). `child` follows a lookup on another table that points back (one-to-many). |
| `hops[].attribute` | For `parent`: the lookup column on the current table. For `child`: the lookup column on the child table that points to the current table. |
| `hops[].table` | The table reached. It is always stated, so polymorphic lookups (customer, owner) are unambiguous. |
| `column` | Logical name of the column read at the end of the path |

Any `child` hop makes the path return a set, so it must be used with an aggregate (validation code `CHILD_PATH_WITHOUT_AGGREGATE`). The maximum hop count is checked at publish (`HOP_LIMIT_EXCEEDED`). The format itself has no limit.

### Fixed values

`FixedValue` is one of:

- a string, number or boolean;
- `null`;
- a reference, `{ "table": "product", "id": "…", "name": "Analytics" }`. `name` is display-only.

Dates are ISO strings. Money is a plain number in the record's currency. Choices use their integer value.

### Value source

A value source is the same choice as the `rs_valuesourcetype` global choice.

```json
{ "type": "fixed", "value": 20 }
{ "type": "fieldPath", "path": { "table": "quote", "hops": [], "column": "rs_country" } }
{ "type": "expression", "expression": "[totalamount] * 0.1" }
{ "type": "lookup", "lookupDefinitionId": "…" }
{ "type": "function", "customFunctionId": "…" }
```

**Inside `rs_matchcriteria`, only `fixed`, `fieldPath` and `expression` are allowed.** This stops a lookup from calling another lookup or a function. On rows (conditions, actions and input mappings) the existing columns hold the same information, so no value-source JSON is stored there:
- `rs_valuesourcetype`, `rs_value`, `rs_valuepath` (a field path as above);
- `rs_lookupdefinitionid`, `rs_customfunctionid`.

## 1. `rs_matchcriteria`: lookup match criteria

Decides which rows of the lookup table match the triggering record. All criteria must hold (AND).

`recordTable` is the table of the record the lookup is evaluated for. Every field path in the criteria starts there (`path.table` = `recordTable`). The rule editor only offers lookups whose `recordTable` equals the rule's target table. A mismatch blocks publishing with `LOOKUP_INVALID`. A lookup usable from several tables would need named inputs, which is deferred to a later version.

```json
{
  "version": 1,
  "recordTable": "quotedetail",
  "criteria": [
    { "operator": "equals", "column": "rs_productid",
      "value": { "type": "fieldPath", "path": { "table": "quotedetail", "hops": [], "column": "productid" } } },
    { "operator": "equals", "column": "rs_country", "emptyMatchesAll": true,
      "value": { "type": "fieldPath", "path": { "table": "quotedetail", "hops": [{ "kind": "parent", "attribute": "quoteid", "table": "quote" }], "column": "rs_country" } } },
    { "operator": "range", "minColumn": "rs_minquantity", "maxColumn": "rs_maxquantity",
      "value": { "type": "fieldPath", "path": { "table": "quotedetail", "hops": [], "column": "quantity" } } },
    { "operator": "in", "column": "rs_segment",
      "values": [{ "type": "fixed", "value": 126250000 }, { "type": "fixed", "value": 126250001 }] }
  ]
}
```

| Operator | Properties | A lookup row matches when |
|---|---|---|
| `equals` | `column`, `value`, optional `emptyMatchesAll` | `row[column] = value` |
| `in` | `column`, `values[]`, optional `emptyMatchesAll` | `row[column]` is one of `values` |
| `range` | `minColumn`, `maxColumn`, `value`, optional `emptyMatchesAll` | `row[minColumn] ≤ value < row[maxColumn]` |
| `isEmpty` | `column` | `row[column]` is empty |

- **Empty bounds on `range`:** an empty `minColumn` means no lower bound, and an empty `maxColumn` means no upper bound.
- **Wildcards:** `emptyMatchesAll: true` makes a row whose column is empty match any value. Each wildcard used to match counts toward "fewest wildcards" in the **Most specific** multi-match policy.
- **Display:** the lookup-definitions page renders each criterion as one line, for example "Product = record → Product" or "Min qty ≤ Quantity < Max qty".

## 2. `rs_inputschema`: custom function inputs

The typed inputs of a registered function. Each `name` must equal the request parameter's unique name on the Custom API.

```json
{
  "version": 1,
  "parameters": [
    { "name": "RecordId", "displayName": "Record", "type": "lookup", "table": "quote", "required": true },
    { "name": "Currency", "displayName": "Currency", "type": "text", "required": true,
      "description": "ISO currency code, e.g. EUR." },
    { "name": "Amount", "type": "money", "required": false }
  ]
}
```

| Property | Meaning |
|---|---|
| `name` | Custom API request parameter unique name; unique within the schema |
| `displayName` | Optional label in the input-mapping editor (defaults to `name`) |
| `type` | One of the `rs_outputtype` values: `text`, `number`, `decimal`, `money`, `boolean`, `date`, `lookup` |
| `table` | Only for `lookup`: the expected table. It filters the field-path picker. |
| `required` | A rule that uses the function must map this input (`REQUIRED_INPUT_MISSING`) |
| `description` | Optional help text |

The register-function dialog replaces its free-text "recordId:Lookup, value:Decimal" field with a small parameter grid: name, type, required, and a table when the type is lookup. The custom-functions page shows the Required column from this schema.

## 3. `rs_validationresult`: publish checks

Produced by the Validate step. The **Validate-only** Custom API returns this document without saving it, and the publish-release dialog shows it before you confirm. **Validate & Publish** stores it on the release.

```json
{
  "version": 1,
  "checkedOn": "2026-10-16T13:58:53Z",
  "outcome": "passedWithWarnings",
  "rules": [
    { "ruleId": "…", "ruleKey": "PRICING.ANALYTICS_DISCOUNT", "ruleVersionId": "…", "versionNumber": 3, "costLevel": "medium" }
  ],
  "issues": [
    { "severity": "warning", "code": "FIELD_SET_CONFLICT",
      "message": "Discount (%) is also set by PRICING.VOLUME_DISCOUNT, which runs later and wins.",
      "ruleKey": "PRICING.ANALYTICS_DISCOUNT", "ruleVersionId": "…",
      "location": { "kind": "action", "id": "…" } }
  ]
}
```

- **`outcome`:** `passed`, `passedWithWarnings` or `failed`. `failed` means at least one `error` and blocks publishing.
- **`rules`:** the versions that were checked. `costLevel` (`low`/`medium`/`high`) is the value that will be written to `rs_ruleversion.rs_costlevel`.
- **`issues[].location.kind`:** `trigger`, `conditionGroup`, `condition`, `action`, `inputMapping` or `release`. Together with `id`, the rule editor can open and highlight the exact row.
- **`message`:** a complete sentence for authors.
- **`code`:** stable, so the UI can link to help. The code list is open; new codes can be added without a version bump.

| Code | Severity | Check |
|---|---|---|
| `TABLE_NOT_FOUND` | error | Target or path table no longer exists |
| `FIELD_NOT_FOUND` | error | A column in a field path or target field no longer exists |
| `TYPE_MISMATCH` | error | Operator or value doesn't fit the column type |
| `OPERATOR_NOT_ALLOWED` | error | e.g. `changed` on a Create trigger |
| `CHILD_PATH_WITHOUT_AGGREGATE` | error | Path with a child hop used without an aggregate |
| `HOP_LIMIT_EXCEEDED` | error | More hops than the configured maximum |
| `FUNCTION_NOT_REGISTERED` | error | Referenced function is missing or disabled |
| `REQUIRED_INPUT_MISSING` | error | A required input has no mapping |
| `INPUT_MAPPING_INVALID` | error | Unknown parameter or wrong type |
| `LOOKUP_INVALID` | error | Lookup table or columns missing, or match criteria invalid |
| `EXPRESSION_INVALID` | error | NCalc parse error, unknown field or unknown function |
| `BLOCK_SAVE_AFTER_SAVE` | error | Block-save action on an after-save rule |
| `NO_DRAFT` | error | A selected rule has no draft to publish |
| `FIELD_SET_CONFLICT` | warning | Two rules in the same chain set the same field |
| `RETRIGGER_CHAIN` | warning | An after-save update may trigger rules again |
| `SIDE_EFFECT_FUNCTION` | warning | Calls a function with side effects; it is mocked in tests |
| `ENGINE_NOT_REGISTERED` | warning | Added after publishing when the engine plug-in is not deployed yet: the rules are live but do not run until the engine is deployed and its steps reconciled (docs/engine.md) |
| `STEP_NOT_SUPPORTED` | warning | Added after publishing when Dataverse has no plug-in step for a rule's table and event; that rule cannot run |

## 4. `rs_trace`: execution trace

**One execution log row = one rule version evaluated against one record.** A test run or a live save that runs a chain of rules writes one row per rule, all sharing an `rs_correlationid`. The test-trace page groups them back into a timeline by correlation ID, ordered by `sequence`.

How much is written depends on the effective log level:
- **Off / Errors only:** no row, or a failed/blocked row with only `error` filled in.
- **Summary:** only the header fields.
- **Full trace:** everything. Test runs always use full trace.

```json
{
  "version": 1,
  "level": "fullTrace",
  "sequence": 2,
  "record": { "table": "quote", "id": "…", "name": "Q-1048" },
  "startedOn": "2026-10-16T13:58:53Z",
  "durationMs": 42,
  "outcome": "matched",
  "test": { "mockedFunctions": ["rs_SendApprovalRequest"] },
  "conditions": {
    "type": "group", "id": "…", "operator": "and", "result": true,
    "children": [
      { "type": "condition", "id": "…", "path": { "table": "quote", "hops": [], "column": "totalamount" },
        "operator": "greaterThan", "expected": 10000, "actual": 12500, "result": true },
      { "type": "condition", "id": "…",
        "path": { "table": "quote", "hops": [{ "kind": "child", "attribute": "quoteid", "table": "quotedetail" }], "column": "productid" },
        "aggregate": "any", "operator": "equals",
        "expected": { "table": "product", "id": "…", "name": "Analytics" }, "actual": 1, "result": true },
      { "type": "condition", "id": "…", "operator": "equals", "result": null, "notEvaluated": true }
    ]
  },
  "branch": "then",
  "actions": [
    { "id": "…", "type": "setField", "status": "done", "field": "discountpercentage", "before": 10, "after": 20,
      "lookup": { "lookupDefinitionId": "…", "matchedRowId": "…", "matchCount": 1, "wildcards": 1, "value": 20 } },
    { "id": "…", "type": "callFunction", "status": "done",
      "function": { "customFunctionId": "…", "customApiName": "rs_SendApprovalRequest", "mocked": true,
                    "inputs": { "RecordId": { "table": "quote", "id": "…" } },
                    "result": { "success": true, "message": "Mocked in test mode." }, "durationMs": 0 } },
    { "id": "…", "type": "logMessage", "status": "done", "message": "Analytics discount applied.", "messageLevel": "info" }
  ]
}
```

| Property | Meaning |
|---|---|
| `level` | `summary` or `fullTrace` (also tells the UI why sections are missing) |
| `sequence` | Position of this rule in the chain for this correlation (1-based) |
| `record` | The triggering record. `name` is captured at run time, so the log list doesn't need a lookup per row. |
| `outcome` | `matched`, `notMatched`, `skipped`, `failed` or `blocked` (mirrors `rs_outcome`) |
| `skipReason` | Only for `skipped`: `ruleDisabled`, `columnsNotChanged` or `stoppedByEarlierRule` |
| `test` | Present only for test runs. `mockedFunctions` lists the side-effect functions that weren't really called. |
| `conditions` | Tree that mirrors the condition groups. A condition carries `expected`, `actual` and `result`. A condition that short-circuiting skipped has `notEvaluated: true` and `result: null`. With an aggregate, `actual` is the aggregate result (count, sum, …); for Any, All and None-of it is the number of matching child records and `total` the number checked (All over 0 of 0 is true). Conditions can also carry `lookup` or `function` when their value came from one. A `function` has `timedOut: true` when the call took longer than the function's timeout; its result is then discarded and the error code is `FUNCTION_TIMEOUT`. |
| `branch` | `then`, `else`, or `none` (no actions for that branch) |
| `actions[]` | Performed in order. `status`: `done`, `skipped` or `failed`. `setField` carries `field`, `before` and `after`. `blockSave` carries `message`. `logMessage` carries `message` and `messageLevel` (`info`, `warning`, `error`). |
| `function` | `customApiName`, `inputs` (name → value), `mocked`, `result` (the uniform `success` / `value` / `message` contract) and `durationMs` |
| `lookup` | `matchedRowId` (absent when nothing matched), `matchCount`, `wildcards` and `value`. `policy` (`usedDefault`, `skipped`, `blocked`, `error`) appears when a no-match or multi-match policy decided the result. |
| `error` | `{ "code", "message", "attempt" }` for failed and blocked runs. `attempt` is 1-based. |
| `truncated` | `true` when the engine dropped detail to stay under the column size limit. Arrays are cut from the end. |

**Size:** `rs_trace` is a memo column. The engine stops adding detail once the serialized trace would pass about 90% of the column's maximum length, and then sets `truncated`.

## 5. `rs_compiledsnapshot`: compiled rule version

Written by Validate & Publish (docs/custom-apis.md) and never changed afterwards. The engine runs from this document alone, so it carries everything the version needs: the trigger, the runtime settings, the condition tree, the actions, and copies of the lookup definitions and custom functions it uses. Changing a lookup or function later doesn't change a published rule; it takes effect at the next publish.

```json
{
  "version": 1,
  "ruleId": "…", "ruleKey": "PRICING.ANALYTICS_DISCOUNT", "ruleVersionId": "…", "versionNumber": 3,
  "trigger": { "table": "quote", "event": "update", "stage": "beforeSave", "filteredColumns": ["totalamount"] },
  "runtime": { "priority": 100, "stopProcessing": false, "failurePolicy": "block", "runAs": "callingUser",
               "retryCount": 0, "retryDelayMs": 0, "logLevel": "summary" },
  "conditions": {
    "type": "group", "id": "…", "operator": "and",
    "children": [
      { "type": "condition", "id": "…", "path": { "table": "quote", "hops": [], "column": "totalamount" },
        "aggregate": "none", "operator": "greaterThan", "value": { "type": "fixed", "value": 10000 } }
    ]
  },
  "then": [
    { "id": "…", "type": "setField", "targetField": "discountpercentage",
      "value": { "type": "lookup", "lookupDefinitionId": "…" } },
    { "id": "…", "type": "callFunction", "value": { "type": "function", "customFunctionId": "…" },
      "inputs": [ { "name": "RecordId", "value": { "type": "fieldPath", "path": { "table": "quote", "hops": [], "column": "quoteid" } } } ] }
  ],
  "else": [],
  "lookups": [
    { "id": "…", "name": "Discount matrix", "table": "rs_discountmatrix", "returnColumn": "rs_discount",
      "sortColumn": "rs_order", "noMatch": "useDefault", "defaultValue": 0, "multiMatch": "mostSpecific",
      "matchCriteria": { "version": 1, "recordTable": "quote", "criteria": [] } }
  ],
  "functions": [
    { "id": "…", "name": "Send approval", "apiName": "rs_SendApprovalRequest", "outputType": "boolean",
      "hasSideEffects": true, "safeToRetry": false, "timeoutMs": 10000,
      "inputSchema": { "version": 1, "parameters": [] } }
  ]
}
```

- **Enumerations** use the trace's names: events `create`/`update`/`delete`; stages `beforeSave`/`afterSave`; group operators `and`/`or`; condition operators and aggregates as in `rs_trace`; action types `setField`, `blockSave`, `callFunction`, `logMessage`; message levels `info`/`warning`/`error`; failure policies `block`/`logAndContinue`; run as `callingUser`/`system`; log levels `off`/`errorsOnly`/`summary`/`fullTrace`; no-match `block`/`useDefault`/`skip`; multi-match `mostSpecific`/`firstBySort`/`error`.
- **Values** are value sources (see above). Fixed values follow decision 6: an `inList` condition holds an array, `between` holds `[min, max]`. A lookup's `defaultValue` is parsed the same way.
- **`inputs`** appear on conditions and actions whose value is a function. Inputs without a mapping are left out.
- **`filteredColumns`** is empty or absent when any column triggers the rule.
- **Hash:** `rs_snapshothash` is the lowercase hex SHA-256 of the UTF-8 snapshot text exactly as stored.
- Children are in `rs_order` order. Then and else actions are in their own order.

## Decisions

1. **Field references in expressions:** an NCalc expression refers to fields as bracketed parameters with dotted lookup names: `[totalamount]` for the record itself, and `[customerid.rs_segment]` for a parent path. Each segment before the last is a lookup column, and the engine resolves the target table from metadata. A polymorphic lookup is written `[customerid:account.rs_segment]`. Only parent hops are allowed, because child records are handled by aggregate conditions. Unknown segments block publishing with `EXPRESSION_INVALID`.
2. **Long values in the trace:** `before`/`after` values longer than 1,000 characters are cut to 1,000 characters plus `…`. The trace explains decisions; it doesn't archive data.
3. **Record name in the trace:** `record.name` is captured when the rule runs. The log list needs no extra lookups, and the name still shows after the record is deleted.
4. **One log row per rule:** not one per run. Each row then points at the exact rule version, and "Errors only" can log just the rule that failed.
5. **Choices in fixed values:** stored as integers, because those stay the same in every environment and language, unlike the labels.
6. **`rs_value` on rows:** for source type Fixed, `rs_value` holds the fixed value as JSON: `20`, `"Analytics"`, `true` or a reference object. For the `In list` operator it holds a JSON array, and for `Between` an array of two values `[min, max]`. For source type Expression it holds the NCalc text as is, not JSON. Readers treat text that isn't valid JSON as a single string.

## 6. Portable rules: export bundle and import report

**PortableRule** = a published `rs_compiledsnapshot` (§5) without environment ids, plus rule metadata:

```json
{ "version": 1, "ruleKey": "PRICING.X", "name": "…", "description": null, "enabled": true,
  "trigger": { … }, "runtime": { … }, "conditions": { … }, "then": [ … ], "else": [ … ], "hash": "…" }
```

- Removed from the snapshot: `ruleId`, `ruleVersionId`, `versionNumber`, `lookups`, `functions`, and the `id` of every condition node and action.
- Value sources reference `"lookup": "<lookup definition name>"` instead of `lookupDefinitionId`, and `"function": "<Custom API name>"` instead of `customFunctionId`.
- `hash` = SHA-256 (lowercase hex) of the compact JSON of the document without `hash` and `enabled` (enabling is per environment). It is the value stored in `rs_ruleversion.rs_basehash` and `rs_rule.rs_lastdeployedhash`.

**PortableLookup** = `CompiledLookup` (§5) without `id`, plus `description`.

**Bundle** = `{ "version": 1, "exportedOn": "…Z", "source": "<org unique name>", "rules": [PortableRule], "lookups": [PortableLookup] }`.

**ImportReport** = `{ "version": 1, "mode": "check"|"apply", "outcome": "clean"|"blocked"|"applied", "source"?, "releaseId"?, "rules": [{ ruleKey, name?, status, incomingHash?, currentHash?, baseHash?, message?, incoming?, current? }], "lookups": [{ name, status }], "issues": [ValidationIssue] }`. Status values: docs/custom-apis.md `rs_ImportRules`. `incoming`/`current` (PortableRule) are only set on conflicts, for the side-by-side diff.

C#: `ImportReport`, `ImportRuleResult`, `ImportLookupResult` in SharedDomain; TS: `src/shared/promotionApi.ts`.

**2026-10-17:** `rs_trace.rule.runAs` (optional, `callingUser` | `system`) records the identity the rule ran with.
