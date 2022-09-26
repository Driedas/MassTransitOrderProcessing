using Contracts;

namespace Ordering.Contracts.Events;

public record OrderPlacedEvent
    : IEvent
{
    public Guid OrderId { get; init; }
}