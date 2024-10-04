namespace RESTarauntAPI.Models.Models_Orders;

public class UpdateDishDTO
{
    public int? Price { get; set; }
    public int? QuantityAvailable { get; set; }
    public string? Description { get; set; }
}