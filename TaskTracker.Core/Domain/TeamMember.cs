namespace TaskTracker.Core.Domain;

/// <summary>Учасник команди, який виконує завдання.</summary>
public sealed class TeamMember
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsLead { get; init; }
}
