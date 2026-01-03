namespace SmartQuiz.Data.Models;

public class FlashcardSet : Entity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    private readonly List<Flashcard> _flashcards = new();
    public IReadOnlyCollection<Flashcard> Flashcards => [.. _flashcards];

    private FlashcardSet()
    {
    }

    public FlashcardSet(string title, string? description = null)
    {
        Title = title;
        Description = description;
    }
}