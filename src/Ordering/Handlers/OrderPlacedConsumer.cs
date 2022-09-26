using MassTransit;
using Ordering.Contracts.Events;
using Serilog;

namespace Ordering.Handlers;

public class OrderPlacedConsumer
    : IConsumer<OrderPlacedEvent>
{
    public Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        Log.Information("Order {OrderId} placed", context.Message.OrderId.ToString());

        bool doThrow = false;
        if (doThrow)
        {
            throw new DivideByZeroException("divided");
        }
        return Task.CompletedTask;
    }
}