namespace OnlineStore.Services.Data;

using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.Common;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;
using OnlineStore.Services.Data.Contacts;
using OnlineStore.Web.ViewModels.ShopingCart;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ShoppingCartService : IShoppingCartService
{
    private readonly IRepository repository;

    public ShoppingCartService(IRepository _repository)
    {
        repository = _repository;
    }

    public async Task AddToCartAsync(string userId, string productId, int quantity, List<string> SelectedColor, List<string> SelectedSizes)
    {
        var cart = await repository
            .AllReadOnly<CartItem>()
            .FirstOrDefaultAsync(c => c.UserId.ToString() == userId && c.ProductId.ToString() == productId);

        var order = await repository
            .AllReadOnly<Order>()
            .FirstOrDefaultAsync(o => o.UserId.ToString() == userId && o.OrderStatus == OrderStatus.Cart);

        if (order == null)
        {
            order = new Order
            {
                UserId = Guid.Parse(userId),
                OrderStatus = OrderStatus.Cart,
                CreateOrderdDate = DateTime.Now,
            };

            await repository.AddAsync(order);
        }

        if (cart == null)
        {
            cart = new CartItem
            {
                UserId = Guid.Parse(userId),
                ProductId = Guid.Parse(productId),
                Quantity = quantity,
                SelectedColor = SelectedColor,
                SelectedSizes = SelectedSizes
            };

            await repository.AddAsync(cart);
        }
        else
        {
            cart.Quantity += quantity;
            cart.SelectedColor = SelectedColor;
            cart.SelectedSizes = SelectedSizes;
        }

        var product = await repository
            .AllReadOnly<Product>()
            .FirstOrDefaultAsync(p => p.Id.ToString() == productId);

        if (product == null)
        {
            throw new ArgumentException("Product not found!");
        }

        var orderItem = await repository
            .AllReadOnly<OrderItem>()
            .FirstOrDefaultAsync(or => or.ProductId.ToString() == productId && or.OrderId == order.Id);

        if (orderItem == null)
        {
            orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = Guid.Parse(productId),
                Quantity = quantity,
                Price = product.Price,
                SelectedColor = SelectedColor,
                SelectedSizes = SelectedSizes,
            };

            await repository.AddAsync(orderItem);
        }
        else
        {
            orderItem.SelectedColor = SelectedColor;
            orderItem.SelectedSizes = SelectedSizes;
            orderItem.Quantity += quantity;
           
        }

        await repository.SaveChangesAsync();
    }

    public async Task ClearCartItemsAsync()
    {
        var cartItems = await repository
            .All<CartItem>()
            .ToListAsync();

        if(cartItems != null)
        {
            await repository.DeleteRange(cartItems);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<CartItemViewModel>> GetAllCartItemAsync(string userId)
    {
       var allColors = await repository
            .AllReadOnly<Color>()
            .Select(c => c.Name)
            .ToListAsync();

        var allSizes = await repository
            .AllReadOnly<Size>()
            .Select(s => s.Name)
            .ToListAsync();

        var items = await repository
             .All<CartItem>()
             .Where(c => c.UserId.ToString() == userId)
             .Select(c => new CartItemViewModel
             {
                 Id = c.Id.ToString(),
                 ProductName = c.Product.Name,
                 ProductPrice = c.Product.Price,
                 Quantity = c.Quantity,
                 SelectedColors = c.SelectedColor ?? new List<string>(),
                 SelectedSizes = c.SelectedSizes ?? new List<string>(),
                 AvaibleColors = allColors,
                 AvaibleSizes = allSizes
             }).ToListAsync();

        return items;
    }

    public async Task<string> GetProductNameAsync(string cartItemId)
    {
        var cartItem = await repository
            .AllReadOnly<CartItem>()
            .Include(p => p.Product)
            .FirstOrDefaultAsync(ci => ci.Id.ToString() == cartItemId);

        if(cartItem == null)
        {
            return "Unknown product name";
        }

        return cartItem.Product.Name;
    }

    public async Task<bool> ProductAlreadyExistInCartItemAsync(string productId, string userId)
    {
        return  await repository
            .AllReadOnly<CartItem>()
            .AnyAsync(ci => ci.ProductId.ToString() == productId && ci.UserId.ToString() == userId);
    }

    public async Task RemoveFromCartAsync(string cartItemId)
    {
        Guid cartItemGuid = Guid.Parse(cartItemId);

        CartItem cartItemToRemove = await repository
            .All<CartItem>()
            .FirstAsync(ci => ci.Id ==  cartItemGuid);


        if(cartItemToRemove != null)
        {

            var orderItemToRemove = await repository
                .All<OrderItem>()
                .FirstOrDefaultAsync(oi => oi.ProductId == cartItemToRemove.ProductId &&
                                     oi.Order.UserId ==  cartItemToRemove.UserId &&
                                     oi.Order.OrderStatus == OrderStatus.Cart);

            if (orderItemToRemove != null)
            {
                await repository.DeleteAsync<OrderItem>(orderItemToRemove.Id);
            }

            await repository.DeleteAsync<CartItem>(cartItemToRemove.Id);
            await repository.SaveChangesAsync();
        }
    }

    public async Task UpdateColorsAsync(string cartItemId, List<string> colors)
    {
        var cartItem = await repository
            .All<CartItem>()
            .FirstOrDefaultAsync(ci => ci.Id.ToString() == cartItemId);

        if (cartItem != null)
        {
            cartItem.SelectedColor =
                 colors.Where(color => color != null)
                 .Distinct()
                 .ToList();

            await repository.SaveChangesAsync();
        }
    }

    public async Task UpdateQuantityCartAsync(string cartItemId, int quantity)
    {
        var cartItem = await repository
            .All<CartItem>()
            .FirstOrDefaultAsync(ci => ci.Id.ToString() == cartItemId);

        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        }

        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            await repository.SaveChangesAsync();
        }
    }

    public async Task UpdateOrderItemsColorsAsync(string cartItemId, List<string> colors)
    {
        var cartItem = await repository
            .AllReadOnly<CartItem>()
            .FirstOrDefaultAsync(ci => ci.Id.ToString() == cartItemId);

        if (cartItem == null)
        {
            throw new ArgumentException("Cart item not found!");
        }

        var order = await repository
           .AllReadOnly<Order>()
           .FirstOrDefaultAsync(o => o.UserId == cartItem.UserId && o.OrderStatus == OrderStatus.Cart);

        if (order != null)
        {
            var orderItem = await repository
                .All<OrderItem>()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.ProductId == cartItem.ProductId);

            if (orderItem != null)
            {
                orderItem.SelectedColor = 
                    colors.Where(color => color != null)
                    .Distinct()
                    .ToList();
                await repository.SaveChangesAsync();
            }
        }
    }

    public async Task UpdateOrderItemsSizesAsync(string cartItemId, List<string> sizes)
    {
        var cartItem = await repository
            .AllReadOnly<CartItem>()
            .FirstOrDefaultAsync(ci => ci.Id.ToString() == cartItemId);

        if (cartItem == null)
        {
            throw new ArgumentException("Cart item not found!");
        }

        var order = await repository
            .AllReadOnly<Order>()
            .FirstOrDefaultAsync(o => o.UserId == cartItem.UserId &&
                                 o.OrderStatus == OrderStatus.Cart);

        if(order != null)
        {
            var orderItem = await repository
                .All<OrderItem>()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id &&
                                     oi.ProductId == cartItem.ProductId);

            if(orderItem != null)
            {
                orderItem.SelectedSizes =
                    sizes.Where(size => size != null)
                    .Distinct()
                    .ToList();
                await repository.SaveChangesAsync();
            }
        }
    }

    public async Task UpdateOrderItemQuantityAsync(string cartItemId, int quantity)
    {
        var cartItem = await repository
            .AllReadOnly<CartItem>()
            .FirstOrDefaultAsync(ci => ci.Id.ToString() == cartItemId);

        if (cartItem == null)
        {
            throw new ArgumentException("Cart item not found!");
        }

        var order = await repository
            .AllReadOnly<Order>()
            .FirstOrDefaultAsync(o => o.UserId == cartItem.UserId &&
                                 o.OrderStatus == OrderStatus.Cart);

        if(order != null)
        {
            var orderItem = await repository
                .All<OrderItem>()
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id &&
                                     oi.ProductId == cartItem.ProductId);

            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            if (orderItem != null)
            {
                orderItem.Quantity = quantity;
                await repository.SaveChangesAsync();
            }
        }
    }

    public async Task UpdateSizesAsync(string cartItemId, List<string> sizes)
    {
        var cartItem = await repository
          .All<CartItem>()
          .FirstOrDefaultAsync(ci => ci.Id.ToString() == cartItemId);

        if (cartItem != null)
        {
            cartItem.SelectedSizes = 
                sizes.Where(size => size != null)
                .Distinct()
                .ToList();
            await repository.SaveChangesAsync();
        }
    }

    public async Task<bool> UserHasItemsInCart(string userId)
    {
        return await repository
            .AllReadOnly<CartItem>()
            .AnyAsync(ci => ci.UserId.ToString() == userId);
    }
}
