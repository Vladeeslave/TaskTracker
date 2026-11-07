namespace TaskTracker.Core.Domain;

/// <summary>Проєкт, до якого належать завдання команди.</summary>
public sealed class Project
{
    /// <summary>Код проєкту.</summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>Назва проєкту.</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Заплановані години на проєкт.</summary>
    public decimal PlannedHours { get; init; }

    /// <summary>Ознака активного проєкту.</summary>
    public bool IsActive { get; init; }
}
