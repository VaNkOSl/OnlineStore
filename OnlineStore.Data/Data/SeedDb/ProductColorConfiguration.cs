namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

internal class ProductColorConfiguration : IEntityTypeConfiguration<ProductColor>
{
    public void Configure(EntityTypeBuilder<ProductColor> builder)
    {
        var data = new SeedData();

        builder.HasData(new ProductColor[] { data.FirstProductColor, data.SecondProductColor });

        builder
           .HasOne(p => p.Product)
           .WithMany(p => p.AvailableColors)
           .HasForeignKey(p => p.ProductId)
           .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(c => c.Color)
            .WithMany()
            .HasForeignKey(c => c.ColorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
