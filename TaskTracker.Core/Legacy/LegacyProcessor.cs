using TaskTracker.Core.Domain;

namespace TaskTracker.Core.Legacy;

/// <summary>Успадкований метод-монстр: розрахунок підсумку заявки на роботи.</summary>
public static class LegacyProcessor
{
    /// <summary>
    /// Перевіряє вхід, рахує підсумок заявки зі знижками, змінює стан і друкує звіт.
    /// </summary>
    /// <param name="requestId">Номер заявки.</param>
    /// <param name="requesterName">Ім’я замовника.</param>
    /// <param name="requesterEmail">Пошта замовника.</param>
    /// <param name="isRegular">Ознака постійного замовника.</param>
    /// <param name="lines">Рядки заявки.</param>
    /// <param name="createdAt">Дата створення заявки.</param>
    /// <param name="couponCode">Код купона.</param>
    /// <param name="status">Поточний стан заявки текстом.</param>
    /// <param name="printToConsole">Ознака друку звіту в консоль.</param>
    /// <param name="deliveryPrice">Плата за виїзд, грн.</param>
    /// <param name="newStatus">Новий стан заявки текстом.</param>
    /// <returns>Підсумок заявки або -1, якщо вхід некоректний.</returns>
    public static decimal Process(
        int requestId,
        string requesterName,
        string requesterEmail,
        bool isRegular,
        IReadOnlyList<WorkLine> lines,
        DateOnly createdAt,
        string couponCode,
        string status,
        bool printToConsole,
        decimal deliveryPrice,
        out string newStatus)
    {
        newStatus = status;
        decimal total = 0;

        if (requestId <= 0)
        {
            Console.WriteLine("Помилка: номер заявки");
            return -1;
        }

        if (requesterName == null || requesterName == string.Empty)
        {
            Console.WriteLine("Помилка: немає замовника");
            return -1;
        }

        if (requesterEmail == null || !requesterEmail.Contains('@'))
        {
            Console.WriteLine("Помилка: пошта замовника");
            return -1;
        }

        if (lines == null || lines.Count == 0)
        {
            Console.WriteLine("Помилка: немає рядків");
            return -1;
        }

        if (status != "New"
            && status != "Approved"
            && status != "Done"
            && status != "Cancelled")
        {
            Console.WriteLine("Помилка: невідомий стан");
            return -1;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Hours <= 0)
            {
                Console.WriteLine("Помилка: кількість годин");
                return -1;
            }

            if (lines[i].HourlyRate < 0)
            {
                Console.WriteLine("Помилка: ставка");
                return -1;
            }

            decimal sum = lines[i].Hours * lines[i].HourlyRate;
            if (lines[i].Hours >= 10)
            {
                sum = sum * 0.95m;
            }

            total = total + sum;
        }

        if (isRegular)
        {
            total = total * 0.93m;
        }

        if (total > 5000)
        {
            total = total * 0.9m;
        }
        else if (total > 2000)
        {
            total = total * 0.95m;
        }

        if (couponCode == "SALE10")
        {
            total = total - (total * 0.1m);
        }
        else if (couponCode == "MINUS200" && total > 1000)
        {
            total = total - 200;
        }

        if (createdAt.DayOfWeek == DayOfWeek.Sunday)
        {
            total = total * 0.98m;
        }

        if (total < 1000)
        {
            total = total + deliveryPrice;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = Math.Round(total, 2);

        if (status == "New" && total > 0)
        {
            newStatus = "Approved";
        }
        else if (status == "Approved")
        {
            newStatus = "Done";
        }
        else if (status == "Cancelled")
        {
            newStatus = "Cancelled";
            total = 0;
        }

        if (printToConsole)
        {
            Console.WriteLine("Заявка № " + requestId);
            Console.WriteLine("Замовник: " + requesterName);
            Console.WriteLine("Пошта: " + requesterEmail);
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i].WorkCode + " x " + lines[i].Hours);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
