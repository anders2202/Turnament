using Microsoft.EntityFrameworkCore;
using Turnament.Domain;

namespace Turnament.Infrastructure;

public class TurnamentDbContext : DbContext
{
    public TurnamentDbContext(DbContextOptions<TurnamentDbContext> options) : base(options) {}

    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Division> Divisions => Set<Division>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupStanding> GroupStandings => Set<GroupStanding>();
    public DbSet<GroupTeam> GroupTeams => Set<GroupTeam>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchEvent> MatchEvents => Set<MatchEvent>();
    public DbSet<RuleSet> RuleSets => Set<RuleSet>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Tournament>().HasMany(t => t.Divisions).WithOne().HasForeignKey(d => d.TournamentId);
        modelBuilder.Entity<Division>().HasMany(d => d.Groups).WithOne().HasForeignKey(g => g.DivisionId);
        modelBuilder.Entity<Division>().HasMany(d => d.Teams);
        modelBuilder.Entity<Division>().HasMany(d => d.Matches).WithOne().HasForeignKey(m => m.DivisionId);
        modelBuilder.Entity<Group>().HasMany(g => g.Standings).WithOne().HasForeignKey(s => s.GroupId);
    modelBuilder.Entity<GroupTeam>().HasIndex(gt => new { gt.GroupId, gt.TeamId }).IsUnique();
        modelBuilder.Entity<Match>().OwnsOne(m => m.Score);
    }
}
