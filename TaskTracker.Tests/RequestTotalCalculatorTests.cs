using TaskTracker.Core.Domain;
using TaskTracker.Core.Requests;

namespace TaskTracker.Tests;

public class RequestTotalCalculatorTests
{
    [Fact]
    public void Calculate_RegularRequester_Returns1292()
    {
        TotalRequest request = SampleRequests.RegularRequester();

        TotalResult result = RequestTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(RequestStatus.Approved, result.Status);
    }

    [Fact]
    public void Calculate_SmallRequest_AddsDelivery()
    {
        TotalRequest request = SampleRequests.SmallWithCoupon();

        TotalResult result = RequestTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(RequestStatus.Done, result.Status);
    }

    [Fact]
    public void Calculate_FreeWork_ReturnsOnlyDelivery()
    {
        TotalRequest request = SampleRequests.FreeWork();

        TotalResult result = RequestTotalCalculator.Calculate(request);

        Assert.Equal(60m, result.Total);
    }

    [Fact]
    public void SubtotalOf_BulkLine_AppliesBulkRate()
    {
        List<WorkLine> lines = new()
        {
            new WorkLine { WorkCode = "WRK-1", Hours = 12, HourlyRate = 100m },
            new WorkLine { WorkCode = "WRK-2", Hours = 1, HourlyRate = 250m },
        };

        Assert.Equal(1390m, RequestTotalCalculator.SubtotalOf(lines));
    }

    [Fact]
    public void VolumeDiscountOf_LargeAmount_ReturnsTenPercent()
    {
        Assert.Equal(600m, RequestTotalCalculator.VolumeDiscountOf(6000m));
    }

    [Fact]
    public void CouponDiscountOf_FixedCouponBelowThreshold_ReturnsZero()
    {
        Assert.Equal(0m, RequestTotalCalculator.CouponDiscountOf(900m, "MINUS200"));
    }

    [Fact]
    public void NextStatusOf_ApprovedRequest_ReturnsDone()
    {
        Assert.Equal(
            RequestStatus.Done,
            RequestTotalCalculator.NextStatusOf(RequestStatus.Approved, 10m));
    }
}
