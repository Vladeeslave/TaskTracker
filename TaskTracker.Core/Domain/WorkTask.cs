namespace TaskTracker.Core.Domain;

/// <summary>Робоче завдання команди.</summary>
public sealed class WorkTask
{
    private readonly List<TimeEntry> _entries = new();

    public int Id { get; init; }

    public string ProjectCode { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public int AssigneeId { get; init; }

    public WorkTaskStatus Status { get; set; } = WorkTaskStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<TimeEntry> Entries => _entries;

    /// <summary>Додає запис часу до завдання.</summary>
    public void AddEntry(TimeEntry entry) => _entries.Add(entry);

    /// <summary>Обчислює загальну кількість витрачених годин.</summary>
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
