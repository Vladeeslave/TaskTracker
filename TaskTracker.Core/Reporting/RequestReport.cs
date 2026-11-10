using System.Globalization;
using System.Text;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Requests;

namespace TaskTracker.Core.Reporting;

/// <summary>Формування тексту звіту про заявку.</summary>
public static class RequestReport
{
    /// <summary>Будує текст звіту; результат не залежить від налаштувань системи.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    /// <param name="result">Результат розрахунку.</param>
    /// <returns>Текст звіту з кінцевими розривами рядків.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine($"Заявка № {request.RequestId}");
        text.AppendLine($"Замовник: {request.Requester.Name}");
        foreach (WorkLine line in request.Lines)
        {
            text.AppendLine($"{line.WorkCode} x {line.Hours}");
        }

        text.AppendLine(string.Format(culture, "Разом: {0:F2}", result.Total));
        text.AppendLine($"Стан: {result.Status}");
        return text.ToString();
    }
}
