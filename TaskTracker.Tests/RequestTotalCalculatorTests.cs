using TaskTracker.Core.Domain;
using TaskTracker.Core.Requests;

namespace TaskTracker.Tests;

public class RequestTotalCalculatorTests
{
    [Fact]
    public void Process_RegularRequester_Returns1292()
    {
        List<WorkLine> lines = new()
        {
            new WorkLine { WorkCode = "WRK-1", Hours = 12, HourlyRate = 100m },
            new WorkLine { WorkCode = "WRK-2", Hours = 1, HourlyRate = 250m },
        };

        decimal total = RequestTotalCalculator.Process(
            1,
            "Іваненко",
            "i@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            null,
            RequestStatus.New,
            false,
            60m,
            out RequestStatus next);

        Assert.Equal(1292.70m, total);
        Assert.Equal(RequestStatus.Approved, next);
    }

    [Fact]
    public void Process_SmallRequest_AddsDelivery()
    {
        List<WorkLine> lines = new()
        {
            new WorkLine { WorkCode = "WRK-3", Hours = 2, HourlyRate = 200m },
        };

        decimal total = RequestTotalCalculator.Process(
            2,
            "Петренко",
            "p@ex.com",
            false,
            lines,
            new DateOnly(2026, 3, 10),
            "SALE10",
            RequestStatus.Approved,
            false,
            60m,
            out RequestStatus next);

        Assert.Equal(420m, total);
        Assert.Equal(RequestStatus.Done, next);
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
