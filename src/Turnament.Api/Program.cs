using Microsoft.EntityFrameworkCore;
using Turnament.Domain;
using Turnament.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TurnamentDbContext>(opt =>
    opt.UseInMemoryDatabase("turnament-dev")); // Replace with Npgsql in real env

builder.Services.AddScoped<IFixtureGenerator, RoundRobinFixtureGenerator>();
builder.Services.AddScoped<IStandingsCalculator, StandingsCalculator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/api/tournaments", async (TurnamentDbContext db, Tournament t) =>
{
    db.Tournaments.Add(t);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tournaments/{t.Id}", t);
});

app.MapGet("/api/tournaments", async (TurnamentDbContext db) => await db.Tournaments.ToListAsync());
app.MapGet("/api/tournaments/{id:guid}", async (TurnamentDbContext db, Guid id) => await db.Tournaments.FindAsync(id) is { } t ? Results.Ok(t) : Results.NotFound());

app.MapPost("/api/divisions/{tournamentId:guid}", async (TurnamentDbContext db, Guid tournamentId, Division d) =>
{
    d.TournamentId = tournamentId;
    db.Divisions.Add(d);
    await db.SaveChangesAsync();
    return Results.Created($"/api/divisions/{d.Id}", d);
});

app.MapPost("/api/divisions/{divisionId:guid}/teams", async (TurnamentDbContext db, Guid divisionId, Team team) =>
{
    var division = await db.Divisions.Include(x=>x.Teams).FirstOrDefaultAsync(d=>d.Id==divisionId);
    if (division == null) return Results.NotFound();
    division.Teams.Add(team);
    await db.SaveChangesAsync();
    return Results.Created($"/api/teams/{team.Id}", team);
});

app.MapPost("/api/divisions/{divisionId:guid}/groups", async (TurnamentDbContext db, Guid divisionId, Group g) =>
{
    g.DivisionId = divisionId;
    db.Groups.Add(g);
    await db.SaveChangesAsync();
    return Results.Created($"/api/groups/{g.Id}", g);
});

app.MapPost("/api/groups/{groupId:guid}/add-team/{teamId:guid}", async (TurnamentDbContext db, Guid groupId, Guid teamId) =>
{
    var group = await db.Groups.FindAsync(groupId);
    var team = await db.Teams.FindAsync(teamId);
    if (group == null || team == null) return Results.NotFound();
    bool exists = await db.GroupTeams.AnyAsync(gt => gt.GroupId == groupId && gt.TeamId == teamId);
    if (!exists)
    {
        db.GroupTeams.Add(new GroupTeam { GroupId = groupId, TeamId = teamId });
        // Also create a standing row if absent
        if (!await db.GroupStandings.AnyAsync(s => s.GroupId == groupId && s.TeamId == teamId))
        {
            db.GroupStandings.Add(new GroupStanding { GroupId = groupId, TeamId = teamId });
        }
        await db.SaveChangesAsync();
    }
    return Results.Ok();
});

app.MapPost("/api/groups/{groupId:guid}/standings/init", async (TurnamentDbContext db, Guid groupId) =>
{
    var group = await db.Groups.FindAsync(groupId);
    if (group == null) return Results.NotFound();
    // In real impl we derive teams from registrations. Placeholder.
    return Results.Ok();
});

app.MapPost("/api/divisions/{divisionId:guid}/fixtures/generate", async (TurnamentDbContext db, Guid divisionId, IFixtureGenerator gen) =>
{
    var division = await db.Divisions.Include(d=>d.Teams).Include(d=>d.Groups).FirstOrDefaultAsync(d=>d.Id==divisionId);
    if (division == null) return Results.NotFound();
    foreach (var group in division.Groups)
    {
        // Placeholder: currently no team->group map. Assign all teams into one group if none.
        var teamIds = division.Teams.Select(t=>t.Id).ToList();
        var matches = gen.GenerateRoundRobin(division.Id, group.Id, teamIds);
        foreach (var m in matches) db.Matches.Add(m);
    }
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.MapPut("/api/matches/{matchId:guid}/result", async (TurnamentDbContext db, Guid matchId, StandingsCalculator calc, int homeGoals, int awayGoals) =>
{
    var match = await db.Matches.FindAsync(matchId);
    if (match == null) return Results.NotFound();
    match.Score = new Score(homeGoals, awayGoals);
    match.Status = MatchStatus.Final;
    // Apply standings update if group match
    if (match.GroupId.HasValue)
    {
        var ruleSet = await db.RuleSets.FirstOrDefaultAsync(); // simplistic; later use division rules
        var homeStanding = await db.GroupStandings.FirstOrDefaultAsync(s => s.TeamId == match.HomeTeamId && s.GroupId == match.GroupId);
        var awayStanding = await db.GroupStandings.FirstOrDefaultAsync(s => s.TeamId == match.AwayTeamId && s.GroupId == match.GroupId);
        if (homeStanding != null && awayStanding != null && ruleSet != null)
        {
            calc.ApplyResult(match, homeStanding, awayStanding, ruleSet);
        }
    }
    await db.SaveChangesAsync();
    return Results.Ok(match);
});

app.MapGet("/api/groups/{groupId:guid}/standings", async (TurnamentDbContext db, Guid groupId) =>
{
    var standings = await db.GroupStandings.Where(s => s.GroupId == groupId)
        .OrderByDescending(s => s.Points)
        .ThenByDescending(s => s.GoalDifference)
        .ThenByDescending(s => s.GoalsFor)
        .ToListAsync();
    return Results.Ok(standings);
});

app.Run();
