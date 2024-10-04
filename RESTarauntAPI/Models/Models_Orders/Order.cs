namespace RESTarauntAPI.Models.Models_Orders;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; } // идентификатор пользователя, разместившего заказ
    public string Status { get; set; }
    public string SpecialRequests { get; set; } // дополнительные запросы или инструкции от пользователя
    
    
    public ICollection<OrderDish> OrderDishes { get; set; }  // Связь с таблицей OrderDish
    // Order -> several OrderDish's -> type of Dish's
    
    public DateTime CreatedAt { get;}
    public DateTime UpdatedAt { get; set; }
    
    
    
    public Order()
    {
        OrderDishes = new List<OrderDish>();
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
    
    
}