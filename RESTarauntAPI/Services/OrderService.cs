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
    
}