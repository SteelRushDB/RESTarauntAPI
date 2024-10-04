using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RESTarauntAPI.Models.Models_Orders;
using RESTarauntAPI.Services;

namespace RESTarauntAPI.Controllers;



[ApiController]
[Route("api/[controller]")]
public class MenuController: ControllerBase
{
    private readonly OrderService _orderService;
    private readonly UserService _userService;
    private readonly MenuService _menuService;

    public MenuController(OrderService orderService, UserService userService, MenuService menuService)
    {
        _orderService = orderService;
        _userService = userService;
        _menuService = menuService;
    }

    //TODO принимает значения -1

    //Create
    [Authorize(Roles = "Admin, Manager")]
    [HttpPost("new_dish")]
    public async Task<IActionResult> CreateDish([FromBody] NewDishDTO newDishDto)
    {
        if (newDishDto.QuantityAvailable < 0)
        {
            return BadRequest(new {message = "Quantity available can't be below 0"});
        }
        
        if (newDishDto.Price <= 0)
        {
            return BadRequest(new {message = "Price can't be 0 or below"});
        }
        
        var dish = await _menuService.CreateDish(newDishDto);
        
        return Ok(new
        {
            message = "Dish was added"
        });
    }
    
    //Read
    [HttpGet("menu")]
    public async Task<IActionResult> GetMenu()
    {
        List<Dish> dishes = _menuService.GetAllDishesList();
        
        if (dishes == null || dishes.Count == 0)
        {
            return NotFound(new { message = "Menu is empty." });
        }
        
        
        if (User.IsInRole("Employee") || User.IsInRole("Manager") || User.IsInRole("Admin"))
        {
            return Ok(new
            {
                message = "Full menu (including unavailable dishes):",
                dishes, // возвращаем все блюда
            });
        }
        
        List<Dish> availableDishes = new List<Dish>();
        foreach (var dish in dishes)
        {
            if (dish.QuantityAvailable > 0) availableDishes.Add(dish);
        }
        
        if (availableDishes.Count == 0)
        {
            return NotFound(new { message = "There is no any dish available." });
        }
        
        return Ok(new
        {
            message = "Available menu:",
            availableDishes,
        });
    }
    
    
    //Update
    [Authorize(Roles = "Admin, Manager")]
    [HttpPut("update_dish/{dishId}")]
    public async Task<IActionResult> UpdateDish(int dishId, [FromBody] UpdateDishDTO updateDishDto)
    {
        Dish dish = _menuService.FindDishById(dishId);
        if (dish == null)
        {
            return NotFound(new { message = "Dish not found." });
        }

        if (updateDishDto.Price.HasValue)
        {
            if (updateDishDto.Price <= 0) return BadRequest(new { message = "Price can't be 0 or below" });
            dish.Price = updateDishDto.Price.Value;
        }

        if (updateDishDto.QuantityAvailable.HasValue)
        {
            if (updateDishDto.QuantityAvailable < 0) return BadRequest(new { message = "Quantity available can't be below 0" });
            dish.QuantityAvailable = updateDishDto.QuantityAvailable.Value;
        }
        
        if (updateDishDto.Description != null)
        {
            dish.Description = updateDishDto.Description;
        }
        
        await _menuService.UpdateOrAddDishToDbAsync(dish);
        
        return Ok(new
        {
            message = "Dish was updated:",
            dish
        });
    }
    
    
    //Delete
    [Authorize(Roles = "Admin, Manager")]
    [HttpPut("delete_dish/{dishId}")]
    public async Task<IActionResult> DeleteDish(int dishId)
    {
        Dish dish = _menuService.FindDishById(dishId);
        if (dish == null)
        {
            return NotFound(new { message = "Dish not found." });
        }
        
        await _menuService.DeleteDish(dishId);
        
        return Ok(new
        {
            message = $"Dish {dishId} was deleted."
        });
    }
}