using MassTransit;
using Shared;
using Shared.OrderEvents;
using Shared.PaymentEvents;
using Shared.StockEvents;
using StateMachine.Service.StateInstances;

namespace StateMachine.Service.StateMachines;

public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
{
    public Event<OrderStartedEvent> OrderStartedEvent { get; set; }
    public Event<StockReservedEvent> StockReservedEvent { get; set; }
    public Event<StockNotReservedEvent> StockNotReservedEvent { get; set; }
    public Event<PaymentCompletedEvent> PaymentCompletedEvent { get; set; }
    public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }

    public State OrderCreated { get; set; }
    public State StockReserved { get; set; }
    public State StockNotReserved { get; set; }
    public State PaymentCompleted { get; set; }
    public State PaymentFailed { get; set; }

    public OrderStateMachine()
    {

        InstanceState(instance => instance.CurrentState);

        Event(() => OrderStartedEvent,
            orderStateInstance => orderStateInstance.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId)
            .SelectId(y => Guid.NewGuid())
            );


        Event(() => PaymentCompletedEvent,
            orderStateInstance =>
                orderStateInstance.CorrelateById(@event =>
                    @event.Message.CorrelationId));


        Initially(When(OrderStartedEvent)
            .Then(context =>
            {
                context.Saga.OrderId = context.Message.OrderId;
                context.Saga.BuyerId = context.Message.BuyerId;
                context.Saga.TotalPrice = context.Message.TotalPrice;
                context.Saga.CreatedDate = DateTime.UtcNow;
            }).TransitionTo(OrderCreated)
            .Send(new Uri($"queue:{RabbitMqSettings.Stock_OrderCreatedEventQueue}"),
                context => new OrderCreatedEvent(context.Saga.CorrelationId)
                {
                    OrderItemMessages = context.Message.OrderItemMessages
                })
        );

        During(OrderCreated,
            When(StockReservedEvent)
                .TransitionTo(StockReserved)
                .Send(new Uri($"queue:{RabbitMqSettings.Payment_PaymentStartedEventQueue}"),
                    context => new PaymentStartedEvent(context.Saga.CorrelationId)
                    {
                        TotalPrice = context.Saga.TotalPrice,
                        OrderItemMessages = context.Message.OrderItemMessages,
                        OrderId = context.Saga.OrderId,
                    }),
            When(StockNotReservedEvent)
                .TransitionTo(StockNotReserved)
                .Send(new Uri($"queue:{RabbitMqSettings.Order_OrderFailedEventQueue}"),
                    context => new OrderFailedEvent(context.Saga.CorrelationId)
                    {
                        OrderId = context.Saga.OrderId,
                        Message = context.Message.Message
                    }
                )
        );

        During(StockReserved,
            When(PaymentCompletedEvent)
                .TransitionTo(PaymentCompleted)
                .Publish(
                    context => new PaymentCompletedEvent(context.Saga.CorrelationId)
                    {
                        OrderId = context.Saga.OrderId,
                        OrderItemMessages = context.Message.OrderItemMessages
                    })
                .Finalize(),
            When(PaymentFailedEvent)
                .TransitionTo(PaymentFailed)
                .Send(new Uri($"queue:{RabbitMqSettings.Order_OrderFailedEventQueue}"),
                    context => new OrderFailedEvent(context.Saga.CorrelationId)
                    {
                        OrderId = context.Saga.OrderId
                    })
        );
        SetCompletedWhenFinalized();
    }
}