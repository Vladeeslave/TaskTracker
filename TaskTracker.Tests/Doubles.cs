using TaskTracker.Core.Abstractions;
using TaskTracker.Core.Domain;

namespace TaskTracker.Tests;

internal sealed class FakeWorkOrderRepository : IWorkOrderRepository
{
    private readonly Dictionary<int, WorkOrder> _items = new();

    public int AddCalls { get; private set; }

    public void Add(WorkOrder order)
    {
        _items[order.Id] = order;
        AddCalls++;
    }

    public WorkOrder? GetById(int id) => _items.GetValueOrDefault(id);

    public IReadOnlyList<WorkOrder> GetAll() => _items.Values.ToList();
}

internal sealed class StubPricingPolicy : IPricingPolicy
{
    private readonly decimal _price;

    public StubPricingPolicy(decimal price)
    {
        _price = price;
    }

    public decimal PriceOf(WorkOrderLine line) => _price;
}
