namespace TaskTracker.Core.Domain;

/// <summary>Незмінний запис довідника замовників наряду на роботи.</summary>
public sealed record Customer
{
    /// <summary>Створює замовника та перевіряє його інваріант.</summary>
    /// <param name="id">Ідентифікатор, більший за нуль.</param>
    /// <param name="fullName">Повне ім’я, не порожнє.</param>
    /// <param name="email">Адреса пошти, що містить символ @.</param>
    public Customer(int id, string fullName, string email)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        if (!email.Contains('@'))
        {
            throw new ArgumentException("Пошта має містити символ @.", nameof(email));
        }

        Id = id;
        FullName = fullName;
        Email = email;
    }

    /// <summary>Ідентифікатор замовника.</summary>
    public int Id { get; }

    /// <summary>Повне ім’я замовника.</summary>
    public string FullName { get; }

    /// <summary>Адреса пошти замовника.</summary>
    public string Email { get; }

    /// <summary>Ознака постійного замовника; задається лише під час створення.</summary>
    public bool IsRegular { get; init; }
}
