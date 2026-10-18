// Field help (docs/ui-standard.md, "Field help"): the one list of plain-language explanations
// shown as tooltips. Tag a control with data-help="<key>" (HTML) or withHelp(el, "<key>") (code).
// Keep each text short: what the field is, what goes in it, and what happens.
export const FIELD_HELP = {
  // Rule identity and trigger
  ruleKey: "A short, unique ID for the rule, like PRICING.DISCOUNT. It stays the same in every environment, so imports can find the rule.",
  ruleTable: "The Dataverse table whose records start this rule, for example quote. Field paths in conditions start from this table.",
  ruleEvent: "What happens to the record that starts the rule: it is created, updated or deleted.",
  ruleStage: "Before save: the rule runs first and can change or block the save. After save: the rule runs once the record is saved and cannot block it.",
  ruleFiltered: "Only for Update. List the columns that should start the rule, separated by commas. Leave empty to run on any change.",
  ruleEnabled: "An inactive rule never runs, even when it is published. Deactivate to pause a rule without removing it; activate to run it again.",
  // Runtime behaviour
  priority: "The order rules run in on the same table and event. Lower numbers run first, so 10 runs before 100.",
  failurePolicy: "What happens if the rule breaks. Block: the save is stopped with an error. Log and continue: the error is logged and the save goes on.",
  runAs: "Whose permissions the rule uses. Calling user: the person saving. System: full access, so it can change records the user cannot.",
  retryCount: "How many extra times to try again if the rule fails. 0 means try once only.",
  retryDelay: "How long to wait between tries, in milliseconds. 1000 = 1 second.",
  logLevel: "How much is written to the execution log each time the rule runs. More detail helps debugging but stores more rows.",
  stopProcessing: "When this rule matches, no later rules run for the same table, event and stage.",
  versionNote: "A short note on what changed in this version. It shows in the version history and when a release is reviewed.",
  // Conditions and actions
  aggregate: "For child records only (for example the lines of a quote). Any / All / None-of check each child. Count, Sum, Min, Max and Average turn the children into one number.",
  operator: "How the field is compared with the value, for example Equals or Greater than. Changed is true when the save changes the field.",
  groupOperator: "AND: every condition in the group must be true. OR: at least one must be true.",
  valueSource: "Where the value comes from. Fixed: a value you type. Field: another field on the record. Lookup: a row found in a table. Function: a custom function. Expression: a small formula; an empty field in it makes the result empty, which clears the column.",
  actionType: "What the action does: set a field, call a custom function, block the save with a message, or write a message to the log.",
  targetField: "The field on the record that gets the new value.",
  blockMessage: "The error the user sees when the save is blocked. Say what is wrong and how to fix it.",
  messageLevel: "How important the log message is: info, warning or error.",
  customFunction: "A registered custom function (developer-made Custom API) that the action runs.",
  // Custom functions
  customApi: "The unique name of the Custom API in Dataverse that does the work. Use the search button to pick one from this environment.",
  outputType: "The kind of value the function returns, for example text, number or yes/no. Rules can only use it where that kind fits.",
  timeout: "The longest the function may run, in milliseconds. A call that takes longer counts as failed and its result is ignored (it cannot be stopped halfway). Leave empty for no limit.",
  functionEnabled: "An inactive function cannot be chosen in new rules, and rules that call it fail when they run.",
  sideEffects: "Turn on if the function changes data or calls other systems. Tests then use a fake result instead of running it.",
  safeToRetry: "Turn on if running the function twice does no harm. Only then may a failed rule try it again.",
  // Lookup definitions
  recordTable: "The table of the record the rule runs on. Criteria compare its fields with the rows of the lookup table.",
  lookupTable: "The table that holds the rows to search, for example a price list or a discount table.",
  returnColumn: "The column whose value the lookup gives back when a row matches.",
  sortColumn: "Used when several rows match and the policy is First by sort. The first row in this order wins.",
  noMatch: "What happens when no row matches. Block: stop the save. Use default: return the default value. Skip: do nothing.",
  multiMatch: "What happens when several rows match. Most specific: the row with the most matching criteria wins. First by sort: the first row by the sort column. Error: treat it as a failure.",
  // Execution logs
  correlationId: "One ID shared by every rule run in a single save. Paste it to see everything that happened in that save.",
  logOutcome: "Show every run, or only the runs that failed or blocked a save.",
  logMode: "Production: real saves by users. Test runs: runs started from Test & trace.",
  adminFlag: "Show only runs an admin marked for follow-up.",
  retentionDays: "How many days normal log rows are kept before the daily cleanup deletes them.",
  retentionErrorDays: "How many days failed and blocked runs are kept. Usually longer, so problems can still be checked.",
  // Test & trace
  traceVersion: "Draft where available: test the unpublished changes. Published only: test what runs in production today.",
  mockAll: "Decides whether the test really calls custom functions. On: no function runs; each returns an empty fake result, so the test is quick and changes nothing. Off: functions that only read or calculate really run, so the rules see their real values. Functions with side effects are never run in a test.",
  traceRecord: "Pick a real record to test with its data. Leave empty to test with values you type.",
  // Releases
  releaseName: "A name for this release, so it is easy to find in the release history, for example 2026-10 pricing update.",
} as const;

export type HelpKey = keyof typeof FIELD_HELP;
