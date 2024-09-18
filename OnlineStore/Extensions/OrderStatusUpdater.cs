namespace OnlineStore.Extensions;

using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Data.Models;
using Microsoft.EntityFrameworkCore;

public class OrderStatusUpdater : IHostedService, IDisposable
{
    private Timer timer;
    private readonly IServiceProvider serviceProvider;

    public OrderStatusUpdater(IServiceProvider _serviceProvider)
    {
        serviceProvider = _serviceProvider; 
    }

    private async void UpdateOrderStatuses(object state)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRepository>();

            var shippedOrders = await repository
               .All<Order>()
               .Where(o => o.OrderStatus == OrderStatus.Shipped && o.ShippedDate.Value.AddSeconds(20) <= DateTime.UtcNow)
               .ToListAsync();

            foreach (var order in shippedOrders)
            {
                order.OrderStatus = OrderStatus.ReadyForPickup;
                await repository.UpdateAsync<Order>(order);
            }

            await repository.SaveChangesAsync();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        timer = new Timer(UpdateOrderStatuses, null, TimeSpan.Zero, TimeSpan.FromSeconds(20)); 
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}
