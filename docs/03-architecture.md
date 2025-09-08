# 3. Architecture - Modular Monolith

Projects:
- Turnament.Domain: Entities, Value Objects, Aggregates, Domain Services, Interfaces.
- Turnament.Infrastructure: EF Core DbContext, Configurations, Repositories, Migrations, Serilog, Persistence adapters.
- Turnament.Api: ASP.NET Core (Minimal APIs or Controllers) + DI wiring + Auth + Swagger.
- Turnament.Tests: xUnit tests (Domain, Algorithms, API minimal tests via WebApplicationFactory).

## Layering & Dependencies
Api -> Domain (contracts) & Infrastructure (for implementations)
Infrastructure -> Domain
Domain -> no project references

## Cross-Cutting
- Logging (Serilog)
- Validation (FluentValidation planned)
- Mapping (Manual or Mapster later)
- Configuration binding (options pattern)

## Domain Model (Initial)
Entities/Aggregates (simplified):
- Tournament(Id, Name, SeasonId, Sports, Divisions, RuleSets)
- Division(Id, TournamentId, Name, AgeGroup, SportId, RuleSetId, Groups, Teams, Bracket)
- Team(Id, Name, DivisionRegistrations, Players)
- Player(Id, TeamId, FirstName, LastName, DOB, Number)
- Venue(Id, Name, Pitches)
- Pitch(Id, VenueId, Name)
- Slot(Id, PitchId, StartUtc, EndUtc, IsBooked)
- Match(Id, DivisionId, HomeTeamId, AwayTeamId, GroupId?, Round, ScheduledStartUtc, PitchId?, Status, Score, Events)
- MatchEvent(Id, MatchId, Type, Minute, TeamId, PlayerId?, Data JSON)
- Group(Id, DivisionId, Name, Teams, Standings)
- GroupStanding(Id, GroupId, TeamId, Played, Won, Drawn, Lost, GoalsFor, GoalsAgainst, GoalDifference, Points, FairPlayPoints)
- KnockoutBracket(Id, DivisionId, Nodes)
- BracketNode(Id, BracketId, Round, Position, Parent1Id?, Parent2Id?, HomeSource, AwaySource, MatchId?)
- RuleSet(Id, SportId, PointsWin, PointsDraw, PointsLoss, TiebreakerOrder[], MatchDurationMinutes, ExtraTimeMinutes?, AllowPenalties, MinRestMinutes, RefRestMinutes)
- OfficialAssignment(Id, MatchId, RefereeUserId, Role)
- Registration(Id, DivisionId, TeamId, Status)

Value Objects: Score (Home, Away), TiebreakerOrder enum list, AgeGroup, SportCode.

## Persistence Strategy
- Fluent configurations per entity.
- Concurrency: optimistic via rowversion (bytea) where needed (Match, Standing).
- Seeding: minimal sample tournament via IHostedService in dev environment.

## Scheduling Algorithm (Outline)
1. Generate round-robin pairings per group (circle method).
2. Assign rounds to available Slots respecting rest & conflicts.
3. Heuristic improvement pass for minimizing gaps.

## Standings Calculation (Outline)
- On match finalization: recalc only affected group standings; update points, goals, fair play.
- Tiebreaker chain evaluation until unique ordering; if still tie, mark needsCoinToss flag.

## Knockout Bracket Seeding
- Pull top N teams per group according to advancement rules (config future). Seed to bracket positions to avoid early same-group conflicts.
