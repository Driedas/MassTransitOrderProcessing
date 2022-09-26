using Microsoft.EntityFrameworkCore;
using Ordering;

namespace Persistence;

public class OrderingContext
    : DbContext
{
    public OrderingContext(DbContextOptions<OrderingContext> options)
        : base(options)
    {
    }
    
    public static event EventHandler<ModelBuilder>? ModelBuilding;

    private void OnModelBuilding(ModelBuilder e)
    {
        ModelBuilding?.Invoke(null, e);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>();
        
        OnModelBuilding(modelBuilder);
    }
}