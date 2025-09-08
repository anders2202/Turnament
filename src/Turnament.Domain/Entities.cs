namespace Turnament.Domain;

public enum MatchStatus { Pending, Scheduled, InProgress, Final, Cancelled }
public enum MatchEventType { Goal, YellowCard, RedCard, Substitution }

public record Score(int Home, int Away)
{
    public static Score Empty => new(0,0);
}

public class Tournament
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid SeasonId { get; set; }
    public List<Division> Divisions { get; set; } = new();
}

public class Division
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TournamentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AgeGroup { get; set; } = string.Empty;
    public Guid SportId { get; set; }
    public Guid RuleSetId { get; set; }
    public List<Group> Groups { get; set; } = new();
    public List<Team> Teams { get; set; } = new();
    public List<Match> Matches { get; set; } = new();
}

public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<Player> Players { get; set; } = new();
}

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TeamId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int? Number { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}

public class Group
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DivisionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<GroupStanding> Standings { get; set; } = new();
}

public class GroupStanding
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GroupId { get; set; }
    public Guid TeamId { get; set; }
    public int Played { get; set; }
    public int Won { get; set; }
    public int Drawn { get; set; }
    public int Lost { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference => GoalsFor - GoalsAgainst;
    public int Points { get; set; }
    public int FairPlayPoints { get; set; }
}

public class Match
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DivisionId { get; set; }
    public Guid? GroupId { get; set; }
    public Guid HomeTeamId { get; set; }
    public Guid AwayTeamId { get; set; }
    public int Round { get; set; }
    public DateTime? ScheduledStartUtc { get; set; }
    public Guid? PitchId { get; set; }
    public MatchStatus Status { get; set; } = MatchStatus.Pending;
    public Score Score { get; set; } = Score.Empty;
    public List<MatchEvent> Events { get; set; } = new();
}

public class MatchEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MatchId { get; set; }
    public MatchEventType Type { get; set; }
    public int Minute { get; set; }
    public Guid TeamId { get; set; }
    public Guid? PlayerId { get; set; }
    public string? Data { get; set; }
}

public class RuleSet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SportId { get; set; }
    public int PointsWin { get; set; } = 3;
    public int PointsDraw { get; set; } = 1;
    public int PointsLoss { get; set; } = 0;
    public int MatchDurationMinutes { get; set; } = 60;
    public int MinRestMinutes { get; set; } = 60;
    public int RefRestMinutes { get; set; } = 30;
    public bool AllowPenalties { get; set; } = true;
    public string TiebreakerOrder { get; set; } = "Points,GoalDifference,GoalsFor,HeadToHead,FairPlay,CoinToss";
}
