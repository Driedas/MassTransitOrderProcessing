using System.Reflection;
using Hosting.Logging;
using Hosting.MassTransit;
using Hosting.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Contracts;
using Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Configure()
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddJsonFile("appsettings.json");
    })
    .UseSerilog()
    .AddOpenTelemetry(ServiceEndpoint.Name)
    .AddMassTransitConsumerEndpoint(ServiceEndpoint.Name, Assembly.GetExecutingAssembly())
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<OrderingContext>(options =>
        {
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("Ordering"));
        });
    })
    .UseConsoleLifetime()
    .Build();

await host.RunAsync();