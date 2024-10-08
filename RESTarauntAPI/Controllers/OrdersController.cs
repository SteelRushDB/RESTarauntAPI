using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RESTarauntAPI.Models;
using RESTarauntAPI.Models.Models_Orders;
using RESTarauntAPI.Services;

namespace RESTarauntAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController: ControllerBase
{
    private readonly OrderService _orderService;
    private readonly UserService _userService;
    private readonly MenuService _menuService;
    
    public OrdersController(OrderService orderService, UserService userService, MenuService menuService)
    {
        _orderService = orderService;
        _userService = userService;
        _menuService = menuService;
    }

    //Create
    [Authorize]
    [HttpPost("new_order")]
    public async Task<IActionResult> CreateOrder([FromBody] NewOrderDTO newOrderDto)
    {
        if (newOrderDto.OrderDishDtos == null || newOrderDto.OrderDishDtos.Count == 0)
        {
            return BadRequest(new { message = "No dishes in the order." });
        }
        
        var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var user = await _userService.GetUserByEmailAsync(email);
        
        var order = new Order
        {
            UserId = user.Id,
            Status = "Waiting",
            SpecialRequests = newOrderDto.SpecialRequests
        };
        
        foreach (var orderDishDto in newOrderDto.OrderDishDtos)
        {
            var dish = await _orderService.FindDishByIdAsync(orderDishDto.DishId);

            if (orderDishDto.Quantity <= 0)
            {
                return BadRequest(new { message = $"{dish?.Name ?? "Unknown"} указан в недопустимом количестве" });
            }
            
            if (dish == null || dish.QuantityAvailable < orderDishDto.Quantity)
            {
                return BadRequest(new { message = $"Недостаточное количество для блюда: {dish?.Name ?? "Unknown"}" });
            }
            
            var orderDish = new OrderDish()
            {
                OrderId = order.Id,
                DishId = orderDishDto.DishId,
                Quantity = orderDishDto.Quantity,
                Price = dish.Price
            };

            dish.QuantityAvailable -= orderDishDto.Quantity; //уменьшение доступных
            await _menuService.UpdateOrAddDishToDbAsync(dish);
            
            order.OrderDishes.Add(orderDish);
            _orderService.AddOrderDishInDb(orderDish);
            
        }

        await _orderService.AddOrderInDbAsync(order);
        
        return Ok(new
        {
            message = "Order created successfully", 
            OrderId = order.Id
        });
    }
    
    //Delete == cancel
    [Authorize(Roles = "Admin, Manager, Employee")]
    [HttpPut("cancel_order/{orderId}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var orderToCancel = await _orderService.FindOrderByIdAsync(orderId);
        
        if(orderToCancel == null) return NotFound(new { message = "Order not found" });
        
        if (orderToCancel.Status != "Waiting" && orderToCancel.Status != "Processing")
        {
            return BadRequest(new { message = "Order cannot be cancelled at this stage." });
        }
        
        orderToCancel.Status = "Canceled";
        await _orderService.SaveDb();
        
        return Ok(new { message = "Order cancelled successfully." });
    }

    //Read ALL (info)
    [Authorize(Roles = "Admin, Manager, Employee")]
    [HttpGet("orders_info")]
    public async Task<IActionResult> OrderInfo()
    {
        var orders = _orderService.GetAllOrders(); 
        if (orders == null || orders.Count == 0)
        {
            return NotFound(new { message = "There was no any orders" });
        }
        
        return Ok(new
        {
            message = $"Orders found:",
            orders,
        });
    }
    
    //Read (info)
    [Authorize(Roles = "Admin, Manager, Employee")]
    [HttpGet("orders_info/{orderId}")]
    public async Task<IActionResult> OrderInfo(int orderId)
    {
        var order = await _orderService.FindOrderByIdAsync(orderId);
        if (order == null)
        {
            return NotFound(new { message = "Order not found." });
        }
        
        return Ok(new
        {
            order,
            order.OrderDishes,
            message = $"Order status:{order.Status}"
        });
    }
    
}