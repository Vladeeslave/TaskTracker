using System.Text;

namespace TaskTracker.Core.Reporting;

/// <summary>Формує текст простого звіту з готових рядків.</summary>
public sealed class SummaryReportBuilder
{
    /// <summary>Заголовок звіту; друкується великими літерами.</summary>
    public string Title { get; init; } = "Звіт";

    /// <summary>Збирає звіт: заголовок і по одному рядку на кожен елемент.</summary>
    /// <param name="rows">Рядки звіту.</param>
    /// <returns>Текст звіту; кожен рядок завершується переходом на новий рядок.</returns>
    public string Build(IReadOnlyList<string> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        StringBuilder text = new();
        text.AppendLine(Title.ToUpperInvariant());
        foreach (string row in rows)
        {
            text.AppendLine(row);
        }

        return text.ToString();
    }

    /// <summary>Навмисно некоректний метод для перевірки конвеєра.</summary>
    /// <param name="count">Кількість позицій.</param>
    /// <returns>Текст із кількістю позицій.</returns>
    public string Describe(int count)
    {
        string note;
        return "Позицій: " + count;
    }

}
