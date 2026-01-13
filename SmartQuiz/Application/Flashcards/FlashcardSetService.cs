using System.Reactive;
using ActualLab.Collections;
using SmartQuiz.Client.Data.Commands;
using SmartQuiz.Client.Data.Services;

namespace SmartQuiz.Application.Flashcards;

public class FlashcardSetService(IServiceProvider services, IUserContext userContext)
    : DbServiceBase<ApplicationDbContext>(services), IFlashcardSetService
{
    // ============ COMPUTE METHODS - Cached, read-only ============

    public virtual async Task<IEnumerable<FlashcardSetDto>> GetFlashcardSetsAsync(
        CancellationToken cancellationToken = default)
    {
        // Use CreateDbContext for read operations
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var items = await dbContext.FlashcardSets
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        // Materialize the collection to avoid LINQ iterator serialization issues
        return items.Adapt<List<FlashcardSetDto>>();
    }

    public virtual async Task<FlashcardSetDto?> GetFlashcardSetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await DbHub.CreateDbContext(cancellationToken);
        var item = await dbContext.FlashcardSets
            .Include(x => x.Flashcards)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return item?.Adapt<FlashcardSetDto>();
    }

    // ============ COMMAND METHODS - Write operations ============

    public virtual async Task<FlashcardSetDto> CreateFlashcardSetAsync(
        CreateFlashcardSetCommand command,
        CancellationToken cancellationToken = default)
    {
        // Invalidation phase - invalidate related queries
        if (Invalidation.IsActive)
        {
            _ = GetFlashcardSetsAsync(CancellationToken.None);
            return null!; // Return value ignored during invalidation
        }

        // Get current user ID from session in command
        var session = command.Session;
        var userId = await userContext.GetCurrentUserIdAsync(session, cancellationToken);

        // Use CreateOperationDbContext for write operations
        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);

        var flashcardSet = new FlashcardSet(
            userId,
            command.Title,
            command.Description,
            command.IsPublic
        );

        // Add flashcards to the set
        foreach (var flashcardItem in command.Flashcards)
        {
            flashcardSet.AddFlashcard(flashcardItem.Term, flashcardItem.Definition);
        }

        dbContext.FlashcardSets.Add(flashcardSet);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Reload with flashcards for proper DTO mapping
        var result = await dbContext.FlashcardSets
            .Include(x => x.Flashcards)
            .FirstAsync(x => x.Id == flashcardSet.Id, cancellationToken);

        return result.Adapt<FlashcardSetDto>();
    }

    public virtual async Task<FlashcardSetDto> UpdateFlashcardSetAsync(
        UpdateFlashcardSetCommand command,
        CancellationToken cancellationToken = default)
    {
        var context = CommandContext.GetCurrent();

        // Invalidation phase
        if (Invalidation.IsActive)
        {
            // Retrieve DTO from operation items
            var dto = context.Operation.Items.Get<FlashcardSetDto>("flashcardSet");
            if (dto != null)
            {
                _ = GetFlashcardSetByIdAsync(dto.Id, CancellationToken.None);
                _ = GetFlashcardSetsAsync(CancellationToken.None);
            }

            return null!;
        }

        // Execution phase
        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);

        var flashcardSet = await dbContext.FlashcardSets
            .FindAsync([command.Id], cancellationToken);

        if (flashcardSet == null)
            throw new InvalidOperationException($"FlashcardSet with ID {command.Id} not found");

        // Update entity properties
        flashcardSet.Title = command.Title;
        flashcardSet.Description = command.Description;

        await dbContext.SaveChangesAsync(cancellationToken);

        var result = flashcardSet.Adapt<FlashcardSetDto>();

        // Store in operation items for invalidation phase
        context.Operation.Items["flashcardSet"] = result;

        return result;
    }

    public virtual async Task<Unit> DeleteFlashcardSetAsync(
        DeleteFlashcardSetCommand command,
        CancellationToken cancellationToken = default)
    {
        var context = CommandContext.GetCurrent();

        if (Invalidation.IsActive)
        {
            var deletedDto = context.Operation.Items.Get<FlashcardSetDto>("flashcardSet");
            if (deletedDto != null)
            {
                _ = GetFlashcardSetByIdAsync(deletedDto.Id, CancellationToken.None);
                _ = GetFlashcardSetsAsync(CancellationToken.None);
            }

            return default!;
        }

        await using var dbContext = await DbHub.CreateOperationDbContext(cancellationToken);

        var flashcardSet = await dbContext.FlashcardSets
            .FindAsync([command.Id], cancellationToken);

        if (flashcardSet == null)
            throw new InvalidOperationException($"FlashcardSet with ID {command.Id} not found");

        // Store DTO before deletion for invalidation phase
        var deletedSetDto = flashcardSet.Adapt<FlashcardSetDto>();
        context.Operation.Items["flashcardSet"] = deletedSetDto;

        dbContext.FlashcardSets.Remove(flashcardSet);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Default;
    }
}