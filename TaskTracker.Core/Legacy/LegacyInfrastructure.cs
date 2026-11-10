// Навмисно «поганий» успадкований код для аудиту SOLID; теку Legacy буде видалено в кроці 6.
#pragma warning disable
namespace TaskTracker.Core.Legacy;

public class FileWorkOrderStore : IWorkOrderStore
{
    public void SaveToFile(LegacyWorkOrder order, string path)
    {
        File.WriteAllText(path, order.Id.ToString());
    }

    public void Add(LegacyWorkOrder order) => throw new NotSupportedException();

    public LegacyWorkOrder GetById(int id) => throw new NotSupportedException();

    public void SendEmail(string to, string body) => throw new NotSupportedException();

    public string ExportCsv() => throw new NotSupportedException();

    public void Backup(string folder) => throw new NotSupportedException();

    public void PrintInvoice(LegacyWorkOrder order) => throw new NotSupportedException();
}

public class SmtpMailer
{
    public void Send(string to, string subject, string body)
    {
        // Заглушка: у навчальному проєкті лист не надсилається.
    }
}
