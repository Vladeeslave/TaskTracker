namespace TaskTracker.Core.Errors;

/// <summary>Фабрика результатів: створює успішний або невдалий <see cref="Result{T}"/>.</summary>
public static class Result
{
    /// <summary>Створює успішний результат зі значенням.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="value">Значення результату.</param>
    /// <returns>Успішний результат.</returns>
    public static Result<T> Ok<T>(T value) => new(true, value, string.Empty);

    /// <summary>Створює невдалий результат із поясненням помилки.</summary>
    /// <typeparam name="T">Тип значення.</typeparam>
    /// <param name="error">Пояснення помилки для користувача.</param>
    /// <returns>Невдалий результат.</returns>
    public static Result<T> Fail<T>(string error)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);
        return new(false, default, error);
    }
}
