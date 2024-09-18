namespace OnlineStore.Web.ViewModels.Orders;

using OnlineStore.Data.Models.Enums;

public class OrderViewModel
{
    public OrderViewModel()
    {
        OrderItems = new HashSet<OrderItemViewModel>();
    }

    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Adress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public DeliveryOption DeliveryOption { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public ICollection<OrderItemViewModel> OrderItems { get; set; }
}
