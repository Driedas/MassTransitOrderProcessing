using Hosting;
using Hosting.Logging;
using Hosting.MassTransit;
using Hosting.OpenTelemetry;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Configure()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Host.AddMassTransitSenderEndpoint(
    typeof(Ordering.Contracts.ServiceEndpoint).Assembly,
    typeof(Payments.Contracts.ServiceEndpoint).Assembly);
builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddOpenTelemetry("Ordering.Api");
builder.Services.AddControllers();

builder.Services.AddDbContext<OrderingContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Ordering"));
});

var app = builder.Build();
app.MapControllers();

await app.RunAsync();