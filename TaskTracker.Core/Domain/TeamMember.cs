namespace TaskTracker.Core.Domain;

/// <summary>Учасник команди, який виконує завдання.</summary>
public sealed class TeamMember
{
    /// <summary>Ідентифікатор учасника команди.</summary>
    public int Id { get; init; }

    /// <summary>Ім’я учасника команди.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Ознака керівника команди.</summary>
    public bool IsLead { get; init; }
}
