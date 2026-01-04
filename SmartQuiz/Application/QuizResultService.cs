using ActualLab.Collections;
using SmartQuiz.Client.Data.Commands;
using SmartQuiz.Client.Data.Services;

namespace SmartQuiz.Application;

public class QuizResultService(IServiceProvider services)
    : DbServiceBase<ApplicationDbContext>(services), IQuizResultService
{
    // ============ COMPUTE METHODS - Cached, read-only ============

    public virtual async Task<QuizResultDto?> GetQuizResultByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var quizResult = await dbContext.QuizResults
            .Include(x => x.TopicPerformances)
            .Include(x => x.Answers.OrderBy(a => a.QuestionNumber))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (quizResult == null)
            return null;

        return MapToDto(quizResult);
    }

    public virtual async Task<IEnumerable<QuizResultDto>> GetUserQuizResultsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var quizResults = await dbContext.QuizResults
            .Where(x => x.UserId == userId)
            .Include(x => x.TopicPerformances)
            .Include(x => x.Answers.OrderBy(a => a.QuestionNumber))
            .OrderByDescending(x => x.CompletedAt)
            .ToListAsync(cancellationToken);

        return quizResults.Select(MapToDto);
    }

    // ============ COMMAND METHODS - Write operations ============

    public async Task<QuizResultDto?> CreateQuizResultAsync(
        CreateQuizResultCommand command,
        CancellationToken cancellationToken = default)
    {
        var context = CommandContext.GetCurrent();

        // Invalidation phase
        if (Invalidation.IsActive)
        {
            var dto = context.Operation.Items.Get<QuizResultDto>("Result");
            if (dto != null)
            {
                _ = GetQuizResultByIdAsync(dto.Id, default);
                _ = GetUserQuizResultsAsync(dto.UserId, default);
            }

            return null;
        }

        // Execution phase
        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);

        var quizResult = new QuizResult(
            command.UserId,
            command.FlashcardSetId,
            command.QuizTitle,
            command.TotalQuestions,
            command.CorrectAnswers,
            command.TimeTaken,
            command.PointsEarned,
            command.GlobalRankPercentile
        );

        dbContext.QuizResults.Add(quizResult);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Add topic performances
        foreach (var topic in command.TopicPerformances)
        {
            var topicPerformance = new TopicPerformance(
                quizResult.Id,
                topic.TopicName,
                topic.CorrectCount,
                topic.TotalCount
            );
            dbContext.TopicPerformances.Add(topicPerformance);
        }

        // Add question answers
        foreach (var answer in command.Answers)
        {
            var questionAnswer = new QuestionAnswer(
                quizResult.Id,
                answer.QuestionNumber,
                answer.IsCorrect,
                answer.Question,
                answer.CorrectAnswer,
                answer.UserAnswer
            );
            dbContext.QuestionAnswers.Add(questionAnswer);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Reload with related data
        var result = await dbContext.QuizResults
            .Include(x => x.TopicPerformances)
            .Include(x => x.Answers.OrderBy(a => a.QuestionNumber))
            .FirstOrDefaultAsync(x => x.Id == quizResult.Id, cancellationToken);

        var resultDto = result != null ? MapToDto(result) : null;

        // Store in operation items for invalidation phase
        if (resultDto != null)
            context.Operation.Items.Set("Result", resultDto);

        return resultDto;
    }

    // ============ HELPER METHODS ============

    private static QuizResultDto MapToDto(QuizResult quizResult)
    {
        return new QuizResultDto
        {
            Id = quizResult.Id,
            UserId = quizResult.UserId,
            FlashcardSetId = quizResult.FlashcardSetId,
            QuizTitle = quizResult.QuizTitle,
            TotalQuestions = quizResult.TotalQuestions,
            CorrectAnswers = quizResult.CorrectAnswers,
            ScorePercentage = quizResult.ScorePercentage,
            TimeTaken = quizResult.TimeTaken,
            PointsEarned = quizResult.PointsEarned,
            GlobalRankPercentile = quizResult.GlobalRankPercentile,
            IsPassed = quizResult.IsPassed,
            TopicPerformances = quizResult.TopicPerformances.Select(tp => new TopicPerformanceDto
            {
                TopicName = tp.TopicName,
                CorrectCount = tp.CorrectCount,
                TotalCount = tp.TotalCount,
                Percentage = tp.Percentage
            }).ToList(),
            Answers = quizResult.Answers.Select(a => new QuestionAnswerDto
            {
                QuestionNumber = a.QuestionNumber,
                IsCorrect = a.IsCorrect,
                Question = a.Question,
                CorrectAnswer = a.CorrectAnswer,
                UserAnswer = a.UserAnswer
            }).ToList(),
            CompletedAt = quizResult.CompletedAt,
            CreatedAt = quizResult.CreatedAt
        };
    }
}