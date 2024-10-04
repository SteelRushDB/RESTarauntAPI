using RESTarauntAPI.Data;
using RESTarauntAPI.Models;
using RESTarauntAPI.Models.Models_Orders;

namespace RESTarauntAPI.Services;

public class OrderService
{
    private readonly AppDbContext _context;
    
    public OrderService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task SaveDb()
    {
        await _context.SaveChangesAsync();  
    }
    
    
    public async Task<Dish> FindDishByIdAsync(int id)
    {
        return _context.Dishes.Find(id);
    }

    public async Task<Order> FindOrderByIdAsync(int id)
    {
        return _context.Orders.Find(id);
    }
    
    
    public async Task AddOrderInDbAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
    
    public void AddOrderDishInDb(OrderDish orderDish)
    {
        _context.OrderDishes.Add(orderDish);
    }
    
    public List<Order> GetAllOrders()
    {
        return _context.Orders.ToList();
    }
}