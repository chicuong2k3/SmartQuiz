using SmartQuiz.Client.Data.Commands;

namespace SmartQuiz.Client.Data.Services;

public interface IFlashcardService : IComputeService
{
    [CommandHandler]
    Task<FlashcardDto?> CreateFlashcardAsync(CreateFlashcardCommand command,
        CancellationToken cancellationToken = default);

    [CommandHandler]
    Task<FlashcardDto?> UpdateFlashcardAsync(UpdateFlashcardCommand command,
        CancellationToken cancellationToken = default);

    [ComputeMethod]
    Task<IEnumerable<FlashcardDto>> GetFlashcardsAsync(CancellationToken cancellationToken = default);

    [ComputeMethod]
    Task<FlashcardDto?> GetFlashcardByIdAsync(Guid id, CancellationToken cancellationToken = default);
}