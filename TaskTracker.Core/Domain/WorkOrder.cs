namespace TaskTracker.Core.Domain;

/// <summary>Наряд на роботи: агрегат зі станом і контрольованими переходами.</summary>
public sealed class WorkOrder
{
    private readonly List<WorkOrderLine> _lines = new();

    /// <summary>Створює наряд у стані чернетки.</summary>
    /// <param name="id">Номер наряду, більший за нуль.</param>
    /// <param name="customerId">Ідентифікатор замовника, більший за нуль.</param>
    /// <param name="createdAt">Дата створення наряду.</param>
    public WorkOrder(int id, int customerId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(customerId);

        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt;
        Status = WorkOrderStatus.Draft;
    }

    /// <summary>Номер наряду.</summary>
    public int Id { get; }

    /// <summary>Ідентифікатор замовника.</summary>
    public int CustomerId { get; }

    /// <summary>Дата створення наряду.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Поточний стан наряду; змінюється лише методами-переходами.</summary>
    public WorkOrderStatus Status { get; private set; }

    /// <summary>Рядки наряду лише для читання.</summary>
    public IReadOnlyList<WorkOrderLine> Lines => _lines.AsReadOnly();

    /// <summary>Додає рядок до наряду; дозволено лише в стані чернетки.</summary>
    /// <param name="line">Рядок наряду.</param>
    public void AddLine(WorkOrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Status != WorkOrderStatus.Draft)
        {
            throw new InvalidOperationException("Рядки додають лише до чернетки наряду.");
        }

        _lines.Add(line);
    }

    /// <summary>Переводить наряд зі стану Draft у Planned; потрібен хоча б один рядок.</summary>
    public void Plan()
    {
        EnsureStatus(WorkOrderStatus.Draft);
        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Наряд без рядків запланувати не можна.");
        }

        Status = WorkOrderStatus.Planned;
    }

    /// <summary>Переводить наряд зі стану Planned у InProgress.</summary>
    public void Start()
    {
        EnsureStatus(WorkOrderStatus.Planned);
        Status = WorkOrderStatus.InProgress;
    }

    /// <summary>Переводить наряд зі стану InProgress у Done.</summary>
    public void Complete()
    {
        EnsureStatus(WorkOrderStatus.InProgress);
        Status = WorkOrderStatus.Done;
    }

    /// <summary>Скасовує наряд; можливо лише до початку робіт.</summary>
    public void Cancel()
    {
        if (Status != WorkOrderStatus.Draft && Status != WorkOrderStatus.Planned)
        {
            throw new InvalidOperationException($"Скасувати наряд у стані {Status} не можна.");
        }

        Status = WorkOrderStatus.Cancelled;
    }

    private void EnsureStatus(WorkOrderStatus expected)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException(
                $"Перехід заборонено: очікувався стан {expected}, а поточний — {Status}.");
        }
    }
}
