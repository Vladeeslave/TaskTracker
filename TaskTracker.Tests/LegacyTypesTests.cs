using TaskTracker.Core.Domain;
using TaskTracker.Core.Reporting;
using TaskTracker.Core.Requests;

namespace TaskTracker.Tests;

public class WorkRequestRulesTests
{
    [Fact]
    public void CalculateTotal_RegularOverThreshold_AppliesRegularDiscount()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");
        request.AddLine("WRK-1", 1, 2000m);

        // Act
        decimal actual = request.CalculateTotal(true);

        // Assert
        Assert.Equal(2160m, actual);
    }

    [Fact]
    public void CalculateTotal_LargeOrderNotRegular_AppliesLargeOrderDiscount()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");
        request.AddLine("WRK-1", 1, 6000m);

        // Act
        decimal actual = request.CalculateTotal(false);

        // Assert
        Assert.Equal(6120m, actual);
    }

    [Fact]
    public void CalculateTotal_MoreThanTenLines_SubtractsBulkDiscount()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");
        for (int i = 0; i < 11; i++)
        {
            request.AddLine("WRK-1", 1, 100m);
        }

        // Act
        decimal actual = request.CalculateTotal(false);

        // Assert
        Assert.Equal(1200m, actual);
    }

    [Fact]
    public void CalculateTotal_BulkDiscountExceedsTotal_ClampsToZero()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");
        for (int i = 0; i < 11; i++)
        {
            request.AddLine("WRK-1", 1, 5m);
        }

        // Act
        decimal actual = request.CalculateTotal(false);

        // Assert
        Assert.Equal(0m, actual);
    }

    [Fact]
    public void TryChangeStatus_NewToApproved_ReturnsTrue()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");

        // Act
        bool changed = request.TryChangeStatus(WorkRequest.StatusApproved);

        // Assert
        Assert.True(changed);
        Assert.Equal(WorkRequest.StatusApproved, request.Status);
    }

    [Fact]
    public void TryChangeStatus_ApprovedToDone_ReturnsTrue()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");
        request.TryChangeStatus(WorkRequest.StatusApproved);

        // Act
        bool changed = request.TryChangeStatus(WorkRequest.StatusDone);

        // Assert
        Assert.True(changed);
        Assert.Equal(WorkRequest.StatusDone, request.Status);
    }

    [Fact]
    public void TryChangeStatus_NewToCancelled_ReturnsTrue()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");

        // Act
        bool changed = request.TryChangeStatus(WorkRequest.StatusCancelled);

        // Assert
        Assert.True(changed);
    }

    [Fact]
    public void TryChangeStatus_NewToDone_ReturnsFalse()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");

        // Act
        bool changed = request.TryChangeStatus(WorkRequest.StatusDone);

        // Assert
        Assert.False(changed);
        Assert.Equal(WorkRequest.StatusNew, request.Status);
    }

    [Fact]
    public void IsValid_WithLineAndName_ReturnsTrue()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");
        request.AddLine("WRK-1", 1, 100m);

        // Act and Assert
        Assert.True(request.IsValid());
    }

    [Fact]
    public void IsValid_WithoutLines_ReturnsFalse()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");

        // Act and Assert
        Assert.False(request.IsValid());
    }

    [Fact]
    public void IsValid_ShortName_ReturnsFalse()
    {
        // Arrange
        WorkRequest request = new("T-1", "Ів");
        request.AddLine("WRK-1", 1, 100m);

        // Act and Assert
        Assert.False(request.IsValid());
    }

    [Fact]
    public void FindById_ExistingAndMissing_ReturnsRequestOrNull()
    {
        // Arrange
        WorkRequest first = new("T-1", "Іваненко");
        List<WorkRequest> requests = new() { first, new WorkRequest("T-2", "Петренко") };

        // Act
        WorkRequest? found = WorkRequest.FindById(requests, "T-1");
        WorkRequest? missing = WorkRequest.FindById(requests, "T-9");

        // Assert
        Assert.Same(first, found);
        Assert.Null(missing);
    }

    [Fact]
    public void BuildReport_OneLine_ContainsLineAndTotal()
    {
        // Arrange
        WorkRequest request = new("T-1", "Іваненко");
        request.AddLine("WRK-1", 2, 100m);

        // Act
        string report = request.BuildReport();

        // Assert
        Assert.Contains("Робота: WRK-1", report);
        Assert.Contains("Разом:", report);
    }
}

public class Lab1TypesTests
{
    [Fact]
    public void CalculateTotalHours_TwoEntries_SumsHours()
    {
        // Arrange
        WorkTask task = new() { Id = 1, Title = "Звіт", ProjectCode = "TT" };
        task.AddEntry(new TimeEntry { MemberId = 1, Description = "аналіз", Hours = 2.5m });
        task.AddEntry(new TimeEntry { MemberId = 2, Description = "тести", Hours = 1.5m });

        // Act
        decimal actual = task.CalculateTotalHours();

        // Assert
        Assert.Equal(4m, actual);
        Assert.Equal(2, task.Entries.Count);
    }

    [Fact]
    public void Status_NewTask_IsNewAndCanChange()
    {
        // Arrange
        WorkTask task = new();

        // Act
        task.Status = WorkTaskStatus.Done;

        // Assert
        Assert.Equal(WorkTaskStatus.Done, task.Status);
    }

    [Fact]
    public void Initializers_ProjectAndMember_KeepValues()
    {
        // Act
        Project project = new()
        {
            Code = "TT",
            Title = "Трекер",
            PlannedHours = 120m,
            IsActive = true,
        };
        TeamMember member = new() { Id = 3, Name = "Олена", IsLead = true };

        // Assert
        Assert.Equal("TT", project.Code);
        Assert.Equal(120m, project.PlannedHours);
        Assert.True(project.IsActive);
        Assert.Equal("Олена", member.Name);
        Assert.True(member.IsLead);
    }
}

public class RequestReportTests
{
    [Fact]
    public void TextOf_SampleRequest_ContainsTotalAndStatus()
    {
        // Arrange
        TotalRequest request = SampleRequest();
        TotalResult result = RequestTotalCalculator.Calculate(request);

        // Act
        string text = RequestReport.TextOf(request, result);

        // Assert
        Assert.Contains("Разом: 1292.70", text);
        Assert.Contains("Стан: Approved", text);
    }

    [Fact]
    public void Print_Text_WritesToWriter()
    {
        // Arrange
        StringWriter writer = new();
        RequestReportPrinter printer = new(writer);

        // Act
        printer.Print("Разом: 1292.70");

        // Assert
        Assert.Contains("Разом: 1292.70", writer.ToString());
    }

    private static TotalRequest SampleRequest()
    {
        List<WorkLine> lines = new()
        {
            new WorkLine { WorkCode = "WRK-1", Hours = 12, HourlyRate = 100m },
            new WorkLine { WorkCode = "WRK-2", Hours = 1, HourlyRate = 250m },
        };

        return new TotalRequest(
            RequestId: 1,
            Requester: new RequesterInfo("Іваненко", "i@ex.com", true),
            Lines: lines,
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: RequestStatus.New,
            DeliveryPrice: 60m);
    }
}
