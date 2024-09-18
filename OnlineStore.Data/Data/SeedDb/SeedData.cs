namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.AspNetCore.Identity;
using OnlineStore.Data.Models;
using OnlineStore.Data.Models.Enums;

internal class SeedData
{
    public ApplicationUser FirstUser { get; set; }
    public ApplicationUser SecondUser { get; set; }
    public Brand FirstBrand { get; set; }
    public Brand SecondBrand { get; set; }
    public Brand ThirdBrand { get; set; }
    public Seller FirstSeller { get; set; }
    public Seller SecondSeller { get; set; }
    public Color FirstColor { get; set; }
    public Color SecondColor { get; set; }
    public Size FirstSize { get; set; }
    public Size SecondSize { get; set; }
    public Product FirstProduct { get; set; }
    public Product SecondProduct { get; set; }
    public Category FirstCategory { get; set; }
    public Category SecondCategory { get; set; }
    public ProductColor FirstProductColor { get; set; }
    public ProductColor SecondProductColor { get; set; }
    public ProductSize FirstProductSize { get; set; }
    public ProductSize SecondProductSize { get; set; }
    public ProductImage FirstProductImage { get; set; }
    public ProductImage SecondProductImage { get; set; }
    public Review FirstReview { get; set; }
    public Review SecondReview { get; set; }

    public SeedData()
    {
        SeedUsers();
        SeedSellers();
        SeedBrands();
        SeedColors();
        SeedSizes();
        SeedCategories();
        SeedProducts();
        SeedProductColors();
        SeedProductSizes();
        SeedProductImages();
        SeedReviews();
    }

    private void SeedUsers()
    {
        var hasher = new PasswordHasher<ApplicationUser>();

        FirstUser = new ApplicationUser
        {
            Id = Guid.Parse("dea12856-c198-4129-b3f3-b893d8395082"),
            UserName = "user@gmail.com",
            NormalizedUserName = "user@gmail.com",
            Email = "user@gmail.com",
            NormalizedEmail = "user@gmail.com",
            FirstName = "User",
            LastName = "User"
        };

        FirstUser.PasswordHash = hasher
            .HashPassword(FirstUser, "password123");

        SecondUser = new ApplicationUser
        {
            Id = Guid.Parse("dea11856-c198-4129-b3f3-b893d8395082"),
            UserName = "userSecong@gmail.com",
            NormalizedUserName = "userSecond@gmail.com",
            Email = "userSecond@gmail.com",
            NormalizedEmail = "userSecond@gmail.com",
            FirstName = "UserSecond",
            LastName = "UserSecond"
        };

        SecondUser.PasswordHash = hasher
            .HashPassword(SecondUser, "password456");
    }
    private void SeedBrands()
    {
        FirstBrand = new Brand
        {
          Id = 1,
          Name = "Nike"
        };

        SecondBrand = new Brand
        {
            Id = 2,
            Name = "Adidas"
        };

        ThirdBrand = new Brand
        {
          Id = 3,
          Name = "Emporio Armani"
        };
    }
    private void SeedSellers()
    {
        FirstSeller = new Seller
        {
            Id = Guid.NewGuid(),
            FirstName = "First Seller",
            LastName = "Seller",
            Description = "Hello this is description for seeding data base",
            DateOfBirth = new DateTime(1985, 5, 20),
            Egn = "9525478562",
            IsApproved = true,
            PhoneNumber = "0855555555",
            UserId = Guid.Parse("dea12856-c198-4129-b3f3-b893d8395082"),
        };

        SecondSeller = new Seller
        {
            Id = Guid.NewGuid(),
            FirstName = "Second Seller",
            LastName = "Seller",
            Description = "Hello this is description for seeding data base",
            DateOfBirth = new DateTime(1985, 4, 20),
            Egn = "9525478772",
            IsApproved = true,
            PhoneNumber = "0855544555",
            UserId = Guid.Parse("dea11856-c198-4129-b3f3-b893d8395082"),
        };
    }
    private void SeedColors()
    {
        FirstColor = new Color
        {
            Id = 1,
            Name = "Blue"
        };

        SecondColor = new Color
        {
            Id = 2,
            Name = "Green"
        };
    }
    private void SeedSizes()
    {
        FirstSize = new Size
        {
            Id = 1,
            Name = "L"
        };

        SecondSize = new Size
        {
            Id = 2,
            Name = "M"
        };
    }
    private void SeedProducts()
    {
        FirstProduct = new Product
        {
            Name = "First Product",
            Description = "Description for the first product",
            StockQuantity = 100,
            DateAdded = DateTime.UtcNow,
            Price = 29.99M,
            IsAvaible = true,
            IsApproved = true,
            SuitableSeason = Season.Summer,
            SellerId = FirstSeller.Id,
            CategoryId = 1,
            BrandId = FirstBrand.Id
        };

        SecondProduct = new Product
        {
            Name = "Second Product",
            Description = "Description for the second product",
            StockQuantity = 200,
            DateAdded = DateTime.UtcNow,
            Price = 49.99M,
            IsAvaible = true,
            IsApproved = true,
            SuitableSeason = Season.Winter,
            SellerId = SecondSeller.Id,
            CategoryId = 2,
            BrandId = SecondBrand.Id
        };
    }
    private void SeedProductColors()
    {
        FirstProductColor = new ProductColor
        {
          Id = Guid.NewGuid(),
          ColorId = 1,
          ProductId = Guid.NewGuid(),
        };

        SecondProductColor = new ProductColor
        {
            Id = Guid.NewGuid(),
            ColorId = 2,
            ProductId = Guid.NewGuid(),
        };
    }
    private void SeedProductSizes()
    {
        FirstProductSize = new ProductSize
        {
            Id = Guid.NewGuid(),
            SizeId = 1,
            ProductId = Guid.NewGuid(),
        };

        SecondProductSize = new ProductSize
        {
            Id = Guid.NewGuid(),
            SizeId = 2,
            ProductId = Guid.NewGuid(),
        };
    }
    private void SeedProductImages()
    {
        FirstProductImage = new ProductImage
        {
            Id = Guid.NewGuid(),
            FilePath = "file/path/firstImage",
            ProductId = Guid.NewGuid(),
        };

        SecondProductImage = new ProductImage
        {
            Id = Guid.NewGuid(),
            FilePath = "file/path/secondImage",
            ProductId = Guid.NewGuid(),
        };
    }
    private void SeedReviews()
    {
        FirstReview = new Review
        {
            Id = Guid.NewGuid(),
            Content = "Test review",
            Rating = 5,
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
        };

        SecondReview = new Review
        {
            Id = Guid.NewGuid(),
            Content = "Test second product review",
            Rating = 2,
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
        };
    }
    private void SeedCategories()
    {
        FirstCategory = new Category
        {
            Id = 1,
            Name = "Clothes"
        };

        SecondCategory = new Category
        {
            Id = 2,
            Name = "Shoes"
        };
    }
}
