using System.Collections.Immutable;

namespace Turnament.Domain;

public interface IFixtureGenerator
{
    IReadOnlyList<Match> GenerateRoundRobin(Guid divisionId, Guid? groupId, IReadOnlyList<Guid> teamIds);
}

public class RoundRobinFixtureGenerator : IFixtureGenerator
{
    public IReadOnlyList<Match> GenerateRoundRobin(Guid divisionId, Guid? groupId, IReadOnlyList<Guid> teamIds)
    {
        var teams = teamIds.ToList();
        if (teams.Count < 2) return Array.Empty<Match>();
        bool addBye = teams.Count % 2 == 1;
        Guid? bye = null;
        if (addBye)
        {
            bye = Guid.Empty; // sentinel
            teams.Add(bye.Value);
        }
        int n = teams.Count;
        int rounds = n - 1;
        int half = n / 2;
        var schedule = new List<Match>();
        for (int round = 0; round < rounds; round++)
        {
            for (int i = 0; i < half; i++)
            {
                var home = teams[i];
                var away = teams[n - 1 - i];
                if (home == bye || away == bye) continue; // skip byes
                // Alternate home/away per round for fairness
                if (round % 2 == 1)
                    (home, away) = (away, home);
                schedule.Add(new Match
                {
                    DivisionId = divisionId,
                    GroupId = groupId,
                    HomeTeamId = home,
                    AwayTeamId = away,
                    Round = round + 1
                });
            }
            // rotate
            var fixedTeam = teams[0];
            var rotating = teams.Skip(1).ToList();
            var last = rotating[^1];
            rotating.RemoveAt(rotating.Count - 1);
            rotating.Insert(0, last);
            teams = new List<Guid> { fixedTeam };
            teams.AddRange(rotating);
        }
        return schedule;
    }
}

public interface IStandingsCalculator
{
    void ApplyResult(Match match, GroupStanding homeStanding, GroupStanding awayStanding, RuleSet ruleSet);
}

public class StandingsCalculator : IStandingsCalculator
{
    public void ApplyResult(Match match, GroupStanding home, GroupStanding away, RuleSet rules)
    {
        if (match.Status != MatchStatus.Final) return;
        home.Played++; away.Played++;
        home.GoalsFor += match.Score.Home; home.GoalsAgainst += match.Score.Away;
        away.GoalsFor += match.Score.Away; away.GoalsAgainst += match.Score.Home;

        if (match.Score.Home > match.Score.Away)
        {
            home.Won++; away.Lost++; home.Points += rules.PointsWin; away.Points += rules.PointsLoss;
        }
        else if (match.Score.Home < match.Score.Away)
        {
            away.Won++; home.Lost++; away.Points += rules.PointsWin; home.Points += rules.PointsLoss;
        }
        else
        {
            home.Drawn++; away.Drawn++; home.Points += rules.PointsDraw; away.Points += rules.PointsDraw;
        }
    }
}
