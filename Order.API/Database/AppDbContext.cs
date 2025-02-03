using Microsoft.EntityFrameworkCore;
using Order.API.Entity;

namespace Order.API.Database;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options){ }
    public DbSet<Entity.Order> Order { get; set; }
    public DbSet<OrderItem> OrderItem { get; set; }
    
}