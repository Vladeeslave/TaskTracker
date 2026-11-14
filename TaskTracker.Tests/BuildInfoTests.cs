using TaskTracker.Core;
using TaskTracker.Core.Reporting;

namespace TaskTracker.Tests;

public class BuildInfoTests
{
    [Fact]
    public void Current_ReturnsSemanticVersionWithoutHash()
    {
        // Act
        string version = VersionInfo.Current();

        // Assert
        Assert.Matches(@"^\d+\.\d+\.\d+$", version);
        Assert.DoesNotContain("+", version, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_TwoRows_ReturnsUpperCaseTitleAndRows()
    {
        // Arrange
        SummaryReportBuilder builder = new() { Title = "Звіт" };
        string[] rows = ["Рядок 1", "Рядок 2"];

        // Act
        string text = builder.Build(rows);

        // Assert
        string expected = "ЗВІТ" + Environment.NewLine
            + "Рядок 1" + Environment.NewLine
            + "Рядок 2" + Environment.NewLine;
        Assert.Equal(expected, text);
    }

    [Fact]
    public void Build_NullRows_ThrowsArgumentNull()
    {
        // Arrange
        SummaryReportBuilder builder = new();

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => builder.Build(null!));
    }
}
