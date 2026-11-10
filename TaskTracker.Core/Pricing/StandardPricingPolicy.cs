using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Pricing;

/// <summary>Стандартна політика: ціна рядка без знижки.</summary>
public sealed class StandardPricingPolicy : IPricingPolicy
{
    /// <summary>Рахує ціну рядка без знижки.</summary>
    /// <param name="line">Рядок наряду.</param>
    /// <returns>Ціна рядка, грн.</returns>
    public decimal PriceOf(WorkOrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return line.Amount;
    }
}
