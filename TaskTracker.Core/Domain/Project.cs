namespace TaskTracker.Core.Domain;

/// <summary>Проєкт, до якого належать завдання команди.</summary>
public sealed class Project
{
    public string Code { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

    public decimal PlannedHours { get; init; }

    public bool IsActive { get; init; }
}
