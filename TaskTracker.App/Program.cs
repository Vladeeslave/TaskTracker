using System.Text;
using TaskTracker.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;
var request = new WorkRequest("T-1001", "Іваненко");
request.AddLine("WRK-1", 3, 250m);
request.AddLine("WRK-2", 12, 90m);
Console.WriteLine(request.CalculateTotal(false));
Console.WriteLine(request.CalculateTotal(true));
Console.WriteLine(request.IsValid());
Console.WriteLine(request.TryChangeStatus(1));
Console.Write(request.BuildReport());