using Contracts;

namespace Ordering.Contracts.Events;

public record OrderCancelledEvent
    : IEvent
{
    public Guid OrderId { get; init; } 
}