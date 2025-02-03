using Shared.Message;

namespace Shared.OrderEvents;

public class OrderStartedEvent
{
    public Guid CorrelationId { get; set; }
    public int OrderId { get; set; }
    public int BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderItemMessage> OrderItemMessages { get; set; }
}