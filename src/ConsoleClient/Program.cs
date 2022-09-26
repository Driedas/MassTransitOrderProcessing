using Contracts;
using Hosting.Logging;
using MassTransit;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Ordering.Contracts;
using Ordering.Contracts.Commands;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Configure()
    .CreateLogger();

var bus = Bus.Factory.CreateUsingRabbitMq();

CommandConventions.Apply(typeof(ServiceEndpoint).Assembly);

try
{
    Sdk.CreateTracerProviderBuilder()
        .AddSource("Ordering.Client")
        .AddSource("MassTransit")
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("Ordering.Client"))
        .AddConsoleExporter()
        .AddJaegerExporter()
        .Build();

    await bus.StartAsync();
    
    while (true)
    {
        Console.WriteLine("Press any key to send place order command");
        Console.ReadKey(true);
        
        Guid orderId = Guid.NewGuid();
        await bus.Send(new PlaceOrderCommand() { Id = orderId });
        
        Log.Information("Order {OrderId} placed", orderId);
        
        // Console.WriteLine("Press any key to send cancel order command");
        // Console.ReadKey(true);
        // await bus.Send(new CancelOrderCommand() { Id = orderId });
    }
}
finally
{
    await bus.StopAsync();

    Console.WriteLine("Done.");    
}
