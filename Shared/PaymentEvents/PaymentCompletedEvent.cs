using MassTransit;
using Shared.Message;

namespace Shared.PaymentEvents;

public class PaymentCompletedEvent:CorrelatedBy<Guid>
{
    public PaymentCompletedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public Guid CorrelationId { get; }
    public int OrderId { get; set; }
    public List<OrderItemMessage> OrderItemMessages { get; set; }
}