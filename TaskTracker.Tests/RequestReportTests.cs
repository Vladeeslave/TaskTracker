using TaskTracker.Core.Reporting;
using TaskTracker.Core.Requests;

namespace TaskTracker.Tests;

public class RequestReportTests
{
    [Fact]
    public void TextOf_ContainsTotalLine()
    {
        TotalRequest request = SampleRequests.RegularRequester();
        TotalResult result = RequestTotalCalculator.Calculate(request);

        string text = RequestReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text);
    }

    [Fact]
    public void Print_WritesReportToOutput()
    {
        StringWriter writer = new();
        RequestReportPrinter printer = new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString());
    }
}
