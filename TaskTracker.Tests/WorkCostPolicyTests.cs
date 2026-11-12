using TaskTracker.Core.Pricing;

namespace TaskTracker.Tests;

public class WorkCostPolicyTests
{
    [Fact]
    public void HourCostFor_NegativeRate_ThrowsOutOfRange()
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkCostPolicy.HourCostFor(-0.01m, 3, false));

        // Assert
        Assert.Equal("hourlyRate", ex.ParamName);
    }

    [Fact]
    public void HourCostFor_ZeroHours_ThrowsOutOfRange()
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => WorkCostPolicy.HourCostFor(1000m, 0, false));

        // Assert
        Assert.Equal("hours", ex.ParamName);
    }

    [Fact]
    public void HourCostFor_MinimalRate_ReturnsOneKopeck()
    {
        // Act
        decimal actual = WorkCostPolicy.HourCostFor(0.01m, 1, false);

        // Assert
        Assert.Equal(0.01m, actual);
    }

    [Fact]
    public void HourCostFor_OneHourRegular_ReturnsRate()
    {
        // Act
        decimal actual = WorkCostPolicy.HourCostFor(1000m, 1, false);

        // Assert
        Assert.Equal(1000m, actual);
    }

    [Fact]
    public void HourCostFor_OneHourUrgent_AddsSurcharge()
    {
        // Act
        decimal actual = WorkCostPolicy.HourCostFor(1000m, 1, true);

        // Assert
        Assert.Equal(1200m, actual);
    }

    [Fact]
    public void HourCostFor_SevenHours_NoDiscount()
    {
        // Arrange
        int hours = WorkCostPolicy.BulkFrom - 1;

        // Act
        decimal actual = WorkCostPolicy.HourCostFor(1000m, hours, false);

        // Assert
        Assert.Equal(1000m, actual);
    }

    [Fact]
    public void HourCostFor_EightHours_AppliesDiscount()
    {
        // Arrange
        int hours = WorkCostPolicy.BulkFrom;

        // Act
        decimal actual = WorkCostPolicy.HourCostFor(1000m, hours, false);

        // Assert
        Assert.Equal(900m, actual);
    }

    [Fact]
    public void HourCostFor_EightHoursUrgent_CombinesBothRules()
    {
        // Arrange
        int hours = WorkCostPolicy.BulkFrom;

        // Act
        decimal actual = WorkCostPolicy.HourCostFor(1000m, hours, true);

        // Assert
        Assert.Equal(1100m, actual);
    }

    [Fact]
    public void HourCostFor_NonRoundProduct_RoundsToKopecks()
    {
        // Arrange
        int hours = WorkCostPolicy.BulkFrom;

        // Act
        decimal actual = WorkCostPolicy.HourCostFor(333.33m, hours, true);

        // Assert
        Assert.Equal(366.66m, actual);
    }
}
