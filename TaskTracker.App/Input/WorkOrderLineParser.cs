using System.Globalization;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;

namespace TaskTracker.App.Input;

/// <summary>Барикада вводу: перетворює рядки з консолі на перевірений рядок наряду.</summary>
public static class WorkOrderLineParser
{
    /// <summary>Розбирає код, години й ставку; помилку вводу повертає як Result.</summary>
    /// <param name="workCode">Код виду роботи.</param>
    /// <param name="hours">Кількість годин текстом.</param>
    /// <param name="hourlyRate">Погодинна ставка текстом; дробова частина через кому.</param>
    /// <returns>Готовий рядок наряду або пояснення помилки.</returns>
    public static Result<WorkOrderLine> Parse(string? workCode, string? hours, string? hourlyRate)
    {
        CultureInfo ukrainian = CultureInfo.GetCultureInfo("uk-UA");
        if (string.IsNullOrWhiteSpace(workCode))
        {
            return Result.Fail<WorkOrderLine>("Код роботи не може бути порожнім.");
        }

        if (!int.TryParse(hours, NumberStyles.Integer, ukrainian, out int parsedHours)
            || parsedHours <= 0)
        {
            return Result.Fail<WorkOrderLine>("Кількість годин — ціле число більше 0.");
        }

        if (!decimal.TryParse(hourlyRate, NumberStyles.Number, ukrainian, out decimal rate)
            || rate < 0m)
        {
            return Result.Fail<WorkOrderLine>(
                "Ставка — число не менше 0, дробова частина через кому, наприклад 250,50.");
        }

        return Result.Ok(new WorkOrderLine(workCode.Trim().ToUpperInvariant(), parsedHours, rate));
    }
}
