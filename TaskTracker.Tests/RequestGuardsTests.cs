using TaskTracker.Core.Domain;
using TaskTracker.Core.Requests;

namespace TaskTracker.Tests;

public class RequestGuardsTests
{
    [Fact]
    public void EnsureLinesValid_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            RequestGuards.EnsureLinesValid(new List<WorkLine>()));
    }

    [Fact]
    public void EnsureLineValid_ZeroHours_ThrowsRange()
    {
        WorkLine line = new() { WorkCode = "WRK-1", Hours = 0, HourlyRate = 10m };
        Assert.Throws<ArgumentOutOfRangeException>(
            () => RequestGuards.EnsureLineValid(line));
    }
}
