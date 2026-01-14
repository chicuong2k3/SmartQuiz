using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartQuiz.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(2000);
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.IsRead).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
    }
}