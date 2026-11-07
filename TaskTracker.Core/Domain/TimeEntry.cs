namespace TaskTracker.Core.Domain;

/// <summary>Запис часу: хто, що робив і скільки годин витратив.</summary>
public sealed class TimeEntry
{
    /// <summary>Ідентифікатор учасника, який виконував роботу.</summary>
    public int MemberId { get; init; }

    /// <summary>Опис виконаної роботи.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Кількість витрачених годин.</summary>
    public decimal Hours { get; init; }
}
