namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

internal class ColorConfiguration : IEntityTypeConfiguration<Color>
{
    public void Configure(EntityTypeBuilder<Color> builder)
    {
        var data = new SeedData();

        builder.HasData(new Color[] { data.FirstColor, data.SecondColor});
    }
}
