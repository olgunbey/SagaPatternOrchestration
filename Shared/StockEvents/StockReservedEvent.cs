using MassTransit;
using Shared.Message;

namespace Shared.StockEvents;

public class StockReservedEvent
{
    
    public List<OrderItemMessage> OrderItemMessages { get; set; }

}