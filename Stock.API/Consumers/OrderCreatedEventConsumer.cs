using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.OrderEvents;
using Shared.StockEvents;
using Stock.API.Mongo;

namespace Stock.API.Consumers;

public class OrderCreatedEventConsumer:IConsumer<OrderCreatedEvent>
{
    private readonly MongoService _mongoService;
    public OrderCreatedEventConsumer(MongoService mongoService)
    {
        _mongoService = mongoService;   
    }
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
       var stockControls= context.Message.OrderItemMessages.Select(o => new StockControl()
        {
            ProductId = o.ProductId,
            Count = o.Count,
        }).ToList();
       bool control=await _mongoService.StockControl(stockControls);

       var sendEndPoint =await context.GetSendEndpoint(new Uri($"queue:{RabbitMqSettings.StateMachine}"));
       if (control)
       {
           StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
           {
               OrderItemMessages = context.Message.OrderItemMessages
           };
          await sendEndPoint.Send(stockReservedEvent);
       }
       else
       {
           StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
           {
               Message = "Stock Not Reserved"
           };
           await sendEndPoint.Send(stockNotReservedEvent);
       }
       
    }
}