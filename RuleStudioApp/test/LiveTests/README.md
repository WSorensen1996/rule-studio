# Live tests

Opt-in tests against a real Dataverse environment. They cover what XrmMockup can't show:
- the deployed engine on real saves;
- after-save updates;
- FetchXML aggregates over real child records.

Without `RULESTUDIO_LIVE_URL` every test is skipped, so `dotnet test` and CI never touch an
environment.

```bash
RULESTUDIO_LIVE_URL=https://yourorg.crm.dynamics.com dotnet test test/LiveTests
```

- `RULESTUDIO_LIVE_CREDENTIAL` picks the sign-in: `browser` (default), `azcli` or `devicecode`.
- **Before running:** the RuleStudio solution, plug-ins and engine must be deployed in that
  environment. The tests test what is deployed, not your working copy.
- **What the tests create:** every rule, record and release is named `TEST – live …`. The test class
  rolls its releases back and deletes everything it created when it finishes.
- **Timing:** the engine caches a chain for 30 seconds, so each test retries its save for up to
  45 seconds after publishing.
- **Where to run them:** use a development or test environment, never production.
