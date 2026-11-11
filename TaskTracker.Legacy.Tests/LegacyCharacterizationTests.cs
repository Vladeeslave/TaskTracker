using System.Globalization;

namespace TaskTracker.Legacy.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // формат чисел залежить від культури середовища, тому фіксуємо її явно
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [Theory]
    [InlineData(null, OrderState.New, "a@b.c", "ERR: null")]
    [InlineData("empty", OrderState.New, "a@b.c", "ERR: empty")]
    [InlineData("two", OrderState.Draft, "a@b.c", "ERR: state")]
    [InlineData("two", OrderState.New, "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfLines,
        OrderState state,
        string mail,
        string expected)
    {
        List<JobLine>? lines = kindOfLines switch
        {
            "two" => TwoLines(),
            "empty" => new List<JobLine>(),
            _ => null,
        };
        var sut = new LegacyWorkOrderProcessor();

        string actual = sut.Handle(
            Request(NewClient(ClientKind.Regular, 0, mail), lines, state));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipOrder_ReturnsExactReport()
    {
        var sut = new LegacyWorkOrderProcessor();

        string report = sut.Handle(
            Request(NewClient(ClientKind.Vip, 3, "a@b.c"), TwoLines(), OrderState.New));

        string expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "WRK-1 x2 = 300.00 UAH\n" +
            "WRK-2 x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";
        Assert.Equal(expected, report);
    }

    [Theory]
    [InlineData(ClientKind.Regular, 0, 760)]
    [InlineData(ClientKind.Vip, 0, 655)]
    [InlineData(ClientKind.Staff, 0, 550)]
    [InlineData(ClientKind.Regular, 11, 725)]
    public void Preview_ByKind_ReturnsTotal(ClientKind kind, int done, decimal expected)
    {
        var sut = new LegacyWorkOrderProcessor();

        decimal total = sut.Preview(NewClient(kind, done, "a@b.c"), TwoLines());

        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_TotalExactly1000_NoShipping()
    {
        var sut = new LegacyWorkOrderProcessor();
        var lines = new List<JobLine>
        {
            new JobLine { JobCode = "WRK-3", Hours = 1, HourlyRate = 1000m },
        };

        decimal total = sut.Preview(NewClient(ClientKind.Regular, 0, "a@b.c"), lines);

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void Preview_VipDiscountOver500_IsCapped()
    {
        var sut = new LegacyWorkOrderProcessor();
        var lines = new List<JobLine>
        {
            new JobLine { JobCode = "WRK-4", Hours = 1, HourlyRate = 5000m },
        };

        decimal total = sut.Preview(NewClient(ClientKind.Vip, 0, "a@b.c"), lines);

        Assert.Equal(4500m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var client = new Client
        {
            Name = " іван ",
            Email = "IVAN@MAIL.COM",
            Kind = ClientKind.Vip,
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1),
        };

        // метод залежить від системного годинника, тому очікування обчислюємо, а не вписуємо
        int years = DateTime.Now.Year - 2020;
        string expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        string actual = new LegacyWorkOrderProcessor().DescribeClient(client);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void DumpLog_AfterHandleWithMail_ContainsOkAndMailLines()
    {
        var sut = new LegacyWorkOrderProcessor();
        sut.Handle(
            Request(
                NewClient(ClientKind.Regular, 0, "a@b.c"),
                TwoLines(),
                OrderState.New,
                sendMail: true));

        Assert.Equal("ok 1001\nmail -> a@b.c\n", sut.DumpLog());
    }

    private static List<JobLine> TwoLines() => new()
    {
        new JobLine { JobCode = "WRK-1", Hours = 2, HourlyRate = 150m },
        new JobLine { JobCode = "WRK-2", Hours = 1, HourlyRate = 400m },
    };

    private static Client NewClient(ClientKind kind, int done, string mail) => new()
    {
        Id = 7,
        Name = "Іван",
        Email = mail,
        Kind = kind,
        DoneCount = done,
    };

    private static WorkOrderRequest Request(
        Client client,
        IReadOnlyList<JobLine>? lines,
        OrderState state,
        bool sendMail = false) =>
        new(1001, client, lines, state, "UAH", sendMail);
}
