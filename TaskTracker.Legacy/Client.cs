namespace TaskTracker.Legacy;
 
public enum ClientKind
{
    Regular,
    Vip,
    Staff,
}
 
public enum OrderState
{
    Draft,
    New,
    Paid,
    Closed,
}
 
public sealed class Client
{
    public int Id { get; init; }
 
    public string Name { get; init; } = "";
 
    public string Email { get; init; } = "";
 
    public ClientKind Kind { get; init; }
 
    public int DoneCount { get; init; }
 
    public DateTime SinceUtc { get; init; }
 
    public bool HasValidEmail =>
        Email is not null && Email.Contains("@");
 
    public bool IsLoyal =>
        DoneCount > PricingRules.LoyalOrders;
}
