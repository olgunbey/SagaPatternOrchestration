using MassTransit;
using Order.API.Database;
using Order.API.Entity;
using Shared.OrderEvents;

namespace Order.API.Consumers;

public class OrderFailedEventConsumer(AppDbContext appDbContext):IConsumer<OrderFailedEvent>
{
    public async Task Consume(ConsumeContext<OrderFailedEvent> context)
    {
        var data= await appDbContext.Order.FindAsync(context.Message.OrderId);
        data!.OrderStatus = OrderStatus.Fail;
        await appDbContext.SaveChangesAsync();
    }
}