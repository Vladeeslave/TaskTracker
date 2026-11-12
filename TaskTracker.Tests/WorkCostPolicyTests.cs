using TaskTracker.Core.Pricing;

namespace TaskTracker.Tests;

public class WorkCostPolicyTests
{
    [Theory]
    [InlineData(0.01, 1, false, 0.01)]
    [InlineData(1000, 1, false, 1000)]
    [InlineData(1000, 1, true, 1200)]
    [InlineData(1000, 7, false, 1000)]
    [InlineData(1000, 8, false, 900)]
    [InlineData(1000, 8, true, 1100)]
    [InlineData(333.33, 8, true, 366.66)]
    [InlineData(1000, 24, true, 1100)]
    [InlineData(1000, 25, true, 900)]
    public void HourCostFor_Boundaries_MatchesTable(
        double hourlyRate, int hours, bool isUrgent, double expected)
    {
        // Act
        decimal actual = WorkCostPolicy.HourCostFor((decimal)hourlyRate, hours, isUrgent);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void HourCostFor_InvalidRate_ThrowsOutOfRange(double hourlyRate)
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkCostPolicy.HourCostFor((decimal)hourlyRate, 3, false));

        // Assert
        Assert.Equal("hourlyRate", ex.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void HourCostFor_InvalidHours_ThrowsOutOfRange(int hours)
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkCostPolicy.HourCostFor(1000m, hours, false));

        // Assert
        Assert.Equal("hours", ex.ParamName);
    }

    [Fact]
    public void HourCostFor_UrgentOverMaxHours_SurchargeNotApplied()
    {
        // Arrange
        int hours = 25;

        // Act
        decimal actual = WorkCostPolicy.HourCostFor(1000m, hours, true);

        // Assert
        Assert.Equal(900m, actual);
    }
}
