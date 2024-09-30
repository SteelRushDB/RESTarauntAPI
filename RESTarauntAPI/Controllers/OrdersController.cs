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
    private readonly IConfiguration _configuration;
    
    public OrdersController(UserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    
    
}