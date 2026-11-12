using Microsoft.Extensions.Logging.Abstractions;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Notifications;
using TaskTracker.Core.Services;

namespace TaskTracker.Tests;

public class WorkOrderServiceTests
{
    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromPolicy()
    {
        // Arrange
        WorkOrderService service = NewService(
            new FakeWorkOrderRepository(),
            new StubPricingPolicy(25m));
        WorkOrder order = NewOrder(7);
        order.AddLine(new WorkOrderLine("WRK-1", 2, 10m));
        order.AddLine(new WorkOrderLine("WRK-2", 1, 30m));

        // Act
        decimal actual = service.TotalOf(order);

        // Assert
        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Place_Order_StoresAndAddsOnce()
    {
        // Arrange
        FakeWorkOrderRepository repository = new();
        WorkOrderService service = NewService(repository);
        WorkOrder order = NewOrder(1);

        // Act
        service.Place(order);

        // Assert
        Assert.NotNull(repository.GetById(1));
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public void Load_UnknownId_ThrowsDomainRule()
    {
        // Arrange
        WorkOrderService service = NewService(new FakeWorkOrderRepository());

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => service.Load(404));

        // Assert
        Assert.Equal("order.exists", ex.Rule);
    }

    private static WorkOrder NewOrder(int id) => new(id, 100, new DateOnly(2026, 11, 10));

    private static WorkOrderService NewService(
        IWorkOrderRepository repository,
        IPricingPolicy? pricing = null) =>
        new(
            repository,
            pricing ?? new StubPricingPolicy(1m),
            new TextWriterNotifier(TextWriter.Null),
            NullLogger<WorkOrderService>.Instance);
}
