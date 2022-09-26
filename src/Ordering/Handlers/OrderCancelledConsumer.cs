using MassTransit;
using Ordering.Contracts.Events;
using Serilog;

namespace Ordering.Handlers;

public class OrderCancelledConsumer
    : IConsumer<OrderCancelledEvent>
{
    public Task Consume(ConsumeContext<OrderCancelledEvent> context)
    {
        Log.Information("Order {OrderId} cancelled", context.Message.OrderId.ToString());

        return Task.CompletedTask;
    }
}