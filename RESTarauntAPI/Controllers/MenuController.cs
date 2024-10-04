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



    [Authorize(Roles = "Admin, Manager")]
    [HttpPost("new_dish")]
    public async Task<IActionResult> CreateDish([FromBody] NewDishDTO newDishDto)
    {
        var dish = await _menuService.CreateDish(newDishDto);
        await _menuService.UpdateOrAddDishToDbAsync(dish);
        return Ok(new
        {
            message = "Dish was added"
        });
    }
    
    
    [HttpGet("menu")]
    public async Task<IActionResult> GetMenu()
    {
        List<Dish> dishes = _menuService.GetAllDishesList();
        if (dishes == null || dishes.Count == 0)
        {
            return NotFound(new { message = "Menu is empty." });
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
            dish.Price = updateDishDto.Price.Value;
        }

        if (updateDishDto.QuantityAvailable.HasValue)
        {
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
}