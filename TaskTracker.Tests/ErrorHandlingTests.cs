using Microsoft.Extensions.Logging.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Notifications;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

namespace TaskTracker.Tests;

public class ErrorHandlingTests
{
    [Fact]
    public void Pay_UnknownOrder_ThrowsOrderExists()
    {
        WorkOrderService service = NewService();

        DomainRuleException error = Assert.Throws<DomainRuleException>(
            () => service.Pay(99, 100m));

        Assert.Equal("order.exists", error.Rule);
    }

    [Fact]
    public void Pay_Underpaid_ThrowsOrderAmountWithSum()
    {
        WorkOrderService service = NewService();
        WorkOrder order = new(1, 100, DateOnly.FromDateTime(DateTime.Today));
        order.AddLine(new WorkOrderLine("WRK-1", 2, 250m));
        order.Plan();
        service.Place(order);

        DomainRuleException error = Assert.Throws<DomainRuleException>(
            () => service.Pay(1, 400m));

        Assert.Equal("order.amount", error.Rule);
        Assert.Contains("500", error.Message);
    }

    private static WorkOrderService NewService() =>
        new(
            new InMemoryWorkOrderRepository(),
            new StandardPricingPolicy(),
            new TextWriterNotifier(TextWriter.Null),
            NullLogger<WorkOrderService>.Instance);
}
