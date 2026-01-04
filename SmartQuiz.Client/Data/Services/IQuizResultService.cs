namespace SmartQuiz.Client.Data.Services;

public interface IQuizResultService : IComputeService
{
    [ComputeMethod]
    Task<QuizResultDto?> GetQuizResultByIdAsync(Guid id, CancellationToken cancellationToken = default);

    [ComputeMethod]
    Task<IEnumerable<QuizResultDto>>
        GetUserQuizResultsAsync(Guid userId, CancellationToken cancellationToken = default);

    [CommandHandler]
    Task<QuizResultDto?> CreateQuizResultAsync(CreateQuizResultCommand command,
        CancellationToken cancellationToken = default);
}