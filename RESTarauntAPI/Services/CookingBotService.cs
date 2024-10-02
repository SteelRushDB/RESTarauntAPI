using RESTarauntAPI.Data;

namespace RESTarauntAPI.Services;

public class CookingBotService: BackgroundService
{
    private readonly AppDbContext _context;

    public CookingBotService(AppDbContext context)
    {
        _context = context;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CookingAsync();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Задержка перед следующей обработкой
        }
        
    }


    private async Task CookingAsync()
    {
        var orderToCook = _context.Orders.SingleOrDefault(o => o.Status == "Waiting");
        
        orderToCook.Status = "Processing";
        await _context.SaveChangesAsync();
        
        await Task.Delay(TimeSpan.FromSeconds(10)); // Имитация времени обработки заказа

        orderToCook.Status = "Ready";
        await _context.SaveChangesAsync();
    }
}
