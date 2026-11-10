using System.Text;
using TaskTracker.App.Input;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;
using TaskTracker.Core.Notifications;
using TaskTracker.Core.Pricing;
using TaskTracker.Core.Reporting;
using TaskTracker.Core.Services;
using TaskTracker.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Верхній обробник: єдине місце, де дозволено catch (Exception)
try
{
    RunApplication();
    return 0;
}
catch (DomainRuleException ex)
{
    Console.WriteLine($"Порушено правило {ex.Rule}: {ex.Message}");
    return 1;
}
catch (IOException ex)
{
    Console.WriteLine($"Помилка середовища: {ex.Message}");
    return 3;
}
catch (Exception ex)
{
    Console.WriteLine($"Непередбачений збій: {ex}");
    return 2;
}

static void RunApplication()
{
    var policies = new Dictionary<string, IPricingPolicy>
    {
        ["regular"] = new DiscountPricingPolicy(0.05m),
        ["vip"] = new VipPricingPolicy(),
        ["staff"] = new DiscountPricingPolicy(0.30m),
    };

    var customer = new Customer(100, "Іваненко Петро", "petro@example.com") { IsRegular = true };
    string customerType = customer.IsRegular ? "regular" : "new";
    IPricingPolicy pricing = policies.TryGetValue(customerType, out IPricingPolicy? found)
        ? found
        : new StandardPricingPolicy();

    var repository = new InMemoryWorkOrderRepository();
    var notifier = new TextWriterNotifier(Console.Out);
    var service = new WorkOrderService(repository, pricing, notifier);

    var order = new WorkOrder(1, customer.Id, DateOnly.FromDateTime(DateTime.Today));
    service.Place(order);
    service.AddLine(order.Id, ReadWorkOrderLine());
    order.Plan();

    decimal total = service.TotalOf(order);
    Console.WriteLine($"Сума: {total:0.00}");
    service.Pay(order.Id, total);
    Console.WriteLine($"Стан наряду: {order.Status}");
    Console.Write(WorkOrderCsvReport.BuildCsv(repository.GetAll()));
}

static WorkOrderLine ReadWorkOrderLine()
{
    while (true)
    {
        Result<WorkOrderLine> parsed = WorkOrderLineParser.Parse(
            Ask("Код роботи: "),
            Ask("Кількість годин: "),
            Ask("Ставка за годину, грн: "));
        if (parsed.IsSuccess)
        {
            return parsed.Value!;
        }

        Console.WriteLine($"Помилка вводу: {parsed.Error}");
    }
}

static string Ask(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine() ?? throw new EndOfStreamException("Потік введення закрито.");
}
