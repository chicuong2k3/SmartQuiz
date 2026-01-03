using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartQuiz.Data.Configurations;

public sealed class FlashcardConfiguration : IEntityTypeConfiguration<Flashcard>
{
    public void Configure(EntityTypeBuilder<Flashcard> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id)
            .ValueGeneratedNever();

        builder.Property(f => f.StudySetId)
            .IsRequired();

        builder.Property(f => f.FrontText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(f => f.BackText)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(f => f.ImageUrl)
            .HasMaxLength(500);

        builder.Property(f => f.AudioUrl)
            .HasMaxLength(500);

        // builder.HasOne(f => f.StudySet)
        //     .WithMany()
        //     .HasForeignKey(f => f.StudySetId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}