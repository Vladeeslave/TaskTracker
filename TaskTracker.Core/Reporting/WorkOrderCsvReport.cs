using System.Globalization;
using System.Text;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Reporting;

/// <summary>Формування CSV-звіту за нарядами; нічого не пише на диск чи в консоль.</summary>
public static class WorkOrderCsvReport
{
    /// <summary>Будує CSV-текст: номер, стан, години й сума кожного наряду.</summary>
    /// <param name="orders">Наряди на роботи.</param>
    /// <returns>Текст CSV із заголовком.</returns>
    public static string BuildCsv(IEnumerable<WorkOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(orders);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine("id;status;hours;amount");
        foreach (WorkOrder order in orders)
        {
            int hours = order.Lines.Sum(line => line.Hours);
            decimal amount = order.Lines.Sum(line => line.Amount);
            text.AppendLine(string.Format(
                culture,
                "{0};{1};{2};{3:F2}",
                order.Id,
                order.Status,
                hours,
                amount));
        }

        return text.ToString();
    }
}
