using System.Text;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Reporting;
using TaskTracker.Core.Requests;

Console.OutputEncoding = Encoding.UTF8;
TotalRequest request = BuildSampleRequest();
TotalResult result = RequestTotalCalculator.Calculate(request);
Console.WriteLine(result.Total);
string report = RequestReport.TextOf(request, result);
new RequestReportPrinter(Console.Out).Print(report);

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
