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
 
    public string Handle(int orderId, int clientId,
        string clientName, string? clientMail,
        ClientKind clientKind, int clientDone,
        List<JobLine>? lines, OrderState state,
        string currency, DateTime createdAt, bool sendMail)
    {
        string? error = Validate(lines, state, clientMail);
        if (error != null)
        {
            return error;
        }
 
        _log.Add("ok " + orderId);
        List<JobLine> validLines = lines!;
 
        _tmpSum = 0m;
        for (int i = 0; i < validLines.Count; i++)
        {
            _tmpSum += validLines[i].Hours * validLines[i].HourlyRate;
        }
 
        _tmpDiscount = 0m;
        if (clientKind == ClientKind.Vip)
        {
            _tmpDiscount = _tmpSum * PricingRules.VipRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (clientKind == ClientKind.Staff)
        {
            _tmpDiscount = _tmpSum * PricingRules.StaffRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
        else if (clientDone > PricingRules.LoyalOrders)
        {
            _tmpDiscount = _tmpSum * PricingRules.LoyalRate;
            if (_tmpDiscount > PricingRules.MaxDiscount) _tmpDiscount = PricingRules.MaxDiscount;
        }
 
        decimal ship = 0m;
        if (_tmpSum - _tmpDiscount < PricingRules.FreeShipFrom) ship = PricingRules.ShipCost;
        decimal total = _tmpSum - _tmpDiscount + ship;
 
        string txt = "Документ #" + orderId + "\n";
        txt += "Клієнт: " + clientName + "\n";
        for (int i = 0; i < validLines.Count; i++)
        {
            txt += validLines[i].JobCode + " x" + validLines[i].Hours + " = " +
                (validLines[i].Hours * validLines[i].HourlyRate)
                    .ToString("0.00") + " " + currency + "\n";
        }
        txt += "Знижка: " + _tmpDiscount.ToString("0.00") +
            " " + currency + "\n";
        txt += "Доставка: " + ship.ToString("0.00") +
            " " + currency + "\n";
        txt += "Разом: " + total.ToString("0.00") +
            " " + currency + "\n";
 
        if (sendMail)
        {
            _log.Add("mail -> " + clientMail);
        }
        return txt;
    }
 
    private static string? Validate(List<JobLine>? lines, OrderState state, string? mail)
    {
        if (lines is null)
        {
            return "ERR: null";
        }
 
        if (lines.Count == 0)
        {
            return "ERR: empty";
        }
 
        if (state is not (OrderState.New or OrderState.Paid))
        {
            return "ERR: state";
        }
 
        if (mail is null || !mail.Contains("@"))
        {
            return "ERR: mail";
        }
 
        return null;
    }
 
    public decimal Preview(ClientKind clientKind, int clientDone,
        List<JobLine> lines)
    {
        decimal s = 0m;
        foreach (var it in lines)
        {
            s += it.Hours * it.HourlyRate;
        }
        decimal d = 0m;
        if (clientKind == ClientKind.Vip)
        {
            d = s * PricingRules.VipRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (clientKind == ClientKind.Staff)
        {
            d = s * PricingRules.StaffRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        else if (clientDone > PricingRules.LoyalOrders)
        {
            d = s * PricingRules.LoyalRate;
            if (d > PricingRules.MaxDiscount) d = PricingRules.MaxDiscount;
        }
        decimal sh = 0m;
        if (s - d < PricingRules.FreeShipFrom) sh = PricingRules.ShipCost;
        return s - d + sh;
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
