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
        _tmpDiscount = DiscountOf(_tmpSum, req.Client);
        decimal ship = ShippingOf(_tmpSum - _tmpDiscount);
        decimal total = _tmpSum - _tmpDiscount + ship;
 
        string txt = "Документ #" + req.OrderId + "\n";
        txt += "Клієнт: " + req.Client.Name + "\n";
        for (int i = 0; i < lines.Count; i++)
        {
            txt += lines[i].JobCode + " x" + lines[i].Hours + " = " +
                (lines[i].Hours * lines[i].HourlyRate)
                    .ToString("0.00") + " " + req.Currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + req.Currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + req.Currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + req.Currency + "\n";
 
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
 
    private static decimal DiscountOf(decimal subtotal, Client client)
    {
        decimal discount = 0m;
        if (client.Kind == ClientKind.Vip)
        {
            discount = subtotal * PricingRules.VipRate;
            if (discount > PricingRules.MaxDiscount) discount = PricingRules.MaxDiscount;
        }
        else if (client.Kind == ClientKind.Staff)
        {
            discount = subtotal * PricingRules.StaffRate;
            if (discount > PricingRules.MaxDiscount) discount = PricingRules.MaxDiscount;
        }
        else if (client.IsLoyal)
        {
            discount = subtotal * PricingRules.LoyalRate;
            if (discount > PricingRules.MaxDiscount) discount = PricingRules.MaxDiscount;
        }
        return discount;
    }
 
    private static decimal ShippingOf(decimal payable)
    {
        decimal ship = 0m;
        if (payable < PricingRules.FreeShipFrom) ship = PricingRules.ShipCost;
        return ship;
    }
 
    public decimal Preview(Client client, IReadOnlyList<JobLine> lines)
    {
        decimal subtotal = Subtotal(lines);
        decimal discount = DiscountOf(subtotal, client);
        decimal payable = subtotal - discount;
        return payable + ShippingOf(payable);
    }
 
    public string DescribeClient(Client c)
    {
        string s = c.Name.Trim().ToUpper();
        if (c.Kind == ClientKind.Vip) s += " [VIP]";
        if (c.IsLoyal) s += " [ЛОЯЛЬНИЙ]";
        s += " <" + c.Email.ToLower() + ">";
        int years = DateTime.Now.Year - c.SinceUtc.Year;
        s += " стаж " + years;
        return s;
    }
 
    public string DumpLog()
    {
        string r = "";
        foreach (var l in _log) r += l + "\n";
        return r;
    }
}
