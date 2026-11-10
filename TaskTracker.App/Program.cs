using System.Text;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Reporting;
using TaskTracker.Core.Requests;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;
TotalRequest request = BuildSampleRequest();
TotalResult result = RequestTotalCalculator.Calculate(request);
Console.WriteLine(result.Total);
string report = RequestReport.TextOf(request, result);
new RequestReportPrinter(Console.Out).Print(report);

// ЛР 4. Композиційний корінь: єдине місце, де створюються конкретні реалізації
Console.WriteLine();
var repository = new InMemoryWorkOrderRepository();
IPricingPolicy pricing = new DiscountPricingPolicy(0.05m);
var service = new WorkOrderService(repository, pricing);

var customer = new Customer(100, "Іваненко Петро", "petro@example.com") { IsRegular = true };
var order = new WorkOrder(1, customer.Id, DateOnly.FromDateTime(DateTime.Today));
order.AddLine(new WorkOrderLine("WRK-1", 2, 250m));
service.Place(order);

Console.WriteLine($"Сума: {service.TotalOf(order):0.00}");

static TotalRequest BuildSampleRequest()
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
