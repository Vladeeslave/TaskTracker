namespace TaskTracker.Core.Domain;

/// <summary>Незмінний рядок наряду на роботи: код роботи, години й погодинна ставка.</summary>
public sealed record WorkOrderLine
{
    /// <summary>Створює рядок наряду та перевіряє його інваріант.</summary>
    /// <param name="workCode">Код виду роботи.</param>
    /// <param name="hours">Кількість годин, більша за нуль.</param>
    /// <param name="hourlyRate">Погодинна ставка, не менша за нуль.</param>
    public WorkOrderLine(string workCode, int hours, decimal hourlyRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hours);
        if (hourlyRate < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hourlyRate),
                hourlyRate,
                "Ставка не може бути від’ємною.");
        }

        WorkCode = workCode;
        Hours = hours;
        HourlyRate = hourlyRate;
    }

    /// <summary>Код виду роботи.</summary>
    public string WorkCode { get; }

    /// <summary>Кількість годин.</summary>
    public int Hours { get; }

    /// <summary>Погодинна ставка, грн.</summary>
    public decimal HourlyRate { get; }

    /// <summary>Вартість рядка без знижок, грн.</summary>
    public decimal Amount => Hours * HourlyRate;
}
