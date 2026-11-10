using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Pricing;

/// <summary>Політика зі знижкою у вигляді частки від вартості рядка.</summary>
public sealed class DiscountPricingPolicy : IPricingPolicy
{
    private const decimal MaxRate = 0.5m;
    private readonly decimal _rate;

    /// <summary>Створює політику зі знижкою.</summary>
    /// <param name="rate">Частка знижки від 0 до 0,5.</param>
    public DiscountPricingPolicy(decimal rate)
    {
        if (rate < 0m || rate > MaxRate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rate),
                rate,
                "Знижка має бути від 0 до 0,5.");
        }

        _rate = rate;
    }

    /// <summary>Рахує ціну рядка з урахуванням знижки.</summary>
    /// <param name="line">Рядок наряду.</param>
    /// <returns>Ціна рядка, грн, округлена до двох знаків.</returns>
    public decimal PriceOf(WorkOrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - _rate), 2);
    }
}
