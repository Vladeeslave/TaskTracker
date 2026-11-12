using System.Text;

namespace TaskTracker.Legacy;

public sealed class ReportBuilder
{
    public string Build(WorkOrderRequest req, decimal discount, decimal shipping, decimal total)
    {
        var sb = new StringBuilder();
        sb.Append("Документ #").Append(req.OrderId).Append('\n');
        sb.Append("Клієнт: ").Append(req.Client.Name).Append('\n');
        foreach (JobLine line in req.Lines!)
        {
            sb.Append(Line(line, req.Currency));
        }

        sb.Append(Money("Знижка", discount, req.Currency));
        sb.Append(Money("Доставка", shipping, req.Currency));
        sb.Append(Money("Разом", total, req.Currency));
        return sb.ToString();
    }

    private static string Line(JobLine line, string currency) =>
        line.JobCode + " x" + line.Hours + " = " +
        (line.Hours * line.HourlyRate).ToString("0.00") + " " + currency + "\n";

    private static string Money(string label, decimal value, string currency) =>
        label + ": " + value.ToString("0.00") + " " + currency + "\n";
}
