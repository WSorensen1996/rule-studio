# Rule Studio

A general-purpose rule engine for Dataverse / Dynamics 365: Dataverse plug-ins and
Custom APIs, TypeScript web resource pages and XrmMockup tests. Built on the
XrmBedrock project template.
Requires .NET 10 SDK and Node.js 24 LTS. Run `nvm install && nvm use` from this
folder (`.nvmrc` selects Node.js 24).

## Configure your environment

`appsettings.json` is the shared DataverseConnection and generator/sync configuration
(solution, output paths, extra entities). Its `DataverseUrl` is a placeholder; point the
tools at your own environment in one of these ways:

- set the `DataverseUrl` environment variable, or
- create `appsettings.local.json` (git-ignored) next to `appsettings.json`:

```json
{
  "DataverseUrl": "https://yourorg.crm.dynamics.com"
}
```

The pinned Dataverse tools (XrmContext, XrmSync and others) read `appsettings.json` and
the `DataverseUrl` environment variable; the Rule Studio tools (`DeployDataModel`, `DeployApp`,
`DeploySecurityRoles`, `DeployEngine`, `TestData`) also read `appsettings.local.json`.

Set `DataverseCredentialType=devicecode` in the environment for headless setup, or `azcli`
to use an authenticated Azure CLI session; `browser` is the default. Sign-ins are cached
per environment URL and reused across all tools. Do not commit tokens or secrets.

## Install the solution

The quickest way to try Rule Studio: import [`Solution/RuleStudio.zip`](Solution/RuleStudio.zip)
(unmanaged) in Power Apps → Solutions → Import solution. It contains the tables, choices, pages,
model-driven app, security roles, plug-ins, Custom APIs and the runtime engine. After the import,
give users **Rule Studio Author** or **Rule Studio Viewer** together with Basic User. The engine's
plug-in steps are not part of the solution: they are created when you publish rules.

## Install from source

Run every command from this folder, in this order. Each tool signs in with your browser, works
only in the `RuleStudio` solution and can be rerun safely.

1. In your environment, create a publisher with prefix `rs` and choice value prefix `12625`
   (the code uses choice values 126250000 and up), and an unmanaged solution named `RuleStudio`
   with that publisher.
2. Point the tools at the environment (see [Configure your environment](#configure-your-environment)).
3. Create the tables and choices:
   `dotnet run --project DeployDataModel`
4. Set up the repository and build: `dotnet run --project Setup/PostSetup` (see [Setup](#setup)).
5. Deploy the management plug-in, its Custom APIs and the runtime engine (see [Deploy](#deploy)).
6. Build the pages (`npm ci && npm run build` in `src/Dataverse/WebResources`), then upload them
   and create the model-driven app with its navigation:
   `dotnet run --project DeployApp`
7. Create the **Rule Studio Author** and **Rule Studio Viewer** security roles and link them to the app:
   `dotnet run --project DeploySecurityRoles`
8. Give users one of the two roles together with Basic User.

DeployApp uses the model-driven app in the `RuleStudio` solution, or creates one named
`rs_RuleStudio` when the solution has none. It only replaces the "Rule Studio" area of the
app's navigation and keeps a backup of the previous sitemap in `DeployApp/backups`.

## Setup

Post-setup generates the signing key (`RuleStudio.snk`, git-ignored), restores local
dotnet tools, installs npm dependencies and builds the solution. It preserves an existing
signing key when rerun. No Dataverse environment is needed to build and run the tests.

```bash
dotnet run --project Setup/PostSetup
```

Add `-- --generate` to regenerate the contexts and test metadata from your environment
(see [Dataverse tools](#dataverse-tools)).

## Dataverse tools

All tools are pinned in `.config/dotnet-tools.json`. Run them **from this folder**:

```bash
dotnet tool restore
dotnet tool run xrmcontext
dotnet tool run xdt
dotnet tool run xrmmockup-metadata
dotnet build --configuration Release
dotnet test --configuration Release
```

`dotnet test` skips `test/LiveTests`, which run against a real environment only when
`RULESTUDIO_LIVE_URL` is set (see `test/LiveTests/README.md`).

XrmContext writes proxies to `src/Shared/SharedContext/Generated`, XDT writes declarations
and runtime option-set objects to `src/Dataverse/WebResources/typings/XRM`, and XrmMockup
writes metadata to `test/Tests/MetadataGenerated`. All three are committed, generated from an
environment with only Rule Studio installed. Regenerate them only after changing the data
model, and check the diff: the generators also pick up any custom columns your environment
has on the standard tables (account, contact, …), which should not be committed.

## Deploy

```bash
# Management plug-in and Custom APIs
dotnet build src/Dataverse/Plugins -c Release
dotnet tool run xrmsync -- plugins --assembly src/Dataverse/Plugins/bin/Release/net462/Merged.RuleStudio.Dataverse.Plugins.dll --solution RuleStudio

# Runtime engine (not managed by XrmSync, see docs/engine.md)
dotnet build src/Dataverse/RuleEngine -c Release
dotnet run --project DeployEngine
```

Plug-in dependencies are merged with ILRepack and signed with `RuleStudio.snk`.

**XrmSync removes obsolete components.** Always inspect a dry run (`--dry-run`) before
applying changes. Sync does not create the Dataverse solution/publisher or the tables: follow
[Install from source](#install-from-source) first.

## Web resource pages

Pages live in `src/Dataverse/WebResources/src/html/<page-name>/`, each with
`index.html`, `main.ts` and `styles.css`; shared code is in `src/shared`. The UI
conventions are in [docs/ui-standard.md](docs/ui-standard.md).

`index.html` is the document shell. It contains **exactly one** `<!--STYLE-->` and
`<!--SCRIPT-->` marker, which the build replaces with the bundled CSS and JavaScript.

- Each deployed page is one self-contained HTML file: no external scripts, stylesheets,
  image URLs or CDN references. Import dependencies from `main.ts`; local CSS imports and
  images/fonts are embedded as data URLs.
- Use safe DOM construction (`createElement`, `append`, `textContent`), not unsanitized
  HTML insertion. Do not disable the lint security checks.
- HTML linting enforces accessibility (labels, alt text, explicit button types, headings).

Run these commands from `src/Dataverse/WebResources`:

```bash
npm ci
npm run typecheck
npm run lint
npm test
npm run build                    # typecheck + strict lint + scripts and HTML
```

Pages become `dist/rs_RuleStudio/html/<page-name>.html`. Do not edit generated `dist/`
files. Preview a deployed page at
`<DataverseUrl>/WebResources/rs_RuleStudio/html/<page-name>.html`.

## Documentation

- [docs/user-guide.md](docs/user-guide.md): every page of the app; source of the in-app Documentation page
- [docs/engine.md](docs/engine.md): the runtime engine plug-in
- [docs/custom-apis.md](docs/custom-apis.md): the Rule Studio Custom APIs
- [docs/json-formats.md](docs/json-formats.md): the rule, trace and release JSON formats
- [docs/ui-standard.md](docs/ui-standard.md): the web resource page standard
