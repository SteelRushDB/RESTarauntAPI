using RESTarauntAPI.Data;

namespace RESTarauntAPI.Services;

public class CookingBotService: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CookingBotService> _logger;

    public CookingBotService(IServiceScopeFactory scopeFactory, ILogger<CookingBotService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("CookingBotService is running...");
            await CookingAsync();
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Задержка перед следующей обработкой
        }
        
    }


    private async Task CookingAsync()
    {
        using (var scope = _scopeFactory.CreateScope()) // Создаем новый scope
        {
            var _context =
                scope.ServiceProvider.GetRequiredService<AppDbContext>(); // Получаем AppDbContext через scope

            var orderToCook = _context.Orders.FirstOrDefault(o => o.Status == "Waiting");


            if (orderToCook != null)
            {
                _logger.LogInformation($"Processing order {orderToCook.Id}");

                orderToCook.Status = "Processing";
                await _context.SaveChangesAsync();

                await Task.Delay(TimeSpan.FromSeconds(60)); // Имитация времени обработки заказа

                orderToCook.Status = "Ready";
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order {orderToCook.Id} is ready.");
            }
            else
            {
                _logger.LogInformation("No orders waiting for processing.");
            }
        }
    }
}
