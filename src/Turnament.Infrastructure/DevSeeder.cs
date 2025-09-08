using Turnament.Domain;
using Microsoft.EntityFrameworkCore;

namespace Turnament.Infrastructure;

public static class DevSeeder
{
    public static async Task SeedAsync(TurnamentDbContext db)
    {
        if (await db.Tournaments.AnyAsync()) return;
        var rule = new RuleSet { SportId = Guid.NewGuid() };
        db.RuleSets.Add(rule);
        var tournament = new Tournament { Name = "Spring Invitational", SeasonId = Guid.NewGuid() };
        var division = new Division { Name = "U12 Boys", TournamentId = tournament.Id, RuleSetId = rule.Id };
        tournament.Divisions.Add(division);
        db.Tournaments.Add(tournament);
        // Teams
        var teamNames = new[] { "Tigers", "Eagles", "Sharks", "Wolves" };
        foreach (var name in teamNames) division.Teams.Add(new Team { Name = name });
        // Group
        var group = new Group { DivisionId = division.Id, Name = "Group A" };
        db.Groups.Add(group);
        await db.SaveChangesAsync();
        // Assign teams to group with standings rows
        foreach (var team in division.Teams)
        {
            db.GroupTeams.Add(new GroupTeam { GroupId = group.Id, TeamId = team.Id });
            db.GroupStandings.Add(new GroupStanding { GroupId = group.Id, TeamId = team.Id });
        }
        await db.SaveChangesAsync();
    }
}