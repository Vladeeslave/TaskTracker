using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Abstractions;

/// <summary>Сховище нарядів на роботи: відповідає лише за зберігання.</summary>
public interface IWorkOrderRepository
{
    /// <summary>Додає наряд до сховища.</summary>
    /// <param name="order">Наряд на роботи.</param>
    void Add(WorkOrder order);

    /// <summary>Шукає наряд за номером.</summary>
    /// <param name="id">Номер наряду.</param>
    /// <returns>Наряд або null, якщо такого немає.</returns>
    WorkOrder? GetById(int id);

    /// <summary>Повертає всі збережені наряди.</summary>
    /// <returns>Копія списку нарядів.</returns>
    IReadOnlyList<WorkOrder> GetAll();
}
