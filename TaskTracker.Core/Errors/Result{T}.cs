namespace TaskTracker.Core.Errors;

/// <summary>Результат операції: або значення, або пояснення помилки.</summary>
/// <typeparam name="T">Тип значення.</typeparam>
public sealed class Result<T>
{
    /// <summary>Створює результат; викликається лише фабрикою <see cref="Result"/>.</summary>
    /// <param name="isSuccess">Ознака успіху.</param>
    /// <param name="value">Значення результату.</param>
    /// <param name="error">Пояснення помилки.</param>
    internal Result(bool isSuccess, T? value, string error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>Ознака успіху.</summary>
    public bool IsSuccess { get; }

    /// <summary>Значення; читається лише після перевірки <see cref="IsSuccess"/>.</summary>
    public T? Value { get; }

    /// <summary>Пояснення помилки; для успішного результату порожнє.</summary>
    public string Error { get; }
}
