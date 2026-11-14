namespace TaskTracker.Legacy;

public interface IDiscountRule
{
    decimal? RateFor(Client client);
}
