using MassTransit;
using Shared.Message;

namespace Shared.PaymentEvents;

public class PaymentFailedEvent
{

    public string Message { get; set; }
    public List<OrderItemMessage> OrderItemMessages { get; set; }
}