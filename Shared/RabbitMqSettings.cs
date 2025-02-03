namespace Shared;

public static class RabbitMqSettings
{
    public const string StateMachine = $"state-machine-queue";
    public const string Stock_OrderCreatedEventQueue = $"stock-ordercreated-event-queue";
    public const string Order_PaymentCompletedEventQueue = $"order-paymentcompleted-event-queue";
    public const string Stock_PaymentCompletedEventQueue = $"stock-paymentcompleted-event-queue";
    public const string Order_OrderFailedEventQueue = $"order-orderfailed-event-queue";
    public const string Payment_PaymentStartedEventQueue = $"payment-payment-started-event-queue";
}