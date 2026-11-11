using TaskTracker.Core.Storage;

namespace TaskTracker.Tests;

public class FileAuditLogTests
{
    [Fact]
    public void Dispose_AfterExceptionInUsing_ReleasesFile()
    {
        string path = Path.GetTempFileName();

        Assert.Throws<InvalidOperationException>(() => WriteThenFail(path));

        string text = File.ReadAllText(path);
        File.Delete(path);
        Assert.Contains("order.paid;1", text);
        Assert.False(File.Exists(path));
    }

    private static void WriteThenFail(string path)
    {
        using var audit = new FileAuditLog(path);
        audit.Write("order.paid", 1);
        throw new InvalidOperationException("збій після запису");
    }
}
