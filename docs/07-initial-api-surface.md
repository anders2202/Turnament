# 7. Initial API Surface (Milestone 1)
Base URL prefix: /api

## Tournaments
- POST /api/tournaments { name, seasonId }
- GET /api/tournaments
- GET /api/tournaments/{id}
- PUT /api/tournaments/{id}
- DELETE /api/tournaments/{id}

## Divisions
- POST /api/tournaments/{tournamentId}/divisions { name, ageGroup, sportId, ruleSet }
- GET /api/tournaments/{tournamentId}/divisions
- GET /api/divisions/{id}

## Teams & Registration
- POST /api/divisions/{divisionId}/teams { name }
- GET /api/divisions/{divisionId}/teams

## Groups
- POST /api/divisions/{divisionId}/groups { name }
- POST /api/divisions/{divisionId}/groups/auto-balance { groupCount }
- GET /api/divisions/{divisionId}/groups

## Fixtures / Matches
- POST /api/divisions/{divisionId}/fixtures/generate (round-robin)
- GET /api/divisions/{divisionId}/matches
- PUT /api/matches/{matchId}/result { homeGoals, awayGoals, status }

## Standings
- GET /api/groups/{groupId}/standings

## Auth (Temporary)
- POST /api/auth/login { username, role }
- GET /api/auth/me

Swagger/OpenAPI auto-generated.
