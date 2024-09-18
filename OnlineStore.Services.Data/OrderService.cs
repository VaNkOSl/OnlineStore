namespace OnlineStore.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.Orders;
using OnlineStore.Web.ViewModels.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

public class OrderService : IOrderService
{
    private readonly IRepository repository;

    public OrderService(IRepository _repository)
    {
        repository = _repository;
    }
    public async Task<string> CreateOrderAsync(OrderFormModel model, string userId)
    {
        if(!Guid.TryParse(userId, out Guid userGuidId))
        {
            throw new ArgumentException("Invalid user id");
        }

        var order = await repository
          .All<Order>()
          .Include(or => or.OrderItems)
          .ThenInclude(p => p.Product)
          .ThenInclude(p => p.ProductImages)
          .FirstOrDefaultAsync(o => o.UserId == userGuidId && o.OrderStatus == OrderStatus.Cart);

        if(order == null)
        {
            throw new ArgumentException("No cart found for user");
        }

        order.FirstName = model.FirstName;
        order.LastName = model.LastName;
        order.PhoneNumber = model.PhoneNumber;
        order.Email = model.Email;
        order.CreateOrderdDate = DateTime.Now;
        order.DeliveryOption = model.DeliveryOption;
        order.Adress = model.Adress;
        order.OrderStatus = OrderStatus.Completed;
        

        foreach(var item in order.OrderItems)
        {
            var product = item.Product;

            if (product.StockQuantity < item.Quantity)
            {
                throw new Exception($"Insufficient stock for product {product.Name}");
            }

            product.StockQuantity -= item.Quantity;

            if (product.StockQuantity <= 0)
            {
                product.IsAvaible = false;
            }

            foreach(var image in product.ProductImages)
            {
                order.ProductImages.Add(image);
            }
        }

        await repository.SaveChangesAsync();
        return order.Id.ToString();
    }

    public async Task<List<OrderViewModel>> GetOrderByUserIdAsync(string userId)
    {
        var orders = await repository
            .All<Order>()
            .Include(u => u.User)
            .Include(or => or.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.ProductImages)
            .Include(p => p.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.AvailableColors)
            .Include(p => p.OrderItems)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.AvailableSizes)
            .Where(o => o.UserId.ToString() == userId && !o.IsTaken)
            .ToListAsync();

        if (orders == null)
        {
            return null!;
        }

        return orders.Select(order => new OrderViewModel
        {
            Id = order.Id.ToString(),
            FirstName = order.FirstName,
            LastName = order.LastName,
            DeliveryOption = order.DeliveryOption,
            Email = order.Email,
            Adress = order.Adress,
            OrderStatus = order.OrderStatus,
            PhoneNumber = order.PhoneNumber,
            OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                Price = oi.Price,
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                SelectedColors = oi.SelectedColor ?? new List<string>(),
                SelectedSizes = oi.SelectedSizes ?? new List<string>(),
                ImageUrl = oi.Product.ProductImages.FirstOrDefault()?.FilePath ?? string.Empty,
                ProductImages = oi.Product.ProductImages.Select(pi => new ProductImageServiceModel
                {
                    FilePath = pi.FilePath,
                    ProductId = pi.ProductId.ToString(),
                }).ToList(),
            }).ToList(),
            TotalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity)
        }).ToList();                                                                  
    }

    public async Task<IEnumerable<OrderViewModel>> GetOrdersByProductAndSellerAsync(string sellerId)
    {
        var orders = await repository
        .All<Order>()
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .ThenInclude(p => p.ProductImages)
        .Where(o => o.OrderItems.Any(oi => oi.Product.SellerId.ToString() == sellerId 
              && o.OrderStatus != OrderStatus.Shipped && o.OrderStatus != OrderStatus.ReadyForPickup))
        .ToListAsync();

        return orders.Select(order => new OrderViewModel
        {
            Id = order.Id.ToString(),
            FirstName = order.FirstName,
            LastName = order.LastName,
            Email = order.Email,
            PhoneNumber = order.PhoneNumber,
            Adress = order.Adress,
            DeliveryOption = order.DeliveryOption,
            OrderStatus = order.OrderStatus,
            OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                Price = oi.Price,
                ProductName = oi.Product.Name,
                Quantity = oi.Quantity,
                SelectedColors = oi.SelectedColor ?? new List<string>(),
                SelectedSizes = oi.SelectedSizes ?? new List<string>(),
                ImageUrl = oi.Product.ProductImages.FirstOrDefault()?.FilePath ?? string.Empty,
            }).ToList(),
            TotalPrice = order.OrderItems.Sum(oi => oi.Price * oi.Quantity)
        }).ToList();
    }

    public async Task<bool> OrderExistsAsync(string orderId)
    {
        return await repository
            .AllReadOnly<Order>()
            .AnyAsync(o => o.Id.ToString() == orderId);
    }

    public async Task<bool> SendOrderAsync(string orderId)
    {
        var order = await repository
            .AllReadOnly<Order>()
            .FirstOrDefaultAsync(o => o.Id.ToString() == orderId);

        if (order == null)
        {
            return false;
        }

        if(order.OrderStatus == OrderStatus.Shipped)
        {
            return false;
        }

        order.OrderStatus = OrderStatus.Shipped;
        order.ShippedDate = DateTime.UtcNow;
        await repository.UpdateAsync(order);
        await repository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TakeOrderAsync(string orderId)
    {
        var order = await repository
         .AllReadOnly<Order>()
         .FirstOrDefaultAsync(o => o.Id.ToString() == orderId);

        if (order == null)
        {
            return false;
        }

        if(order.IsTaken == true)
        {
            return true;
        }

        order.IsTaken = true;
        await repository.UpdateAsync(order);
        await repository.SaveChangesAsync();
        return true;
    }
}
