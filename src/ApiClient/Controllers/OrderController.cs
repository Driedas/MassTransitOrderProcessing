using System.Transactions;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.AspNetCore.Mvc;
using Ordering;
using Ordering.Contracts.Commands;
using Ordering.Contracts.Events;
using Persistence;

namespace ApiClient.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController
    : ControllerBase
{
    private readonly OrderingContext _dataContext;
    private readonly IPublishEndpoint _publish;
    private readonly IBus _send;
    private readonly ISendEndpointProvider _sendProvider;

    public OrderController(OrderingContext dataContext, IPublishEndpoint publish, IBus send, ISendEndpointProvider sendProvider)
    {
        _dataContext = dataContext;
        _publish = publish;
        _send = send;
        _sendProvider = sendProvider;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok();
    }
    
    [HttpPost]
    [Route("publish")]
    public async Task<IActionResult> Publish()
    {
        using (var transaction = await _dataContext.Database.BeginTransactionAsync())
        {
            var order = new Order() { DateCreated = DateTime.UtcNow, ExternalId = Guid.NewGuid() };
            await _dataContext.AddAsync(order);
            await _dataContext.SaveChangesAsync();

            await _publish.Publish<OrderPlacedEvent>(new OrderPlacedEvent() { OrderId = order.ExternalId });

            await _dataContext.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return Ok();
        }
    }

    [HttpPost]
    [Route("send")]
    public async Task<IActionResult> Send()
    {
        await _sendProvider.Send(new PlaceOrderCommand());

        var changes = _dataContext.Set<OutboxMessage>()
            .ToArray();
        await _dataContext.SaveChangesAsync();

        return Ok();
    }
}