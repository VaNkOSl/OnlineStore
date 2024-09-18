using OnlineStore.Web.ViewModels.Orders;

namespace OnlineStore.Services.Data.Contacts;
public interface IOrderService
{
    Task<string> CreateOrderAsync(OrderFormModel model,string userId);
    Task<List<OrderViewModel>> GetOrderByUserIdAsync(string userId);
    Task<IEnumerable<OrderViewModel>> GetOrdersByProductAndSellerAsync(string sellerId);
    Task<bool> SendOrderAsync(string orderId);
    Task<bool> TakeOrderAsync(string orderId);
    Task<bool> OrderExistsAsync(string orderId);
}
