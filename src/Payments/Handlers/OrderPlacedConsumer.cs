using MassTransit;
using Ordering.Contracts.Events;
using Payments.Contracts.Events;
using Serilog;

namespace Payments.Handlers;

public class OrderPlacedConsumer
    : IConsumer<OrderPlacedEvent>
{
    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        Log.Information("Generating payment instructions for order {OrderId}...", context.Message.OrderId.ToString());

        await context.Publish<PaymentInstructionsGeneratedEvent>(new() { OrderId = context.Message.OrderId });
    }
}