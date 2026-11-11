using System;
using System.Collections.Generic;
 
namespace TaskTracker.Legacy;
 
public class JobLine
{
    public string JobCode = "";
    public int Hours;
    public decimal HourlyRate;
    public string Category = "";
}
 
public class LegacyWorkOrderProcessor
{
    private readonly DiscountPolicy _discounts = new();
    private readonly ShippingPolicy _shipping = new();
    private readonly ReportBuilder _report = new();
    private decimal _tmpSum;
    private decimal _tmpDiscount;
    private readonly List<string> _log = new();
 
    public string Handle(WorkOrderRequest req)
    {
        string? error = Validate(req);
        if (error != null)
        {
            return error;
        }
 
        _log.Add("ok " + req.OrderId);
        IReadOnlyList<JobLine> lines = req.Lines!;
 
        _tmpSum = Subtotal(lines);
        _tmpDiscount = _discounts.For(_tmpSum, req.Client);
        decimal ship = _shipping.For(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;
 
        string txt = _report.Build(req, _tmpDiscount, ship, total);
 
        if (req.SendMail)
        {
            _log.Add("mail -> " + req.Client.Email);
        }
        return txt;
    }
 
    private static string? Validate(WorkOrderRequest req)
    {
        if (req.Lines is null)
        {
            return "ERR: null";
        }
 
        if (req.Lines.Count == 0)
        {
            return "ERR: empty";
        }
 
        if (req.State is not (OrderState.New or OrderState.Paid))
        {
            return "ERR: state";
        }
 
        if (!req.Client.HasValidEmail)
        {
            return "ERR: mail";
        }
 
        return null;
    }
 
    private static decimal Subtotal(IReadOnlyList<JobLine> lines)
    {
        decimal sum = 0m;
        foreach (var line in lines)
        {
            sum += line.Hours * line.HourlyRate;
        }
        return sum;
    }
 
    public decimal Preview(Client client, IReadOnlyList<JobLine> lines)
    {
        decimal subtotal = Subtotal(lines);
        decimal discount = _discounts.For(subtotal, client);
        decimal payable = subtotal - discount;
        return payable + _shipping.For(payable);
    }
 
    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
