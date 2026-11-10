using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;

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

        System.Diagnostics.Debug.Assert(total >= 0m, "Сума наряду не може бути від’ємною.");
        return total;
    }

    /// <summary>Додає рядок до наряду; перевіряє, що наряд існує й редагується.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <param name="line">Перевірений рядок наряду.</param>
    public void AddLine(int orderId, WorkOrderLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        WorkOrder order = _repository.GetById(orderId)
            ?? throw new DomainRuleException(
                "order.exists",
                $"Наряд № {orderId} не знайдено.");
        if (order.Status != WorkOrderStatus.Draft)
        {
            throw new DomainRuleException(
                "order.editable",
                $"Наряд № {orderId} у стані {order.Status} не редагується.");
        }

        order.AddLine(line);
    }

    /// <summary>Приймає оплату: після повної оплати наряд переходить у стан InProgress.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <param name="amount">Сплачена сума.</param>
    public void Pay(int orderId, decimal amount)
    {
        WorkOrder order = _repository.GetById(orderId)
            ?? throw new DomainRuleException(
                "order.exists",
                $"Наряд № {orderId} не знайдено.");
        if (order.Status != WorkOrderStatus.Planned)
        {
            throw new DomainRuleException(
                "order.payable",
                $"Стан {order.Status} не дозволяє оплату наряду № {orderId}.");
        }

        decimal total = TotalOf(order);
        if (amount < total)
        {
            throw new DomainRuleException(
                "order.amount",
                $"Сплачено {amount:0.00} грн, а треба {total:0.00} грн.");
        }

        order.Start();
    }

    /// <summary>Завантажує наряд; якщо його немає — кидає виняток, а не повертає null.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <returns>Знайдений наряд.</returns>
    public WorkOrder Load(int orderId)
    {
        return _repository.GetById(orderId)
            ?? throw new DomainRuleException(
                "order.exists",
                $"Наряд № {orderId} не знайдено.");
    }

    /// <summary>Скасовує наряд; порушення правила повідомляє винятком із контекстом.</summary>
    /// <param name="orderId">Номер наряду.</param>
    /// <param name="reason">Причина скасування.</param>
    public void Cancel(int orderId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        WorkOrder order = Load(orderId);
        try
        {
            order.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            throw new DomainRuleException(
                "order.cancellable",
                $"Наряд № {orderId} у стані {order.Status} скасувати не можна.",
                ex);
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
