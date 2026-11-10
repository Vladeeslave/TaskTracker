using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

/// <summary>Прикладний сервіс нарядів: приймає наряд, сповіщає й рахує суму.</summary>
public sealed class WorkOrderService
{
    private readonly IWorkOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Створює сервіс із трьома залежностями.</summary>
    /// <param name="repository">Сховище нарядів.</param>
    /// <param name="pricing">Політика розрахунку ціни.</param>
    /// <param name="notifier">Сповіщувач.</param>
    public WorkOrderService(
        IWorkOrderRepository repository,
        IPricingPolicy pricing,
        INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    /// <summary>Зберігає наряд і сповіщує про його прийняття.</summary>
    /// <param name="order">Наряд на роботи.</param>
    public void Place(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _repository.Add(order);
        _notifier.Notify($"наряд № {order.Id} прийнято.");
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
