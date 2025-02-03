using MassTransit;
using Shared.PaymentEvents;
using Stock.API.Mongo;

namespace Stock.API.Consumers;

public class StockPaymentCompletedEventConsumer(MongoService mongoService):IConsumer<PaymentCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var stockControls = context.Message.OrderItemMessages.Select(y=> new StockControl()
        {
            Count = y.Count,
            ProductId = y.ProductId,
        }).ToList();
        await mongoService.InventoryReductionAsync(stockControls);
    }
}