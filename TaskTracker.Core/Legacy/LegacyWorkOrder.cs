// Навмисно «поганий» успадкований код для аудиту SOLID; теку Legacy буде видалено в кроці 6.
#pragma warning disable
namespace TaskTracker.Core.Legacy;

public class LegacyWorkOrderLine
{
    public string WorkCode { get; set; } = "";

    public int Hours { get; set; }

    public decimal HourlyRate { get; set; }
}

public class LegacyWorkOrder
{
    public int Id { get; set; }

    public int Status { get; set; }

    public decimal Total { get; set; }

    public string Email { get; set; } = "";

    public List<LegacyWorkOrderLine> Lines { get; set; } = new();

    public virtual void AddLine(LegacyWorkOrderLine line) => Lines.Add(line);
}

public class ArchivedWorkOrder : LegacyWorkOrder
{
    public ArchivedWorkOrder(int id) => Id = id;

    public override void AddLine(LegacyWorkOrderLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}
