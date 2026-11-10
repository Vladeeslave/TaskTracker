namespace TaskTracker.Core.Domain;

/// <summary>Архівний наряд: містить наряд усередині й дає лише читання.</summary>
public sealed class ArchivedWorkOrder
{
    private readonly WorkOrder _order;

    /// <summary>Архівує наряд, який уже виконано або скасовано.</summary>
    /// <param name="order">Наряд на роботи.</param>
    /// <param name="archivedAt">Дата архівування.</param>
    public ArchivedWorkOrder(WorkOrder order, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (order.Status != WorkOrderStatus.Done && order.Status != WorkOrderStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Архівувати можна лише виконаний або скасований наряд.");
        }

        _order = order;
        ArchivedAt = archivedAt;
    }

    /// <summary>Дата архівування.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Номер наряду.</summary>
    public int Id => _order.Id;

    /// <summary>Стан наряду.</summary>
    public WorkOrderStatus Status => _order.Status;

    /// <summary>Рядки наряду лише для читання.</summary>
    public IReadOnlyList<WorkOrderLine> Lines => _order.Lines;

    // Методу AddLine немає взагалі: порушити контракт нічим
}
