namespace TaskTracker.Core.Requests;

/// <summary>Стан заявки на роботи.</summary>
public enum RequestStatus
{
    /// <summary>Нова заявка.</summary>
    New,

    /// <summary>Погоджена заявка.</summary>
    Approved,

    /// <summary>Виконана заявка.</summary>
    Done,

    /// <summary>Скасована заявка.</summary>
    Cancelled,
}
