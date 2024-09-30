namespace RESTarauntAPI.Models.Models_Orders;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; } // идентификатор пользователя, разместившего заказ
    public string Status { get; set; }
    public string SpecialRequests { get; set; } // дополнительные запросы или инструкции от пользователя
    
    public DateTime CreatedAt { get;}
    public DateTime UpdatedAt { get; set; }
    
    
    

    public ICollection<OrderDish> OrderDishes { get; set; }  // Связь с таблицей OrderDish
    
    
    public Order()
    {
        CreatedAt = DateTime.Now;
        UpdatedAt = DateTime.Now;
    }
    
    
    
    // Order -> several OrderDish's -> of Dish's
}