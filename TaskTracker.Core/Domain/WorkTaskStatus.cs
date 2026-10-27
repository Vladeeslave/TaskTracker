namespace TaskTracker.Core.Domain;

/// <summary>Стан робочого завдання в його життєвому циклі.</summary>
public enum WorkTaskStatus
{
    New = 0,
    InProgress = 1,
    Review = 2,
    Done = 3,
    Cancelled = 4,
}
