namespace TaskTracker.App.Input;

/// <summary>Допоміжний імпорт рядків годин з тексту.</summary>
public static class ImportRows
{
    /// <summary>АНТИПАТЕРН: try/catch замість звичайної умови.</summary>
    /// <param name="rows">Рядки з кількістю годин.</param>
    /// <returns>Кількість коректних значень.</returns>
    public static int CountValidHours(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var hours = int.Parse(row);
                if (hours > 0)
                {
                    valid++;
                }
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }

        return valid;
    }
}
