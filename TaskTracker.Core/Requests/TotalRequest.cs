using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Requests;

/// <summary>Запит на розрахунок підсумку заявки.</summary>
/// <param name="RequestId">Номер заявки.</param>
/// <param name="Requester">Дані замовника.</param>
/// <param name="Lines">Рядки заявки.</param>
/// <param name="CreatedAt">Дата створення заявки.</param>
/// <param name="CouponCode">Код купона або null.</param>
/// <param name="Status">Поточний стан заявки.</param>
/// <param name="DeliveryPrice">Вартість виїзду, грн.</param>
public sealed record TotalRequest(
    int RequestId,
    RequesterInfo Requester,
    IReadOnlyList<WorkLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    RequestStatus Status,
    decimal DeliveryPrice);
