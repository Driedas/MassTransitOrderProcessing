namespace Payments.Contracts.Events;

public record PaymentInstructionsGeneratedEvent
{
    public Guid OrderId { get; init; }
}