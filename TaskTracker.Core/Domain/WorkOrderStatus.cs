namespace TaskTracker.Core.Domain;

/// <summary>Стан наряду на роботи в його життєвому циклі.</summary>
public enum WorkOrderStatus
{
    /// <summary>Чернетка: рядки ще можна додавати.</summary>
    Draft,

    /// <summary>Заплановано: рядки зафіксовано.</summary>
    Planned,

    /// <summary>Роботи виконуються.</summary>
    InProgress,

    /// <summary>Роботи виконано.</summary>
    Done,

    /// <summary>Наряд скасовано.</summary>
    Cancelled,
}
