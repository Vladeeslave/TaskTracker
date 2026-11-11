using Microsoft.Extensions.Logging;
using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;
using TaskTracker.Core.Errors;

namespace TaskTracker.Core.Services;

/// <summary>Прикладний сервіс нарядів: приймає наряд, оплачує, скасовує й рахує суму.</summary>
public sealed class WorkOrderService
{
    private readonly IWorkOrderRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<WorkOrderService> _log;

    /// <summary>Створює сервіс із чотирма залежностями.</summary>
    /// <param name="repository">Сховище нарядів.</param>
    /// <param name="pricing">Політика розрахунку ціни.</param>
    /// <param name="notifier">Сповіщувач.</param>
    /// <param name="log">Журнал подій.</param>
    public WorkOrderService(
        IWorkOrderRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<WorkOrderService> log)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>Зберігає наряд і сповіщує про його прийняття.</summary>
    /// <param name="order">Наряд на роботи.</param>
    public void Place(WorkOrder order)
    {
        ArgumentNullException.ThrowIfNull(order);
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", order.Id);
        _repository.Add(order);
        _notifier.Notify($"наряд № {order.Id} прийнято.");
        _log.LogInformation("Наряд {OrderId} прийнято", order.Id);
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
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", orderId);
        _log.LogDebug("Додавання рядка {WorkCode} на {Hours} год", line.WorkCode, line.Hours);
        WorkOrder order = Load(orderId);
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
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", orderId);
        _log.LogDebug("Початок оплати, сума {Amount}", amount);
        WorkOrder order = Load(orderId);
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

        if (amount > total)
        {
            _log.LogWarning("Переплата {Extra} грн", amount - total);
        }

        order.Start();
        _log.LogInformation("Оплачено {Amount} грн, роботи розпочато", amount);
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
        using IDisposable? scope = _log.BeginScope("Order:{OrderId}", orderId);
        WorkOrder order = Load(orderId);
        try
        {
            order.Cancel();
            _log.LogInformation("Наряд скасовано, причина: {Reason}", reason);
        }
        catch (InvalidOperationException ex)
        {
            _log.LogError(ex, "Скасування неможливе у стані {Status}", order.Status);
            throw new DomainRuleException(
                "order.cancellable",
                $"Наряд № {orderId} у стані {order.Status} скасувати не можна.",
                ex);
        }
    }

    /// <summary>Записує звіт про наряд у файл; потік гарантовано закривається.</summary>
    /// <param name="path">Шлях до файлу звіту.</param>
    /// <param name="order">Наряд на роботи.</param>
    public void ExportReport(string path, WorkOrder order)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(order);
        try
        {
            using var writer = new StreamWriter(path);
            writer.WriteLine($"Наряд {order.Id}");
            writer.WriteLine(TotalOf(order));
        }
        catch (IOException ex)
        {
            _log.LogError(ex, "Не вдалося зберегти звіт у {Path}", path);
            throw;
        }

        _log.LogInformation("Звіт збережено у {Path}", path);
    }
}
