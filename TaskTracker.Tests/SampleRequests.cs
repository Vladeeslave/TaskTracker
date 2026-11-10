using TaskTracker.Core.Domain;
using TaskTracker.Core.Requests;

namespace TaskTracker.Tests;

internal static class SampleRequests
{
    public static TotalRequest RegularRequester()
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

    public static TotalRequest SmallWithCoupon()
    {
        List<WorkLine> lines = new()
        {
            new WorkLine { WorkCode = "WRK-3", Hours = 2, HourlyRate = 200m },
        };

        return new TotalRequest(
            RequestId: 2,
            Requester: new RequesterInfo("Петренко", "p@ex.com", false),
            Lines: lines,
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: "SALE10",
            Status: RequestStatus.Approved,
            DeliveryPrice: 60m);
    }

    public static TotalRequest FreeWork()
    {
        List<WorkLine> lines = new()
        {
            new WorkLine { WorkCode = "WRK-4", Hours = 1, HourlyRate = 0m },
        };

        return new TotalRequest(
            RequestId: 3,
            Requester: new RequesterInfo("Коваль", "k@ex.com", false),
            Lines: lines,
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: RequestStatus.New,
            DeliveryPrice: 60m);
    }
}
