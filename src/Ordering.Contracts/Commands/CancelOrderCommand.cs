using Contracts;

namespace Ordering.Contracts.Commands;

public class CancelOrderCommand
    : ICommand
{
    public Guid Id { get; set; }
}