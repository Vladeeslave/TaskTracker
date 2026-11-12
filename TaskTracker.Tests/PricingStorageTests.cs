using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Notifications;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Reporting;
using TaskTracker.Core.Storage;

namespace TaskTracker.Tests;

public class PricingPolicyTests
{
    [Fact]
    public void StandardPolicy_PriceOf_ReturnsAmountWithoutDiscount()
    {
        // Arrange
        StandardPricingPolicy policy = new();

        // Act
        decimal actual = policy.PriceOf(new WorkOrderLine("WRK-1", 2, 150m));

        // Assert
        Assert.Equal(300m, actual);
    }

    [Fact]
    public void StandardPolicy_NullLine_ThrowsArgumentNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new StandardPricingPolicy().PriceOf(null!));
    }

    [Fact]
    public void DiscountPolicy_PriceOf_AppliesRate()
    {
        // Arrange
        DiscountPricingPolicy policy = new(0.05m);

        // Act
        decimal actual = policy.PriceOf(new WorkOrderLine("WRK-1", 2, 250m));

        // Assert
        Assert.Equal(475m, actual);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(0.51)]
    public void DiscountPolicy_RateOutOfRange_ThrowsOutOfRange(double rate)
    {
        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiscountPricingPolicy((decimal)rate));
    }

    [Fact]
    public void DiscountPolicy_NullLine_ThrowsArgumentNull()
    {
        // Arrange
        DiscountPricingPolicy policy = new(0.05m);

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => policy.PriceOf(null!));
    }

    [Fact]
    public void VipPolicy_PriceOf_AppliesTenPercent()
    {
        // Arrange
        VipPricingPolicy policy = new();

        // Act
        decimal actual = policy.PriceOf(new WorkOrderLine("WRK-1", 2, 250m));

        // Assert
        Assert.Equal(450m, actual);
    }

    [Fact]
    public void VipPolicy_NullLine_ThrowsArgumentNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new VipPricingPolicy().PriceOf(null!));
    }
}

public class InMemoryRepositoryTests
{
    [Fact]
    public void Add_ThenGetById_ReturnsSameOrder()
    {
        // Arrange
        InMemoryWorkOrderRepository repository = new();
        WorkOrder order = new(1, 100, new DateOnly(2026, 11, 10));

        // Act
        repository.Add(order);

        // Assert
        Assert.Same(order, repository.GetById(1));
    }

    [Fact]
    public void Add_DuplicateId_ThrowsInvalidOperation()
    {
        // Arrange
        InMemoryWorkOrderRepository repository = new();
        repository.Add(new WorkOrder(1, 100, new DateOnly(2026, 11, 10)));

        // Act and Assert
        Assert.Throws<InvalidOperationException>(
            () => repository.Add(new WorkOrder(1, 200, new DateOnly(2026, 11, 11))));
    }

    [Fact]
    public void Add_NullOrder_ThrowsArgumentNull()
    {
        // Arrange
        InMemoryWorkOrderRepository repository = new();

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => repository.Add(null!));
    }

    [Fact]
    public void GetById_UnknownId_ReturnsNull()
    {
        // Arrange
        InMemoryWorkOrderRepository repository = new();

        // Act
        WorkOrder? actual = repository.GetById(404);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public void GetAll_TwoOrders_ReturnsBoth()
    {
        // Arrange
        InMemoryWorkOrderRepository repository = new();
        repository.Add(new WorkOrder(1, 100, new DateOnly(2026, 11, 10)));
        repository.Add(new WorkOrder(2, 100, new DateOnly(2026, 11, 10)));

        // Act
        IReadOnlyList<WorkOrder> all = repository.GetAll();

        // Assert
        Assert.Equal(2, all.Count);
    }
}

public class CsvReportTests
{
    [Fact]
    public void BuildCsv_PlannedOrder_ReturnsHeaderAndRow()
    {
        // Arrange
        WorkOrder order = new(1, 100, new DateOnly(2026, 11, 10));
        order.AddLine(new WorkOrderLine("WRK-1", 2, 250m));
        order.Plan();

        // Act
        string csv = WorkOrderCsvReport.BuildCsv(new[] { order });
        string[] rows = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Equal("id;status;hours;amount", rows[0]);
        Assert.Equal("1;Planned;2;500.00", rows[1]);
    }

    [Fact]
    public void BuildCsv_NoOrders_ReturnsHeaderOnly()
    {
        // Act
        string csv = WorkOrderCsvReport.BuildCsv(Array.Empty<WorkOrder>());
        string[] rows = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.Single(rows);
    }

    [Fact]
    public void BuildCsv_Null_ThrowsArgumentNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => WorkOrderCsvReport.BuildCsv(null!));
    }
}

public class ErrorTypesTests
{
    [Fact]
    public void ResultOk_HoldsValueWithoutError()
    {
        // Act
        Result<int> result = Result.Ok(5);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
        Assert.Equal(string.Empty, result.Error);
    }

    [Fact]
    public void ResultFail_HoldsErrorWithoutValue()
    {
        // Act
        Result<int> result = Result.Fail<int>("помилка вводу");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("помилка вводу", result.Error);
    }

    [Fact]
    public void ResultFail_BlankError_ThrowsArgument()
    {
        // Act and Assert
        Assert.Throws<ArgumentException>(() => Result.Fail<int>(" "));
    }

    [Fact]
    public void DomainRuleException_WithInner_KeepsRuleAndInner()
    {
        // Arrange
        InvalidOperationException inner = new("причина");

        // Act
        DomainRuleException ex = new("order.test", "повідомлення", inner);

        // Assert
        Assert.Equal("order.test", ex.Rule);
        Assert.Same(inner, ex.InnerException);
    }

    [Fact]
    public void DomainRuleException_BlankRule_ThrowsArgument()
    {
        // Act and Assert
        Assert.Throws<ArgumentException>(() => new DomainRuleException(" ", "повідомлення"));
    }
}

public class NotifierAndAuditTests
{
    [Fact]
    public void Notify_Message_WritesLineToWriter()
    {
        // Arrange
        StringWriter writer = new();
        TextWriterNotifier notifier = new(writer);

        // Act
        notifier.Notify("тест");

        // Assert
        Assert.Contains("Сповіщення: тест", writer.ToString());
    }

    [Fact]
    public void Notify_BlankMessage_ThrowsArgument()
    {
        // Arrange
        TextWriterNotifier notifier = new(TextWriter.Null);

        // Act and Assert
        Assert.Throws<ArgumentException>(() => notifier.Notify(" "));
    }

    [Fact]
    public void Constructor_NullWriter_ThrowsArgumentNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new TextWriterNotifier(null!));
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        // Arrange
        string path = Path.GetTempFileName();
        FileAuditLog audit = new(path);

        try
        {
            // Act
            audit.Dispose();
            Exception? error = Record.Exception(() => audit.Dispose());

            // Assert
            Assert.Null(error);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Write_AfterDispose_ThrowsObjectDisposed()
    {
        // Arrange
        string path = Path.GetTempFileName();
        FileAuditLog audit = new(path);
        audit.Dispose();

        try
        {
            // Act and Assert
            Assert.Throws<ObjectDisposedException>(() => audit.Write("order.paid", 1));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
