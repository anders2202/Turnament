# Turnament Platform

Initial scaffold for multi-sport tournament management (soccer-first).

## Tech Stack
- .NET 8, ASP.NET Core Minimal APIs
- EF Core (InMemory for dev; PostgreSQL planned)
- xUnit tests
- CI via GitHub Actions

## Run
```
dotnet build
cd src/Turnament.Api
dotnet run
```
Browse Swagger at http://localhost:5000/swagger

## Next Steps
- Replace InMemory DB with PostgreSQL + migrations
- Implement proper team-to-group assignment
- Add standings calculation endpoint
- Add scheduling & venue entities
