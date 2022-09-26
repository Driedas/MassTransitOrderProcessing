using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;

namespace Ordering.Handlers;

public class CancelOrderConsumer
    : IConsumer<CancelOrderCommand>
{
    private readonly ILogger<CancelOrderConsumer> _log;

    public CancelOrderConsumer(ILogger<CancelOrderConsumer> log)
    {
        _log = log;
    }

    public async Task Consume(ConsumeContext<CancelOrderCommand> context)
    {
        _log.LogInformation("Cancelling order {OrderId}...", context.Message.Id.ToString());

        await context.Publish<OrderCancelledEvent>(new OrderCancelledEvent(){ OrderId = context.Message.Id });
    }
}