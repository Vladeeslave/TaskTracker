using TaskTracker.Core.Domain;
using TaskTracker.Core.Notifications;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

namespace TaskTracker.Tests;

public class DomainInvariantTests
{
    [Fact]
    public void WorkOrderLine_WithZeroHours_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WorkOrderLine("WRK-1", 0, 10m));
    }

    [Fact]
    public void WorkOrder_AfterPlan_RejectsNewLines()
    {
        WorkOrder order = NewOrder(1);
        order.AddLine(new WorkOrderLine("WRK-1", 1, 10m));
        order.Plan();

        Assert.Throws<InvalidOperationException>(
            () => order.AddLine(new WorkOrderLine("WRK-2", 1, 5m)));
    }

    [Fact]
    public void TotalOf_WithTenPercentDiscount_Returns180()
    {
        WorkOrderService service = new(
            new InMemoryWorkOrderRepository(),
            new DiscountPricingPolicy(0.10m),
            new TextWriterNotifier(TextWriter.Null));
        WorkOrder order = NewOrder(2);
        order.AddLine(new WorkOrderLine("WRK-1", 2, 100m));

        Assert.Equal(180m, service.TotalOf(order));
    }

    private static WorkOrder NewOrder(int id) =>
        new(id, 100, DateOnly.FromDateTime(DateTime.Today));
}
