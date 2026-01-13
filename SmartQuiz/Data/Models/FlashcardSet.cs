namespace SmartQuiz.Data.Models;

public class FlashcardSet : Entity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public string UserId { get; set; } = string.Empty;

    private readonly List<Flashcard> _flashcards = [];
    public IReadOnlyCollection<Flashcard> Flashcards => [.. _flashcards];

    private FlashcardSet()
    {
    }

    public FlashcardSet(string userId, string title, string? description = null, bool isPublic = true)
    {
        UserId = userId;
        Title = title;
        Description = description;
        IsPublic = isPublic;
    }

    public void AddFlashcard(string? term, string? definition, string? imageUrl = null, string? audioUrl = null)
    {
        var flashcard = new Flashcard(Id, term, definition, imageUrl, audioUrl);
        _flashcards.Add(flashcard);
    }
}