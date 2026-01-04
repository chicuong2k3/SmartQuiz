using ActualLab.Collections;
using SmartQuiz.Client.Data.Commands;
using SmartQuiz.Client.Data.Services;

namespace SmartQuiz.Application;

public class FlashcardService(
    IServiceProvider services)
    : DbServiceBase<ApplicationDbContext>(services), IFlashcardService
{
    public virtual async Task<FlashcardDto?> CreateFlashcardAsync(
        CreateFlashcardCommand command,
        CancellationToken cancellationToken = default)
    {
        if (Invalidation.IsActive)
        {
            _ = GetFlashcardsAsync(CancellationToken.None);
            return null;
        }

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);
        var flashcard = new Flashcard(
            command.StudySetId,
            command.FrontText,
            command.BackText,
            command.ImageUrl,
            command.AudioUrl
        );

        dbContext.Flashcards.Add(flashcard);
        await dbContext.SaveChangesAsync(cancellationToken);

        return flashcard.Adapt<FlashcardDto>();
    }

    public virtual async Task<FlashcardDto?> UpdateFlashcardAsync(UpdateFlashcardCommand command,
        CancellationToken cancellationToken = default)
    {
        var context = CommandContext.GetCurrent();
        if (Invalidation.IsActive)
        {
            var itemDto = context.Operation.Items.Get<FlashcardDto>("flashcard");
            if (itemDto != null)
            {
                _ = GetFlashcardByIdAsync(itemDto.Id, CancellationToken.None);
                return new FlashcardDto();
            }
        }

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);
        var item = await dbContext.Flashcards.FindAsync(DbKey.Compose(command.Id), cancellationToken);
        if (item is null)
        {
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        context.Operation.Items.Set("flashcard", item.Adapt<FlashcardDto>());

        throw new NotImplementedException();
    }

    public virtual async Task<IEnumerable<FlashcardDto>> GetFlashcardsAsync(
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var items = await dbContext.Flashcards
            .ToListAsync(cancellationToken);
        // Materialize the collection to avoid LINQ iterator serialization issues
        return items.Adapt<List<FlashcardDto>>();
    }

    public virtual async Task<FlashcardDto?> GetFlashcardByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var item = await dbContext.Flashcards
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return item?.Adapt<FlashcardDto>();
    }
}