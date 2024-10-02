using Microsoft.EntityFrameworkCore;
using RESTarauntAPI.Models;
using RESTarauntAPI.Models.Models_Orders;

namespace RESTarauntAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    
    public DbSet<Dish> Dishes { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDish> OrderDishes { get; set; } = null!;
    
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Перед сохранением изменений
        foreach (var entry in ChangeTracker.Entries<User>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now;  // Если объект изменён, обновляем время
            }
        }
        
        foreach (var entry in ChangeTracker.Entries<Order>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }
}