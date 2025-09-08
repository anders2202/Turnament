# 4. Risk Register

| ID | Risk | Impact | Likelihood | Mitigation | Owner | Status |
|----|------|--------|------------|------------|-------|--------|
| R1 | Scheduling algorithm complexity leads to slow generation for large tournaments | High | Medium | Start with deterministic basic algorithm; profile early; add heuristic passes later | Tech Lead | Open |
| R2 | Data model changes after initial feedback cause heavy refactors | Medium | High | Keep domain model modular; use migrations; delay premature optimization | Architect | Open |
| R3 | Live updates feature scope creep (SignalR, caching) | Medium | Medium | Defer real-time push to later milestone; start with polling | PM | Open |
| R4 | Multi-sport rule divergence complicates RuleSet design | Medium | Medium | Use strategy pattern per sport; keep base interfaces small | Domain Lead | Open |
| R5 | Authentication integration delays MVP | Medium | Low | Provide in-memory role store and swap later | Dev | Open |
| R6 | Time zone confusion / incorrect conversions | High | Medium | Enforce UTC storage; add tests; document clearly | Dev | Open |
| R7 | Referee availability conflicts unresolved in schedule | Medium | Medium | Add validation pass with actionable errors; allow manual adjustments | Scheduling Dev | Open |
| R8 | Tiebreaker edge cases (multi-team cycles) mishandled | High | Medium | Write unit tests for complex scenarios early | QA | Open |
| R9 | Performance issues with incremental standings updates | Medium | Low | Optimize queries; selective recompute | Dev | Open |
| R10 | Overfitting initial architecture, slowing delivery | Medium | Medium | Focus on vertical slice milestone first | PM | Open |
