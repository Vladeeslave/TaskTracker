namespace TaskTracker.Core.Domain;

/// <summary>Запис часу: хто, що робив і скільки годин витратив.</summary>
public sealed class TimeEntry
{
    public int MemberId { get; init; }

    public string Description { get; init; } = string.Empty;

    public decimal Hours { get; init; }
}
