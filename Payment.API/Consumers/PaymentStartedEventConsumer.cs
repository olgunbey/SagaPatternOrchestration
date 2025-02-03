using MassTransit;
using Shared;
using Shared.OrderEvents;
using Shared.PaymentEvents;

namespace Payment.API.Consumers;

public class PaymentStartedEventConsumer:IConsumer<PaymentStartedEvent>
{
    public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
    {
        if (true)
        {
            PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId)
            {
                OrderItemMessages = context.Message.OrderItemMessages,
                OrderId = context.Message.OrderId
            };
            
           await context.Send(new Uri($"queue:{RabbitMqSettings.StateMachine}"),paymentCompletedEvent);
        }
        else
        {
            PaymentFailedEvent paymentFailedEvent = new(context.Message.CorrelationId)
            {
                Message = "Yetersiz bakiye...",
                OrderItemMessages = context.Message.OrderItemMessages,
            };
            await context.Send(new Uri($"queue:{RabbitMqSettings.StateMachine}"),paymentFailedEvent);

        }
    }
}