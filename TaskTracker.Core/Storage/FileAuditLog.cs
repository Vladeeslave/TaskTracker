namespace TaskTracker.Core.Storage;

/// <summary>Журнал аудиту у файлі: дописує рядки й закриває файл у Dispose.</summary>
public sealed class FileAuditLog : IDisposable
{
    private readonly StreamWriter _writer;
    private bool _disposed;

    /// <summary>Відкриває файл журналу для дописування.</summary>
    /// <param name="path">Шлях до файлу журналу.</param>
    public FileAuditLog(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true);
    }

    /// <summary>Дописує рядок аудиту: час, операція, номер документа.</summary>
    /// <param name="operation">Назва операції.</param>
    /// <param name="documentId">Номер документа.</param>
    public void Write(string operation, int documentId)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        _writer.WriteLine($"{DateTimeOffset.Now:O};{operation};{documentId}");
    }

    /// <summary>Скидає буфер і закриває файл; повторний виклик нічого не робить.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _writer.Flush();
        _writer.Dispose();
        _disposed = true;
    }
}
