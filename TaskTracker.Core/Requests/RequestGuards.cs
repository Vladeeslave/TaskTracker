using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Requests;

/// <summary>Перевірки передумов для розрахунку підсумку заявки.</summary>
internal static class RequestGuards
{
    /// <summary>Перевіряє заголовок заявки: номер, ім’я та пошту замовника.</summary>
    /// <param name="requestId">Номер заявки.</param>
    /// <param name="requesterName">Ім’я замовника.</param>
    /// <param name="requesterEmail">Пошта замовника.</param>
    public static void EnsureHeaderValid(
        int requestId,
        string requesterName,
        string requesterEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(requestId);
        ArgumentException.ThrowIfNullOrWhiteSpace(requesterName);
        ArgumentException.ThrowIfNullOrWhiteSpace(requesterEmail);
        if (!requesterEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @.",
                nameof(requesterEmail));
        }
    }

    /// <summary>Перевіряє рядок заявки: код роботи, години й ставку.</summary>
    /// <param name="line">Рядок заявки.</param>
    public static void EnsureLineValid(WorkLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.WorkCode);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            line.Hours, nameof(line.Hours));
        ArgumentOutOfRangeException.ThrowIfNegative(
            line.HourlyRate, nameof(line.HourlyRate));
    }

    /// <summary>Перевіряє колекцію рядків: вона не порожня й кожен рядок коректний.</summary>
    /// <param name="lines">Рядки заявки.</param>
    public static void EnsureLinesValid(IReadOnlyList<WorkLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException(
                "Заявка не містить жодного рядка.",
                nameof(lines));
        }

        foreach (WorkLine line in lines)
        {
            EnsureLineValid(line);
        }
    }

    /// <summary>Перевіряє проміжну суму: вона не може бути від’ємною.</summary>
    /// <param name="amount">Проміжна сума, грн.</param>
    public static void EnsureAmountValid(decimal amount)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Сума не може бути від’ємною.");
        }
    }
}
