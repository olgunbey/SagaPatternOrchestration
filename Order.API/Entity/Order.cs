namespace Order.API.Entity;

public class Order
{
    public int Id { get; set; }
    public int BuyerId { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal TotalPrice { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}

public enum OrderStatus
{
    Fail,Success,Pending
}