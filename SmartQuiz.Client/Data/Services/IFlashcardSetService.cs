using System.Reactive;
using SmartQuiz.Client.Data.Commands;

namespace SmartQuiz.Client.Data.Services;

public interface IFlashcardSetService : IComputeService
{
    [ComputeMethod]
    Task<IEnumerable<FlashcardSetDto>> GetFlashcardSetsAsync(CancellationToken cancellationToken = default);

    [ComputeMethod]
    Task<FlashcardSetDto?> GetFlashcardSetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    [CommandHandler]
    Task<FlashcardSetDto> CreateFlashcardSetAsync(CreateFlashcardSetCommand command,
        CancellationToken cancellationToken = default);

    [CommandHandler]
    Task<FlashcardSetDto> UpdateFlashcardSetAsync(UpdateFlashcardSetCommand command,
        CancellationToken cancellationToken = default);

    [CommandHandler]
    Task<Unit> DeleteFlashcardSetAsync(DeleteFlashcardSetCommand command,
        CancellationToken cancellationToken = default);
}