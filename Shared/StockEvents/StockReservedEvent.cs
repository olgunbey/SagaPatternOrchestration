using MassTransit;
using Shared.Message;

namespace Shared.StockEvents;

public class StockReservedEvent:CorrelatedBy<Guid>
{
    
   public StockReservedEvent(Guid id)
    {
        CorrelationId = id;
    }
    public List<OrderItemMessage> OrderItemMessages { get; set; }

    public Guid CorrelationId { get; set; }
}