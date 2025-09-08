# 5. Milestones

## Milestone 1: Vertical Slice (Core Group Stage CRUD + Standings)
- Scaffold solution & projects
- Basic entities: Tournament, Division, Team, Group, Match, RuleSet
- EF Core PostgreSQL context + migrations
- Create tournament, add division, register teams, generate round-robin fixtures
- Enter match results; compute standings
- Minimal auth roles (in-memory)
- Swagger UI

## Milestone 2: Scheduling & Venues
- Add Venue, Pitch, Slot entities
- Scheduling service assigning matches to slots with constraints
- Referee assignment basics
- Conflict detection & validation endpoints

## Milestone 3: Knockout Brackets
- Bracket generation from group results
- Support match progression updates
- Extra time & penalties flags

## Milestone 4: Match Events & Live Updates
- MatchEvent model (goals, cards, subs)
- Incremental standings update per event
- Fair play calculation
- Basic SignalR hub (optional if time)

## Milestone 5: Hardening & Multi-Sport Readiness
- Sport-specific RuleSet injection
- Extended tiebreakers (head-to-head mini tables)
- Refactoring for Swiss (scaffolding only)
- Export / Import (CSV rosters)

## Milestone 6: Production Readiness
- OIDC integration option
- Serilog sinks (Seq / Console / File)
- Dockerfile & containerization
- CI enhancements (code coverage, analyzers)
