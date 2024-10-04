namespace RESTarauntAPI.Models.Models_Orders;

public class NewOrderDTO
{
    public string SpecialRequests { get; set; } // дополнительные запросы или инструкции от пользователя
    public List<OrderDishDTO> OrderDishDtos { get; set; }
}


public class OrderDishDTO
{
    public int DishId { get; set; }
    public int Quantity{ get; set; }
}