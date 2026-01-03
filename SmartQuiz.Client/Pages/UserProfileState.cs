using System.Collections.Immutable;

namespace SmartQuiz.Client.Pages;

public record UserProfileState
{
    public ImmutableList<FlashcardSetDto> UserSets { get; init; } = ImmutableList<FlashcardSetDto>.Empty;
}