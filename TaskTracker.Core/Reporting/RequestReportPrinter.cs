namespace TaskTracker.Core.Reporting;

/// <summary>Команда виведення готового звіту в потік.</summary>
public sealed class RequestReportPrinter
{
    private readonly TextWriter _output;

    /// <summary>Створює друкарку, яка пише у вказаний потік.</summary>
    /// <param name="output">Потік виведення.</param>
    public RequestReportPrinter(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Записує звіт у потік виведення.</summary>
    /// <param name="report">Текст звіту.</param>
    public void Print(string report)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(report);
        _output.Write(report);
    }
}
