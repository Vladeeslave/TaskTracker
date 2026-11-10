namespace TaskTracker.Core.Errors;

/// <summary>Порушено правило предметної області: виняток з ідентифікатором правила.</summary>
public class DomainRuleException : Exception
{
    /// <summary>Створює виняток із ідентифікатором правила та повідомленням.</summary>
    /// <param name="rule">Ідентифікатор правила, наприклад order.editable.</param>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        Rule = rule;
    }

    /// <summary>Створює виняток із внутрішнім винятком, що зберігає початкову причину.</summary>
    /// <param name="rule">Ідентифікатор правила.</param>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    /// <param name="innerException">Початковий виняток.</param>
    public DomainRuleException(string rule, string message, Exception innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        Rule = rule;
    }

    /// <summary>Ідентифікатор порушеного правила.</summary>
    public string Rule { get; }
}
