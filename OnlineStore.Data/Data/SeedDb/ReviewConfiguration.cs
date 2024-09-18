
namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

internal class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        var data = new SeedData();

        builder.HasData(new Review[] { data.FirstReview, data.SecondReview });

        builder
            .HasOne(x => x.Product)
            .WithMany(r => r.Reviews)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Product)
            .WithMany(r => r.Reviews)
            .HasForeignKey(p => p.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
             .HasOne(x => x.User)
             .WithMany(r => r.Reviews)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}
