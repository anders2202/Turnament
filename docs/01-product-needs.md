# 1. Product / Tournament Needs

## Scope & Supported Sports
- Initial sport: Soccer (Football). Extensible to Futsal, Basketball later via RuleSet abstraction.
- Multi-division & multi-age-group per Tournament (e.g., U10, U12, Open, Masters).
- Seasonal context: Season entity ties multiple tournaments.

## Tournament Formats
- Group Stage (Round-Robin) feeding Knockout Stage.
- Support single group or multiple groups per division.
- Planned (later): Swiss-system & league-only tournaments.

## Locations & Scheduling
- Venues contain multiple Pitches.
- Pitch availability represented by Slots (start/end UTC) with capacity 1 match.
- Constraints: team rest period (default 60 min), referee rest (30 min), daylight window per venue (configurable), max concurrent matches per venue (pitch count), avoid overlapping matches for same team/referee.

## Roles & Personas
- Organizer: full admin of tournament.
- Referee: can view assignments, enter match events (cards, goals, subs), finalize match.
- TeamRep: manage team roster, confirm schedules, submit match sheet.
- Spectator: read-only public portal (standings, schedules, brackets, live scores).

## Internationalization & Time Zones
- All stored in UTC; client displays in user locale/time zone.
- Text content prepared for localization (resource keys on UI layer — future).

## Data Freshness / Live
- Match events posted in real time; standings recomputed after each status change to InProgress or Final.
- Webhooks / SignalR (future iteration) for push updates.

## Non-Functional Needs
- Target .NET 8 LTS, PostgreSQL 15+, container-friendly.
- Logging: Serilog (structured), minimal PII.
- Security: Role-based; OIDC integration placeholder.
- Performance: Scheduling algorithms must handle up to 500 matches / weekend efficiently (<5s generation target).
