# Testing — senior quick guide

Scope
- Unit tests live under `tests/App.Tests`. They do not require cloud resources. Target framework is .NET 9.
- Goal: fast feedback on handlers, repositories with fakes, and domain mapping.

Fast path
```powershell
# Clean restore build test (debug)
dotnet --version
dotnet clean WorkflowService.sln -nologo
dotnet restore WorkflowService.sln -nologo
dotnet build WorkflowService.sln -c Debug -nologo --no-restore
dotnet test WorkflowService.sln -c Debug -nologo --no-build --logger "trx;LogFileName=TestResults.trx"
```

When to add tests
- New command/query handler behavior, edge cases, and failure paths.
- Bug fix regression coverage.
- Complex mapping logic (e.g., migration adapters) and Result<T> cases.

Patterns and helpers
- Use the existing Result<T> pattern; assert on Success/Failure and Error codes/messages.
- Keep tests hermetic: stub infrastructure via interfaces in `Workflow.Infrastructure`.
- If HTTP triggers need coverage, test the application layer underneath the trigger instead.

Coverage in CI
- The pipeline publishes Cobertura from $(Agent.TempDirectory)/*/coverage.cobertura.xml. If you add coverage tooling, write to that path.

Tips
- Minimize mocking by preferring in-memory fakes for repositories/services where practical.
- Use data builders for verbose entities. Co-locate fixtures in `tests/App.Tests/**`.
- Keep assertions precise; avoid over-specifying implementation details.

Troubleshooting
- Flaky or slow tests: parallelize only pure tests, avoid relying on DateTime.UtcNow without abstraction.
- SDK mismatch: ensure .NET 9.x locally; CI uses UseDotNet@2 with 9.x.
- If tests fail only in Release linux-x64, replicate with:
```powershell
dotnet build WorkflowService.sln -c Release -r linux-x64 --no-restore
```
