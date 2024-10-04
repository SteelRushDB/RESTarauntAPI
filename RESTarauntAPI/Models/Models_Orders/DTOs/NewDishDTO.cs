namespace RESTarauntAPI.Models.Models_Orders;

public class NewDishDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityAvailable { get; set; }
}