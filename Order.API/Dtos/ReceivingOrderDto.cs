using Order.API.Entity;

namespace Order.API.Dtos;

public class ReceivingOrderDto
{
    public int BuyerId { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}