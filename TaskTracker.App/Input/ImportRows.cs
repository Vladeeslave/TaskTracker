namespace TaskTracker.App.Input;

/// <summary>Допоміжний імпорт рядків годин з тексту.</summary>
public static class ImportRows
{
    /// <summary>Рахує коректні значення годин через TryParse, без винятків.</summary>
    /// <param name="rows">Рядки з кількістю годин.</param>
    /// <returns>Кількість коректних значень.</returns>
    public static int CountValidHours(string[] rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(row, out int hours) && hours > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
