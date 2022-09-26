using System.Reflection;
using Contracts;
using Hosting.MassTransit.ConsumerDefinitions;
using Hosting.MassTransit.Consumers;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;

namespace Hosting.MassTransit;

public static class ConfigurationExtensions
{
    public static IHostBuilder AddMassTransitConsumerEndpoint(this IHostBuilder builder,
        string serviceName,
        params Assembly[] handlerAssemblies)
    {
        builder.ConfigureServices(services =>
            services.AddMassTransit(mt =>
            {
                mt.SetEndpointNameFormatter(
                    new KebabCaseEndpointNameFormatter(prefix: serviceName, includeNamespace: false));

                mt.AddEntityFrameworkOutbox<OrderingContext>(outbox =>
                {
                    outbox.UseSqlServer()
                        .UseBusOutbox();

                    OrderingContext.ModelBuilding += ((sender, modelBuilder) =>
                    {
                        modelBuilder.AddInboxStateEntity(e => e.ToTable("InboxState", "MassTransit"));
                        modelBuilder.AddOutboxStateEntity(e => e.ToTable("OutboxState", "MassTransit"));
                        modelBuilder.AddOutboxMessageEntity(e => e.ToTable("OutboxMessage", "MassTransit"));
                    });
                });

                services.AddSingleton(typeof(IConsumerDefinition<>), typeof(OurDefaultConsumerDefinition<>));

                mt.AddConsumers(handlerAssemblies);
                mt.AddConsumer<FaultNotificationConsumer, FaultNotificationConsumerDefinition>();
                mt.AddSagas(handlerAssemblies);

                mt.UsingRabbitMq((context, configuration) =>
                {
                    configuration.UseMessageRetry(retry =>
                    {
                        retry.Ignore<ApplicationException>();
                        retry.Intervals(TimeSpan.Zero, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5));
                    });

                    configuration.UseDelayedRedelivery(redelivery =>
                    {
                        redelivery.Ignore<ApplicationException>();
                        redelivery.Intervals(TimeSpan.FromHours(1));
                    });

                    configuration.UseInstrumentation();

                    configuration.ConfigureEndpoints(context);
                });
            }));

        return builder;
    }

    public static IHostBuilder AddMassTransitSenderEndpoint(this IHostBuilder builder,
        params Assembly[] contractAssemblies)
    {
        foreach (Assembly ass in contractAssemblies)
        {
            CommandConventions.Apply(ass);
        }

        builder.ConfigureServices(services =>
            services.AddMassTransit(mt =>
            {
                mt.AddEntityFrameworkOutbox<OrderingContext>(outbox =>
                {
                    outbox.UseSqlServer();
                    outbox.DisableInboxCleanupService();
                    outbox.UseBusOutbox();
                });

                mt.UsingRabbitMq();
            }));

        return builder;
    }
}