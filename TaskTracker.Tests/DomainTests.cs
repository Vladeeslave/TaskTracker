using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

public class WorkOrderTests
{
    [Fact]
    public void Plan_WithoutLines_ThrowsInvalidOperation()
    {
        // Arrange
        WorkOrder order = NewOrder();

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => order.Plan());
    }

    [Fact]
    public void Start_AfterPlan_MovesToInProgress()
    {
        // Arrange
        WorkOrder order = PlannedOrder();

        // Act
        order.Start();

        // Assert
        Assert.Equal(WorkOrderStatus.InProgress, order.Status);
    }

    [Fact]
    public void Start_FromDraft_ThrowsInvalidOperation()
    {
        // Arrange
        WorkOrder order = NewOrder();

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => order.Start());
    }

    [Fact]
    public void Complete_AfterStart_MovesToDone()
    {
        // Arrange
        WorkOrder order = PlannedOrder();
        order.Start();

        // Act
        order.Complete();

        // Assert
        Assert.Equal(WorkOrderStatus.Done, order.Status);
    }

    [Fact]
    public void Complete_FromPlanned_ThrowsInvalidOperation()
    {
        // Arrange
        WorkOrder order = PlannedOrder();

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => order.Complete());
    }

    [Fact]
    public void Cancel_FromDraft_MovesToCancelled()
    {
        // Arrange
        WorkOrder order = NewOrder();

        // Act
        order.Cancel();

        // Assert
        Assert.Equal(WorkOrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_FromInProgress_ThrowsInvalidOperation()
    {
        // Arrange
        WorkOrder order = PlannedOrder();
        order.Start();

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void AddLine_Null_ThrowsArgumentNull()
    {
        // Arrange
        WorkOrder order = NewOrder();

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => order.AddLine(null!));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    public void Constructor_NonPositiveIds_ThrowsOutOfRange(int id, int customerId)
    {
        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WorkOrder(id, customerId, new DateOnly(2026, 11, 10)));
    }

    private static WorkOrder NewOrder() => new(1, 100, new DateOnly(2026, 11, 10));

    private static WorkOrder PlannedOrder()
    {
        WorkOrder order = NewOrder();
        order.AddLine(new WorkOrderLine("WRK-1", 2, 250m));
        order.Plan();
        return order;
    }
}

public class ArchivedWorkOrderTests
{
    [Fact]
    public void Constructor_DraftOrder_ThrowsInvalidOperation()
    {
        // Arrange
        WorkOrder order = new(1, 100, new DateOnly(2026, 11, 10));

        // Act and Assert
        Assert.Throws<InvalidOperationException>(
            () => new ArchivedWorkOrder(order, new DateOnly(2026, 11, 11)));
    }

    [Fact]
    public void Constructor_NullOrder_ThrowsArgumentNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(
            () => new ArchivedWorkOrder(null!, new DateOnly(2026, 11, 11)));
    }

    [Fact]
    public void Constructor_CancelledOrder_ExposesReadOnlyData()
    {
        // Arrange
        WorkOrder order = new(5, 100, new DateOnly(2026, 11, 10));
        order.AddLine(new WorkOrderLine("WRK-1", 2, 250m));
        order.Cancel();

        // Act
        ArchivedWorkOrder archive = new(order, new DateOnly(2026, 11, 11));

        // Assert
        Assert.Equal(5, archive.Id);
        Assert.Equal(WorkOrderStatus.Cancelled, archive.Status);
        Assert.Equal(new DateOnly(2026, 11, 11), archive.ArchivedAt);
        Assert.Single(archive.Lines);
    }
}

public class CustomerTests
{
    [Theory]
    [InlineData(0, "Іван", "a@b.c")]
    [InlineData(1, " ", "a@b.c")]
    [InlineData(1, "Іван", " ")]
    [InlineData(1, "Іван", "no-mail")]
    public void Constructor_InvalidData_Throws(int id, string fullName, string email)
    {
        // Act and Assert
        Assert.ThrowsAny<ArgumentException>(() => new Customer(id, fullName, email));
    }

    [Fact]
    public void Constructor_ValidData_KeepsValues()
    {
        // Act
        Customer customer = new(7, "Іван Петренко", "ivan@example.com") { IsRegular = true };

        // Assert
        Assert.Equal(7, customer.Id);
        Assert.Equal("Іван Петренко", customer.FullName);
        Assert.Equal("ivan@example.com", customer.Email);
        Assert.True(customer.IsRegular);
    }
}

public class WorkOrderLineTests
{
    [Fact]
    public void Constructor_NegativeRate_ThrowsOutOfRange()
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => new WorkOrderLine("WRK-1", 1, -0.01m));

        // Assert
        Assert.Equal("hourlyRate", ex.ParamName);
    }

    [Fact]
    public void Constructor_BlankCode_Throws()
    {
        // Act and Assert
        Assert.Throws<ArgumentException>(() => new WorkOrderLine(" ", 1, 10m));
    }

    [Fact]
    public void Amount_TwoHours_MultipliesHoursByRate()
    {
        // Arrange
        WorkOrderLine line = new("WRK-1", 2, 150m);

        // Act
        decimal actual = line.Amount;

        // Assert
        Assert.Equal(300m, actual);
    }
}
