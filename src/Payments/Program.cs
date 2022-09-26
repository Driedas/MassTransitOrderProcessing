using System.Reflection;
using Hosting.Logging;
using Hosting.MassTransit;
using Hosting.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Payments.Contracts;
using Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Configure()
    .CreateLogger();
    
var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddJsonFile("appsettings.json");
    })
    .AddMassTransitConsumerEndpoint(ServiceEndpoint.Name, Assembly.GetExecutingAssembly())
    .AddOpenTelemetry(ServiceEndpoint.Name)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<OrderingContext>(options =>
        {
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("Ordering"));
        });
    })    .UseConsoleLifetime()
    .Build();

await host.RunAsync();