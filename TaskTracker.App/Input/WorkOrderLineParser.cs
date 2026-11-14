using System.Globalization;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;

namespace TaskTracker.App.Input;

/// <summary>Барикада вводу: перетворює рядки з консолі й файлів на перевірений рядок наряду.</summary>
public static class WorkOrderLineParser
{
    /// <summary>Найбільша довжина коду виду роботи.</summary>
    public const int MaxCodeLength = 20;

    /// <summary>Найбільша кількість годин у рядку наряду.</summary>
    public const int MaxHours = 1000;

    /// <summary>Найбільша погодинна ставка, грн.</summary>
    public const decimal MaxHourlyRate = 100000m;

    private const char FieldSeparator = ';';

    private const int FieldCount = 3;

    /// <summary>Розбирає код, години й ставку; помилку вводу повертає як Result.</summary>
    /// <param name="workCode">Код виду роботи.</param>
    /// <param name="hours">Кількість годин текстом.</param>
    /// <param name="hourlyRate">Погодинна ставка текстом; дробова частина через кому.</param>
    /// <returns>Готовий рядок наряду або пояснення помилки.</returns>
    public static Result<WorkOrderLine> Parse(string? workCode, string? hours, string? hourlyRate)
    {
        CultureInfo ukrainian = CultureInfo.GetCultureInfo("uk-UA");
        if (string.IsNullOrWhiteSpace(workCode) || workCode.Trim().Length > MaxCodeLength)
        {
            return Result.Fail<WorkOrderLine>(
                "Код роботи — непорожній рядок не довший за " + MaxCodeLength.ToString(ukrainian) + " символів.");
        }

        if (!int.TryParse(hours, NumberStyles.Integer, ukrainian, out int parsedHours)
            || parsedHours is < 1 or > MaxHours)
        {
            return Result.Fail<WorkOrderLine>(
                "Кількість годин — ціле число від 1 до " + MaxHours.ToString(ukrainian) + ".");
        }

        if (!decimal.TryParse(hourlyRate, NumberStyles.Number, ukrainian, out decimal rate)
            || rate < 0m
            || rate > MaxHourlyRate)
        {
            return Result.Fail<WorkOrderLine>(
                "Ставка — число від 0 до " + MaxHourlyRate.ToString(ukrainian)
                + ", дробова частина через кому, наприклад 250,50.");
        }

        return Result.Ok(new WorkOrderLine(workCode.Trim().ToUpperInvariant(), parsedHours, rate));
    }

    /// <summary>Розбирає один рядок формату «код;години;ставка» без винятків.</summary>
    /// <param name="raw">Вхідний рядок, наприклад WRK-1;3;250,50.</param>
    /// <param name="line">Розібраний рядок наряду або null, якщо вхід некоректний.</param>
    /// <returns>true, якщо вхід коректний; інакше false.</returns>
    public static bool TryParse(string? raw, out WorkOrderLine? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        string[] parts = raw.Split(FieldSeparator);
        if (parts.Length != FieldCount)
        {
            return false;
        }

        Result<WorkOrderLine> parsed = Parse(parts[0], parts[1], parts[2]);
        line = parsed.Value;
        return parsed.IsSuccess;
    }
}
