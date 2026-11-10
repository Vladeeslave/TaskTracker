namespace TaskTracker.Core.Abstractions;

/// <summary>Сповіщувач: надсилає повідомлення про подію.</summary>
public interface INotifier
{
    /// <summary>Надсилає повідомлення.</summary>
    /// <param name="message">Текст повідомлення.</param>
    void Notify(string message);
}
