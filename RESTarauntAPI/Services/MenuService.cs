using RESTarauntAPI.Data;
using RESTarauntAPI.Models.Models_Orders;

namespace RESTarauntAPI.Services;

public class MenuService
{
    private readonly AppDbContext _context;
    
    public MenuService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Dish> UpdateOrAddDishToDbAsync(Dish dish)
    {
        var existingDish = FindDishById(dish.Id);
        if (existingDish == null) 
        {
            _context.Dishes.Add(dish); //add a new one
        }
        
        else
        {
            existingDish.Name = dish.Name;
            existingDish.Description = dish.Description;
            existingDish.Price = dish.Price;
            existingDish.QuantityAvailable = dish.QuantityAvailable;
        }
        
        await _context.SaveChangesAsync(); 
        return dish;
    }

    public Dish FindDishById(int id)
    {
        return _context.Dishes.Find(id);
    }
    
    public List<Dish> GetAllDishesList()
    {
        return _context.Dishes.ToList();
    }

    public async Task<Dish> CreateDish(NewDishDTO dto)
    {
        Dish dish = new Dish
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            QuantityAvailable = dto.QuantityAvailable
        };
        
        _context.Dishes.Add(dish);
        await _context.SaveChangesAsync(); 
        
        return dish;
    }

    public async Task DeleteDish(int dishId)
    {
        Dish dish = _context.Dishes.Find(dishId);
        _context.Dishes.Remove(dish);
        
        await _context.SaveChangesAsync(); 
    }
}