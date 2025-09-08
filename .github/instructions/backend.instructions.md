# Backend development — senior quick guide

Architecture
- Azure Functions isolated (net9.0) with thin triggers over application services.
- Projects:
  - Functions: HTTP + Service Bus triggers, DI and middleware (`src/Workflow.Functions`).
  - Application: CQRS-style handlers, orchestrations, Result<T> (`src/Workflow.Application`).
  - Infrastructure: repositories, messaging, Azure SDK clients (`src/Workflow.Infrastructure`).

Daily dev loop
```powershell
# Build + tests (fast)
dotnet restore WorkflowService.sln -nologo
dotnet build WorkflowService.sln -c Debug -nologo --no-restore
dotnet test WorkflowService.sln -c Debug -nologo --no-build
```

Where to put code
- New business logic: add a command/query + handler in Application. Keep it deterministic.
- Data access or external calls: add/update repository or client in Infrastructure.
- Triggers: route request/queue messages to Application handlers; no business logic in triggers.
- DI: wire in `src/Workflow.Application/ServiceCollectionExtension.cs` and consumed in `Program.cs`.

Local run (optional)
- Create `src/Workflow.Functions/local.settings.json` with `Values` including:
  - `AzureWebJobsStorage`, `FUNCTIONS_WORKER_RUNTIME=dotnet-isolated`, `shared:iam:authority`, and any connection strings/URIs referenced by your handler.
- Start the host via debugger or Functions Core Tools. Missing config keys will throw at startup; add them incrementally.

Contracts and error handling
- Prefer Result<T> returns for recoverable errors. Throw for truly exceptional states.
- Validate inputs early; log with structured templates. Avoid leaking infrastructure exceptions across layers.

Performance and reliability
- Keep handlers idempotent where invoked from queues. Use message dedup or idempotency keys if available.
- For Cosmos DB, batch or parallelize with care, respecting RU limits. Add retry Policies (exponential backoff) around Azure SDK calls.

Release build parity
```powershell
# Replicate CI conditions locally (linux-x64, Release)
dotnet build WorkflowService.sln -c Release -r linux-x64 --no-restore
# Publish Functions only
dotnet publish src/Workflow.Functions/Workflow.Functions.csproj -c Release -r linux-x64 --self-contained false -o ./.out/func --no-build
```

Code review checklist
- Trigger thinness: no business logic in Functions.
- Tests exist for new handlers and failure paths.
- Config keys are named consistently; secrets are read from configuration providers, not hardcoded.
- Logging is actionable and not noisy; no PII unless required and approved.

References
- `src/Workflow.Functions/Program.cs` for middleware/OpenAPI/DI wiring.
- `pipeline/stages/build_and_test.yml` for CI build/publish expectations.
- `.github/testing.instructions.md` for testing patterns.
