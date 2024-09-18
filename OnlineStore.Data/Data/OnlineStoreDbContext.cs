namespace OnlineStore.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data.Data.SeedDb;
using OnlineStore.Data.Models;

public class OnlineStoreDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public OnlineStoreDbContext(DbContextOptions<OnlineStoreDbContext> options)
        : base(options)
    {
    }

    public OnlineStoreDbContext()
    {    
    }

    public virtual DbSet<Product> Products { get; set; } = null!;
    public virtual DbSet<ProductColor> ProductColors { get; set; } = null!;
    public virtual DbSet<ProductSize> ProductSizes { get; set; } = null!;
    public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<Brand> Brands { get; set; } = null!;
    public virtual DbSet<Color> Colors { get; set; } = null!;
    public virtual DbSet<Size> Sizes { get; set; } = null!;
    public virtual DbSet<Order> Orders { get; set; } = null!;
    public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
    public virtual DbSet<CartItem> CartItems { get; set; } = null!;
    public virtual DbSet<Seller> Sellers { get; set; } = null!;
    public virtual DbSet<Review> Reviews { get; set; } = null!;
    public virtual DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new UserConfiguration());
        builder.ApplyConfiguration(new SellerConfiguration());
        builder.ApplyConfiguration(new BrandConfiguration());
        builder.ApplyConfiguration(new ColorConfiguration());
        builder.ApplyConfiguration(new SizeConfiguration());
        builder.ApplyConfiguration(new CategoryConfiguration());
        builder.ApplyConfiguration(new ProductColorConfiguration());
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.ApplyConfiguration(new CartItemConfiguration());
        builder.ApplyConfiguration(new ProductSizeConfiguration());
        builder.ApplyConfiguration(new ProductImageConfiguration());
        builder.ApplyConfiguration(new ReviewConfiguration());
        builder.ApplyConfiguration(new OrderItemConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());

        base.OnModelCreating(builder);
    }
}
