using MassTransit;

namespace Shared.StockEvents;

public class StockNotReservedEvent
{

    public string Message { get; set; }
}