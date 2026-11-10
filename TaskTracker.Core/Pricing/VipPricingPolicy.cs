using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Pricing;

/// <summary>Політика для VIP-замовників: знижка 10 %.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    private const decimal VipRate = 0.10m;

    /// <summary>Рахує ціну рядка зі знижкою для VIP-замовника.</summary>
    /// <param name="line">Рядок наряду.</param>
    /// <returns>Ціна рядка, грн, округлена до двох знаків.</returns>
    public decimal PriceOf(WorkOrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - VipRate), 2);
    }
}
