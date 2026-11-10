using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Abstractions;

/// <summary>Політика розрахунку ціни одного рядка наряду.</summary>
public interface IPricingPolicy
{
    /// <summary>Рахує ціну рядка наряду.</summary>
    /// <param name="line">Рядок наряду.</param>
    /// <returns>Ціна рядка, грн.</returns>
    decimal PriceOf(WorkOrderLine line);
}
