using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Notifications;
using TaskTracker.Core.Services;

namespace TaskTracker.Tests;

public class WorkOrderServiceFlowTests
{
    [Fact]
    public void AddLine_UnknownOrder_ThrowsOrderExists()
    {
        // Arrange
        WorkOrderService service = NewService(new FakeWorkOrderRepository());

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.AddLine(404, new WorkOrderLine("WRK-1", 1, 10m)));

        // Assert
        Assert.Equal("order.exists", ex.Rule);
    }

    [Fact]
    public void AddLine_PlannedOrder_ThrowsEditable()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        repository.Add(PlannedOrder(1));
        WorkOrderService service = NewService(repository);

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.AddLine(1, new WorkOrderLine("WRK-2", 1, 10m)));

        // Assert
        Assert.Equal("order.editable", ex.Rule);
    }

    [Fact]
    public void AddLine_DraftOrder_AddsLine()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        WorkOrder order = NewOrder(1);
        repository.Add(order);
        WorkOrderService service = NewService(repository);

        // Act
        service.AddLine(1, new WorkOrderLine("WRK-1", 1, 10m));

        // Assert
        Assert.Single(order.Lines);
    }

    [Fact]
    public void Pay_ExactAmount_StartsWork()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        WorkOrder order = PlannedOrder(1);
        repository.Add(order);
        WorkOrderService service = NewService(repository, new StubPricingPolicy(25m));

        // Act
        service.Pay(1, 25m);

        // Assert
        Assert.Equal(WorkOrderStatus.InProgress, order.Status);
    }

    [Fact]
    public void Pay_Overpayment_StillStartsWork()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        WorkOrder order = PlannedOrder(1);
        repository.Add(order);
        WorkOrderService service = NewService(repository, new StubPricingPolicy(25m));

        // Act
        service.Pay(1, 40m);

        // Assert
        Assert.Equal(WorkOrderStatus.InProgress, order.Status);
    }

    [Fact]
    public void Pay_DraftOrder_ThrowsPayable()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        repository.Add(NewOrder(1));
        WorkOrderService service = NewService(repository);

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.Pay(1, 100m));

        // Assert
        Assert.Equal("order.payable", ex.Rule);
    }

    [Fact]
    public void Cancel_DraftOrder_Cancels()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        WorkOrder order = NewOrder(1);
        repository.Add(order);
        WorkOrderService service = NewService(repository);

        // Act
        service.Cancel(1, "замовник відмовився");

        // Assert
        Assert.Equal(WorkOrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_InProgressOrder_ThrowsCancellableWithInner()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        WorkOrder order = PlannedOrder(1);
        order.Start();
        repository.Add(order);
        WorkOrderService service = NewService(repository);

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.Cancel(1, "пізно"));

        // Assert
        Assert.Equal("order.cancellable", ex.Rule);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    [Fact]
    public void Cancel_BlankReason_ThrowsArgument()
    {
        // Arrange
        WorkOrderService service = NewService(new FakeWorkOrderRepository());

        // Act and Assert
        Assert.Throws<ArgumentException>(() => service.Cancel(1, " "));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Constructor_NullDependency_ThrowsArgumentNull(int missing)
    {
        // Arrange
        IWorkOrderRepository? repository = missing == 0 ? null : new FakeWorkOrderRepository();
        IPricingPolicy? pricing = missing == 1 ? null : new StubPricingPolicy(1m);
        INotifier? notifier = missing == 2 ? null : new TextWriterNotifier(TextWriter.Null);
        ILogger<WorkOrderService>? log =
            missing == 3 ? null : NullLogger<WorkOrderService>.Instance;

        // Act and Assert
        Assert.Throws<ArgumentNullException>(
            () => new WorkOrderService(repository!, pricing!, notifier!, log!));
    }

    [Fact]
    public void Place_NullOrder_ThrowsArgumentNull()
    {
        // Arrange
        WorkOrderService service = NewService(new FakeWorkOrderRepository());

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => service.Place(null!));
    }

    [Fact]
    public void TotalOf_NullOrder_ThrowsArgumentNull()
    {
        // Arrange
        WorkOrderService service = NewService(new FakeWorkOrderRepository());

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => service.TotalOf(null!));
    }

    [Fact]
    public void ExportReport_ValidPath_WritesOrderAndTotal()
    {
        // Arrange
        WorkOrder order = PlannedOrder(7);
        WorkOrderService service = NewService(new FakeWorkOrderRepository());
        string path = Path.GetTempFileName();

        try
        {
            // Act
            service.ExportReport(path, order);
            string[] lines = File.ReadAllLines(path);

            // Assert
            Assert.Equal("Наряд 7", lines[0]);
            Assert.Equal(service.TotalOf(order).ToString(), lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ExportReport_MissingFolder_ThrowsDirectoryNotFound()
    {
        // Arrange
        WorkOrderService service = NewService(new FakeWorkOrderRepository());
        string folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string path = Path.Combine(folder, "report.txt");

        // Act and Assert
        Assert.Throws<DirectoryNotFoundException>(
            () => service.ExportReport(path, PlannedOrder(7)));
    }

    [Fact]
    public void ExportReport_BlankPath_ThrowsArgument()
    {
        // Arrange
        WorkOrderService service = NewService(new FakeWorkOrderRepository());

        // Act and Assert
        Assert.Throws<ArgumentException>(() => service.ExportReport(" ", PlannedOrder(7)));
    }

    private static WorkOrder NewOrder(int id) => new(id, 100, new DateOnly(2026, 11, 10));

    private static WorkOrder PlannedOrder(int id)
    {
        WorkOrder order = NewOrder(id);
        order.AddLine(new WorkOrderLine("WRK-1", 2, 250m));
        order.Plan();
        return order;
    }

    private static WorkOrderService NewService(
        IWorkOrderRepository repository,
        IPricingPolicy? pricing = null) =>
        new(
            repository,
            pricing ?? new StubPricingPolicy(1m),
            new TextWriterNotifier(TextWriter.Null),
            NullLogger<WorkOrderService>.Instance);
}
