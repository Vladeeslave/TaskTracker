using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Requests;

/// <summary>Розрахунок підсумку заявки на роботи.</summary>
public static class RequestTotalCalculator
{
    /// <summary>Рахує суму рядків заявки з урахуванням знижки за обсяг годин.</summary>
    /// <param name="lines">Рядки заявки.</param>
    /// <returns>Сума рядків до застосування інших знижок.</returns>
    public static decimal SubtotalOf(IReadOnlyList<WorkLine> lines)
    {
        RequestGuards.EnsureLinesValid(lines);
        decimal subtotal = 0m;
        foreach (WorkLine line in lines)
        {
            decimal sum = line.Hours * line.HourlyRate;
            if (line.Hours >= PricingRules.BulkHours)
            {
                sum -= sum * PricingRules.BulkRate;
            }

            subtotal += sum;
        }

        return subtotal;
    }

    /// <summary>Рахує знижку постійного замовника.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <param name="isRegularRequester">Ознака постійного замовника.</param>
    /// <returns>Сума знижки; для звичайного замовника — нуль.</returns>
    public static decimal ClientDiscountOf(decimal amount, bool isRegularRequester)
    {
        RequestGuards.EnsureAmountValid(amount);
        return isRegularRequester
            ? amount * PricingRules.RegularRequesterRate
            : 0m;
    }

    /// <summary>Рахує знижку за обсягом заявки.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <returns>Сума знижки; для малої заявки — нуль.</returns>
    public static decimal VolumeDiscountOf(decimal amount)
    {
        RequestGuards.EnsureAmountValid(amount);
        if (amount > PricingRules.LargeRequestFrom)
        {
            return amount * PricingRules.LargeRequestRate;
        }

        if (amount > PricingRules.MediumRequestFrom)
        {
            return amount * PricingRules.MediumRequestRate;
        }

        return 0m;
    }

    /// <summary>Рахує знижку за кодом купона.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <returns>Сума знижки; для невідомого купона — нуль.</returns>
    public static decimal CouponDiscountOf(decimal amount, string? couponCode)
    {
        RequestGuards.EnsureAmountValid(amount);
        return couponCode switch
        {
            PricingRules.SaleCoupon
                => amount * PricingRules.CouponSaleRate,
            PricingRules.FixedCoupon when amount > PricingRules.CouponFixedFrom
                => PricingRules.CouponFixed,
            _ => 0m,
        };
    }

    /// <summary>Рахує знижку на заявку, створену в неділю.</summary>
    /// <param name="amount">Сума, від якої рахується знижка.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <returns>Сума знижки; в інші дні — нуль.</returns>
    public static decimal SundayDiscountOf(decimal amount, DateOnly createdAt)
    {
        RequestGuards.EnsureAmountValid(amount);
        return createdAt.DayOfWeek == DayOfWeek.Sunday
            ? amount * PricingRules.SundayRate
            : 0m;
    }

    /// <summary>Рахує плату за виїзд.</summary>
    /// <param name="amount">Сума заявки після знижок.</param>
    /// <param name="deliveryPrice">Вартість виїзду, грн.</param>
    /// <returns>Плата за виїзд; для великої суми — нуль.</returns>
    public static decimal DeliveryFeeOf(decimal amount, decimal deliveryPrice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(deliveryPrice);
        return amount < PricingRules.FreeDeliveryFrom
            ? deliveryPrice
            : 0m;
    }

    /// <summary>Визначає наступний стан заявки.</summary>
    /// <param name="current">Поточний стан заявки.</param>
    /// <param name="total">Підсумок заявки.</param>
    /// <returns>Наступний стан; якщо перехід неможливий — поточний.</returns>
    public static RequestStatus NextStatusOf(RequestStatus current, decimal total)
        => current switch
        {
            RequestStatus.New when total > 0m => RequestStatus.Approved,
            RequestStatus.Approved => RequestStatus.Done,
            _ => current,
        };

    /// <summary>Диспетчер: перевіряє заголовок, рахує кроки й за потреби друкує звіт.</summary>
    /// <param name="requestId">Номер заявки.</param>
    /// <param name="requesterName">Ім’я замовника.</param>
    /// <param name="requesterEmail">Пошта замовника.</param>
    /// <param name="isRegular">Ознака постійного замовника.</param>
    /// <param name="lines">Рядки заявки.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <param name="couponCode">Код купона або null.</param>
    /// <param name="status">Поточний стан заявки.</param>
    /// <param name="printToConsole">Ознака друку звіту в консоль.</param>
    /// <param name="deliveryPrice">Вартість виїзду, грн.</param>
    /// <param name="newStatus">Стан заявки після розрахунку.</param>
    /// <returns>Підсумок заявки, грн.</returns>
    public static decimal Process(
        int requestId,
        string requesterName,
        string requesterEmail,
        bool isRegular,
        IReadOnlyList<WorkLine> lines,
        DateOnly createdAt,
        string? couponCode,
        RequestStatus status,
        bool printToConsole,
        decimal deliveryPrice,
        out RequestStatus newStatus)
    {
        RequestGuards.EnsureHeaderValid(requestId, requesterName, requesterEmail);
        decimal total = SubtotalOf(lines);
        total -= ClientDiscountOf(total, isRegular);
        total -= VolumeDiscountOf(total);
        total -= CouponDiscountOf(total, couponCode);
        total -= SundayDiscountOf(total, createdAt);
        total += DeliveryFeeOf(total, deliveryPrice);
        total = status == RequestStatus.Cancelled
            ? 0m
            : Math.Round(Math.Max(total, 0m), 2);
        newStatus = NextStatusOf(status, total);

        if (printToConsole)
        {
            Console.WriteLine("Заявка № " + requestId);
            Console.WriteLine("Замовник: " + requesterName);
            foreach (WorkLine line in lines)
            {
                Console.WriteLine(line.WorkCode + " x " + line.Hours);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
