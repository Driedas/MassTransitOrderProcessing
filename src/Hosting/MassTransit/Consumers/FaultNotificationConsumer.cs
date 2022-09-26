using MassTransit;
using Serilog;

namespace Hosting.MassTransit.Consumers;

public class FaultNotificationConsumer
    : IConsumer<Fault>
{
    public Task Consume(ConsumeContext<Fault> context)
    {
        Log.Error("message retries failed.");
        // Send message to slack or something
        
        return Task.CompletedTask;
    }
}

public class FaultNotificationConsumerDefinition
    : ConsumerDefinition<FaultNotificationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<FaultNotificationConsumer> consumerConfigurator)
    {
        Endpoint(endpoint => endpoint.Name = "fault-notification");
    }
}