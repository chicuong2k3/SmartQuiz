using System.Collections.Immutable;

namespace SmartQuiz.Client.Pages;

public record HomeState
{
    public ImmutableList<FlashcardSetDto> RecentSets { get; init; } = ImmutableList<FlashcardSetDto>.Empty;
    public ImmutableList<FlashcardSetDto> RecommendedSets { get; init; } = ImmutableList<FlashcardSetDto>.Empty;
}