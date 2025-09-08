# 6. Backlog

## Epics Overview
1. Tournament Management
2. Division & Registration
3. Scheduling & Venues
4. Group Stage Fixtures & Standings
5. Knockout Brackets
6. Match Events & Live Data
7. Referees & Assignments
8. Security & Roles
9. Multi-Sport Rule Engine
10. Platform & DevOps

---
## Epic 1: Tournament Management
### Stories
- T1.1 Create Tournament (name, season, sports)
- T1.2 List Tournaments
- T1.3 View Tournament Details
- T1.4 Update Tournament
- T1.5 Archive Tournament

## Epic 2: Division & Registration
- D2.1 Add Division to Tournament
- D2.2 Register Team to Division
- D2.3 List Division Teams
- D2.4 Configure RuleSet per Division
- D2.5 Create Groups (auto-balance)

## Epic 3: Scheduling & Venues
- S3.1 CRUD Venue
- S3.2 CRUD Pitch
- S3.3 Define Slots
- S3.4 Generate Schedule (assign matches to slots)
- S3.5 Validate Schedule Conflicts

## Epic 4: Group Stage Fixtures & Standings
- G4.1 Generate Round-Robin Fixtures
- G4.2 List Fixtures by Division/Group
- G4.3 Enter Match Result
- G4.4 Compute & View Standings
- G4.5 Tiebreaker Edge Case Handling (head-to-head)

## Epic 5: Knockout Brackets
- K5.1 Generate Bracket from Group Results
- K5.2 View Bracket
- K5.3 Update Knockout Match Result (progression)

## Epic 6: Match Events & Live Data
- M6.1 Record Goal
- M6.2 Record Card
- M6.3 Record Substitution
- M6.4 Live Match Feed Endpoint

## Epic 7: Referees & Assignments
- R7.1 Assign Referee to Match
- R7.2 List Assignments per Referee
- R7.3 Validate Referee Scheduling Conflicts

## Epic 8: Security & Roles
- A8.1 In-Memory Role Setup
- A8.2 Authorization Policies
- A8.3 OIDC Integration (later)

## Epic 9: Multi-Sport Rule Engine
- MS9.1 Sport Entity + RuleSet Linking
- MS9.2 Points & Duration Config
- MS9.3 Plug-in Sport Strategy

## Epic 10: Platform & DevOps
- P10.1 CI Pipeline (.NET build/test)
- P10.2 Code Quality (Analyzers)
- P10.3 Dockerfile & Container Run
- P10.4 Migrations Automation

---
## Sample Tasks (Milestone 1 Focus)
- Create solution + projects (P10.1)
- Add Domain entities (T1.1, D2.1, G4.1 subset)
- Configure DbContext + Migrations (P10.4)
- Implement round-robin generator service (G4.1)
- CRUD endpoints for Tournament, Division, Team, Group, Match (T1.*, D2.*, G4.*)
- Standings calculator (G4.4)
- Minimal auth roles (A8.1)
- Tests: round-robin, standings tiebreakers basic
