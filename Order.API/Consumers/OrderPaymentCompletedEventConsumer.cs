using MassTransit;
using Order.API.Database;
using Order.API.Entity;
using Shared.PaymentEvents;

namespace Order.API.Consumers;

public class OrderPaymentCompletedEventConsumer(AppDbContext appDbContext):IConsumer<PaymentCompletedEvent>
{
    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var order= await appDbContext.Order.FindAsync(context.Message.OrderId);

        if (order != null)
        {
            order.OrderStatus = OrderStatus.Success;
            await appDbContext.SaveChangesAsync();
        }
    }
}