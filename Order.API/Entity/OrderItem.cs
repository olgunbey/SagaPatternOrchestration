namespace Order.API.Entity;

public class OrderItem
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    public decimal Price { get; set; }
}