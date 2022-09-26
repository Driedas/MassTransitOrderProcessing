using MassTransit;
using MassTransit.Configuration;
using Persistence;

namespace Hosting.MassTransit.ConsumerDefinitions;

public class OurDefaultConsumerDefinition<TConsumer>
    : DefaultConsumerDefinition<TConsumer>
    where TConsumer : class, IConsumer
{
    private readonly IServiceProvider _serviceProvider;

    public OurDefaultConsumerDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<TConsumer> consumerConfigurator)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);

        endpointConfigurator.UseEntityFrameworkOutbox<OrderingContext>(_serviceProvider);
    }
}
