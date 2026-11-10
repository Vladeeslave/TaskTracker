using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Services;

/// <summary>Прикладний сервіс нарядів: приймає наряд, сповіщає й рахує суму.</summary>
public sealed class WorkOrderService
{
    private readonly IWorkOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Створює сервіс із трьома залежностями.</summary>
    /// <param name="repository">Сховище нарядів.</param>
    /// <param name="pricing">Політика розрахунку ціни.</param>
    /// <param name="notifier">Сповіщувач.</param>
    public WorkOrderService(
        IWorkOrderRepository repository,
        IPricingPolicy pricing,
        INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    /// <summary>Зберігає наряд і сповіщує про його прийняття.</summary>
    /// <param name="order">Наряд на роботи.</param>
    public void Place(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        _repository.Add(order);
        _notifier.Notify($"наряд № {order.Id} прийнято.");
    }

    /// <summary>Рахує суму наряду за підставленою політикою.</summary>
    /// <param name="order">Наряд на роботи.</param>
    /// <returns>Сума наряду, грн.</returns>
    public decimal TotalOf(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        decimal total = 0m;
        foreach (WorkOrderLine line in order.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }

    /// <summary>АНТИПАТЕРН: ядро саме розбирає рядки з консолі.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <param name="workCode">Код роботи.</param>
    /// <param name="hours">Кількість годин текстом.</param>
    /// <param name="hourlyRate">Ставка текстом.</param>
    public void AddLine(int orderId, string workCode, string hours, string hourlyRate)
    {
        var order = _repository.GetById(orderId);
        var parsedHours = int.Parse(hours);
        var rate = decimal.Parse(hourlyRate);
        order!.AddLine(new WorkOrderLine(workCode, parsedHours, rate));
    }

    /// <summary>АНТИПАТЕРН: коди повернення без назв і сенсу.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <param name="amount">Сплачена сума.</param>
    /// <returns>0 — успіх, від’ємне число — код помилки.</returns>
    public int Pay(int orderId, decimal amount)
    {
        var order = _repository.GetById(orderId);
        if (order == null)
        {
            return -1;
        }

        if (order.Status != WorkOrderStatus.Planned)
        {
            return -2;
        }

        if (amount < TotalOf(order))
        {
            return -3;
        }

        order.Start();
        return 0;
    }

    /// <summary>АНТИПАТЕРН: порожній catch і повернення null.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <returns>Наряд або null.</returns>
    public WorkOrder? Load(int orderId)
    {
        try
        {
            return _repository.GetById(orderId);
        }
        catch (Exception)
        {
        }

        return null;
    }

    /// <summary>АНТИПАТЕРНИ: широкий catch і throw ex.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <param name="reason">Причина скасування.</param>
    public void Cancel(int orderId, string reason)
    {
        try
        {
            var order = _repository.GetById(orderId);
            order!.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
#pragma warning disable CA2200 // стан «до» для ЛР 5; виправляється в завданні 5
            throw ex;
#pragma warning restore CA2200
        }
    }

    /// <summary>АНТИПАТЕРН: файл не закривається, якщо трапиться виняток.</summary>
    /// <param name="path">Шлях до файлу звіту.</param>
    /// <param name="order">Наряд на роботи.</param>
    public void ExportReport(string path, WorkOrder order)
    {
        var writer = new StreamWriter(path);
        writer.WriteLine($"Наряд {order.Id}");
        writer.WriteLine(TotalOf(order));
        writer.Close();
    }
}
