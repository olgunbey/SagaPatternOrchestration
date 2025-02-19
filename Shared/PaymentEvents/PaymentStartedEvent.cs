using MassTransit;
using Shared.Message;

namespace Shared.PaymentEvents;

public class PaymentStartedEvent:CorrelatedBy<Guid>
{
    public PaymentStartedEvent(Guid correlationId)
    {
        CorrelationId = correlationId;
    }
    public Guid CorrelationId { get; }
    public decimal TotalPrice { get; set; }
    public int OrderId { get; set; }
    public List<OrderItemMessage> OrderItemMessages { get; set; }
    
}