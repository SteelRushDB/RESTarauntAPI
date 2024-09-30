namespace RESTarauntAPI.Models.Models_Orders;

public class OrderDish
{
    public int Id { get; set; }
    public int OrderId { get; set; } //ID ассоциированного ордера
    public int DishId { get; set; } //идентификатор связанного пункта меню.
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    
    
    public Order Order { get; set; }
    public Dish Dish { get; set; }
}