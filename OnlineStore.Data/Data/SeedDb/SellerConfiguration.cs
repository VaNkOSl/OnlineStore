namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

internal class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        var data = new SeedData();

        builder.HasData(new Seller[] { data.FirstSeller, data.SecondSeller});
    }
}
