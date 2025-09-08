using Turnament.Domain;
using Xunit;

namespace Turnament.Tests;

public class RoundRobinTests
{
    [Fact]
    public void Generates_Correct_Number_Of_Matches_For_Even_Teams()
    {
        var gen = new RoundRobinFixtureGenerator();
        var teams = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToList();
        var matches = gen.GenerateRoundRobin(Guid.NewGuid(), Guid.NewGuid(), teams);
        // For 4 teams single round-robin: n*(n-1)/2 = 6
        Assert.Equal(6, matches.Count);
    }

    [Fact]
    public void Skips_Byes_For_Odd_Teams()
    {
        var gen = new RoundRobinFixtureGenerator();
        var teams = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();
        var matches = gen.GenerateRoundRobin(Guid.NewGuid(), Guid.NewGuid(), teams);
        // For 5 teams -> treat as 6 with bye -> 5 rounds * 2-3 matches per round -> total n*(n-1)/2 = 10
        Assert.Equal(10, matches.Count);
    }
}
