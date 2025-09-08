# 8. Seeding Strategy

## Approach
Provide a DevelopmentSeeder that runs on app startup in Development environment.

## Seed Data
1. Sports: Soccer
2. RuleSet: Default soccer rule set (Win 3 / Draw 1 / Loss 0, 60 min matches, 60 min rest, 30 min ref rest)
3. Tournament: "Spring Invitational"
4. Division: "U12 Boys"
5. Teams: 6 sample teams (Tigers, Eagles, Sharks, Wolves, Kings, Falcons)
6. Groups: 2 groups of 3 auto-balanced
7. Fixtures: Round-robin for each group (3 teams => 3 matches per group)
8. Standings: Initially zeroed

## Implementation Notes
- Use deterministic GUIDs (or static IDs) for doc reproducibility in tests.
- Guard seeder to not duplicate if data exists.
- Provide extension method AddDevSeeder().
