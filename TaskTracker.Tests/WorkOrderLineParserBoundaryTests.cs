using TaskTracker.App.Input;
using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

public class WorkOrderLineParserBoundaryTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("WRK-1;2")]
    [InlineData("WRK-1;2;250,50;extra")]
    [InlineData(";2;250,50")]
    [InlineData("WRK-123456789012345678901;2;250,50")]
    [InlineData("WRK-1;0;250,50")]
    [InlineData("WRK-1;-2;250,50")]
    [InlineData("WRK-1;abc;250,50")]
    [InlineData("WRK-1;1001;250,50")]
    [InlineData("WRK-1;2;-250,50")]
    [InlineData("WRK-1;2;250.50")]
    [InlineData("WRK-1;2;abc")]
    [InlineData("WRK-1;2;100000,01")]
    public void TryParse_InvalidInput_ReturnsFalse(string? raw)
    {
        // Act
        bool ok = WorkOrderLineParser.TryParse(raw, out WorkOrderLine? line);

        // Assert
        Assert.False(ok);
        Assert.Null(line);
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsNormalizedLine()
    {
        // Act
        bool ok = WorkOrderLineParser.TryParse(" wrk-1;3;250,50", out WorkOrderLine? line);

        // Assert
        Assert.True(ok);
        Assert.NotNull(line);
        Assert.Equal("WRK-1", line.WorkCode);
        Assert.Equal(3, line.Hours);
        Assert.Equal(250.50m, line.HourlyRate);
    }
}
