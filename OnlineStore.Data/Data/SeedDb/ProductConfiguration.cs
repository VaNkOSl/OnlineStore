namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;
internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        var data = new SeedData();

        builder.HasData(new Product[] { data.FirstProduct, data.SecondProduct });

        builder
          .HasOne(c => c.Category)
          .WithMany(p => p.Products)
          .HasForeignKey(c => c.CategoryId)
          .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Seller)
            .WithMany(p => p.OwnedProducts)
            .HasForeignKey(s => s.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Brand)
            .WithMany(p => p.Products)
            .HasForeignKey(b => b.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(p => p.Price)
            .HasPrecision(18, 2);

    

        builder
            .HasMany(pi => pi.ProductImages)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(pc => pc.AvailableColors)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(ps => ps.AvailableSizes)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(r => r.Reviews)
            .WithOne(p => p.Product)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
