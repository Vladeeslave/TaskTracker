namespace TaskTracker.Core.Requests;

/// <summary>Дані замовника заявки.</summary>
/// <param name="Name">Ім’я замовника.</param>
/// <param name="Email">Пошта замовника.</param>
/// <param name="IsRegular">Ознака постійного замовника.</param>
public sealed record RequesterInfo(string Name, string Email, bool IsRegular);
