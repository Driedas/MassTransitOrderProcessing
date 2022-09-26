using Contracts;

namespace Ordering.Contracts.Commands;

public record PlaceOrderCommand
    : ICommand
{
    public Guid Id { get; init; }
}