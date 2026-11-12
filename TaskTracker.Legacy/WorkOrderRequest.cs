namespace TaskTracker.Legacy;
 
public sealed record WorkOrderRequest(
    int OrderId,
    Client Client,
    IReadOnlyList<JobLine>? Lines,
    OrderState State,
    string Currency,
    bool SendMail);
