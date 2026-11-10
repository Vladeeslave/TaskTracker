using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

public class WorkRequestTests
{
    [Fact]
    public void CalculateTotal_TwoLines_KeepsLab2Result()
    {
        WorkRequest request = new("T-1001", "Іваненко");
        request.AddLine("WRK-1", 3, 250m);
        request.AddLine("WRK-2", 12, 90m);

        Assert.Equal(2196.0m, request.CalculateTotal(false));
        Assert.Equal(1976.40m, request.CalculateTotal(true));
    }
}
