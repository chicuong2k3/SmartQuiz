using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartQuiz.Data.Configurations;

public class QuizResultConfiguration : IEntityTypeConfiguration<QuizResult>
{
    public void Configure(EntityTypeBuilder<QuizResult> builder)
    {
        builder.ToTable("QuizResults");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuizTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.TotalQuestions)
            .IsRequired();

        builder.Property(x => x.CorrectAnswers)
            .IsRequired();

        builder.Property(x => x.ScorePercentage)
            .IsRequired();

        builder.Property(x => x.PointsEarned)
            .IsRequired();

        builder.Property(x => x.GlobalRankPercentile)
            .IsRequired();

        builder.Property(x => x.IsPassed)
            .IsRequired();

        builder.Property(x => x.CompletedAt)
            .IsRequired();

        builder.HasOne(x => x.FlashcardSet)
            .WithMany()
            .HasForeignKey(x => x.FlashcardSetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.TopicPerformances)
            .WithOne(x => x.QuizResult)
            .HasForeignKey(x => x.QuizResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Answers)
            .WithOne(x => x.QuizResult)
            .HasForeignKey(x => x.QuizResultId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.FlashcardSetId);
        builder.HasIndex(x => x.CompletedAt);
    }
}

public class TopicPerformanceConfiguration : IEntityTypeConfiguration<TopicPerformance>
{
    public void Configure(EntityTypeBuilder<TopicPerformance> builder)
    {
        builder.ToTable("TopicPerformances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TopicName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CorrectCount)
            .IsRequired();

        builder.Property(x => x.TotalCount)
            .IsRequired();

        builder.Property(x => x.Percentage)
            .IsRequired();

        builder.HasIndex(x => x.QuizResultId);
    }
}

public class QuestionAnswerConfiguration : IEntityTypeConfiguration<QuestionAnswer>
{
    public void Configure(EntityTypeBuilder<QuestionAnswer> builder)
    {
        builder.ToTable("QuestionAnswers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.QuestionNumber)
            .IsRequired();

        builder.Property(x => x.IsCorrect)
            .IsRequired();

        builder.Property(x => x.Question)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CorrectAnswer)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.UserAnswer)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(x => x.QuizResultId);
        builder.HasIndex(x => x.QuestionNumber);
    }
}