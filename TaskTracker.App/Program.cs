using System.Globalization;
using TaskTracker.Core.Domain;

WorkTask task = new()
{
    Id = 1,
    ProjectCode = "PRJ-001",
    Title = "Підготувати звіт за спринт",
    AssigneeId = 10,
    CreatedAt = DateTimeOffset.Now,
};

task.AddEntry(new TimeEntry
{
    MemberId = 10, Description = "Збір даних", Hours = 2.5m,
});
task.AddEntry(new TimeEntry
{
    MemberId = 11, Description = "Редагування тексту", Hours = 1.25m,
});

string total = task.CalculateTotalHours()
    .ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine($"Завдання #{task.Id}");
Console.WriteLine($"Стан: {task.Status}");
Console.WriteLine($"Записів часу: {task.Entries.Count}");
Console.WriteLine($"Годин усього: {total}");
