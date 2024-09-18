namespace OnlineStore.Tests;

using Microsoft.AspNetCore.Identity;
using OnlineStore.Data;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;

public static class DataBaseSeeder
{
    public static ApplicationUser? SellerUser;
    public static ApplicationUser? NotApprovedSellerUser;
    public static ApplicationUser? User;
    public static ApplicationUser? AdminUser;
    public static Seller? Seller;
    public static Seller? NotApprovedSeller;
    public static Product? Product;
    public static Category? Category;
    public static Brand? Brand;
    public static Order? Order;
    public static Color? Color;
    public static Size? Size;
    public static ProductColor? ProductColor;
    public static ProductSize? ProductSize;
    public static CartItem? CartItem;
    public static IdentityRole<Guid>? AdministratorRole;
    public static async void SeedDataBase(OnlineStoreDbContext data)
    {
        var sellerId = Guid.Parse("93e72464-4832-4483-952f-41d221ab1091");
        var notApprovedSellerUserId = Guid.Parse("f8b8ce23-d7eb-4e7a-a72e-7a9f178f95f3");
        var userId = Guid.Parse("f8b8ce23-d7eb-4e7a-a72e-7a9f178f9518");
        var orderId = Guid.Parse("88A2419A-82EA-42D2-AA22-6A39D33E7E13");
        var adminId = Guid.Parse("09078562-87B8-4950-9293-97285BED265B");

        User = new ApplicationUser
        {
            Id = userId,
            UserName = "Stefan",
            NormalizedUserName = "STEFAN",
            Email = "stefanUser@abv.bg",
            NormalizedEmail = "STEFANUSER@ABV.BG",
            EmailConfirmed = true,
            PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",
            ConcurrencyStamp = "caf271d7-0ba7-4ab1-8d8d-6d0e3711c27d",
            SecurityStamp = "ca32c787-626e-4234-a4e4-8c94d85a2b1c",
            TwoFactorEnabled = false,
            FirstName = "Stefan",
            LastName = "User"
        };

        AdminUser = new ApplicationUser
        {
            Id = adminId,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            EmailConfirmed = true,
            PasswordHash = "AQAAAAIAAYagAAAAEGJVx2MaTXDrU9JCtBadSbOVGNtnJFsgWLxWze/2tSeP7jA6x9gwHSQzYkb2j7xb0Q==",
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            FirstName = "Admin",
            LastName = "User"
        };

        AdministratorRole = new IdentityRole<Guid>
        {
          Id = Guid.NewGuid(),
          Name = "Administrator",
          NormalizedName = "ADMINISTRATOR"
        };

        SellerUser = new ApplicationUser
        {
            Id = sellerId,
            UserName = "Pesho",
            NormalizedUserName = "PESHO",
            Email = "pesho@seller.com",
            NormalizedEmail = "PESHO@SELLER.COM",
            EmailConfirmed = true,
            PasswordHash = "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92",
            ConcurrencyStamp = "caf271d7-0ba7-4ab1-8d8d-6d0e3711c27d",
            SecurityStamp = "ca32c787-626e-4234-a4e4-8c94d85a2b1c",
            TwoFactorEnabled = false,
            FirstName = "Pesho",
            LastName = "Petrov"
        };

        NotApprovedSellerUser = new ApplicationUser
        {
            Id = notApprovedSellerUserId,
            UserName = "Ivan",
            NormalizedUserName = "IVAN",
            Email = "ivan@user.com",
            NormalizedEmail = "IVAN@USER.COM",
            EmailConfirmed = true,
            PasswordHash = "2y04$9Hkk2kb4fH72A0DdqFstvOPExe1W18n46bnY4tlCm4aOZlmSHJrzK",
            ConcurrencyStamp = "caf271d7-0bw7-4ab1-8d8d-6d0e3711c27d",
            SecurityStamp = "ca32c787-626e-4274-a4e4-8c94d85a2b1c",
            TwoFactorEnabled = false,
            FirstName = "Ivan",
            LastName = "Petrov"
        };

        Seller = new Seller
        {
            Id = Guid.NewGuid(),
            Egn = "0341815267",
            DateOfBirth = new DateTime(2003, 05, 12),
            Description = "Hello I just want to test my application",
            FirstName = "Ivan",
            LastName = "Petrov",
            PhoneNumber = "+3591478569",
            UserId = sellerId,
            IsApproved = true
        };

        NotApprovedSeller = new Seller
        {
            Id = Guid.NewGuid(),
            Egn = "0000000000",
            DateOfBirth = new DateTime(2003, 05, 12),
            Description = "Hello I just want to test my application",
            FirstName = "Not",
            LastName = "Approved",
            PhoneNumber = "+3591478569",
            UserId = notApprovedSellerUserId,
            IsApproved = false,
            IsAdminReject = true,
            User = NotApprovedSellerUser,
            RejectionReason = "Test reject reason"
        };

        Category = new Category
        {
          Id = 15,
          Name = "Test"
        };

        Brand = new Brand
        {
          Id = 22,
          Name = "Test",
          ImageUrl = "Test image url"
        };

        Color = new Color
        {
            Id = 3,
            Name = "Blue"
        };

        Size = new Size
        {
            Id = 3,
            Name = "X"
        };

        ProductColor = new ProductColor
        {
            Id = Guid.NewGuid(),
            ColorId = Color.Id,
            Color = Color
        };

        ProductSize = new ProductSize
        {
            Id = Guid.NewGuid(),
            SizeId = Size.Id,
            Size = Size
        };

        Product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "First Product",
            Description = "Description for the first product",
            StockQuantity = 100,
            DateAdded = DateTime.UtcNow,
            Price = 29.99M,
            CategoryId = Category.Id,
            BrandId = Brand.Id,
            SuitableSeason = Season.Summer,
            SellerId = sellerId,
            Seller = Seller,
            IsAvaible = true,
            AvailableColors = new List<ProductColor>
            {
                ProductColor
            },
            AvailableSizes = new List<ProductSize>
            {
                ProductSize
            }
        };

        var OrderItems = new List<OrderItem>
        {
            new OrderItem
            {
                  ProductId = Product.Id,
                  Quantity = 2,
                  Product = Product,
                  Price = 59.98M,
                  OrderId = Guid.NewGuid(),
                  SelectedColor = new List<string> { "Blue" },
                  SelectedSizes = new List<string> { "L", "M" }
            }
        };

        CartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            UserId = User.Id,
            ProductId = Product.Id,
            Quantity = 2,
            SelectedColor = new List<string> { "Blue" },
            SelectedSizes = new List<string> { "M" }
        };

        Order = new Order
        {
            Id = orderId,
            UserId = notApprovedSellerUserId,
            OrderStatus = OrderStatus.Cart,
            Adress = "123 Main St",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "123456789",
            Email = "john.doe@example.com",
            OrderItems = OrderItems,
            User = User
        };


        await data.Roles.AddAsync(AdministratorRole);
        await data.Users.AddAsync(AdminUser!);

        await data.UserRoles.AddAsync(new IdentityUserRole<Guid>
        {
            UserId = adminId,
            RoleId = AdministratorRole.Id
        });

        await data.Users.AddAsync(User);
        await data.Users.AddAsync(SellerUser);
        await data.Users.AddAsync(NotApprovedSellerUser);
        await data.Sellers.AddAsync(Seller);
        await data.Sellers.AddAsync(NotApprovedSeller);
        await data.Categories.AddAsync(Category);
        await data.Brands.AddAsync(Brand);
        await data.Colors.AddAsync(Color);
        await data.Sizes.AddAsync(Size);
        await data.ProductColors.AddAsync(ProductColor);
        await data.ProductSizes.AddAsync(ProductSize);
        await data.Products.AddAsync(Product);
        await data.CartItems.AddAsync(CartItem);
        await data.Orders.AddAsync(Order);
        await data.SaveChangesAsync();  
    }
}
                                                             