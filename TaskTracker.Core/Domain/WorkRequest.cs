namespace TaskTracker.Core.Domain;

/// <summary>
/// Заявка на виконання робіт: рядки з годинами й погодинною ставкою
/// та правила розрахунку вартості робіт.
/// </summary>
public class WorkRequest
{
    /// <summary>Стан заявки: нова.</summary>
    public const int StatusNew = 0;

    /// <summary>Стан заявки: погоджена.</summary>
    public const int StatusApproved = 1;

    /// <summary>Стан заявки: виконана.</summary>
    public const int StatusDone = 2;

    /// <summary>Стан заявки: скасована.</summary>
    public const int StatusCancelled = 3;

    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для знижки постійному замовнику.</summary>
    private const decimal RegularDiscountThreshold = 1000m;

    /// <summary>Частка знижки постійному замовнику.</summary>
    private const decimal RegularDiscountRate = 0.10m;

    /// <summary>Поріг суми для знижки на велику заявку.</summary>
    private const decimal LargeOrderThreshold = 5000m;

    /// <summary>Частка знижки на велику заявку.</summary>
    private const decimal LargeOrderDiscountRate = 0.15m;

    /// <summary>Кількість рядків, з якої діє гуртова знижка.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Сума гуртової знижки, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Максимальна кількість рядків у заявці.</summary>
    private const int MaxLineCount = 100;

    /// <summary>Мінімальна довжина імені замовника.</summary>
    private const int MinRequesterNameLength = 3;

    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Створює заявку.
    /// </summary>
    /// <param name="id">Ідентифікатор заявки.</param>
    /// <param name="requesterName">Ім’я замовника.</param>
    public WorkRequest(string id, string requesterName)
    {
        Id = id;
        RequesterName = requesterName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>Ідентифікатор заявки.</summary>
    public string Id { get; private set; }

    /// <summary>Ім’я замовника.</summary>
    public string RequesterName { get; private set; }

    /// <summary>Поточний стан заявки (див. константи Status…).</summary>
    public int Status { get; private set; }

    /// <summary>Дата й час створення заявки.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає заявку за ідентифікатором.
    /// </summary>
    /// <param name="requests">Список заявок.</param>
    /// <param name="requestId">Ідентифікатор, який шукаємо.</param>
    /// <returns>Знайдена заявка або null, якщо такої немає.</returns>
    public static WorkRequest? FindById(List<WorkRequest> requests, string requestId)
    {
        for (int i = 0; i < requests.Count; i++)
        {
            if (requests[i].Id == requestId)
            {
                return requests[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до заявки рядок із роботою.
    /// </summary>
    /// <param name="workCode">Код виду роботи.</param>
    /// <param name="hours">Кількість годин.</param>
    /// <param name="hourlyRate">Погодинна ставка, грн.</param>
    public void AddLine(string workCode, int hours, decimal hourlyRate)
    {
        string[] line = new string[3];
        line[0] = workCode;
        line[1] = hours.ToString();
        line[2] = hourlyRate.ToString();
        _lines.Add(line);
    }

    /// <summary>
    /// Обчислює вартість робіт за заявкою з урахуванням знижок і ПДВ.
    /// </summary>
    /// <param name="isRegularRequester">Ознака постійного замовника.</param>
    /// <returns>Сума до сплати, заокруглена до двох знаків.</returns>
    public decimal CalculateTotal(bool isRegularRequester)
    {
        decimal total = 0;
        int lineCount = 0;

        for (int i = 0; i < _lines.Count; i++)
        {
            int hours = int.Parse(_lines[i][1]);
            decimal hourlyRate = decimal.Parse(_lines[i][2]);
            total += hours * hourlyRate;
            lineCount++;
        }

        // Порядок нарахування: спочатку знижка за сумою (постійному замовнику
        // або на велику заявку, діє лише одна), потім гуртова знижка.
        if (isRegularRequester && total > RegularDiscountThreshold)
        {
            total *= 1m - RegularDiscountRate;
        }
        else if (total > LargeOrderThreshold)
        {
            total *= 1m - LargeOrderDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ нараховується на суму вже після всіх знижок
        total += total * VatRate;
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан заявки, якщо перехід дозволений правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан заявки.</param>
    /// <returns>true, якщо перехід виконано; false, якщо він заборонений.</returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusApproved)
        {
            Status = StatusApproved;
            return true;
        }

        if (Status == StatusApproved && newStatus == StatusDone)
        {
            Status = StatusDone;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє, чи заявка містить усі обов’язкові дані.
    /// </summary>
    /// <returns>true, якщо заявку заповнено коректно; інакше false.</returns>
    public bool IsValid()
    {
        if (Id != null && Id != string.Empty
            && RequesterName != null
            && RequesterName.Length >= MinRequesterNameLength
            && _lines.Count > 0
            && _lines.Count < MaxLineCount
            && Status >= StatusNew
            && Status <= StatusCancelled)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Формує текстовий звіт за заявкою.
    /// </summary>
    /// <returns>Звіт: рядки заявки та підсумкова сума.</returns>
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Робота: " + _lines[i][0]
                + "; годин: " + _lines[i][1]
                + "; ставка: " + _lines[i][2]
                + "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
