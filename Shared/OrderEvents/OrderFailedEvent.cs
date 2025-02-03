using MassTransit;

namespace Shared.OrderEvents;

public class OrderFailedEvent : CorrelatedBy<Guid>
{
    public OrderFailedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public int OrderId { get; set; }
    public string Message { get; set; }

    public Guid CorrelationId { get; }
}