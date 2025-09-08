# 2. Assumptions & Constraints

## Operational Assumptions
- Max divisions per tournament: 20 (scalable).
- Max teams per division: 24 (groups of 3–8 typical).
- Group size flexible; attempt balanced groups (difference <=1 team).
- Each team guaranteed minimum group matches: group_size - 1 (round-robin single leg).

## Scheduling Constraints
- Team rest: default 60 minutes between match end and next kickoff (configurable per RuleSet / division).
- Referee rest: 30 minutes.
- Pitch buffer: 5 minutes turnover.
- Daylight window: venue-level (e.g., 08:00–20:00 local) — store as UTC offsets precomputed per date.
- Avoid overlapping assignments for: Team, Referee, Pitch.
- Knockout seeding must ensure teams from same group avoid meeting until semifinals when possible.

## Technical Constraints
- PostgreSQL as primary store; EF Core migrations.
- Deterministic algorithms (pure services) for: fixture generation, standings calculation, bracket seeding.
- All date/times stored UTC (DateTime/Instant). Use DateOnly for calendaring where needed.
- Soft deletes avoided initially (archive via status flag later).

## Security / Auth
- JWT / OIDC integration not implemented in first slice; use in-memory user store + roles for vertical slice.

## Performance Constraints
- Fixture generation O(n * m) where n=teams, m=rounds; optimize unsatisfied constraints via heuristic passes.
- Standings recompute incremental: update only affected teams in match events.

## Risks
See risk register (separate doc).
