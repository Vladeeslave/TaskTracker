namespace TaskTracker.Core.Requests;

/// <summary>Тарифні правила розрахунку підсумку заявки.</summary>
public static class PricingRules
{
    /// <summary>Кількість годин у рядку, з якої діє знижка за обсяг.</summary>
    public const int BulkHours = 10;

    /// <summary>Частка знижки за обсяг годин у рядку.</summary>
    public const decimal BulkRate = 0.05m;

    /// <summary>Частка знижки постійному замовнику.</summary>
    public const decimal RegularRequesterRate = 0.07m;

    /// <summary>Сума, з якої діє знижка на велику заявку.</summary>
    public const decimal LargeRequestFrom = 5000m;

    /// <summary>Частка знижки на велику заявку.</summary>
    public const decimal LargeRequestRate = 0.10m;

    /// <summary>Сума, з якої діє знижка на середню заявку.</summary>
    public const decimal MediumRequestFrom = 2000m;

    /// <summary>Частка знижки на середню заявку.</summary>
    public const decimal MediumRequestRate = 0.05m;

    /// <summary>Частка знижки на заявку, створену в неділю.</summary>
    public const decimal SundayRate = 0.02m;

    /// <summary>Частка знижки за купоном SALE10.</summary>
    public const decimal CouponSaleRate = 0.10m;

    /// <summary>Фіксована знижка за купоном MINUS200, грн.</summary>
    public const decimal CouponFixed = 200m;

    /// <summary>Сума, з якої діє купон MINUS200.</summary>
    public const decimal CouponFixedFrom = 1000m;

    /// <summary>Сума, з якої виїзд безкоштовний.</summary>
    public const decimal FreeDeliveryFrom = 1000m;

    /// <summary>Код купона зі знижкою у відсотках.</summary>
    public const string SaleCoupon = "SALE10";

    /// <summary>Код купона з фіксованою знижкою.</summary>
    public const string FixedCoupon = "MINUS200";
}
