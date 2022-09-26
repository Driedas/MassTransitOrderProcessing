using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Persistence;
using Serilog;

namespace Ordering.Handlers;

public class PlaceOrderConsumer
    : IConsumer<PlaceOrderCommand>
{
    private readonly ILogger<PlaceOrderConsumer> _log;
    private readonly OrderingContext _dataContext;

    public PlaceOrderConsumer(ILogger<PlaceOrderConsumer> log, OrderingContext dataContext)
    {
        _log = log;
        _dataContext = dataContext;
    }

    public async Task Consume(ConsumeContext<PlaceOrderCommand> context)
    {
        Log.Logger.Information("Placing order {OrderId}...", context.Message.Id.ToString());

        var order = new Order() { ExternalId = context.Message.Id, DateCreated = DateTime.UtcNow };
        _dataContext.Set<Order>()
            .Add(order);
        await _dataContext.SaveChangesAsync();
        
        await context.Publish(new OrderPlacedEvent(){ OrderId = context.Message.Id });
    }
}
