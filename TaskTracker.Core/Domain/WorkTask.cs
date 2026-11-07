namespace TaskTracker.Core.Domain;

/// <summary>Робоче завдання команди.</summary>
public sealed class WorkTask
{
    private readonly List<TimeEntry> _entries = new();

    /// <summary>Ідентифікатор завдання.</summary>
    public int Id { get; init; }

    /// <summary>Код проєкту, до якого належить завдання.</summary>
    public string ProjectCode { get; init; } = string.Empty;

    /// <summary>Назва завдання.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Ідентифікатор виконавця.</summary>
    public int AssigneeId { get; init; }

    /// <summary>Поточний стан завдання.</summary>
    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.New;

    /// <summary>Дата й час створення завдання.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Записи часу, внесені до завдання.</summary>
    public IReadOnlyList<TimeEntry> Entries => _entries;

    /// <summary>Додає запис часу до завдання.</summary>
    /// <param name="entry">Запис часу, який додається.</param>
    public void AddEntry(TimeEntry entry) => _entries.Add(entry);

    /// <summary>Обчислює загальну кількість витрачених годин.</summary>
    /// <returns>Сума годин усіх записів часу завдання.</returns>
    public decimal CalculateTotalHours()
    {
        decimal sum = 0m;

        foreach (TimeEntry entry in _entries)
        {
            sum += entry.Hours;
        }

        return sum;
    }
}
