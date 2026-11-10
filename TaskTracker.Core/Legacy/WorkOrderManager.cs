// Навмисно «поганий» успадкований код для аудиту SOLID; теку Legacy буде видалено в кроці 6.
#pragma warning disable
namespace TaskTracker.Core.Legacy;

public class WorkOrderManager
{
    private readonly FileWorkOrderStore _store = new FileWorkOrderStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    public decimal Place(LegacyWorkOrder order, string customerType)
    {
        decimal total = 0m;
        foreach (var line in order.Lines)
        {
            total += line.HourlyRate * line.Hours;
        }

        if (customerType == "regular")
        {
            total = total * 0.95m;
        }
        else if (customerType == "vip")
        {
            total = total * 0.90m;
        }
        else if (customerType == "staff")
        {
            total = total * 0.70m;
        }

        if (total > 1000m)
        {
            total = total - 50m;
        }

        order.Total = total;
        order.Status = 1;
        _store.SaveToFile(order, "orders.json");
        _mailer.Send(order.Email, "Order " + order.Id, "" + total);
        return total;
    }

    public string BuildCsvReport(List<LegacyWorkOrder> orders)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var o in orders)
        {
            text += o.Id + ";" + o.Total + ";" + o.Status + Environment.NewLine;
        }

        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyWorkOrder order)
    {
        LegacyWorkOrder archived = new ArchivedWorkOrder(order.Id);
        foreach (var line in order.Lines)
        {
            archived.AddLine(line);
        }

        _store.SaveToFile(archived, "archive.json");
    }
}
