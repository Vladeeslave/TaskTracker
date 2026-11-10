namespace TaskTracker.Core.Domain;

/// <summary>Рядок заявки на роботи: вид роботи, години й погодинна ставка.</summary>
public sealed class WorkLine
{
    /// <summary>Код виду роботи.</summary>
    public string WorkCode { get; init; } = string.Empty;

    /// <summary>Кількість годин.</summary>
    public int Hours { get; init; }

    /// <summary>Погодинна ставка, грн.</summary>
    public decimal HourlyRate { get; init; }
}
