using TaskTracker.Core.Abstractions;

namespace TaskTracker.Core.Notifications;

/// <summary>Сповіщувач, який пише повідомлення в переданий потік.</summary>
public sealed class TextWriterNotifier : INotifier
{
    private readonly TextWriter _output;

    /// <summary>Створює сповіщувач для потоку виведення.</summary>
    /// <param name="output">Потік виведення (консоль підставляє лише застосунок).</param>
    public TextWriterNotifier(TextWriter output)
    {
        ArgumentNullException.ThrowIfNull(output);
        _output = output;
    }

    /// <summary>Записує повідомлення в потік.</summary>
    /// <param name="message">Текст повідомлення.</param>
    public void Notify(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        _output.WriteLine($"Сповіщення: {message}");
    }
}
