namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

internal class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        var data = new SeedData();

        builder.HasData(new Brand[] {data.FirstBrand,data.SecondBrand,data.ThirdBrand });

        builder
           .HasMany(p => p.Products)
           .WithOne(b => b.Brand)
           .HasForeignKey(p => p.BrandId)
           .OnDelete(DeleteBehavior.Restrict);
    }
}
