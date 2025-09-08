# Copilot onboarding: workflow-service

Audience: Senior .NET backend developers

Purpose
- This repository is an Azure Functions (isolated worker) backend that handles workflow data (appointments, recalls, resources, notifications, schedules) for clinics. It exposes HTTP endpoints and processes Service Bus messages, persisting data to Cosmos DB and updating Azure AI Search.

Stack and repo facts
- Language/runtime: C# on .NET 9 (net9.0)
- App model: Azure Functions isolated worker (Microsoft.Azure.Functions.Worker)
- Projects (solution: `WorkflowService.sln`)
  - `src/Workflow.Functions` – function triggers (HTTP + Service Bus) and DI bootstrap
  - `src/Workflow.Application` – application logic (CQRS-style command/query handlers)
  - `src/Workflow.Infrastructure` – repositories/clients (Cosmos DB, Service Bus, Search, Blob Storage, Key Vault) and shared primitives (Result, messaging envelopes, constants)
  - `tests/App.Tests` – xUnit unit tests (no integration env required)
  - `tests/Workflow.Console` – small console harness for manual test/diagnostics
- IaC: Bicep under `.bicep/` (not required to build the app locally)
- CI/CD: Azure DevOps pipelines under `pipeline/` (build, publish, and deploy function artifacts)
- Verified SDK: .NET SDK 9.x (observed 9.0.304). Use 9.x to match pipelines.

Related guides
- Infrastructure (Bicep): `.github/instructions/infrastructure.instructions.md`
- Backend development: `.github/instructions/backend.instructions.md`
- Testing: `.github/instructions/testing.instructions.md`

How to build, test, and validate (Windows PowerShell)
- Always install .NET SDK 9.x before building.
- A clean, reliable sequence that works locally:
  1) Clean (optional when switching branches)
  2) Restore
  3) Build (Debug)
  4) Test

Commands
- The following were executed and validated successfully on this repo with many warnings but no errors. Tests passed (11/11).

```powershell
# Check SDK
dotnet --version

# Clean/restore/build/test
dotnet clean WorkflowService.sln -nologo
dotnet restore WorkflowService.sln -nologo
dotnet build WorkflowService.sln -c Debug -nologo --no-restore
dotnet test WorkflowService.sln -c Debug -nologo --no-build --logger "trx;LogFileName=TestResults.trx"
```

Notes and pitfalls
- Warnings: You will see hundreds of compiler/analyzer warnings across projects. They do not currently fail the build. Do not spend time resolving warnings unless your change introduces errors.
- Test suite: xUnit tests run fast and do not require cloud resources. If they fail after your change, fix the app logic or test setup before opening a PR.
- SDK mismatch: If build fails due to missing SDK, install .NET 9.x and retry. The CI uses `UseDotNet@2` with version `9.x`.
- No dedicated linter step is configured; build warnings act as the effective static analysis today.

Run locally (optional)
- Primary validation is via build + tests. Running the Functions host may require environment configuration.
- If you need to run `src/Workflow.Functions` locally:
  - Create `local.settings.json` in `src/Workflow.Functions` with at least `Values` for the keys your triggers need (Service Bus, Cosmos, Search, etc.). Connection names are referenced via constants (see `ConfigurationPathConstants` usage in triggers and `Workflow.Infrastructure.Constants`).
  - Program.cs requires `shared:iam:authority` in configuration. Provide it as an environment variable or in `local.settings.json` under `Values`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "shared:iam:authority": "https://id.example",
    "<other-required-connection-keys>": "<value>"
  }
}
```
- Start with your IDE’s debugger or (if Functions Core Tools are installed) use `func start`. If the app throws at startup about missing configuration, add the missing key under `Values`.

Project layout and where to make changes
- HTTP triggers live in `src/Workflow.Functions/Triggers/*` (resource/appointment/schedule/recall/etc.).
- Service Bus triggers live under `src/Workflow.Functions/Triggers/*/Subscriber/*`.
- Handlers: for new logic, add command/query handlers to `src/Workflow.Application/**` and wire them via DI.
- Repositories and external services are in `src/Workflow.Infrastructure/**`.
- Solution entry point for DI: `src/Workflow.Functions/Program.cs` adds middleware, OpenAPI, JSON config, and calls `services.AddWorkflowApplication(...)`.
- Tests for handlers belong in `tests/App.Tests/**`.

CI/CD details you should care about
- Pipeline entry: `pipeline/azure-pipelines-services-main.yml` triggers on `main` for `src/**`, `tests/**`, `pipeline/**`.
- Build/test/publish template: `pipeline/stages/build_and_test.yml`
  - Uses .NET SDK `9.x`
  - Restores all csproj files
  - Builds with `--configuration Release --no-restore -r linux-x64`
  - Publishes `src/Workflow.Functions` as a zip artifact (`func`) and copies `config/*.settings.json` as a separate artifact
- If your change breaks Release builds for `linux-x64`, fix the issue locally by replicating the same publish settings.

Local replication of CI publish (optional)
```powershell
# Equivalent local publish (Release, linux-x64)
dotnet restore WorkflowService.sln
# Build all projects for linux-x64 to catch RID issues
dotnet build WorkflowService.sln -c Release -r linux-x64 --no-restore
# Publish the Functions project
 dotnet publish src/Workflow.Functions/Workflow.Functions.csproj -c Release -r linux-x64 --self-contained false -o ./.out/func --no-build
```

Conventions
- CQRS-ish separation: keep orchestration in `Workflow.Functions` thin; put business logic in `Workflow.Application`; keep data access in `Workflow.Infrastructure`.
- Use the existing `Result<T>` for method outcomes rather than throwing where practical; log with structured templates.
- JSON options are centralized; prefer the existing provider where available.

Trust these instructions first
- Use the commands above for build/test/publish. Only search or change the sequence if these fail or if your task explicitly requires different steps. If something fails:
  1) Verify .NET 9.x is installed.
  2) Run restore before build; use `--no-restore` on subsequent steps.
  3) For local run, add missing configuration keys to `local.settings.json` under `Values`.

Root files quick reference
- Solution: `WorkflowService.sln`
- Config samples: `config/*.settings.json` (copied to artifacts in CI)
- Pipelines: `pipeline/**` (templates under `pipeline/stages/**`)
- IaC: `.bicep/**` (not needed for local build)
- Docs: `docs/README.md`
