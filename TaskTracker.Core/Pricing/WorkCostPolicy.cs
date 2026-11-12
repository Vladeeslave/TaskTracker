namespace TaskTracker.Core.Pricing;

/// <summary>Вартість години роботи: надбавка за терміновість і знижка за обсяг.</summary>
public static class WorkCostPolicy
{
    /// <summary>Надбавка за терміновість: частка від ставки.</summary>
    public const decimal UrgentRate = 0.20m;

    /// <summary>Знижка за обсяг роботи: частка від ставки.</summary>
    public const decimal BulkRate = 0.10m;

    /// <summary>Кількість годин, з якої діє знижка за обсяг.</summary>
    public const int BulkFrom = 8;

    /// <summary>Найбільший обсяг у годинах, до якого діє надбавка за терміновість.</summary>
    public const int UrgentMaxHours = 24;

    /// <summary>Рахує вартість однієї години роботи з урахуванням надбавки й знижки.</summary>
    /// <param name="hourlyRate">Погодинна ставка, грн; має бути більшою за нуль.</param>
    /// <param name="hours">Обсяг роботи в годинах; не менше однієї.</param>
    /// <param name="isUrgent">Ознака термінової роботи.</param>
    /// <returns>Вартість години, грн, округлена до копійок.</returns>
    public static decimal HourCostFor(decimal hourlyRate, int hours, bool isUrgent)
    {
        if (hourlyRate <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hourlyRate),
                hourlyRate,
                "Ставка має бути більшою за нуль.");
        }

        if (hours < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(hours),
                hours,
                "Обсяг роботи має бути не менше однієї години.");
        }

        decimal factor = 1m;
        if (isUrgent && hours <= UrgentMaxHours)
        {
            factor += UrgentRate;
        }

        if (hours >= BulkFrom)
        {
            factor -= BulkRate;
        }

        return decimal.Round(hourlyRate * factor, 2);
    }
}
