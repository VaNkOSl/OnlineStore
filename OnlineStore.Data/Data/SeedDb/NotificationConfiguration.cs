namespace OnlineStore.Data.Data.SeedDb;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Data.Models;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder
            .HasOne(u => u.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(u => u.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.Seller)
            .WithMany(s => s.Notifications)
            .HasForeignKey(s => s.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
