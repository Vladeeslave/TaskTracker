using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Storage;

/// <summary>Сховище нарядів у пам’яті на словнику за номером.</summary>
public sealed class InMemoryWorkOrderRepository : IWorkOrderRepository
{
    private readonly Dictionary<int, WorkOrder> _items = new();

    /// <summary>Додає наряд; на дублікаті номера кидає виняток.</summary>
    /// <param name="order">Наряд на роботи.</param>
    public void Add(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (!_items.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"Наряд № {order.Id} уже збережено.");
        }
    }

    /// <summary>Шукає наряд за номером.</summary>
    /// <param name="id">Номер наряду.</param>
    /// <returns>Наряд або null, якщо такого немає.</returns>
    public WorkOrder? GetById(int id) => _items.GetValueOrDefault(id);

    /// <summary>Повертає копію списку збережених нарядів.</summary>
    /// <returns>Копія списку нарядів.</returns>
    public IReadOnlyList<WorkOrder> GetAll() => _items.Values.ToList();
}
