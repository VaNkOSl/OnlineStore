namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

internal class ProductSizeConfiguration : IEntityTypeConfiguration<ProductSize>
{
    public void Configure(EntityTypeBuilder<ProductSize> builder)
    {
        var data = new SeedData();

        builder.HasData(new ProductSize[] { data.FirstProductSize, data.SecondProductSize });

        builder
           .HasOne(p => p.Product)
           .WithMany(ps => ps.AvailableSizes)
           .HasForeignKey(p => p.ProductId)
           .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(s => s.Size)
            .WithMany()
            .HasForeignKey(s => s.SizeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
