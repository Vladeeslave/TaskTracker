using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

/// <summary>Прикладний сервіс нарядів: приймає наряд і рахує його суму.</summary>
public sealed class WorkOrderService
{
    private readonly IWorkOrderRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>Створює сервіс із двома залежностями.</summary>
    /// <param name="repository">Сховище нарядів.</param>
    /// <param name="pricing">Політика розрахунку ціни.</param>
    public WorkOrderService(IWorkOrderRepository repository, IPricingPolicy pricing)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }

    /// <summary>Зберігає наряд у сховищі.</summary>
    /// <param name="order">Наряд на роботи.</param>
    public void Place(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _repository.Add(order);
    }

    /// <summary>Рахує суму наряду за підставленою політикою.</summary>
    /// <param name="order">Наряд на роботи.</param>
    /// <returns>Сума наряду, грн.</returns>
    public decimal TotalOf(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        decimal total = 0m;
        foreach (WorkOrderLine line in order.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }
}
