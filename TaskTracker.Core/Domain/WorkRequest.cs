using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskTracker.Core.Domain;

// клас
public class WorkRequest
{
    // public string prim; // примітка, поки не треба
    private readonly List<string[]> _lines = new List<string[]>();

    // конструктор
    public WorkRequest(string id, string requesterName)
    {
        Id = id; // ставимо id
        RequesterName = requesterName; // ставимо cl
        CreatedAt = DateTime.Now; // ставимо дату
    }

    public string Id { get; private set; }

    public string RequesterName { get; private set; }

    public int Status { get; private set; } // 0-нова,1-погодж,2-викон,3-скасов

    public DateTime CreatedAt { get; private set; }

    // метод додавання
    public void AddLine(string workCode, int hours, decimal hourlyRate)
    {
        string[] line = new string[3];
        line[0] = workCode;
        line[1] = hours.ToString();
        line[2] = hourlyRate.ToString();
        _lines.Add(line); // додаємо t у ln
    }

    // ProcessData
    public decimal CalculateTotal(bool isRegularRequester)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int hours = int.Parse(_lines[i][1]);
            decimal hourlyRate = decimal.Parse(_lines[i][2]);
            total = total + hours * hourlyRate; // додаємо до суми
            lineCount = lineCount + 1; // збільшуємо kolvo на одиницю
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка
        if (isRegularRequester == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = total + total * 0.2m;
        return Math.Round(total, 2); // повертаємо sum1
    }

    // міняємо статус
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false; // не можна
    }

    // перевірка
    public bool IsValid()
    {
        if (Id != null && Id != ""
            && RequesterName != null
            && RequesterName.Length > 2
            && _lines.Count > 0
            && _lines.Count < 100
            && Status >= 0
            && Status <= 3)
        {
            return true;
        }

        return false;
    }

    // звіт
    public string BuildReport()
    {
        string report = "";
        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Робота: " + _lines[i][0]
                + "; годин: " + _lines[i][1]
                + "; ставка: " + _lines[i][2]
                + "; сума: " + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2])) + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report; // повертаємо s
    }

    // пошук
    public static WorkRequest FindById(List<WorkRequest> requests, string requestId)
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
}
