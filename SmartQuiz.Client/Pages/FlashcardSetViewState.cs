using System.Collections.Immutable;

namespace SmartQuiz.Client.Pages;

public record FlashcardSetViewState
{
    public FlashcardSetDto? FlashcardSet { get; init; }
    public ImmutableList<FlashcardDto> Flashcards { get; init; } = ImmutableList<FlashcardDto>.Empty;
}