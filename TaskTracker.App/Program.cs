using System.Text;
using Microsoft.Extensions.Logging;
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

using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});
ILogger log = loggerFactory.CreateLogger("TaskTracker.App");

// Верхній обробник: єдине місце, де дозволено catch (Exception)
try
{
    RunApplication(loggerFactory);
    return 0;
}
catch (DomainRuleException ex)
{
    log.LogWarning("Правило {Rule}: {Message}", ex.Rule, ex.Message);
    return 1;
}
catch (IOException ex)
{
    log.LogError(ex, "Помилка середовища");
    return 3;
}
catch (Exception ex)
{
    log.LogCritical(ex, "Непередбачений збій");
    return 2;
}

static void RunApplication(ILoggerFactory loggerFactory)
{
    using var audit = new FileAuditLog("audit.log");
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
    var service = new WorkOrderService(
        repository,
        pricing,
        notifier,
        loggerFactory.CreateLogger<WorkOrderService>());

    var order = new WorkOrder(1, customer.Id, DateOnly.FromDateTime(DateTime.Today));
    service.Place(order);
    service.AddLine(order.Id, ReadWorkOrderLine());
    order.Plan();

    decimal total = service.TotalOf(order);
    Console.WriteLine($"Сума: {total:0.00}");

    // Переплата на 25 грн: для демонстрації рівня Warning
    service.Pay(order.Id, total + 25m);
    audit.Write("order.paid", order.Id);
    Console.WriteLine($"Стан наряду: {order.Status}");

    try
    {
        service.Cancel(order.Id, "демонстрація помилки");
    }
    catch (DomainRuleException ex)
    {
        Console.WriteLine($"Скасування відхилено: {ex.Message}");
    }

    Directory.CreateDirectory("reports");
    service.ExportReport(Path.Combine("reports", "order-1.txt"), order);
    audit.Write("order.exported", order.Id);

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
